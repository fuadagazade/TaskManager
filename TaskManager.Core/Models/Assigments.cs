using System.Text.Json.Serialization;

namespace TaskManager.Core.Models;

public class Assigments : BaseModel
{
    [JsonPropertyName("taskId")]
    public long TaskId { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }
}
