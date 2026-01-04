namespace Domain.Abstraction;

public interface IIpGeolocationService
{
    Task<IpLocationInfo?> GetLocationInfoAsync(string ipAddress, CancellationToken cancellationToken = default);
}