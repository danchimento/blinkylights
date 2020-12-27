using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using Newtonsoft.Json;

var pinMappings = new Dictionary<string, int>
{
    { "A", 1 },
    { "B", 2 },
    { "C", 3 },
};

var ports = SerialPort.GetPortNames();
Console.WriteLine("Select a port:");
for (var i = 0; i < ports.Length; i++)
{
    Console.WriteLine($"{i}: {ports[i]}");
}

var portNumber = int.Parse(Console.ReadLine());
var portName = ports[portNumber];
var port = new SerialPort(portName);

port.Open();

try
{
    while (true)
    {
        List<Result> items;

        using (StreamReader r = new StreamReader("app/file.json"))
        {
            string json = r.ReadToEnd();
            items = JsonConvert.DeserializeObject<List<Result>>(json);
        }

        foreach (var item in items)
        {
            var pin = pinMappings[item.Name];
            var val = item.Value == "ON" ? 1 : 0;
            var write = $"[[{pin},{val}]]";
            
            port.WriteLine(write);
            Thread.Sleep(2000);
        }
    }
}
finally
{
    port.Close();
}
