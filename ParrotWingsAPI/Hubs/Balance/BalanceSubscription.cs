namespace ParrotWingsAPI
{
    using Microsoft.AspNetCore.SignalR;
    using ParrotWingsData;
    using System;
    using TableDependency.SqlClient.Base.Enums;
    using TableDependency.SqlClient.Base.EventArgs;

    public class BalanceSubscription : DatabaseSubscription<BalanceHub, UserBalance>
    {
        public BalanceSubscription(IHubContext<BalanceHub> hubContext)
            : base(hubContext, DmlTriggerType.Insert)
        { 
        }

        protected override async void OnChanged(object sender, RecordChangedEventArgs<UserBalance> e)
        {
            if (e.ChangeType != ChangeType.None)
            {
                var balance = e.Entity;
                await HubContext.Clients.User(balance.UserId.ToString()).SendAsync("UpdateBalance", balance);

                Console.WriteLine($"Updated balance for user with id = {balance.UserId.ToString()} curent balance = {balance.Amount} at {balance.BalanceDate}");
            }
        }

        protected override void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"SqlTableDependency error: {e.Error.Message}");
        }
    }
}
