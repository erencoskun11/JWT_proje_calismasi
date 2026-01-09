using Microsoft.AspNetCore.Mvc;

namespace MiniApp1.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
            "Humid", "Dry", "Windy", "Rainy", "Snowy", "Stormy", "Sunny", "Cloudy", "Foggy", "Hazy"
        };

        private static readonly string[] Locations = new[]
        {
            "London", "Paris", "New York", "Tokyo", "Istanbul", "Berlin", "Moscow", "Beijing", "Sydney", "Toronto"
        };

        private static List<WeatherForecast> _forecasts = new List<WeatherForecast>();
        private readonly ILogger<WeatherForecastController> _logger;

        static WeatherForecastController()
        {
            var rng = new Random();
            _forecasts = Enumerable.Range(1, 20).Select(index => new WeatherForecast
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                Location = Locations[rng.Next(Locations.Length)],
                Humidity = rng.Next(0, 100),
                WindSpeed = rng.Next(0, 100)
            }).ToList();
        }

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult<IEnumerable<WeatherForecast>> Get([FromQuery] string? location, [FromQuery] int? minTemp)
        {
            var query = _forecasts.AsQueryable();

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(f => f.Location != null && f.Location.Contains(location, StringComparison.OrdinalIgnoreCase));
            }

            if (minTemp.HasValue)
            {
                query = query.Where(f => f.TemperatureC >= minTemp.Value);
            }

            var result = query.ToList();

            if (result.Count == 0)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<WeatherForecast> GetById(Guid id)
        {
            var forecast = _forecasts.FirstOrDefault(f => f.Id == id);
            if (forecast == null)
            {
                return NotFound();
            }
            return Ok(forecast);
        }

        [HttpPost]
        public ActionResult<WeatherForecast> Create([FromBody] WeatherForecast forecast)
        {
            if (forecast == null)
            {
                return BadRequest();
            }

            if (forecast.TemperatureC < -100 || forecast.TemperatureC > 100)
            {
                return BadRequest("Temperature is out of realistic range.");
            }

            forecast.Id = Guid.NewGuid();

            if (string.IsNullOrEmpty(forecast.Summary))
            {
                forecast.Summary = "Unknown";
            }

            if (string.IsNullOrEmpty(forecast.Location))
            {
                forecast.Location = "General";
            }

            _forecasts.Add(forecast);
            return CreatedAtAction(nameof(GetById), new { id = forecast.Id }, forecast);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] WeatherForecast updatedForecast)
        {
            var existingForecast = _forecasts.FirstOrDefault(f => f.Id == id);
            if (existingForecast == null)
            {
                return NotFound();
            }

            existingForecast.Date = updatedForecast.Date;
            existingForecast.TemperatureC = updatedForecast.TemperatureC;
            existingForecast.Summary = updatedForecast.Summary;
            existingForecast.Location = updatedForecast.Location;
            existingForecast.Humidity = updatedForecast.Humidity;
            existingForecast.WindSpeed = updatedForecast.WindSpeed;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var forecast = _forecasts.FirstOrDefault(f => f.Id == id);
            if (forecast == null)
            {
                return NotFound();
            }

            _forecasts.Remove(forecast);
            return NoContent();
        }

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            if (!_forecasts.Any())
            {
                return NotFound("No data available for stats.");
            }

            var stats = new
            {
                Count = _forecasts.Count,
                AverageTemp = _forecasts.Average(f => f.TemperatureC),
                MaxTemp = _forecasts.Max(f => f.TemperatureC),
                MinTemp = _forecasts.Min(f => f.TemperatureC),
                HottestLocation = _forecasts.OrderByDescending(f => f.TemperatureC).First().Location
            };

            return Ok(stats);
        }
    }
}

namespace MiniApp1.API
{
    public class WeatherForecast
    {
        public Guid Id { get; set; }

        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }

        public string? Location { get; set; }

        public int Humidity { get; set; }

        public int WindSpeed { get; set; }

        public string GetFullDescription()
        {
            return $"{Date}: {Location} is {Summary}, {TemperatureC}°C ({TemperatureF}°F) with {Humidity}% humidity and wind speeds of {WindSpeed} km/h.";
        }

        public bool IsSevereWeather()
        {
            return TemperatureC > 40 || TemperatureC < -10 || WindSpeed > 80;
        }

        public void ResetData()
        {
            TemperatureC = 0;
            Humidity = 50;
            WindSpeed = 10;
            Summary = "Reset";
            Location = "Unknown";
        }
    }
}