using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManager.Core.Enumerations;

namespace TaskManager.Core.Models;
public class BaseModel
{
    [JsonPropertyName("id")]
    [Key]
    public long? Id { get; set; }

    [JsonPropertyName("status")]
    public Status Status { get; set; }
}