namespace Aro.Admin.Application.Shared.Options;

public class PasswordPolicyOptions
{
    public int MinLength { get; init; }
    public bool RequireDigit { get; init; }
    public int MinUniqueCharacters { get; init; }
    public bool RequireLowerCase { get; init; }
    public bool RequireUpperCase { get; init; }
    public bool RequirePunctuation { get; init; }
}
