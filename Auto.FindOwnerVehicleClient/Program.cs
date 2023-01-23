using System.Runtime.CompilerServices;
using Auto.FindVehicleEngine;
using Auto.Messages;
using EasyNetQ;
using Grpc.Net.Client;

namespace Autobarn.PricingClient
{
    class Program
    {
        private static IBus bus;
        private static FindVehicle.FindVehicleClient grpcClient;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting FindVehicle gRPC service");
            var amqp = "amqp://user:rabbitmq@localhost:5672";
            bus = RabbitHutch.CreateBus(amqp);
            Console.WriteLine("Connected to bus; Listening for newOwnerMessages");
            var grpcAddress = "http://localhost:5220";
            using var channel = GrpcChannel.ForAddress(grpcAddress);
            grpcClient = new FindVehicle.FindVehicleClient(channel);
            Console.WriteLine($"Connected to gRPC on {grpcAddress}!");
            var subscriberId = $"Auto.FindOwnerVehicle@{Environment.MachineName}";
            await bus.PubSub.SubscribeAsync<NewOwnerMessage>(subscriberId, HandleNewOwnerMessage);
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        private static async Task HandleNewOwnerMessage(NewOwnerMessage message)
        {
            //Console.WriteLine($"new owner: {message.Name}");
            var request = new FindVehicleRequest()
            {
                VehicleRegistration = message.VehicleRegistration
            };
            var reply = await grpcClient.GetVehicleAsync(request);
            //var NOVMessage = new NewOwnerVehicleMessage(message, reply.ModelCode, reply.Year, reply.Registration);
            var NOVMessage = new NewOwnerVehicleMessage(
                message, reply.ModelCode, reply.Year, reply.Registration);
            await bus.PubSub.PublishAsync(NOVMessage);
            Console.WriteLine($"Owner {message.Name},{message.Surname} " +
                              $"with {reply.ModelCode} added");
            
        }
    }
}