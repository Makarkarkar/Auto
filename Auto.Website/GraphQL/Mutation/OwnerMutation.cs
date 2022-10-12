using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Mutation;

public class OwnerMutation : ObjectGraphType
{
    private readonly IAutoDatabase _db;

    public OwnerMutation(IAutoDatabase db)
    {
        this._db = db;


        Field<OwnerGraphType>(
            "createOwner",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "surname" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "email" },
                new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "age" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "vehicleRegistration" }
            ),
            resolve: context =>
            {
                var name = context.GetArgument<string>("name");
                var surname = context.GetArgument<string>("surname");
                var email = context.GetArgument<string>("email");
                var age = context.GetArgument<int>("age");
                var vehicleRegistration = context.GetArgument<string>("vehicleRegistration");

                var ownerVehicle = db.FindVehicle(vehicleRegistration);
                var owner = new Owner
                {
                    Name = name,
                    Surname = surname,
                    Email = email,
                    Age = age,
                    OwnerVehicle = ownerVehicle,
                    VehicleRegistration = ownerVehicle.Registration
                };
                _db.CreateOwner(owner);
                return owner;
            });
    }
}