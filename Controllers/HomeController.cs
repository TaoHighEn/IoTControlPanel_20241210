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

        public string InsertData()
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                var data = new TempLog()
                {
                    GUID = Guid.NewGuid().ToString(),
                    Time = DateTime.Now.ToString(),
                    TEMP = "123456",
                    HUMI = "444444"
                };
                context.TempLog.Add(data);
                context.SaveChanges();
            }
            return "200";
        }

        [HttpPost]
        public void InsertData2([FromBody] Data data)
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                var record = new TempLog()
                {
                    GUID = Guid.NewGuid().ToString(),
                    Time = DateTime.Now.ToString(),
                    TEMP = data.TEMP,
                    HUMI = data.HUMI
                };
                context.TempLog.Add(record);
                context.SaveChanges();
            }
        }

        [HttpPost]
        public void InsertData([FromBody] Data data)
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                var input = new test()
                {
                    GUID = Guid.NewGuid().ToString(),
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
                return View(context.TempLog.ToList());
            }
        }
    }
    public class Data
    {
        public string TEMP { get; set; }
        public string HUMI { get; set; }
    }

}
