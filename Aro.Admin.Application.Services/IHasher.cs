namespace Aro.Admin.Application.Services;

public interface IHasher : IService
{
    string Hash(string text);

    bool Verify(string text, string hash);
}
