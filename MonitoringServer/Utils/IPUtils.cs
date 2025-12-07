using System.Net;

public static class IPUtils
{
    public static string ExtractIP(string endpoint)
    {
        if (string.IsNullOrEmpty(endpoint))
            return "Unknown";
        int colonIndex = endpoint.LastIndexOf(':');
        if (colonIndex > 0)
            return endpoint.Substring(0, colonIndex);
        return endpoint;
    }

    public static bool IsLocalhost(string ip)
    {
        return ip == "127.0.0.1" || ip == "::1" || ip == "localhost";
    }
}

