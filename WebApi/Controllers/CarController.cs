using Domain.Models;
using HtmlAgilityPack;
using Infrastructure.Services.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CarParserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController(DataContext dataContext) : ControllerBase
    {
        // HTTP GET метод для парсинга сайта
        [HttpGet("parse")]
        public async Task<IActionResult> ParseCars()
        {
            string url = "https://somon.tj/"; // Замените на реальный URL сайта

            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);

            // Загружаем HTML-контент страницы
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // XPath-запрос для поиска элементов автомобиля
            var carNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='announcement-content-container']"); // Найдите контейнеры для каждой машины

            var cars = new List<Car>();

            if (carNodes != null)
            {
                foreach (var carNode in carNodes)
                {
                    // Парсинг полей автомобиля
                    var model = carNode.SelectSingleNode(".//h1[@class='title-announcement']").InnerText.Trim();
                    // var make = carNode.SelectSingleNode(".//span[@class='make']").InnerText.Trim();
                    // var year = carNode.SelectSingleNode(".//span[@class='year']").InnerText.Trim();
                    // var mileage = carNode.SelectSingleNode(".//span[@class='mileage']").InnerText.Trim();
                    // var imageUrl = carNode.SelectSingleNode(".//img[@class='car-image']").GetAttributeValue("src", string.Empty);

                    // Добавление данных автомобиля в список
                    var car = new Car
                    {
                        Model = model,
                        // Brand = make,
                        // Year = int.Parse(year),
                        // Mileage = int.Parse(mileage),
                        // Photo = imageUrl
                    };
                    dataContext.Cars.Add(car);
                    cars.Add(car);
                    await dataContext.SaveChangesAsync();
                }
                // Возвращаем список автомобилей в формате JSON
                return Ok(cars);
            }
            else
            {
                return NotFound("Не удалось найти информацию о машинах.");
            }
        }
    }
}


