using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BeerSender.API.SqlEventStream.ReadModels;

public class ReadContext : DbContext
{
    public DbSet<PackageBeer> PackageBeer { get; private set; } = null!;

    public DbSet<ProjectionCheckpoint> ProjectionCheckpoint { get; private set; } = null!;

    public ReadContext(DbContextOptions<ReadContext> options) : base(options)
    {
        
    }
}

public class PackageBeer
{
    public int Id { get; private set; }
    
    public Guid PackageId { get; private set; }

    public string Brewery { get; private set; }

    public string Name { get; private set; }

    public PackageBeer(Guid packageId, string brewery, string name)
    {
        PackageId = packageId;
        Brewery = brewery;
        Name = name;
    }

    private PackageBeer()
    {
        
    }
}

public class ProjectionCheckpoint
{
    [Key]
    public string ProjectionName { get; private set; }

    public int EventId { get; private set; }

    public ProjectionCheckpoint(string projectionName, int eventId)
    {
        ProjectionName = projectionName;
        EventId = eventId;
    }

    public void UpdateEventId(int eventId)
    {
        EventId = eventId;
    }
}