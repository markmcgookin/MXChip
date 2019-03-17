using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IotDatabaseContext Database;
        private const string HUMIDITY = "humidity";
        private const string TEMPERATURE = "temperature";
        public HomeController(IotDatabaseContext database)
        {
            Database = database;
        }
        public IActionResult Index()
        {
            var humidityData = Database.SensorData.OrderByDescending(x => x.Modified).FirstOrDefault(x => x.Type == HUMIDITY);
            var tempData = Database.SensorData.OrderByDescending(x => x.Modified).FirstOrDefault(x => x.Type == TEMPERATURE);
            var viewModel = new IndexViewModel
            {
                Temperature = tempData.Value,
                Humidity = humidityData.Value
            };
            
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
