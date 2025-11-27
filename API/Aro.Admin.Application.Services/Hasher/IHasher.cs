using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.Hasher;

public interface IHasher : IService
{
    string Hash(string text);

    bool Verify(string text, string hash);
}
