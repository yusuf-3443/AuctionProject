using AngleSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

public class CarParserService
{
    public async Task<List<Car>> ParseCarsFromUrl(string url)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(url);

        var cars = new List<Car>();
        var carElements = document.QuerySelectorAll(".announcement-content-container"); // Пример селектора

        foreach (var element in carElements)
        {
            var model = element.QuerySelector(".title-announcement").TextContent.Trim();
            // var year = element.QuerySelector(".year").TextContent.Trim();
            // var price = element.QuerySelector(".price").TextContent.Trim();
            // var mileage = element.QuerySelector(".mileage").TextContent.Trim();

            var car = new Car
            {
                Model = model,
                // Year = int.Parse(year),
                // Price = decimal.Parse(price),
                // Mileage = int.Parse(mileage)
            };

            cars.Add(car);
        }

        return cars;
    }
}
