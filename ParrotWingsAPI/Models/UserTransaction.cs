namespace ParrotWingsAPI
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserTransaction
    {
        [Required]
        public Guid RecipientId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
