using Aro.Admin.Presentation.UI.Models;

namespace Aro.Admin.Presentation.UI.Services;

public interface IUserService
{
    Task<CreateUserResponse?> CreateUser(CreateUserRequest request);

    Task<GetUserResponse?> GetUserById(Guid guid);

    Task<GetUserResponse?> GetUserByEmail(GetUserByEmailRequest request);
}
