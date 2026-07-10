using Microsoft.AspNetCore.Mvc;

namespace KunevDevPortfolio.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Projects() => View();

        public IActionResult Contact() => View();

        // Старият URL остава валиден за индексирани страници
        public IActionResult Resume() => RedirectToActionPermanent(nameof(About));

        public IActionResult Consulting() => View();

        public IActionResult Research() => View();

        public IActionResult Writing() => View();
    }
}
