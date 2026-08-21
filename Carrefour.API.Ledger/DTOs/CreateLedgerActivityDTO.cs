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
        private decimal _value;
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
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Value must be 0.01 or greater.")]
        public decimal value
        {
            get => _value;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than or equal to 0.01.");
                }
                _value = value;
            }
        }
    }
}
