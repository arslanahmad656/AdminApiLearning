using Aro.Admin.Application.Mediator.SystemSettings.DTOs;
using Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Application.Mediator.UserRole.DTOs;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Presentation.Api.DTOs;
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
        CreateMap<Role, GetUserRolesResponse>()
            .ForCtorParam(nameof(GetUserRolesResponse.RoleName), opt => opt.MapFrom(r => r.Name))
            .ForCtorParam(nameof(GetUserRolesResponse.RoleId), opt => opt.MapFrom(r => r.Id));
        CreateMap<InitializeSystemResponse, SystemInitializedLog>();

        CreateMap<AssignRolesByIdResponse, RolesAssignedLog>();
        CreateMap<RevokeRolesByIdResponse, RolesRevokedLog>();
        CreateMap<AssignRolesModel, AssignRolesByIdRequest>();
        CreateMap<RevokeRolesModel, RevokeRolesByIdRequest>();
    }
}
