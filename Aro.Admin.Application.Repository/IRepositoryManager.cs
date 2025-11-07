namespace Aro.Admin.Application.Repository;

public interface IRepositoryManager
{
    ISystemSettingsRepository SystemSettingsRepository { get; }

    IRefreshTokenRepository RefreshTokenRepository { get; }

    IPasswordResetTokenRepository PasswordResetTokenRepository { get; }

    IEmailTemplateRepository EmailTemplateRepository { get; }

    IPasswordHistoryRepository PasswordHistoryRepository { get; }
}
