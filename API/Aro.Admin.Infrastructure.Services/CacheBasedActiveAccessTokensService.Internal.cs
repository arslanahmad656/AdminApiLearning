namespace Aro.Admin.Infrastructure.Services;

public partial class CacheBasedActiveAccessTokensService
{
    private readonly string Prefix = "active_tokens_";

    private string GetKey(Guid userId) => $"{Prefix}{userId}";
}
