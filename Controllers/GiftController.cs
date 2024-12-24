using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Net;
using IoTControlPanel.Models;
using Microsoft.AspNetCore.Mvc;

namespace IoTControlPanel.Controllers
{
    public class GiftController : Controller
    {
        private static ConcurrentQueue<R_Member> _messageQueue = new ConcurrentQueue<R_Member>();
        public IActionResult Index()
        {
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                try
                {
                    if (!_messageQueue.IsEmpty)
                    {
                        if (_messageQueue.TryDequeue(out R_Member member))
                        {
                            var rec = context.Gift.Where(x => x.status != 1).ToList();
                            Random random = new Random();
                            int num = random.Next(0, rec.Count);
                            rec[num].Member = member.Name;
                            rec[num].status = 1;
                            rec[num].RemoteAddr = member.RemoteAddr;
                            rec[num].MemberDesc = member.Description;
                            context.Gift.Update(rec[num]);
                            context.SaveChanges();
                        }
                    }
                    var data = context.Gift.ToList();
                    ViewBag.Data = data;
                    return View();
                }
                catch (Exception e)
                {
                    return Error(e.Message);
                }
            }
        }

        [HttpPost]
        public IActionResult Drawing([FromBody] Member member)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress))
            {
                return Error("ip異常");
            }
            using (IoTDBdbContext context = new IoTDBdbContext())
            {
                var data = context.Gift.ToList();
                if (data.Count == 0)
                    return Error("0");
                if (data.Where(x => x.RemoteAddr == ipAddress).Count() > 0)
                    return Error("1");
                if (_messageQueue.Where(x => x.RemoteAddr== ipAddress).Count() > 0)
                    return Error("1");
                if (string.IsNullOrEmpty(member.Name) ||
        string.IsNullOrEmpty(member.Desc) ||
        member.Ans1 == 0 ||
        member.Ans2 == 0 ||
        member.Ans3 == 0)
                {
                    return Error("請檢查傳入數值，記得Ansx不得為空或0");
                }
                context.Member.Add(member);
                R_Member r_member = new R_Member() 
                {
                    Name = member.Name,
                    Description = member.Desc,
                    RemoteAddr = ipAddress
                };
                _messageQueue.Enqueue(r_member);
            }
            return Content(@"
        <html>
            <head>
            </head>
            <body>
                <h2>傳送成功，已加入抽獎隊列</h2>
            </body>
        </html>");
        }

        public IActionResult Error(string msg)
        {
            string r_msg = string.Empty;
            if (msg == "1")
            {
                r_msg = @"
        <html>
            <head>
            </head>
            <body>
                <h2>重複抽獎或已在隊列中，請等待其他同學完成</h2>
            </body>
        </html>";
            }
            else if (msg == "0")
            {
                r_msg = @"
        <html>
            <head>
            </head>
            <body>
                <h2>抽獎已結束</h2>
            </body>
        </html>";
            }
            else
            {

                r_msg = $@"
        <html>
            <head>
            </head>
            <body>
                <h2>{msg}，請再試一次。</h2>
            </body>
        </html>";
            }
            return Content(r_msg, "text/html");
        }
    }
    public class R_Member
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string RemoteAddr { get; set; }
    }
}
