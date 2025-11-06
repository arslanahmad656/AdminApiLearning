using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.AccessToken;

public interface ITokenBlackListService : IService
{
    Task BlackList(string tokenIdentifier, DateTime expiry, CancellationToken cancellationToken = default);

    Task<bool> IsBlackListed(string tokenIdentifier, CancellationToken cancellationToken = default);
}
