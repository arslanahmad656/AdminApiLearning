using Aro.Admin.Application.Services;

namespace Aro.Admin.Infrastructure.Services;

public class BCrypttPasswordHasher : IHasher
{
    public string Hash(string text) => BCrypt.Net.BCrypt.HashPassword(text);

    public bool Verify(string text, string hash) => BCrypt.Net.BCrypt.Verify(text, hash);
}
