namespace ParrotWingsAPI
{
    using Microsoft.AspNetCore.SignalR;
    using ParrotWingsData;
    using System;
    using TableDependency.SqlClient.Base.Enums;
    using TableDependency.SqlClient.Base.EventArgs;

    public class TransactionSubscription : DatabaseSubscription<TransactionHub, Transaction>
    {
        public TransactionSubscription(IHubContext<TransactionHub> hubContext)
            : base(hubContext, DmlTriggerType.All)
        {
        }

        protected override async void OnChanged(object sender, RecordChangedEventArgs<Transaction> e)
        {
            if (e.ChangeType == ChangeType.Update)
            {
                var transaction = e.Entity;

                if (transaction.RecipientId == transaction.SenderId)
                {
                    await HubContext.Clients.User(transaction.RecipientId.ToString()).SendAsync("UpdateTransaction", transaction);
                }
                else
                {
                    await HubContext.Clients.User(transaction.RecipientId.ToString()).SendAsync("UpdateTransaction", transaction);
                    await HubContext.Clients.User(transaction.SenderId.ToString()).SendAsync("UpdateTransaction", transaction);
                }

                Console.WriteLine($"Updated transaction with id = {transaction.Id.ToString()}");
            }

            if (e.ChangeType == ChangeType.Insert)
            {
                var transaction = e.Entity;

                if (transaction.RecipientId == transaction.SenderId)
                {
                    await HubContext.Clients.User(transaction.RecipientId.ToString()).SendAsync("NewTransaction", transaction);
                }
                else
                {
                    await HubContext.Clients.User(transaction.RecipientId.ToString()).SendAsync("NewTransaction", transaction);
                    await HubContext.Clients.User(transaction.SenderId.ToString()).SendAsync("NewTransaction", transaction);
                }

                Console.WriteLine($"Updated transaction with id = {transaction.Id.ToString()}");
            }
        }

        protected override void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"SqlTableDependency error: {e.Error.Message}");
        }
    }
}
