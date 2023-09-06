using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    public class TesteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
