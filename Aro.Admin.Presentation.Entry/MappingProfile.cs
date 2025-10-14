using Aro.Admin.Application.Mediator.Authentication.DTOs;
using Aro.Admin.Application.Mediator.Shared.DTOs;
using Aro.Admin.Application.Mediator.SystemSettings.DTOs;
using Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Application.Mediator.UserRole.DTOs;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
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
            .ForMember(nameof(AuditParameters.UserCreatedLog.AssignedRoles), opt => opt.MapFrom(u => u.UserRoles.Select(r => r.RoleId).ToList()));
        CreateMap<ServiceResponses.CreateUserResponse, UserCreatedLog>();
        CreateMap<CreateUserRequest, ServiceParameters.CreateUserDto>();
        CreateMap<CreateUserModel, CreateUserRequest>();
        CreateMap<ServiceResponses.CreateUserResponse, Application.Mediator.User.DTOs.CreateUserResponse>();
        CreateMap<ServiceResponses.CreateUserResponse, InitializeSystemResponse>()
            .ForMember(nameof(InitializeSystemResponse.BootstrapUsername), opt => opt.MapFrom(u => u.Email))
            .ForMember(nameof(InitializeSystemResponse.BootstrapUserId), opt => opt.MapFrom(u => u.Id));
        CreateMap<BootstrapUser, ServiceParameters.CreateUserDto>();
        CreateMap<InitializeApplicationModel, BootstrapUser>();
        CreateMap<Role, ServiceResponses.GetRoleRespose>();
        CreateMap<Role, ServiceResponses.GetUserRolesResponse>()
            .ForMember(nameof(ServiceResponses.GetUserRolesResponse.RoleName), opt => opt.MapFrom(r => r.Name))
            .ForMember(nameof(ServiceResponses.GetUserRolesResponse.RoleId), opt => opt.MapFrom(r => r.Id));
        CreateMap<InitializeSystemResponse, AuditParameters.SystemInitializedLog>();

        CreateMap<AssignRolesByIdResponse, AuditParameters.RolesAssignedLog>();
        CreateMap<RevokeRolesByIdResponse, AuditParameters.RolesRevokedLog>();
        CreateMap<AssignRolesModel, AssignRolesByIdRequest>();
        CreateMap<RevokeRolesModel, RevokeRolesByIdRequest>();
        CreateMap<ServiceResponses.GetRoleRespose, GetRoleResponse>();

        CreateMap<User, ServiceResponses.GetUserResponse>()
            .ForMember(nameof(ServiceResponses.GetUserResponse.Roles), opt => opt.MapFrom(u => u.UserRoles.Select(r => r.Role ?? new Role()).ToList()));

        CreateMap<AuthenticateUserResponse, ServiceResponses.CompositeToken>();
        CreateMap<ServiceResponses.AccessTokenResponse, SuccessfulAuthenticationData>();
        CreateMap<SuccessfulAuthenticationData, AuthenticationSuccessfulLog>();
        CreateMap<FailedAuthenticationData, AuthenticationFailedLog>();
        CreateMap<CompositeToken, SuccessfulAuthenticationData>();
        CreateMap<CompositeToken, AuthenticateUserResponse>();

        CreateMap<Domain.Entities.RefreshToken, ServiceResponses.RefreshToken>()
            .ForMember(nameof(ServiceResponses.RefreshToken.Token), opt => opt.MapFrom(rt => rt.TokenHash));
        CreateMap<Domain.Entities.RefreshToken, ServiceResponses.UserRefreshToken>()
            .ForMember(nameof(ServiceResponses.RefreshToken.Token), opt => opt.MapFrom(rt => rt.TokenHash));
        CreateMap<UserLoggedOutNotificationData, UserSessionLoggedOutLog>();
        CreateMap<UserLoggedOutAllNotificationData, UserSessionsLoggedOutLog>();
        CreateMap<LogoutUserModel, LogoutUserRequest>();
        CreateMap<LogoutAllUserModel, LogoutUserAllRequest>();
        CreateMap<CompositeToken, RefreshTokenResponse>();
        CreateMap<TokenRefreshedNotificationData, TokenRefreshedLog>();
        CreateMap<RefreshTokenModel, RefreshTokenRequest>();
    }
}
