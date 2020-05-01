namespace ParrotWingsAPI
{
    using Microsoft.AspNetCore.SignalR;

    public class UserGuidProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity.Name;
        }
    }
}
