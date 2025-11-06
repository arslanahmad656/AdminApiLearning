namespace Aro.Admin.Application.Services.Email;

public record EmailAttachement(string FileName, byte[] Data, string ContentType);
