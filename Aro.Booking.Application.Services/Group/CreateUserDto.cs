namespace Aro.Booking.Application.Services.Group;

public record CreateUserDto(
    string Email,
    bool IsActive, 
    bool IsSystemUser, 
    string Password, 
    string DisplayName, 
    ICollection<string> AssignedRoles
);

