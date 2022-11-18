using BeerSender.API.Read_models;
using BeerSender.Domain;

namespace BeerSender.API.Projections;

public class Beer_package_projection : IProjection
{
    private readonly Read_context _database;

    public Beer_package_projection(Read_context database)
    {
        _database = database;
    }

    // Events we're interested in
    public Type[] Source_event_types => new[] {
        typeof(Add_beer.Success)
    };


    public void Project(IEvent @event)
    {
        if (@event is Add_beer.Success success)
        {
            var new_beer = new Package_beer
            {
                PackageId = success.package_id,
                Brewery = success.beer.Brewery,
                Beer_name = success.beer.Name
            };
            _database.Package_beers.Add(new_beer);
        }
        
    }

    public void Commit()
    {
        _database.SaveChanges();
    }
}