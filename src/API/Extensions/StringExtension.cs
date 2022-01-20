using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace API.Extensions;

public static class StringExtension
{
    private static string[] DefaultExcludeStringsForLogForging =>
        new[] { "%0a", "%0d", "%0A", "%0D", "\r", "\n" };

    public static SecureString ToSecureString(this string rawString)
    {
        var secureString = new SecureString();

        foreach (var rawChar in rawString)
        {
            secureString.AppendChar(rawChar);
        }

        secureString.MakeReadOnly();

        return secureString;
    }

    public static string PreventLogForging(this string? content, string[]? excludeStrings = null)
    {
        content ??= string.Empty;

        excludeStrings ??= DefaultExcludeStringsForLogForging;

        var normalizedLogContent = content.Normalize(NormalizationForm.FormKC);

        return excludeStrings.Aggregate(normalizedLogContent, (current, excludeString) => current.Replace(excludeString, string.Empty));
    }

    public static (byte[], byte[]) GetHashAndSalt(this string password)
    {
        using var hmac = new HMACSHA512();

        return (hmac.ComputeHash(Encoding.UTF8.GetBytes(password)), hmac.Key);
    }

    public static bool ValidatePassword(this string password, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return !computedHash.Where((b, i) => b != hash[i]).Any();
    }
}
