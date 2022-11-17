using BeerSender.Domain;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var label1 = new Shipping_label("123456", Carrier.DHL);
            var label2 = new Shipping_label("123456", Carrier.DHL);

            Console.WriteLine(label1);

            Console.WriteLine(label1 == label2);

            var label3 = label1 with { Shipping_code = "234567" };

            Console.WriteLine(label3);
        }
    }
}