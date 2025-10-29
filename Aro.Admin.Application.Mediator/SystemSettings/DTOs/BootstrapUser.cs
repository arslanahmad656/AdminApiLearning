namespace Aro.Admin.Application.Mediator.SystemSettings.DTOs;

public record BootstrapUser(
    string Email, 
    string PhoneNumber,
    string Password, 
    string DisplayName, 
    string BootstrapPassword
);

