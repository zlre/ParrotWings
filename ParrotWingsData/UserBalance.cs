namespace ParrotWingsData
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    [Table("UserBalance")]
    public class UserBalance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public Guid UserId { get; set; }        
        
        [JsonIgnore]
        public Transaction Transaction { get; set; }

        public Guid TransactionId { get; set; }

        public DateTime BalanceDate { get; set; }

        public decimal Amount { get; set; }
    }
}
