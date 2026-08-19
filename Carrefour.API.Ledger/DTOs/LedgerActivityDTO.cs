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

        [Required]
        public OperationEnum operation { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be 0 or greater.")]
        public decimal value { get; set; }

        [Required]
        public DateTime dateTimeInclusion { get; set; }
    }
}
