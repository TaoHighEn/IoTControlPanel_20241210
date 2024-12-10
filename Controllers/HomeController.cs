using IoTControlPanel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IoTControlPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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

        [HttpPost]
        public void InsertData([FromBody] Data data)
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                var input = new test()
                {
                    GUID = Guid.NewGuid(),
                    TEMP = data.TEMP.ToString(),
                    HUMI = data.HUMI.ToString()
                };
                context.test.Add(input);
                context.SaveChanges();
            }
        }

        public ActionResult SelectData()
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                return View(context.test.ToList());
            }
        }
    }
    public class Data
    {
        public float TEMP { get; set; }
        public float HUMI { get; set; }
    }
}
