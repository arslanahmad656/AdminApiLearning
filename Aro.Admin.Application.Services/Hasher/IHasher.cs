using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.Hasher;

public interface IHasher : IService
{
    string Hash(string text);

    bool Verify(string text, string hash);
}
