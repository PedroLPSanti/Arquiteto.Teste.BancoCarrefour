using Carrefour.API.Ledger.Enums;
using Carrefour.API.Ledger.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Carrefour.API.Ledger.DTOs
{
    public class LedgerActivityDTO
    {
        public LedgerActivityDTO() { }
        public LedgerActivityDTO(LedgerActivity ledgerActivity)
        {
            this.operation = ledgerActivity.operation;
            this.value = ledgerActivity.value;
            this.dateTimeInclusion = ledgerActivity.dateTimeInclusion;
        }
        public OperationEnum operation { get; set; }
        public decimal value { get; set; }
        public DateTime dateTimeInclusion { get; set; }
    }
}
