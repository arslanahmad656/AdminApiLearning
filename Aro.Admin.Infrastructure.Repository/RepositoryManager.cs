using Aro.Admin.Application.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository;

public class RepositoryManager(AroDbContext dbContext) : IRepositoryManager
{
    private readonly Lazy<SystemSettingsRepository> systemSettingsRepository = new(() => new SystemSettingsRepository(dbContext));
    private readonly Lazy<RefreshTokenRepository> refreshTokenRepository = new(() => new RefreshTokenRepository(dbContext));
    private readonly Lazy<PasswordResetTokenRepository> passwordResetTokenRepository = new(() => new PasswordResetTokenRepository(dbContext));
    private readonly Lazy<EmailTemplateRepository> emailTemplateRepository = new(() => new EmailTemplateRepository(dbContext));
    private readonly Lazy<PasswordHistoryRepository> passwordHistoryRepository = new(() => new PasswordHistoryRepository(dbContext));


    public ISystemSettingsRepository SystemSettingsRepository => systemSettingsRepository.Value;

    public IRefreshTokenRepository RefreshTokenRepository => refreshTokenRepository.Value;

    public IPasswordResetTokenRepository PasswordResetTokenRepository => passwordResetTokenRepository.Value;

    public IEmailTemplateRepository EmailTemplateRepository => emailTemplateRepository.Value;

    public IPasswordHistoryRepository PasswordHistoryRepository => passwordHistoryRepository.Value;
}