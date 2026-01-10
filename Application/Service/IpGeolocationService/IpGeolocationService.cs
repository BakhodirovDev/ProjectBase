using AutoMapper;
using Domain.Abstraction;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Application.Service;

public class IpGeolocationService : IIpGeolocationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IpGeolocationService> _logger;
    private readonly IMapper _mapper;
    private const string BaseUrl = "https://ipapi.co";

    public IpGeolocationService(HttpClient httpClient, ILogger<IpGeolocationService> logger, IMapper mapper)
    {
        _httpClient = httpClient;
        _logger = logger;
        _mapper = mapper;
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
    }

    public async Task<IpLocationInfo?> GetLocationInfoAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            _logger.LogWarning("IP address is empty");
            return null;
        }

        if (IsPrivateOrLocalIp(ipAddress))
        {
            _logger.LogDebug("Skipping geolocation for private/local IP: {IpAddress}", ipAddress);
            return new IpLocationInfo
            {
                Ip = ipAddress,
                City = "Local",
                Region = "Local",
                Country = "Local",
                CountryName = "Local Network",
                IsLocal = true
            };
        }

        try
        {
            var url = $"{BaseUrl}/{ipAddress}/json/";
            _logger.LogDebug("Fetching geolocation data from: {Url}", url);

            var response = await _httpClient.GetFromJsonAsync<IpApiResponse>(url, cancellationToken);

            if (response == null)
            {
                _logger.LogWarning("No response from IP API for: {IpAddress}", ipAddress);
                return null;
            }

            if (response.Error == true)
            {
                _logger.LogWarning("IP API returned error for {IpAddress}: {Reason}", ipAddress, response.Reason);
                return null;
            }

            return _mapper.Map<IpLocationInfo>(response);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while fetching geolocation for IP: {IpAddress}", ipAddress);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Timeout while fetching geolocation for IP: {IpAddress}", ipAddress);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching geolocation for IP: {IpAddress}", ipAddress);
            return null;
        }
    }

    private static bool IsPrivateOrLocalIp(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return true;

        if (ipAddress == "::1" || ipAddress == "127.0.0.1" || ipAddress.StartsWith("127."))
            return true;

        if (ipAddress.StartsWith("10."))
            return true;

        if (ipAddress.StartsWith("192.168."))
            return true;

        if (ipAddress.StartsWith("172."))
        {
            var parts = ipAddress.Split('.');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int secondOctet))
            {
                if (secondOctet >= 16 && secondOctet <= 31)
                    return true;
            }
        }

        if (ipAddress.StartsWith("169.254."))
            return true;

        if (ipAddress.StartsWith("fe80:", StringComparison.OrdinalIgnoreCase))
            return true;

        if (ipAddress.StartsWith("fc00:", StringComparison.OrdinalIgnoreCase) ||
            ipAddress.StartsWith("fd00:", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
}