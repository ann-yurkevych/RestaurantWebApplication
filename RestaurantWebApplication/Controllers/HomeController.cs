using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestaurantWebApplication.Models;
using RestSharp;
using System.Diagnostics;
using System.Net;
using RestaurantWebApplication.RabbitMQ;

namespace RestaurantWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRabbitMqService _rabbitMqService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _rabbitMqService = new RabbitMQService();
        }

        public ActionResult GoogleLoginCallback(string code)
        {
            if (code != null)
            {
                var client = new RestClient("https://www.googleapis.com/oauth2/v3/token");
                var request = new RestRequest(Method.POST);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("code", code);
                request.AddParameter("redirect_uri", "https://localhost:7293/Home/GoogleLoginCallback");

                request.AddParameter("client_id", "Enter your clientid here");
                request.AddParameter("client_secret", "Enter your client secret");

                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var res = (JObject)JsonConvert.DeserializeObject(content);
                var client2 = new RestClient("https://www.googleapis.com/oauth2/v1/userinfo");
                client2.AddDefaultHeader("Authorization", "Bearer " + res["access_token"]);

                request = new RestRequest(Method.GET);


                var response2 = client2.Execute(request);

                var content2 = response2.Content;

                var user = (JObject)JsonConvert.DeserializeObject(content2);
                return RedirectToAction("Index", "Home", new { loggedIn = true });

            }
            else
            {
                ViewBag.ReturnData = "";
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index(bool? loggedIn)
        {
            if (loggedIn == true)
            {
                ViewBag.Logged = "Logged In!";
            }
            else
            {
                ViewBag.Logged = "Not logged In";
            }
            _rabbitMqService.SendMessage(ViewBag.Logged);
            return View();
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