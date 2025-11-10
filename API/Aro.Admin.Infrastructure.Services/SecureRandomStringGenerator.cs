using Aro.Admin.Application.Services.RandomValueGenerator;
using System.Security.Cryptography;

namespace Aro.Admin.Infrastructure.Services;

public class SecureRandomStringGenerator : IRandomValueGenerator
{
    private readonly string _characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:,.<>?/";

    public string GenerateString(int length)
    {
        if (length <= 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be a positive integer.");

        var result = new char[length];
        var randomBytes = new byte[length];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        for (var i = 0; i < length; i++)
        {
            var index = randomBytes[i] % _characters.Length;
            result[i] = _characters[index];
        }

        return new string(result);
    }
}
