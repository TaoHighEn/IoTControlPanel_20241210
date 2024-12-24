using System.Data.Entity;
using IoTControlPanel.Models;
using Microsoft.AspNetCore.Mvc;

namespace IoTControlPanel.Controllers
{
    public class DoorBellController : Controller
    {
        public ActionResult Index()
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                var data = context.AlertBell.ToList();
                ViewBag.LogData = data;
                return View();
            }
        }

        public ActionResult BellSetting()
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                List<BellSetting> datalist = context.BellSetting.OrderByDescending(x => x.SensorIP).ToList();
                ViewBag.BellSetting = datalist;
                return View();
            }
        }
        /// <summary>
        /// 門鈴觸發紀錄
        /// </summary>
        public string AlertBell()
        {
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                List<BellSetting> bells = context.BellSetting.Where(x => x.SensorIP == remoteIpAddress.Trim()).ToList();
                if (bells.Count != 0)
                {
                    AlertBell alert = new AlertBell()
                    {
                        GUID = Guid.NewGuid().ToString(),
                        SensorIP = remoteIpAddress,
                        LogTime = DateTime.Now.ToString(),
                    };
                    context.AlertBell.Add(alert);
                    context.SaveChanges();
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
        }
        /// <summary>
        /// 設定門鈴
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeBellSetting(string ip, string option)
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                ip = ip.Trim();
                List<BellSetting> belllist = context.BellSetting.ToList();
                var bell = belllist.Where(x => x.SensorIP == ip).ToList();
                if (option == "ins")
                {
                    if (bell.Count == 0)
                    {
                        BellSetting newBell = new BellSetting()
                        {
                            GUID = Guid.NewGuid().ToString(),
                            SensorIP = ip,
                            Status = 1
                        };
                        context.BellSetting.Add(newBell);
                        context.SaveChanges();
                    }
                    else
                    {
                        var data = bell.ToList().First();
                        data.Status = 1;
                        context.Update(data);
                        context.SaveChanges();
                    }
                }
                else if (option == "del")
                {
                    if (bell.Count != 0)
                    {
                        var data = bell.ToList().First();
                        data.Status = 0;
                        context.Update(data);
                        context.SaveChanges();
                    }
                }
                return Redirect("./BellSetting");
            }
        }
    }
}
