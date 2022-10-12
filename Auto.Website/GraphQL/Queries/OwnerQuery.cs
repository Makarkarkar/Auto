using System;
using System.Collections.Generic;
using System.Linq;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace Auto.Website.GraphQL.Queries;

public class OwnerQuery : ObjectGraphType
{
    private readonly IAutoDatabase db;

    public OwnerQuery(IAutoDatabase db)
    {
        this.db = db;

        Field<ListGraphType<OwnerGraphType>>("Owners", "Get all owners",
            resolve: GetAllOwners);
        
        Field<OwnerGraphType>("Owner", "Get owner by email",
            new QueryArguments(MakeNonNullStringArgument("email", "Owner's email")),
            resolve: GetOwner);
        
        Field<ListGraphType<OwnerGraphType>>("OwnersByName", "Get owners by name",
            new QueryArguments(MakeNonNullStringArgument("name", "Owner's name")),
            resolve: GetOwnersByName);
    }

    private QueryArgument MakeNonNullStringArgument(string name, string description) {
        return new QueryArgument<NonNullGraphType<StringGraphType>> {
            Name = name, Description = description
        };
    }
    
    private IEnumerable<Owner> GetAllOwners(IResolveFieldContext<object> arg) => db.ListOwners();
    private Owner GetOwner(IResolveFieldContext<object> arg)
    {
        var email = arg.GetArgument<string>("email");
        return db.FindOwner(email);
    }

    private IEnumerable<Owner> GetOwnersByName(IResolveFieldContext<object> arg)
    {
        var name = arg.GetArgument<string>("name");
        var owner = db.ListOwners().Where(o => o.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));
        return owner;
    }

    
}