using Microsoft.AspNetCore.Mvc;
using MovilidadSostenible_YAMAHA.Data;
using MovilidadSostenible_YAMAHA.DBContext;
using MovilidadSostenible_YAMAHA.Models;
using MovilidadSostenible_YAMAHA.ProxyClient;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Diagnostics;

namespace MovilidadSostenible_YAMAHA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ValidateConnectionBD _connectionService;
        private readonly AmazonSecret _amazonSecret;
        private readonly string _googleMapsApiKey = "AIzaSyCc5VLs74oP4AeInUOLM0d5SL0z9KfV7gQ";
        private readonly ColombiaService _colombiaService;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            ValidateConnectionBD connectionService,
            AmazonSecret amazonSecret,
            ColombiaService colombiaService)
        {
            _logger = logger;
            _context = context;
            _connectionService = connectionService;
            _amazonSecret = amazonSecret;
            _colombiaService = colombiaService;
        }

        public async Task<IActionResult> Index()
        {
            //TODO: Se valida si la conexión a la BD de datos se encuentra correcta. se prueba listando las encuestas.
            var (isConnectionValid, errorMessage) = _connectionService.ValidateConnectionAsync().Result;

            if (isConnectionValid)
            {
                try
                {
                    var items = _context.encuestas.ToList();
                    return View(items);
                }
                catch (Exception ex)
                {
                    errorMessage = $"Error al consultar la base de datos: {ex.Message}";
                }
            }

            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
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

        [HttpPost]
        public async Task<IActionResult> CalculateDistance(string origin, string destination)
        {
            string requestUri = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={_googleMapsApiKey}&language=es";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(requestUri);

            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

            var distance = result.rows[0].elements[0].distance.text;
            var duration = result.rows[0].elements[0].duration.text;

            ViewData["Distance"] = distance;
            ViewData["Duration"] = duration;

            return View("Index");
        }

        private async Task<DistanceResult> GetDistanceAndDuration(string origin, string destination)
        {
            var apiKey = "AIzaSyCc5VLs74oP4AeInUOLM0d5SL0z9KfV7gQ";
            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={Uri.EscapeDataString(origin)}&destination={Uri.EscapeDataString(destination)}&key={apiKey}";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(url);
                var directionsResult = JsonConvert.DeserializeObject<GoogleDirectionsResponse>(response);

                if (directionsResult.Status != "OK" || !directionsResult.Routes.Any())
                {
                    throw new Exception("Error al obtener los datos de la API de Google Maps");
                }

                var route = directionsResult.Routes.First();
                var leg = route.Legs.First();

                return new DistanceResult
                {
                    Origin = origin,
                    Destination = destination,
                    Duration = leg.Duration.Text,
                    Distance = leg.Distance.Text
                };
            }
        }

        #region Deserializacióngooglemaps
        public class GoogleDirectionsResponse
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("routes")]
            public List<Route> Routes { get; set; }
        }

        public class Route
        {
            [JsonProperty("legs")]
            public List<Leg> Legs { get; set; }
        }

        public class Leg
        {
            [JsonProperty("distance")]
            public Distance Distance { get; set; }

            [JsonProperty("duration")]
            public Duration Duration { get; set; }
        }

        public class Distance
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }

        public class Duration
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }

        #endregion

    }
}
