namespace BeerSender.Domain;

internal class Beer_package
{
    private Guid _package_id;

    public void Create(Guid package_id)
    {
        _package_id = package_id;
    }
}

