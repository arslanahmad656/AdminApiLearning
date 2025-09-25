using Aro.Admin.Application.Mediator.SystemSettings.DTOs;
using Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using AutoMapper;

namespace Aro.Admin.Presentation.Entry;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserCreatedLog>()
            .ForCtorParam(nameof(UserCreatedLog.AssignedRoles), opt => opt.MapFrom(u => u.UserRoles.Select(r => r.RoleId).ToList()));
        CreateMap<CreateUserRequest, CreateUserDto>();
        CreateMap<Application.Services.DTOs.ServiceResponses.CreateUserResponse, Application.Mediator.User.DTOs.CreateUserResponse>();
        CreateMap<Application.Services.DTOs.ServiceResponses.CreateUserResponse, InitializeSystemResponse>();
        CreateMap<BootstrapUser, CreateUserDto>();
        CreateMap<Role, GetRoleRespose>();

        CreateMap<InitializeSystemResponse, SystemInitializedLog>();
    }
}
