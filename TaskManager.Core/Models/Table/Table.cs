using System.Text.Json.Serialization;

namespace TaskManager.Core.Models.Table;

public class Table
{
    [JsonPropertyName("filter")]
    public Dictionary<string, string> Filter { get; set; }

    [JsonPropertyName("paginator")]
    public Paginator Paginator { get; set; }

    [JsonPropertyName("sorting")]
    public Sorting Sorting { get; set; }

    [JsonPropertyName("search")]
    public string Search { get; set; }
}
