namespace ParrotWingsAPI
{
    using System.ComponentModel.DataAnnotations;

    public class LoginCredentials
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
