namespace ParrotWingsAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ParrotWingsData;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly IUsersService _users;
        private readonly ITransactionsService _transactions;
        private readonly IUserBalancesService _balances;
        private const decimal REGISTRATION_BONUS = 500;

        public UsersController(IUserBalancesService balances, IAuthenticateService authenticateService, ITransactionsService transactions, IUsersService users)
        {
            _authenticateService = authenticateService;
            _balances = balances;
            _transactions = transactions;
            _users = users;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticatedUser>> Authenticate([FromBody]LoginCredentials credentials)
        {
            var user = await _authenticateService.Authenticate(credentials);

            if (user == null)
            {
                return BadRequest(new { message = "Bad email or password" });
            }

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthenticatedUser>> Register([FromBody]RegisterCredentials credentials)
        {
            var isAdded = await _users.AddUserAsync(credentials);

            if (isAdded)
            {
                var user = await _authenticateService.Authenticate(new LoginCredentials() { Email = credentials.Email, Password = credentials.Password });

                if (user != null)
                {
                    var transaction = await _transactions.CreateTransactionAsync(null, user.Id, REGISTRATION_BONUS, DateTime.Now);

                    if (transaction != null)
                    {
                        var result = await _transactions.CommitTransactionAsync(transaction);

                        if (result != null)
                        {
                            return Ok(user);
                        }
                    }
                }
                return BadRequest(new { message = "Registration failed" });
            }
            else
            {
                return BadRequest(new { message = "User already registered" });
            }
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _users.GetUserAsync(id);
            if (user != null)
            {
                return Ok(user);
            }

            return BadRequest(new { message = $"Could not get user" });
        }

        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _users.GetUsersAsync();
            if (users != null)
            {
                return Ok(users);
            }

            return BadRequest(new { message = $"Could not get users" });
        }

    }
}