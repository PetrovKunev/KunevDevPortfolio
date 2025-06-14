using KunevDevPortfolio.ViewModels;
using KunevDevPortfolio.Services;
using Microsoft.AspNetCore.Mvc;

namespace KunevDevPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IEmailService emailService, ILogger<HomeController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Projects() => View();

        public IActionResult Contact() => View();

        public IActionResult Resume() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContact(ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form and try again.";
                return View("Contact", model);
            }

            try
            {
                var emailSent = await _emailService.SendContactEmailAsync(model);

                if (emailSent)
                {
                    TempData["SuccessMessage"] = "Thank you for your inquiry! I'll get back to you as soon as possible.";
                    _logger.LogInformation("Contact form submitted successfully by {Email}", model.Email);

                    // Redirect to prevent resubmission on refresh
                    return RedirectToAction("Contact");
                }
                else
                {
                    TempData["ErrorMessage"] = "There was an error sending your message. Please try again or contact me directly at yavor@kunev.dev";
                    return View("Contact", model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form submission");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                return View("Contact", model);
            }
        }
    }
}