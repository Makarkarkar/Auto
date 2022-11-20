using Auto.FindVehicleEngine;
using Grpc.Net.Client;
using var channel = GrpcChannel.ForAddress("http://localhost:5220");
var grpcClient = new FindVehicle.FindVehicleClient(channel);
Console.WriteLine("Ready! Press any key to send a gRPC request (or Ctrl-C to quit):");
while (true) {
    Console.ReadKey(true);
    var request = new FindVehicleRequest {
        Email = "jennings33@hotmail.com"
    };
    var reply = grpcClient.GetVehicle(request);
    Console.WriteLine($"ModelCode: {reply.ModelCode}\n" +
                      $"Year: {reply.Year}\n" +
                      $"Registration: {reply.Registration}");
}