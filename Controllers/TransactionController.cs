using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Transactions;
using WebApi.Services;

namespace WebApi.Controllers
{
    public class TransactionController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly PopulatorService _populatorService;

        public TransactionController(
            DataContext dataContext,
            PopulatorService populatorService)
        {
            _context = dataContext;
            _populatorService = populatorService;
        }

        /// <summary>
        /// get coin transaction history from the GEM chain
        /// </summary>
        /// <param name="pageIndex">default 0</param>
        /// <param name="pageSize">default 10</param>
        /// <returns></returns>
        /// <response code="200">get list success</response>
        [HttpGet]
        [Route("transaction-history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<TransactionEntry>> GetTransactionHistoryAsync([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var user = (User)HttpContext.Items["User"];

            var dbTxns = await _context.InternalTransacions.OrderByDescending(t => t.Created).Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

            return dbTxns.Select(_populatorService.PopulateTransactionEntry).ToList();
        }
    }
}
