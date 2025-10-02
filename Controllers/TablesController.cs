using Microsoft.AspNetCore.Mvc;

namespace Highdmin.Controllers
{
    public class TablesController : Controller
    {
        public IActionResult Basic()
        {
            return View();
        }
        public IActionResult Datatable()
        {
            return View();
        }
        public IActionResult Gridjs()
        {
            return View();
        }
    }
}
