using Microsoft.AspNetCore.Mvc;

namespace StankinMaps.Controllers
{
    public class MapsController : Controller
    {
        public IActionResult Maps(string building, int floor)
        {
            ViewBag.Building = building;
            ViewBag.Floor = floor;

            return View();
        }
    }
}