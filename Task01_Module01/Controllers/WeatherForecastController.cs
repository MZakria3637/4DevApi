using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task01_Module01.Models;

namespace Task01_Module01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IEmailSender _emailSender;

        //public WeatherForecastController(ILogger<WeatherForecastController> logger, IEmailSender emailSender)
        //{
        //    _logger = logger;
        //    _emailSender = emailSender;
        //}
        public WeatherForecastController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var rng = new Random();
            //int _min = 1000;
            //int _max = 9999;
            //Random _rdm = new Random();
            //int code = _rdm.Next(_min, _max);
            //String Content = "Hi, \n\n" +
            //    "Please Enter this Code to Verify \n" + "Code : " + code + 
            //    "\n\n\nWarm Regards \n4Material.com";
            //var message = new Message(new string[] { "testEmail4material@gmail.com" }, "Verify Email Address", Content);
            ////_emailSender.SendEmail(message);

            //await _emailSender.SendEmailAsync(message);


            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
