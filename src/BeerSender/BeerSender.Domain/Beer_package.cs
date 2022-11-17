namespace BeerSender.Domain;

internal class Beer_package
{
    private Guid package_id;

    public IEnumerable<object> Handle_command(object command)
    {
        switch (command)
        {
            case Create_package create_command:
                return Create(create_command);
        }

        return Enumerable.Empty<object>();
    }

    private IEnumerable<object> Create(Create_package create_command)
    {
        yield return new Package_created(create_command.Package_id);
    }

    public void Apply(object @event)
    {
        switch (@event)
        {
            case Package_created created_event:
                Created(created_event);
                break;
        }
    }

    private void Created(Package_created created_event)
    {
        package_id = created_event.Package_id;
    }
}

