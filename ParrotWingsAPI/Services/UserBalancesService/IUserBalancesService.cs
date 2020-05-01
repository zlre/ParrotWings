namespace ParrotWingsAPI
{
    using ParrotWingsData;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserBalancesService
    {
        Task<UserBalance> GetUserBalanceAsync(Guid userId, DateTime atDate);

        Task<UserBalance> GetUserBalanceAsync(Guid userId);

        Task<List<UserBalance>> GetUserBalancesAsync(Guid userId, DateTime fromDate, DateTime toDate);

        Task<List<UserBalance>> GetUserBalancesAsync(Guid userId);

        Task<UserBalance> UpdateUserBalanceAsync(Guid userId, Guid transactionId, decimal amount, DateTime updateDate);

        Task<UserBalance> UpdateUserBalanceAsync(Guid userId, Guid transactionId, decimal amount);
    }
}