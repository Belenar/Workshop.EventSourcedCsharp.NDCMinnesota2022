using BeerSender.Domain;

namespace BeerSender.Tests;

public class CreatePackageTest : BeerSenderTest
{
    [Fact]
    public void Create_Package_Succeeds()
    {
        var packageId = Guid.NewGuid();
        
        Given();
        
        When(new CreatePackage(packageId));
        
        Expect(new PackageCreated(packageId));
    }
}