namespace ParrotWingsAPI
{
    using Microsoft.EntityFrameworkCore;
    using ParrotWingsData;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TransactionsService : ITransactionsService
    {
        private readonly Func<ParrotWingsDBContext> _contextFactory;

        private readonly IUserBalancesService _balances;

        public TransactionsService(Func<ParrotWingsDBContext> contextFactory,  IUserBalancesService balances)
        {
            _contextFactory = contextFactory;
            _balances = balances;
        }


        public async Task<Transaction> RejectTransactionAsync(Transaction transaction)
        {
            using (var context = _contextFactory.Invoke())
            {
                context.Attach(transaction);
                transaction.Status = TransactionStatuses.Fail;
                transaction.StatusDate = DateTime.Now;
                context.Transactions.Update(transaction);

                await context.SaveChangesAsync();
            }

            return transaction;
        }

        public async Task<Transaction> CommitTransactionAsync(Transaction transaction)
        {
            if (transaction.RecipientId == default)
            {
                return await RejectTransactionAsync(transaction);
            }

            UserBalance senderBalance = null;
            decimal senderAmount = 0;

            if (transaction.SenderId != null) {
                senderBalance = await _balances.GetUserBalanceAsync((Guid)transaction.SenderId);
                senderAmount = senderBalance.Amount;
            }
            
            var recipientBalance = await _balances.GetUserBalanceAsync(transaction.RecipientId);
            decimal recipientAmount = 0;

            if (recipientBalance != null) {
                recipientAmount = recipientBalance.Amount;
            }

            if (senderBalance != null)
            {
                if (senderAmount - transaction.Amount < 0)
                {
                    return await RejectTransactionAsync(transaction);
                }
            }

            using var context = _contextFactory.Invoke();
            using var dbTransaction = context.Database.BeginTransaction();
            try
            {
                if (senderBalance != null)
                {
                    await _balances.UpdateUserBalanceAsync((Guid)transaction.SenderId, transaction.Id, senderAmount - transaction.Amount);
                }
                await _balances.UpdateUserBalanceAsync(transaction.RecipientId, transaction.Id, recipientAmount + transaction.Amount);

                context.Attach(transaction);

                transaction.StatusDate = DateTime.Now;
                transaction.Status = TransactionStatuses.Success;

                await context.SaveChangesAsync();

                dbTransaction.Commit();

                return transaction;
            }
            catch (Exception)
            {
                dbTransaction.Rollback();
                return await RejectTransactionAsync(transaction);
            }
        }

        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            using var context = _contextFactory.Invoke();
            context.Transactions.Remove(transaction);
            await context.SaveChangesAsync();
        }

        public async Task<Transaction> GetTransactionAsync(Guid transactionId)
        {
            using var context = _contextFactory.Invoke();
            return await context.Transactions.Where(t => t.Id == transactionId).FirstOrDefaultAsync();
        }

        public async Task<Transaction> CreateTransactionAsync(Guid? senderId, Guid recipientId, decimal amount, DateTime creationDate)
        {
            using var context = _contextFactory.Invoke();

            if (senderId == recipientId) {
                return null;
            }

            User sender = null;
            if (senderId != null)
            {
                sender = new User() { Id = (Guid)senderId };
                context.Users.Attach(sender);
            }
            var recipient = new User() { Id = recipientId };

            
            context.Users.Attach(recipient);

            var transaction = new Transaction()
            {
                Id = Guid.NewGuid(),
                Sender = sender,
                SenderId = senderId,
                Recipient = recipient,
                RecipientId = recipientId,
                Amount = amount,
                CreationDate = creationDate,
                Status = TransactionStatuses.Created,
                StatusDate = null
            };
            
            context.Transactions.Add(transaction);

            await context.SaveChangesAsync();

            return transaction;
        }

        public async Task<List<Transaction>> GetUserTransactionsAsync(Guid userId)
        {
            using var context = _contextFactory.Invoke();
            return await context.Transactions.Where(t => t.RecipientId == userId || t.SenderId == userId).OrderByDescending(o => o.CreationDate).ToListAsync();
        }

        public async Task<List<Transaction>> GetUserRecentTransactionsAsync(Guid userId, int count)
        {
            using var context = _contextFactory.Invoke();
            return await context.Transactions.Where(t => t.RecipientId == userId || t.SenderId == userId).Take(count).OrderByDescending(o => o.CreationDate).ToListAsync();
        }

        public async Task<List<Transaction>> GetUserTransactionsAsync(Guid userId, DateTime fromDate, DateTime toDate)
        {
            using var context = _contextFactory.Invoke();
            return await context.Transactions.Where(t => (t.RecipientId == userId || t.SenderId == userId) && t.CreationDate <= toDate && t.CreationDate >= fromDate).OrderByDescending(o => o.CreationDate).ToListAsync();
        }
    }
}
