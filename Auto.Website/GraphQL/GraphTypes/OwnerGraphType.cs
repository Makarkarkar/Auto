using Auto.Data.Entities;
using GraphQL.Types;

namespace Auto.Website.GraphQL.GraphTypes;

public class OwnerGraphType:ObjectGraphType<Owner>
{
    public OwnerGraphType() {
        Name = "owner";
        Field(o => o.OwnerVehicle, nullable: false, type: typeof(VehicleGraphType))
            .Description("Автомобиль владельца");
        Field(o => o.Name);
        Field(o => o.Surname);
        Field(o => o.Age);
        Field(o => o.Email);
        Field(o => o.VehicleRegistration);
        
    }
}