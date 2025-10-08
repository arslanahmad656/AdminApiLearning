using Aro.Admin.Application.Mediator.Authentication.DTOs;
using Aro.Admin.Application.Mediator.Shared.DTOs;
using Aro.Admin.Application.Mediator.SystemSettings.DTOs;
using Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Application.Mediator.UserRole.DTOs;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Presentation.Api.DTOs;
using AutoMapper;
using AuditParameters = Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using ServiceParameters = Aro.Admin.Application.Services.DTOs.ServiceParameters;
using ServiceResponses = Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Presentation.Entry;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, AuditParameters.UserCreatedLog>()
            .ForCtorParam(nameof(AuditParameters.UserCreatedLog.AssignedRoles), opt => opt.MapFrom(u => u.UserRoles.Select(r => r.RoleId).ToList()));
        CreateMap<CreateUserRequest, ServiceParameters.CreateUserDto>();
        CreateMap<ServiceResponses.CreateUserResponse, Application.Mediator.User.DTOs.CreateUserResponse>();
        CreateMap<ServiceResponses.CreateUserResponse, InitializeSystemResponse>();
        CreateMap<BootstrapUser, ServiceParameters.CreateUserDto>();
        CreateMap<InitializeApplicationModel, BootstrapUser>();
        CreateMap<Role, ServiceResponses.GetRoleRespose>();
        CreateMap<Role, ServiceResponses.GetUserRolesResponse>()
            .ForCtorParam(nameof(ServiceResponses.GetUserRolesResponse.RoleName), opt => opt.MapFrom(r => r.Name))
            .ForCtorParam(nameof(ServiceResponses.GetUserRolesResponse.RoleId), opt => opt.MapFrom(r => r.Id));
        CreateMap<InitializeSystemResponse, AuditParameters.SystemInitializedLog>();

        CreateMap<AssignRolesByIdResponse, AuditParameters.RolesAssignedLog>();
        CreateMap<RevokeRolesByIdResponse, AuditParameters.RolesRevokedLog>();
        CreateMap<AssignRolesModel, AssignRolesByIdRequest>();
        CreateMap<RevokeRolesModel, RevokeRolesByIdRequest>();
        CreateMap<ServiceResponses.GetRoleRespose, GetRoleResponse>();

        CreateMap<User, ServiceResponses.GetUserResponse>()
            .ForCtorParam(nameof(ServiceResponses.GetUserResponse.Roles), opt => opt.MapFrom(u => u.UserRoles.Select(r => r.Role ?? new Role()).ToList()));

        CreateMap<AuthenticateUserResponse, ServiceResponses.TokenResponse>();
        CreateMap<ServiceResponses.TokenResponse, SuccessfulAuthenticationData>();
        CreateMap<SuccessfulAuthenticationData, AuthenticationSuccessfulLog>();
        CreateMap<FailedAuthenticationData, AuthenticationFailedLog>();
    }
}
