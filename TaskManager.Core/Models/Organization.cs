using System.Text.Json.Serialization;

namespace TaskManager.Core.Models;
public class Organization : BaseModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("tag")]
    public string Tag { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("logo")]
    public string Logo { get; set; }
}
