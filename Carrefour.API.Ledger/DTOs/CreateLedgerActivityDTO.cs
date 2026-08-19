using Carrefour.API.Ledger.Enums;
using Carrefour.API.Ledger.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Carrefour.API.Ledger.DTOs
{
    public class CreateLedgerActivityDTO
    {
        public CreateLedgerActivityDTO() { }
        public CreateLedgerActivityDTO(LedgerActivity ledgerActivity)
        {
            this.operation = ledgerActivity.operation;
            this.value = ledgerActivity.value;
        }

        [Required]
        [EnumDataType(typeof(OperationEnum), ErrorMessage = "Invalid operation. Must be 0 (DEBIT) or 1 (CREDIT).")]
        public OperationEnum operation { get; set; }
        [Required]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Value must be 0 or greater.")]
        public decimal value { get; set; }
    }
}
