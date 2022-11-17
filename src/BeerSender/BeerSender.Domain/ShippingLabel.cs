namespace BeerSender.Domain;

public record ShippingLabel(string ShippingCode, Carrier Carrier)
{
    // Not in CTOR because you want to replay it from a past event!
    public bool IsValid()
    {
        return ShippingCode.Length > 12;
    }
}

public enum Carrier
{
    UPS,
    FedEx,
    DHL
}

public record BeerBottle(string Brewery, string Name);