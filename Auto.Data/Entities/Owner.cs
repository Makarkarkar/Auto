using Newtonsoft.Json;

namespace Auto.Data.Entities;

public class Owner
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }

    public string VehicleRegistration { get; set; }
    public virtual Vehicle OwnerVehicle { get; set; }
}