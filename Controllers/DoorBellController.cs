using Microsoft.AspNetCore.Mvc;

namespace IoTControlPanel.Controllers
{
    public class DoorBellController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
