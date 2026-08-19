using Carrefour.API.Ledger.DTOs;
using Carrefour.API.Ledger.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Carrefour.API.Ledger.Repositories
{
    public class LedgerActivityService : ILedgerActivityService
    {
        private readonly ILedgerActivityRepository _repository;

        public LedgerActivityService(ILedgerActivityRepository repository) => _repository = repository;

        public async Task<LedgerActivityDTO> CreateAsync(CreateLedgerActivityDTO createLedgerActivityDTO, CancellationToken ct = default)
        {
            var entity = new LedgerActivity(createLedgerActivityDTO);
            var created = await _repository.CreateAsync(entity, ct);
            return new LedgerActivityDTO(created);
        }

        public async Task<IEnumerable<LedgerActivityDTO>> ReadAllAsync(CancellationToken ct = default)
        {
            var products = await _repository.ReadAllAsync(ct);
            return products.Select(p => new LedgerActivityDTO(p));
        }
    }
}