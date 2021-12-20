using System.Text.Json.Serialization;

namespace TaskManager.Core.ViewModels;

public class OrganizationUpdate
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }
}
