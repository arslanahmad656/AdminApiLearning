using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services.DataServices;

public interface IUserService
{
    Task<CreateUserResponse> CreateUser(CreateUserDto user, CancellationToken cancellationToken = default);
}
