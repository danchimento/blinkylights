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

SerialPort port = new SerialPort();
var ports = SerialPort.GetPortNames();
Console.WriteLine("\n\n======= BLINKY LIGHTS ========");
Console.WriteLine("\n\nSearching for blinky lights box...");
for (var i = 0; i < ports.Length; i++)
{
    Console.WriteLine($"Attempting to find blinky lights on port {ports[i]}");

    var possiblePort = new SerialPort(ports[i]);
    try {
        possiblePort.Open();
        Thread.Sleep(2000);
        possiblePort.Write("BLINKY");
        possiblePort.ReadTimeout = 3000;
        var response = possiblePort.ReadLine();
        if (response == "LIGHTS")
        {
            Console.WriteLine($"Found blinky lights on port {ports[i]}");
            port = possiblePort;
            break;
        } 
        else 
        {
            Console.WriteLine($"Port {ports[i]} isn't blinky lights");
        }
    } 
    catch (Exception e)
    {
        Console.WriteLine($"Error: {e.Message}");
        continue;
    } 
    finally
    {
        possiblePort.Close();
    }
}

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