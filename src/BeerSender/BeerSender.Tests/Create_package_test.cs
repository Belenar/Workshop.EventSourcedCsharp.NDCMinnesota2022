using BeerSender.Domain;

namespace BeerSender.Tests;

public class Create_package_test : Beer_sender_test
{
    [Fact]
    public void Create_package_succeeds()
    {
        var package_id = Guid.NewGuid();

        Given();

        When(
            new Create_package(package_id, new Package_capacity(24))
        );

        Expect(
            new Package_created(package_id, new Package_capacity(24))
        );
    }
}

public class Add_beer_test : Beer_sender_test
{
    [Fact]
    public void Add_bottle_to_empty_box_succeeds()
    {
        var package_id = Guid.NewGuid();

        Given(
            new Package_created(package_id, new Package_capacity(24))
            );

        When(
            new Add_beer(package_id, new Beer_bottle("Gouden Carolus", "Quadruple Whisky Infused"))
        );

        Expect(
            new Beer_added(package_id, new Beer_bottle("Gouden Carolus", "Quadruple Whisky Infused"))
        );
    }

    [Fact]
    public void Add_bottle_to_full_box_fails()
    {
        var package_id = Guid.NewGuid();

        Given(
            new Package_created(package_id, new Package_capacity(1)),
            new Beer_added(package_id, new Beer_bottle("Gouden Carolus", "Quadruple Whisky Infused"))
        );

        When(
            new Add_beer(package_id, new Beer_bottle("Gouden Carolus", "Quadruple Whisky Infused"))
        );

        Expect(
            new Beer_failed_to_add(package_id, new Beer_bottle("Gouden Carolus", "Quadruple Whisky Infused"), Fail_reason.Box_was_full)
        );
    }
}