using System.Net;

namespace BlockedCountriesApi.Helpers;

/// <summary>
/// Utility class for extracting the client's IP address from the HTTP context.
/// Handles X-Forwarded-For headers for reverse proxy scenarios.
/// </summary>
public static class IpAddressHelper
{
    /// <summary>
    /// Extracts the client IP address from the HttpContext.
    /// Checks X-Forwarded-For header first (for reverse proxy), then falls back to RemoteIpAddress.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>The client's IP address as a string, or "unknown" if it cannot be determined.</returns>
    public static string GetClientIpAddress(HttpContext context)
    {
        // Check X-Forwarded-For header (used by reverse proxies/load balancers)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            // X-Forwarded-For can contain multiple IPs; the first one is the original client
            var ip = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
            if (!string.IsNullOrWhiteSpace(ip))
                return ip;
        }

        // Fall back to the connection's remote IP address
        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp != null)
        {
            // Convert IPv6 loopback (::1) to IPv4 loopback for consistency
            if (IPAddress.IsLoopback(remoteIp))
                return "127.0.0.1";

            // If it's an IPv4-mapped IPv6 address, extract the IPv4 part
            if (remoteIp.IsIPv4MappedToIPv6)
                return remoteIp.MapToIPv4().ToString();

            return remoteIp.ToString();
        }

        return "unknown";
    }

    /// <summary>
    /// Validates whether a string is a valid IPv4 or IPv6 address format.
    /// </summary>
    /// <param name="ipAddress">The IP address string to validate.</param>
    /// <returns>True if the IP address format is valid.</returns>
    public static bool IsValidIpAddress(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return false;

        return IPAddress.TryParse(ipAddress, out _);
    }
}
