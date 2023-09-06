using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    public class Teste2Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
