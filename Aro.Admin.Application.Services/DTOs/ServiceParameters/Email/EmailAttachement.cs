namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Email;

public record EmailAttachement(string FileName, byte[] Data, string ContentType);
