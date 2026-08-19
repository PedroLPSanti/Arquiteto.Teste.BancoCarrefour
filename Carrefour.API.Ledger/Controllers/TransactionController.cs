using Carrefour.API.Ledger.DTOs;
using Carrefour.API.Ledger.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Carrefour.API.Ledger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILedgerActivityService _ledgerActivityService;
        public TransactionController(ILedgerActivityService ledgerActivityService)
        {
            _ledgerActivityService = ledgerActivityService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LedgerActivityDTO>>> GetAll(CancellationToken ct)
        {
            return Ok(await _ledgerActivityService.ReadAllAsync(ct));
        }

        [HttpPost]
        public async Task<ActionResult<LedgerActivityDTO>> Post([FromBody] CreateLedgerActivityDTO createLedgerActivityDTO, CancellationToken ct)
        {
            return Ok(await _ledgerActivityService.CreateAsync(createLedgerActivityDTO, ct));
        }
    }
}
