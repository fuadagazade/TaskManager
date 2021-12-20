using System.Text.Json.Serialization;

namespace TaskManager.Core.Models.Table;

public class Sorting
{
    [JsonPropertyName("column")]
    public string Column { get; set; }

    [JsonPropertyName("direction")]
    public string Direction { get; set; }
}
