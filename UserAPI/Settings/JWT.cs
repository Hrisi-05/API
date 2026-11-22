namespace UserAPI.Settings
{
    public class JWT
    {
     public string? Key { get; set; }
     public string? Issuer { get; set; } = "https://localhost:7016";
     public string? Audience { get; set; } = "Clients";
     public int ExpireMinutes { get; set; } = 30;
    }
}
