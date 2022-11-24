using BeerSender.Domain;

namespace BeerSender.API.SqlEventStream.ReadModels;

public interface IProjection
{
    IEnumerable<Type> SourceEventTypes { get; }
    
    void Project(object @event);

    void Commit();
}

public class PackageBeerProjection : IProjection
{
    private readonly ReadContext _readContext;

    public PackageBeerProjection(ReadContext readContext)
    {
        _readContext = readContext;
    }

    public IEnumerable<Type> SourceEventTypes => new[]
    {
        typeof(ShippingLabelAdded)
    };
    
    public void Project(object @event)
    {
        if (@event is not ShippingLabelAdded shippingLabelAdded) return;
        
        var newPackageBeer = new PackageBeer(shippingLabelAdded.BeerPackageId, "my brewery", "my beer name");
        _readContext.PackageBeer.Add(newPackageBeer);
    }

    public void Commit()
    {
        _readContext.SaveChanges();
    }
}