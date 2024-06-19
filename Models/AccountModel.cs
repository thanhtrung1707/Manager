// Models/AccountModel.cs
using System.ComponentModel.DataAnnotations;

namespace Manager.Models
{
    public class AccountModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public string FullName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)] // This is just for validation purpose
        [Display(Name = "Password")] // This is just for validation purpose
        public string Password { get; set; } // Temporary password storage
    }
}
