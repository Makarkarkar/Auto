using System.Text.Json;
using Auto.Messages;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Auto.Notifier
{ 
 internal class Program
 {
  private const string SIGNALR_HUB_URL = "https://localhost:7041/hub";
  private static HubConnection hub;

  static async Task Main(string[] args)
  {
   hub = new HubConnectionBuilder().WithUrl(SIGNALR_HUB_URL).Build();
   await hub.StartAsync();
   Console.WriteLine("Hub started!");
   //Console.WriteLine("Press any key to send a message (Ctrl-C to quit)");
   var amqp = "amqp://user:rabbitmq@localhost:5672";
   using var bus = RabbitHutch.CreateBus(amqp);
   Console.WriteLine("Connected to bus! Listening for newOwnerMessages");
   var subscriberId = $"Auto.Notifier@{Environment.MachineName}";
   await bus.PubSub.SubscribeAsync<NewOwnerVehicleMessage>(subscriberId, HandleNewVehicleMessage);
   Console.ReadLine();
  }

  private static async Task HandleNewVehicleMessage(NewOwnerVehicleMessage novMessage)
  {
   var csvRow = $"{novMessage.ModelCode} {novMessage.Year} {novMessage.Registration} : " +
                $"{novMessage.Name}, " +
                $"{novMessage.Surname}, " +
                $"{novMessage.Age}, " +
                $"{novMessage.Email}";
   Console.WriteLine(csvRow);
   var json = JsonSerializer.Serialize(novMessage, JsonSettings());
   await hub.SendAsync("NotifyWebUsers", "Auto.Notifier",
   json);
  }
  

  // private static async void HandleNewVehicleMessage(NewOwnerMessage now)
  // {
  //  var csvRow =
  //   $"{now.Name}," +
  //   $"{now.Surname}," +
  //   $"{now.Email}," +
  //   $"{now.VehicleRegistration}," +
  //   $"{now.Age}";
  //  Console.WriteLine(csvRow);
  //  var json = JsonSerializer.Serialize(now, JsonSettings());
  //  await hub.SendAsync("NotifyWebUsers", "Auto.Notifier",
  //   json);
  // }
  static JsonSerializerOptions JsonSettings() =>
   new JsonSerializerOptions
   {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
   };
 }
}

