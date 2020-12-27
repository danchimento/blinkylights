using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

Config config;
using (StreamReader r = new StreamReader("blinkconfig.json"))
{
    string json = r.ReadToEnd();
    config = JsonConvert.DeserializeObject<Config>(json);
}

var ports = SerialPort.GetPortNames();
Console.WriteLine("Select a port:");
for (var i = 0; i < ports.Length; i++)
{
    Console.WriteLine($"{i}: {ports[i]}");
}

var portNumber = int.Parse(Console.ReadLine());
var portName = ports[portNumber];
var port = new SerialPort(portName);
var authClient = new HttpClient();
var healthClient = new HttpClient();

async Task<string> GetToken()
{
    var nvc = new List<KeyValuePair<string, string>>();
    nvc.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
    nvc.Add(new KeyValuePair<string, string>("scope", "diagnostics"));
    nvc.Add(new KeyValuePair<string, string>("client_id", config.ClientId));
    nvc.Add(new KeyValuePair<string, string>("client_secret", config.ClientSecret));

    var result = await authClient.PostAsync(config.AuthUrl,
    new FormUrlEncodedContent(nvc));

    if (!result.IsSuccessStatusCode)
    {
        Console.WriteLine($"Failed to get token: {result.StatusCode}: {result.ReasonPhrase}");
        return null;
    }

    var body = await result.Content.ReadAsStringAsync();
    var auth = JsonConvert.DeserializeObject<Auth>(body);

    return auth.access_token;
}

async Task<List<Health>> GetHealth(string token)
{
    healthClient.DefaultRequestHeaders.Remove("Authorization");
    healthClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    var result = await healthClient.GetAsync(config.HealthUrl);

    if (!result.IsSuccessStatusCode)
    {
        Console.WriteLine($"Failed to get health: {result.StatusCode}: {result.ReasonPhrase}");
        return null;
    }

    var body = await result.Content.ReadAsStringAsync();
    var health = JsonConvert.DeserializeObject<List<Health>>(body);

    return health;
}

try
{
    port.Open();

    while (true)
    {
        Thread.Sleep(config.DelayBetweenHealthChecks);
        Console.WriteLine("\n\nStarting check...");

        Console.WriteLine("Getting auth token...");
        var token = await GetToken();
        if (string.IsNullOrEmpty(token))
        {
            continue;
        }

        Console.WriteLine("Getting system health...");
        var health = await GetHealth(token);
        health = health.Where(h => config.Mapping.ContainsKey(h.dependencyName)).ToList();

        if (health == null)
        {
            continue;
        }

        foreach (var item in health)
        {
            Console.WriteLine($"{item.dependencyName} is {item.status}");
            var pin = config.Mapping[item.dependencyName];
            var val = item.status == "AVAILABLE" ? 0 : 1;
            var write = $"[[{pin},{val}]]";
            var lightStatus = val == 1 ? "ON" : "OFF";

            Console.WriteLine($"Turning {lightStatus} light #{pin}...");

            port.Write(write);

            // Wait for a response fro the Arduino
            var read = port.ReadLine();
        }
    }
}
catch(Exception e) {
    Console.WriteLine($"ERROR: {e.Message}");
    Console.WriteLine("Please restart the app.");
}
finally
{
    port.Close();
}