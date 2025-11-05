using System.ComponentModel.DataAnnotations;

namespace RealEstateMVC.Models
{
    public class Agent
    {
        public int AgentID { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public string Company { get; set; }
    }
}
