using System.ComponentModel.DataAnnotations;

namespace KunevDevPortfolio.ViewModels
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Company name cannot be longer than 100 characters")]
        public string? Company { get; set; }

        [Required(ErrorMessage = "Please select a project type")]
        public string ProjectType { get; set; } = string.Empty;

        public string? Budget { get; set; }

        public string? Timeline { get; set; }

        [Required(ErrorMessage = "Please enter a message")]
        [StringLength(2000, ErrorMessage = "Message cannot be longer than 2000 characters")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters long")]
        public string Message { get; set; } = string.Empty;
    }
}