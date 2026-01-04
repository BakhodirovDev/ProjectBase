namespace Domain.Abstraction;

public class IpLocationInfo
{
    public string Ip { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? RegionCode { get; set; }
    public string? Country { get; set; }
    public string? CountryName { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryCodeIso3 { get; set; }
    public string? CountryCapital { get; set; }
    public string? CountryTld { get; set; }
    public string? ContinentCode { get; set; }
    public string? Postal { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Timezone { get; set; }
    public string? UtcOffset { get; set; }
    public string? CountryCallingCode { get; set; }
    public string? Currency { get; set; }
    public string? CurrencyName { get; set; }
    public string? Languages { get; set; }
    public double? CountryArea { get; set; }
    public long? CountryPopulation { get; set; }
    public string? Asn { get; set; }
    public string? Org { get; set; }
    public bool IsLocal { get; set; }

    /// <summary>
    /// Formatli location string qaytaradi: "City, Country"
    /// </summary>
    public string GetFormattedLocation()
    {
        if (IsLocal)
            return "Local Network";

        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(City))
            parts.Add(City);

        if (!string.IsNullOrWhiteSpace(CountryName))
            parts.Add(CountryName);
        else if (!string.IsNullOrWhiteSpace(Country))
            parts.Add(Country);

        return parts.Any() ? string.Join(", ", parts) : "Unknown Location";
    }

    /// <summary>
    /// Qisqacha location: "City, CountryCode"
    /// </summary>
    public string GetShortLocation()
    {
        if (IsLocal)
            return "Local";

        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(City))
            parts.Add(City);

        if (!string.IsNullOrWhiteSpace(CountryCode))
            parts.Add(CountryCode);

        return parts.Any() ? string.Join(", ", parts) : "Unknown";
    }
}
