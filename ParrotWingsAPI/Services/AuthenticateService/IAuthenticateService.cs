namespace ParrotWingsAPI
{
    using System.Threading.Tasks;

    public interface IAuthenticateService
    {
        Task<AuthenticatedUser> Authenticate(LoginCredentials credentials);
    }
}
