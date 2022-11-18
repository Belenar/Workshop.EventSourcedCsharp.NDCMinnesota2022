using BeerSender.Domain;

namespace BeerSender.Tests;

public class Create_package_test : Beer_sender_test<Beer_package>
{
    [Fact]
    public void Create_package_succeeds()
    {
        var package_id = Guid.NewGuid();

        Given();

        When(
            new Create_package.Command(package_id, new Package_capacity(24))
        );

        Expect(
            new Create_package.Success(package_id, new Package_capacity(24))
        );
    }
}

public class Add_beer_test : Beer_sender_test<Beer_package>
{
    [Fact]
    public void Add_bottle_to_empty_box_succeeds()
    {
        var package_id = Guid.NewGuid();

        Given(
            new Create_package.Success(package_id, new Package_capacity(24))
            );

        When(
            new Add_beer.Command(package_id, new Beer_bottle("Gouden Carolus", "Quadruple Whisky Infused"))
        );

        Expect(
            new Add_beer.Success(package_id, new Beer_bottle("Gouden Carolus", "Quadruple Whisky Infused"))
        );
    }

    [Fact]
    public void Add_bottle_to_full_box_fails()
    {
        var package_id = Guid.NewGuid();

        Given(
            new Create_package.Success(package_id, new Package_capacity(1)),
            new Add_beer.Success(package_id, new Beer_bottle("Gouden Carolus", "Quadruple Whisky Infused"))
        );

        When(
            new Add_beer.Command(package_id, new Beer_bottle("Gouden Carolus", "Quadruple Whisky Infused"))
        );

        Expect(
            new Add_beer.Fail(package_id, new Beer_bottle("Gouden Carolus", "Quadruple Whisky Infused"), Add_beer.Fail_reason.Box_was_full)
        );
    }
}