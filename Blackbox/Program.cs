/*Programmet ska ta emot två argument:
Det första är sökvägen till XML-filen som ska läsas
Det andra är sökvägen till CSV-filen som ska skapas*/

using Laboration.DTOS;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.Serialization;

internal class Program
{
    private static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Användning: Laboration <xml-fil> <csv-fil>");
            return 1;
        }

        var inputfile = args[0];
        var outputfile = args[1];

        if (!File.Exists(inputfile))
        {
            Console.Error.WriteLine($"Filen {inputfile} finns inte");
            return 1;
        }

        if (!Path.GetExtension(outputfile).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine($"{outputfile} måste vara en .csv-fil");
            return 1;
        }

        try
        {
            var order = ReadXml(inputfile);
            WriteCsv(outputfile, order);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Något gick fel: {e.Message}");
            return 1;
        }

        return 0;
    }

    private static Order ReadXml(string xmlFilePath)
    {
        using var reader = new StreamReader(xmlFilePath);
        var document = XDocument.Load(reader);
        MoveStrayElementsIntoOrderLines(document);

        var serializer = new XmlSerializer(typeof(Order));
        var order = (Order)serializer.Deserialize(document.Root!.CreateReader())!;
        return order;
    }

    // Element som hamnat direkt under <Items> (utanför en <OrderLine>) flyttas in i
    // närmast föregående <OrderLine>, annars hoppar XmlSerializer tyst över dem
    private static void MoveStrayElementsIntoOrderLines(XDocument document)
    {
        var items = document.Root?.Element("Items");
        if (items == null)
            return;

        foreach (var stray in items.Elements().Where(e => e.Name != "OrderLine").ToList())
        {
            var previousLine = stray.ElementsBeforeSelf("OrderLine").LastOrDefault();
            if (previousLine == null)
                continue;

            stray.Remove();
            previousLine.Add(stray);
        }
    }

    //skriver orderdatan till csv-fil
    private static void WriteCsv(string csvFilePath, Order order)
    {
        using var writer = new StreamWriter(csvFilePath);
        //skriver rubrikraden
        writer.WriteLine("Order,Datum,Gata,Postnr,Stad,Land,Produkt,Material,Beskrivning,Antal,Styckpris");

        //formaterar orderId
        var orderId = FormatOrderId(order.OrderId);
        var date = order.OrderDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        //i första används ShippingAdress men om den är null används BillingAdress
        var street = order.ShippingAddress?.Street ?? order.BillingAddress.Street;
        var postcode = order.ShippingAddress?.PostalCode ?? order.BillingAddress.PostalCode;
        var city = order.ShippingAddress?.City ?? order.BillingAddress.City;
        var country = order.ShippingAddress?.Country ?? order.BillingAddress.Country;

        foreach (var line in order.Items)
        {
            var product = (string?)GetValue(line, ItemsChoiceType.Product);
            var material = (string?)GetValue(line, ItemsChoiceType.Material);
            var description = (string?)GetValue(line, ItemsChoiceType.CustomDescription);
            var quantity = (ushort)GetValue(line, ItemsChoiceType.Quantity)!;
            var unitPrice = (decimal)GetValue(line, ItemsChoiceType.UnitPrice)!;
            //samlar alla värden som text, kommatecken används som decimaltecken på unitprice oavsett datorns språk.
            var fields = new[]
            {
                orderId, date, street, postcode, city, country,
                product, material, description,
                quantity.ToString(),
                unitPrice.ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ','),
            };
            //joinar ihop värdena och hanterar kommatecken och dubbelfnuttar i alla fält
            var csvLine = string.Join(',', fields.Select(Escape));
            writer.WriteLine(csvLine);


        }
    }

    // Hämtar värdet för ett visst element på orderraden, eller null om det saknas
    private static object? GetValue(OrderOrderLine line, ItemsChoiceType type)
    {
        var index = Array.IndexOf(line.ItemsElementName, type);
        return index >= 0 ? line.Items[index] : null;
    }


    // "O1068197859" blir "106 819 785 9"
    private static string FormatOrderId(string orderId)
    {
        var digits = orderId.TrimStart('O');
        return string.Join(" ", digits.Chunk(3).Select(chunk => new string(chunk)));
    }

    // Sätter citattecken runt värden som innehåller kommatecken eller citattecken
    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        if (value.Contains(',') || value.Contains('"'))
            return $"\"{value.Replace("\"", "\"\"")}\"";

        return value;
    }
}