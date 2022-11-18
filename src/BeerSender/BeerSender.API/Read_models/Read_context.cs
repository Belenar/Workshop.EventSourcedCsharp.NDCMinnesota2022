using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BeerSender.API.Read_models;

public class Read_context : DbContext
{
    public Read_context(DbContextOptions<Read_context> opt)
    : base(opt)
    { }

    public DbSet<Package_beer> Package_beers { get; set; }
    public DbSet<Projection_checkpoint> Projection_checkpoints { get; set; }
}

public class Package_beer
{
    public int Id { get; set; }
    public Guid PackageId { get; set; }
    public string Brewery { get; set; }
    public string Beer_name { get; set; }
}

public class Projection_checkpoint
{
    [Key]
    public string Projection_name { get; set; }
    public int Event_id { get; set; }
}