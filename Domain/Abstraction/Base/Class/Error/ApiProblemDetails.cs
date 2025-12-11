using Newtonsoft.Json;

namespace Domain.Abstraction.Base;

public class ApiProblemDetails
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    [JsonExtensionData]
    public Dictionary<string, object>? Extensions { get; set; }
}
