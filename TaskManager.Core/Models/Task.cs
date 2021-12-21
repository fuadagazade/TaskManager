using System.Text.Json.Serialization;
using TaskManager.Core.Enumerations;

namespace TaskManager.Core.Models;

internal class Task : BaseModel
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("priority")]
    public Priority Priority { get; set; }

    [JsonPropertyName("point")]
    public int Point { get; set; }

    [JsonPropertyName("state")]
    public State State { get; set; }

    [JsonPropertyName("deadline")]
    public DateTime Deadline { get; set; }

    [JsonPropertyName("organizationId")]
    public long OrganizationId { get; set; }

    [JsonPropertyName("creatorId")]
    public long CreatorId { get; set; }

    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }
}
