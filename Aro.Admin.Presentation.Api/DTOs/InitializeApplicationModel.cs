namespace Aro.Admin.Presentation.Api.DTOs;

public record InitializeApplicationModel(
    string Email,
    string PhoneNumber,
    string Password, 
    string DisplayName, 
    string BootstrapPassword
);

