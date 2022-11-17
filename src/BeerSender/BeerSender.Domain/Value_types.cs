namespace BeerSender.Domain;

public record Shipping_label(string Shipping_code, Carrier Carrier) {
    public bool IsValid()
    {
        return Shipping_code.Length > 12;
    }
}

public enum Carrier
{
    UPS,
    FedEx,
    DHL
}

public record Beer_bottle(string Brewery, string Name);
