namespace Aro.Admin.Application.Services;

public interface ITokenBlackListService
{
    Task BlackList(string tokenIdentifier, DateTime expiry, CancellationToken cancellationToken = default);

    Task<bool> IsBlackListed(string tokenIdentifier, CancellationToken cancellationToken = default);
}
