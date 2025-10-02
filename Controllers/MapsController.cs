using Microsoft.AspNetCore.Mvc;

namespace Highdmin.Controllers
{
    public class MapsController : Controller
    {
        public IActionResult Google()
        {
            return View();
        }
        public IActionResult Leaflet()
        {
            return View();
        }
        public IActionResult Vector()
        {
            return View();
        }
    }
}
