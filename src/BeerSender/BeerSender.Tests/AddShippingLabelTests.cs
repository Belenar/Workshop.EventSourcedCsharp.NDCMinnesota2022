using BeerSender.Domain;

namespace BeerSender.Tests;

public class AddShippingLabelTests : BeerSenderTest
{
    [Fact]
    public void Invalid_Shipping_Label_Emits_Correct_Event()
    {
        var packageId = Guid.NewGuid();
        var invalidShippingLabel = new ShippingLabel("1", Carrier.DHL);
       
        Given();
        
        When(new AddShippingLabel(packageId, invalidShippingLabel));
        
        Expect(new ShippingLabelFailedToAdd(packageId, invalidShippingLabel, ShippingLabelFailure.InvalidLabel));
    }

    [Fact]
    public void Valid_Shipping_Label_Emits_Correct_Event()
    {
        var packageId = Guid.NewGuid();
        var shippingLabel = new ShippingLabel("1234567890123", Carrier.FedEx);
        
        Given();

        When(new AddShippingLabel(packageId, shippingLabel));
        
        Expect(new ShippingLabelAdded(packageId, shippingLabel));
    }
}