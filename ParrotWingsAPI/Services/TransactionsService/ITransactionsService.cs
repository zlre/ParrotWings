namespace ParrotWingsAPI
{
    using ParrotWingsData;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITransactionsService
    {
        Task<Transaction> CreateTransactionAsync(Guid? senderId, Guid recipientId, decimal amount, DateTime creationDate);

        Task<Transaction> CommitTransactionAsync(Transaction transaction);

        Task<Transaction> RejectTransactionAsync(Transaction transaction);

        Task<Transaction> GetTransactionAsync(Guid transactionId);

        Task<List<Transaction>> GetUserTransactionsAsync(Guid userId);

        Task<List<Transaction>> GetUserRecentTransactionsAsync(Guid userId, int count);

        Task<List<Transaction>> GetUserTransactionsAsync(Guid userId, DateTime fromDate, DateTime toDate);

        Task DeleteTransactionAsync(Transaction transaction);
    }
}
