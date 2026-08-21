using Carrefour.API.Ledger.DTOs;
using Carrefour.API.Ledger.Models;
using Microsoft.EntityFrameworkCore;

namespace Carrefour.API.Ledger.Services
{
    public interface ILedgerActivityService
    {
        Task<LedgerActivityDTO> CreateAsync(CreateLedgerActivityDTO dto, CancellationToken ct = default);
        Task<IEnumerable<LedgerActivityDTO>> ReadAllAsync(CancellationToken ct = default);
    }
}
