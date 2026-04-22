using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace KunevDevPortfolio.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult About() => RedirectToAction("Index");

        public IActionResult Projects() => View();

        public IActionResult Contact() => View();

        public IActionResult Resume() => View();

        public IActionResult Consulting() => View();

        public IActionResult Research() => View();

        public IActionResult Writing() => View();

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
