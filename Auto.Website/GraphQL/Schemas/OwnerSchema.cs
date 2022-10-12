using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.Mutation;
using Auto.Website.GraphQL.Queries;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Schemas;

public class OwnerSchema : Schema
{
    public OwnerSchema(IAutoDatabase db)
    {
        Query = new OwnerQuery(db);
        Mutation = new OwnerMutation(db);
    }
}