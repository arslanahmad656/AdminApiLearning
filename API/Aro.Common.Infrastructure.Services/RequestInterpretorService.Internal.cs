namespace Aro.Common.Infrastructure.Services;

public partial class RequestInterpretorService
{
    private static string StripPortFromIpAddress(string ipAddress)
    {
        // Handle IPv6 with port: [2001:db8::1]:8080
        if (ipAddress.StartsWith('['))
        {
            var closeBracketIndex = ipAddress.IndexOf(']');
            if (closeBracketIndex > 0)
            {
                // Return just the bracketed part without the port
                return ipAddress[..(closeBracketIndex + 1)];
            }
        }
        
        // Handle IPv4 with port: 192.168.1.1:8080
        // Also handles IPv6 without brackets (though less common)
        var lastColonIndex = ipAddress.LastIndexOf(':');
        if (lastColonIndex > 0)
        {
            // Check if this might be IPv6 without port (contains multiple colons)
            var colonCount = ipAddress.Count(c => c == ':');
            if (colonCount > 1)
            {
                // Likely IPv6 without port, return as-is
                return ipAddress;
            }
            
            // Single colon, likely IPv4 with port
            return ipAddress[..lastColonIndex];
        }

        // No port found, return as-is
        return ipAddress;
    }
}

