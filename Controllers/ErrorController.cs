using Microsoft.AspNetCore.Mvc;

namespace Highdmin.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error400()
        {
            return View();
        }
        public IActionResult Error401()
        {
            return View();
        }
        public IActionResult Error403()
        {
            return View();
        }
        public IActionResult Error404()
        {
            return View();
        }
        public IActionResult Error404Alt()
        {
            return View();
        }
        public IActionResult Error500()
        {
            return View();
        }
        public IActionResult ServiceUnavailable()
        {
            return View();
        }

        public IActionResult DatabaseError()
        {
            ViewBag.ErrorTitle = "Error de Conexión";
            ViewBag.ErrorMessage = "No se pudo conectar a la base de datos. Por favor, inténtelo de nuevo en unos momentos.";
            ViewBag.ErrorDescription = "Si el problema persiste, contacte al administrador del sistema.";
            return View("DatabaseError");
        }
    }
}
