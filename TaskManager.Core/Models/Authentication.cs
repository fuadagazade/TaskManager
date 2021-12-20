using System.Text.Json.Serialization;

namespace TaskManager.Core.Models;
public class Authentication
{
    [JsonPropertyName("organization")]
    public string Organization { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }
}
