using Carrefour.API.Ledger.DTOs;
using Carrefour.API.Ledger.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrefour.API.Ledger.Models
{
    [Table("ledger_activity")]
    public class LedgerActivity
    {
        private decimal _value;
        public LedgerActivity() { }

        public LedgerActivity(CreateLedgerActivityDTO createLedgerActivityDTO)
        {
            this.value = createLedgerActivityDTO.value;
            this.operation = createLedgerActivityDTO.operation;
            this.dateTimeInclusion = DateTime.UtcNow;
        }

        [Key]
        [Column("id_ledger_activity")]
        public long idLedgerActivity { get; set; }

        [Column("operation")]
        public OperationEnum operation { get; set; }

        [Column("value")]
        public decimal value
        {
            get => _value;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than or equal to 0.");
                }
                _value = value;
            }
        }

        [Column("datetime_inclusion")]
        public DateTime dateTimeInclusion { get; set; }
    }
}
