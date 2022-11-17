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
            new Create_package(package_id)
        );

        Expect(
            new Package_created(package_id)
        );
    }
}