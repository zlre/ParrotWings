namespace ParrotWingsAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ParrotWingsData;

    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserBalancesController : ControllerBase
    {
        private readonly IUserBalancesService _balances;

        public UserBalancesController(IUserBalancesService balances)
        {
            _balances = balances;
        }

        private Guid UserId
        {
            get
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                return Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);
            }
        }

        [HttpGet("GetBalance")]
        public async Task<ActionResult<UserBalance>> GetBalance()
        {
            var balance = await _balances.GetUserBalanceAsync(UserId);
            if (balance != null)
            {
                return Ok(balance);
            }

            return BadRequest(new { message = $"Could not get user`s wallet balance" });
        }

        [HttpGet("GetBalanceAt")]
        public async Task<ActionResult<UserBalance>> GetBalanceAt([FromQuery] DateTime atDateTime)
        {
            var balance = await _balances.GetUserBalanceAsync(UserId, atDateTime);
            if (balance != null)
            {
                return Ok(balance);
            }

            return BadRequest(new { message = $"Could not get user`s wallet balance" });
        }

        [HttpGet("GetBalanceBetween")]
        public async Task<ActionResult<UserBalance>> GetBalanceBetween([FromQuery] DateTime fromDateTime, [FromQuery] DateTime toDateTime)
        {
            var balance = await _balances.GetUserBalancesAsync(UserId, fromDateTime, toDateTime);
            if (balance != null)
            {
                return Ok(balance);
            }

            return BadRequest(new { message = $"Could not get user`s wallet balance" });
        }

        [HttpGet("GetAllBalances")]
        public async Task<ActionResult<IEnumerable<UserBalance>>> GetAllBalances()
        {
            var balances = await _balances.GetUserBalancesAsync(UserId);
            if (balances != null)
            {
                return Ok(balances);
            }

            return BadRequest(new { message = $"Could not get user`s wallet balance" });

        }

    }
}