using Domain.Abstraction;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Application.Service;

public class IpGeolocationService : IIpGeolocationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IpGeolocationService> _logger;
    private const string BaseUrl = "https://ipapi.co";

    public IpGeolocationService(HttpClient httpClient, ILogger<IpGeolocationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(5); // 5 soniya timeout
    }

    public async Task<IpLocationInfo?> GetLocationInfoAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            _logger.LogWarning("IP address is empty");
            return null;
        }

        // Localhost va private IP lar uchun ma'lumot qaytarmaymiz
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

            // Agar error bo'lsa
            if (response.Error == true)
            {
                _logger.LogWarning("IP API returned error for {IpAddress}: {Reason}", ipAddress, response.Reason);
                return null;
            }

            return MapToLocationInfo(response);
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

    private static IpLocationInfo MapToLocationInfo(IpApiResponse response)
    {
        return new IpLocationInfo
        {
            Ip = response.Ip ?? string.Empty,
            City = response.City,
            Region = response.Region,
            RegionCode = response.RegionCode,
            Country = response.Country,
            CountryName = response.CountryName,
            CountryCode = response.CountryCode,
            CountryCodeIso3 = response.CountryCodeIso3,
            CountryCapital = response.CountryCapital,
            CountryTld = response.CountryTld,
            ContinentCode = response.ContinentCode,
            Postal = response.Postal,
            Latitude = response.Latitude,
            Longitude = response.Longitude,
            Timezone = response.Timezone,
            UtcOffset = response.UtcOffset,
            CountryCallingCode = response.CountryCallingCode,
            Currency = response.Currency,
            CurrencyName = response.CurrencyName,
            Languages = response.Languages,
            CountryArea = response.CountryArea,
            CountryPopulation = response.CountryPopulation,
            Asn = response.Asn,
            Org = response.Org,
            IsLocal = false
        };
    }

    private static bool IsPrivateOrLocalIp(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return true;

        // Localhost
        if (ipAddress == "::1" || ipAddress == "127.0.0.1" || ipAddress.StartsWith("127."))
            return true;

        // Private IP ranges
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

        // Link-local
        if (ipAddress.StartsWith("169.254."))
            return true;

        // IPv6 private
        if (ipAddress.StartsWith("fe80:", StringComparison.OrdinalIgnoreCase))
            return true;

        if (ipAddress.StartsWith("fc00:", StringComparison.OrdinalIgnoreCase) ||
            ipAddress.StartsWith("fd00:", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
}