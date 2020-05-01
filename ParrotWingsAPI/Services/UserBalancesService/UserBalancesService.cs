namespace ParrotWingsAPI
{
    using Microsoft.EntityFrameworkCore;
    using ParrotWingsData;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserBalancesService : IUserBalancesService
    {
        private readonly Func<ParrotWingsDBContext> _contextFactory;

        public UserBalancesService(Func<ParrotWingsDBContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<UserBalance> GetUserBalanceAsync(Guid userId, DateTime atDate)
        {
            using var context = _contextFactory.Invoke();
            return await context.UserBalances.Where(ub => ub.UserId == userId && ub.BalanceDate <= atDate).OrderByDescending(ub => ub.BalanceDate).Take(1).FirstOrDefaultAsync();
        }

        public async Task<UserBalance> GetUserBalanceAsync(Guid userId)
        {
            using var context = _contextFactory.Invoke();
            return await context.UserBalances.Where(ub => ub.UserId == userId).OrderByDescending(ub => ub.BalanceDate).Take(1).FirstOrDefaultAsync();
        }

        public async Task<List<UserBalance>> GetUserBalancesAsync(Guid userId, DateTime fromDate, DateTime toDate)
        {
            using var context = _contextFactory.Invoke();
            return await context.UserBalances.Where(ub => ub.UserId == userId && ub.BalanceDate >= fromDate && ub.BalanceDate <= toDate).OrderByDescending(ub => ub.BalanceDate).ToListAsync();
        }

        public async Task<List<UserBalance>> GetUserBalancesAsync(Guid userId)
        {
            using var context = _contextFactory.Invoke();
            return await context.UserBalances.Where(ub => ub.UserId == userId).OrderByDescending(ub => ub.BalanceDate).ToListAsync();
        }

        public async Task<UserBalance> UpdateUserBalanceAsync(Guid userId, Guid transactionId, decimal amount, DateTime updateDate)
        {
            using var context = _contextFactory.Invoke();

            var user = new User() { Id = userId };

            var transaction = new Transaction() { Id = transactionId };

            context.Users.Attach(user);
            context.Transactions.Attach(transaction);

            var balance = new UserBalance()
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                BalanceDate = updateDate,
                User = user,
                UserId = userId,
                Transaction = transaction,
                TransactionId = transactionId
            };
            
            context.UserBalances.Add(balance);
            await context.SaveChangesAsync();

            return balance;
        }

        public Task<UserBalance> UpdateUserBalanceAsync(Guid userId, Guid transactionId, decimal amount)
        {
            return UpdateUserBalanceAsync(userId, transactionId, amount, DateTime.Now);
        }
    }
}
