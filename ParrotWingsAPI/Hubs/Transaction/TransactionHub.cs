namespace ParrotWingsAPI
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    [Authorize]
    public class TransactionHub : Hub
    {

    }
}
