using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carrefour.API.BusinessIntelligence.DTOs;
using Carrefour.API.BusinessIntelligence.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Carrefour.API.BusinessIntelligence.Models
{
    [Table("daily_consolidated")]
    public class DailyConsolidated
    {
        public DailyConsolidated() { }

        [Key]
        [Column("id_daily_consolidated")]
        public long idDailyConsolidated { get; set; }

        [Column("value_debit")]
        public decimal valueDebit { get; set; }

        [Column("value_credit")]
        public decimal valueCredit { get; set; }

        [Column("debit_quantity")]
        public int debitQuantity { get; set; }

        [Column("credit_quantity")]
        public int creditQuantity { get; set; }

        [Column("consolidated_date")]
        public DateOnly consolidatedDate { get; set; }

        [Column("id_last_ledger_activity")]
        public long idLastLedgerActivity { get; set; }
    }
}
