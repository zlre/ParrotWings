namespace ParrotWingsData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    [Table("Transaction")]
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? StatusDate { get; set; }

        [JsonIgnore]
        public User Sender { get; set; }

        public Guid? SenderId { get; set; }

        [JsonIgnore]
        public User Recipient { get; set; }

        public Guid RecipientId { get; set; }

        public TransactionStatuses Status { get; set; }

        public decimal Amount { get; set; }

        public ICollection<UserBalance> UserBalances { get; set; }
    }
}
