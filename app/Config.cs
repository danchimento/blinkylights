using System.Collections.Generic;

public class Config
{
    public string AuthUrl { get; set; }

    public string HealthUrl { get; set; }

    public string ClientSecret { get; set; }

    public string ClientId { get; set; }

    public Dictionary<string, int> Mapping { get; set; }

    public int DelayBetweenHealthChecks { get; set; }
}