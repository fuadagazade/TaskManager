using System.Text.Json.Serialization;

namespace TaskManager.Core.Models;

public class Assigment : BaseModel
{
    [JsonPropertyName("taskId")]
    public long TaskId { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }
}
