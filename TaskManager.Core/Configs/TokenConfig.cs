namespace TaskManager.Core.Configs;

public class TokenConfig
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int Expires { get; set; }
}

