using Aro.Admin.Application.Services.Role;
using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.User;

public interface IUserService : IService
{
    Task<CreateUserResponse> CreateUser(CreateUserDto user, CancellationToken cancellationToken = default);

    Task<GetUserResponse> GetUserById(Guid userId, bool includeRoles, bool includePasswordHash, CancellationToken cancellationToken = default);

    Task<GetUserResponse> GetUserByEmail(string email, bool includeRoles, bool includePasswordHash, CancellationToken cancellationToken = default);

    Task<GetUserResponse> GetSystemUser(string systemPassword, CancellationToken cancellationToken = default);

    Task ResetPassword(Guid userId, string newPassword, CancellationToken cancellationToken = default);

    Task ChangePassword(ChangePasswordParameters parameters, CancellationToken cancellationToken = default);
}
