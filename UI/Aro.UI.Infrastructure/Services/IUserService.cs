using Aro.UI.Application.DTOs;

namespace Aro.UI.Infrastructure.Services;

public interface IUserService
{
    Task<CreateUserResponse?> CreateUser(CreateUserRequest request);

    Task<GetUserResponse?> GetUserById(Guid guid);

    Task<GetUserResponse?> GetUserByEmail(GetUserByEmailRequest request);
}
