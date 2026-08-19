using Carrefour.API.Ledger.Models;
using Microsoft.EntityFrameworkCore;

namespace Carrefour.API.Ledger.Repositories
{
    public interface ILedgerActivityRepository
    {
        public Task<LedgerActivity> CreateAsync(LedgerActivity ledgerActivity, CancellationToken ct = default);

        public Task<IEnumerable<LedgerActivity>> ReadAllAsync(CancellationToken ct = default);
    }
}
