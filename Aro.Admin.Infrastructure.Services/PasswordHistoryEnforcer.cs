using Aro.Admin.Application.Services.Hasher;
using Aro.Admin.Application.Services.Password;
using Aro.Admin.Application.Services.UniqueIdGenerator;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Infrastructure.Services;

public class PasswordHistoryEnforcer(
	IOptions<PasswordHistorySettings> passwordHistoryOptions,
	IRepositoryManager repositoryManager,
	IHasher hasher,
	IUniqueIdGenerator idGenerator,
	ILogManager<PasswordHistoryEnforcer> logger,
	ErrorCodes errorCodes
) : IPasswordHistoryEnforcer
{
	private readonly PasswordHistorySettings settings = passwordHistoryOptions.Value;
	private readonly IPasswordHistoryRepository passwordHistoryRepo = repositoryManager.PasswordHistoryRepository;

	public async Task EnsureCanUsePassword(Guid userId, string newPassword)
	{
		logger.LogDebug("Starting {MethodName}", nameof(EnsureCanUsePassword));
		if (!settings.Enabled || settings.HistoryCount <= 0)
		{
			logger.LogDebug("Password history enforcement disabled or HistoryCount <= 0. Skipping checks.");
			return;
		}

		var query = passwordHistoryRepo.GetByUserId(userId);
		if (settings.ExpireAfterDays.HasValue)
		{
			var cutoff = DateTime.UtcNow.AddDays(-settings.ExpireAfterDays.Value);
			query = query.Where(ph => ph.PasswordSetDate >= cutoff);
		}

		query = query.Take(settings.HistoryCount);
		var recent = await query.ToListAsync().ConfigureAwait(false);

		foreach (var item in recent)
		{
			if (hasher.Verify(newPassword, item.PasswordHash))
			{
				logger.LogWarn("Password reuse detected for user: {UserId}", userId);
				throw new AroInvalidOperationException(errorCodes.PASSWORD_NOT_ALLOWED_TO_BE_REUSED, $"New password cannot match the last {settings.HistoryCount} passwords.");
			}
		}

		logger.LogDebug("Completed {MethodName}", nameof(EnsureCanUsePassword));
	}

	public async Task RecordPassword(Guid userId, string passwordHash)
	{
		logger.LogDebug("Starting {MethodName}", nameof(RecordPassword));
		if (!settings.Enabled)
		{
			logger.LogDebug("Password history recording is disabled. Skipping record.");
			return;
		}

		var entity = new PasswordHistory
		{
			Id = idGenerator.Generate(),
			UserId = userId,
			PasswordHash = passwordHash,
			PasswordSetDate = DateTime.UtcNow
		};

		await passwordHistoryRepo.Create(entity).ConfigureAwait(false);
		await repositoryManager.SaveChanges().ConfigureAwait(false);

		logger.LogInfo("Password history recorded for user: {UserId}, historyId: {HistoryId}", userId, entity.Id);
		logger.LogDebug("Completed {MethodName}", nameof(RecordPassword));
	}

	public async Task TrimHistory(Guid userId)
	{
		logger.LogDebug("Starting {MethodName}", nameof(TrimHistory));
		if (!settings.Enabled)
		{
			logger.LogDebug("Password history trim is disabled. Skipping.");
			return;
		}

		var query = passwordHistoryRepo.GetByUserId(userId);
		var all = await query.ToListAsync().ConfigureAwait(false);

		var now = DateTime.UtcNow;
		var toDelete = new List<PasswordHistory>();

		if (settings.ExpireAfterDays.HasValue)
		{
			var cutoff = now.AddDays(-settings.ExpireAfterDays.Value);
			toDelete.AddRange(all.Where(ph => ph.PasswordSetDate < cutoff));
		}

		var remaining = all
			.Except(toDelete)
			.OrderByDescending(ph => ph.PasswordSetDate)
			.ToList();

		if (settings.HistoryCount >= 0 && remaining.Count > settings.HistoryCount)
		{
			toDelete.AddRange(remaining.Skip(settings.HistoryCount));
		}

		if (toDelete.Count != 0)
		{
			passwordHistoryRepo.DeleteRange(toDelete);
			await repositoryManager.SaveChanges().ConfigureAwait(false);
			logger.LogInfo("Trimmed {Count} password history entries for user: {UserId}", toDelete.Count, userId);
		}

		logger.LogDebug("Completed {MethodName}", nameof(TrimHistory));
	}
}


