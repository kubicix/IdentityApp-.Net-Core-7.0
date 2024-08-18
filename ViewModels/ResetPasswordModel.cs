using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class ResetPasswordModel
    {
        [Required]
        public string Token {get;set;} = string.Empty;

        [Required]
        public string Id {get;set;} = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password {get;set;} = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="Passwords don't match.")]
        public string ConfirmPassword {get;set;} = string.Empty;



    }
}