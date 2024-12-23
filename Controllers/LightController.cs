using IoTControlPanel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IoTControlPanel.Controllers
{
    public class LightController : Controller
    {
        [HttpPost]
        public void LogLightChange([FromBody] LightInfo info)
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                //
                var ip = info.SensorIP;
                //取得IP的方式2
                var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                LogLightChange logLightChange = new LogLightChange()
                {
                    GUID = Guid.NewGuid().ToString(),
                    LightNum = info.LightNum,
                    SensorIP = info.SensorIP,
                    UpdateTime = DateTime.Now.ToString()
                };
                context.LogLightChange.Add(logLightChange);
                context.SaveChanges();
            }
        }
        public ActionResult Index()
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                var data = context.LogLightChange.ToList();
                LogLightChange result = new LogLightChange();
                if (data.Count != 0)
                {
                    result = data.OrderByDescending(x => x.UpdateTime).First();
                }
                else
                {
                    result = new LogLightChange();
                }
                var test = data.OrderByDescending(x => x.UpdateTime).ToList();
                ViewBag.LogLightChange = test;
                return View(result);
            }
        }

    }
    public class LightInfo
    {
        public int LightNum { get; set; }
        public string SensorIP { get; set; }
    }
}
