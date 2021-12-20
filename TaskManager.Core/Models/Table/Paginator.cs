using System.Text.Json.Serialization;

namespace TaskManager.Core.Models.Table;
public class Paginator
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }
}