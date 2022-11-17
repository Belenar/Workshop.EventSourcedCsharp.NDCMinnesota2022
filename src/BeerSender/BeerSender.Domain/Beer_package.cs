namespace BeerSender.Domain;

internal class Beer_package
{
    public Guid Package_id { get; private set; }
    public Package_capacity Capacity { get; private set; }
    public List<Beer_bottle> Contents { get; } = new();


    public void Create(Guid package_id, Package_capacity package_capacity)
    {
        Package_id = package_id;
        Capacity = package_capacity;
    }

    public void Add_beer(Beer_bottle beer)
    {
        Contents.Add(beer);
    }
}

