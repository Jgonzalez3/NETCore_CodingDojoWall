using System.ComponentModel.DataAnnotations;
namespace Wall.Models{
    public class Users{
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name can only contain letters")]
        [MinLength(2, ErrorMessage= "First Name must have at least 2 letters")]
        public string FirstName {get;set;}
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name can only contain letters")]
        [Required]
        [MinLength(2, ErrorMessage= "Last Name must have at least 2 letters")]
        public string LastName {get;set;}
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9.+_-]+@[a-zA-Z0-9._-]+\.[a-zA-Z]+$")]
        public string Email {get;set;}
        [Required]
        [MinLength(8, ErrorMessage= "Email must have at least 8 letters")]
        public string Password {get;set;}
        [Required]
        [Compare(nameof(Password))]
        public string PasswordConfirm {get;set;}
    }
}