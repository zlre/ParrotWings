namespace ParrotWingsAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ParrotWingsData;

    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _transactions;

        public TransactionsController(ITransactionsService transactions)
        {
            _transactions = transactions;
        }

        private Guid UserId
        {
            get
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                return Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);
            }
        }

        [HttpPost("CreateTransaction")]
        public async Task<ActionResult<Transaction>> CreateTransaction([FromBody] UserTransaction userTransaction)
        {
            var transaction = await _transactions.CreateTransactionAsync(UserId, userTransaction.RecipientId, userTransaction.Amount, DateTime.Now);
            if (transaction != null)
            {
                return Ok(transaction);
            }

            return BadRequest(new { message = $"Could not create transaction" });
        }        
        
        [HttpPost("CommitTransaction")]
        public async Task<ActionResult<Transaction>> CommitTransaction(Transaction newTransaction)
        {
            var transaction = await _transactions.GetTransactionAsync(newTransaction.Id);
            if (transaction != null && transaction.SenderId == UserId)
            {
                transaction = await _transactions.CommitTransactionAsync(transaction);

                if (transaction != null)
                {
                    return Ok(transaction);
                }
            }

            return BadRequest(new { message = $"Could not commit transaction" });
        }

        [HttpPost("RejectTransaction")]
        public async Task<ActionResult<Transaction>> RejectTransaction(Transaction newTransaction)
        {
            var transaction = await _transactions.GetTransactionAsync(newTransaction.Id);
            if (transaction != null && transaction.SenderId == UserId)
            {
                transaction = await _transactions.RejectTransactionAsync(transaction);

                if (transaction != null)
                {
                    return Ok(transaction);
                }
            }

            return BadRequest(new { message = $"Could not reject transaction" });
        }

        [HttpGet]
        public async Task<ActionResult<Transaction>> GetTransaction(Guid transactionId)
        {
            var transaction = await _transactions.GetTransactionAsync(transactionId);
            if (transaction != null)
            {
                return Ok(transaction);
            }

            return BadRequest(new { message = $"Could not get transaction" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetUserTransactions()
        {
            var transactions = await _transactions.GetUserTransactionsAsync(UserId);
            if (transactions != null)
            {
                return Ok(transactions);
            }

            return BadRequest(new { message = $"Could not get transactions" });
        }

        [HttpGet("GetUserRecentTransactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetUserRecentTransactions([FromQuery] int count)
        {
            var transactions = await _transactions.GetUserRecentTransactionsAsync(UserId, count);
            if (transactions != null)
            {
                return Ok(transactions);
            }

            return BadRequest(new { message = $"Could not get transactions" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetUserTransactions([FromQuery] DateTime fromDateTime, [FromQuery] DateTime toDateTime)
        {
            var transactions = await _transactions.GetUserTransactionsAsync(UserId, fromDateTime, toDateTime);
            if (transactions != null)
            {
                return Ok(transactions);
            }

            return BadRequest(new { message = $"Could not get transactions" });
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteTransaction(Guid transactionId)
        {
            var transaction = await _transactions.GetTransactionAsync(transactionId);
            if (transaction != null)
            {
                await _transactions.DeleteTransactionAsync(transaction);
                return Ok();
            }

            return BadRequest(new { message = $"Could not reject transaction" });
        }
    }
}