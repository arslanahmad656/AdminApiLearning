using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services.DataServices;

public interface IUserService : IService
{
    Task<CreateUserResponse> CreateUser(CreateUserDto user, CancellationToken cancellationToken = default);

    Task<GetUserResponse> GetUserById(Guid userId, bool includeRoles, bool includePasswordHash, CancellationToken cancellationToken = default);

    Task<GetUserResponse> GetUserByEmail(string email, bool includeRoles, bool includePasswordHash, CancellationToken cancellationToken = default);

    Task<GetUserResponse> GetSystemUser(string systemPassword, CancellationToken cancellationToken = default);

    Task ResetPassword(Guid userId, string newPassword, CancellationToken cancellationToken = default);

    Task ChangePassword(ChangePasswordParameters parameters, CancellationToken cancellationToken = default);
}
