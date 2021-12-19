using System.Text.Json.Serialization;
using TaskManager.Core.Enumerations;

namespace TaskManager.Core.Models;
public class User : BaseModel
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; }

    [JsonPropertyName("role")]
    public Role Role { get; set; }

    [JsonPropertyName("approved")]
    public bool Approved { get; set; }

    [JsonPropertyName("organizationId")]
    public long OrganizationId { get; set; }
}