﻿// See https://aka.ms/new-console-template for more information

using BeerSender.Domain;

var label1 = new ShippingLabel("123456", Carrier.DHL);
var label2 = new ShippingLabel("123456", Carrier.DHL);

Console.WriteLine(label1);

Console.WriteLine(label1 == label2);

var label3 = label1 with {ShippingCode = "234567"};

Console.WriteLine(label3);


Console.ReadKey();