namespace ParrotWingsAPI
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterCredentials
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
