using System.Text.Json.Serialization;

namespace TaskManager.Core.ViewModels;

public class TaskUser
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("organizationId")]
    public long OrganizationId { get; set; }
}

