using Azersun.Audit.Utilities;
using System.Text.Json.Serialization;

namespace TaskManager.Core.Models;
public class Authentication
{
    private string organization;
    private string email;
    private string password;


    [JsonPropertyName("organization")]
    public string Organization { get { return organization.ToLower(); } set { organization = value.ToLower(); } }

    [JsonPropertyName("email")]
    public string Email { get { return email.ToLower(); } set { email = value.ToLower(); } }

    [JsonPropertyName("password")]
    public string Password { get { return password; } set { password = value; } }

    public string Encrypted { get { return Cryptographer.Encrypt(password); } }
}
