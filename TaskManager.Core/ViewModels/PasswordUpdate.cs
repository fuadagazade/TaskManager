using Azersun.Audit.Utilities;
using System.Text.Json.Serialization;

namespace TaskManager.Core.ViewModels;

public class PasswordUpdate
{
    [JsonPropertyName("current")]
    public string Current { get; set; }


    [JsonPropertyName("changed")]
    public string Changed { get; set; }

    public string CurrentEncrypted
    {
        get
        {
            return Cryptographer.Encrypt(Current);
        }
    }

    public string ChangedEncrypted
    {
        get
        {
            return Cryptographer.Encrypt(Changed);
        }
    }
}

