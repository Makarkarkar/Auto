using Auto.Data;
using Auto.FindVehicleEngine;
using Grpc.Core;
namespace Auto.PricingServer.Services;

public class FindVehicleService : FindVehicle.FindVehicleBase {
    private readonly ILogger<FindVehicleService> logger;
    private readonly IAutoDatabase db;
    
    public FindVehicleService(ILogger<FindVehicleService> logger, IAutoDatabase db) {
        this.logger = logger;
        this.db = db;
    }
    public override Task<FindVehicleReply> GetVehicle(FindVehicleRequest request, ServerCallContext context)
    {
        var vehicle = db.FindVehicle(request.VehicleRegistration);
        Console.WriteLine("request being processed...");
        return Task.FromResult(new FindVehicleReply() { ModelCode = vehicle.ModelCode, Year = vehicle.Year, Registration = vehicle.Registration});
        
    }
}