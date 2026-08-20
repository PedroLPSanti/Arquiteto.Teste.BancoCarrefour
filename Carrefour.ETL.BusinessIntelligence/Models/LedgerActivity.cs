using Carrefour.ETL.BusinessIntelligence.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrefour.ETL.BusinessIntelligence.Models
{
    [Table("ledger_activity")]
    public class LedgerActivity
    {
        [Key]
        [Column("id_ledger_activity")]
        public long idLedgerActivity { get; set; }

        [Column("operation")]
        public OperationEnum operation { get; set; }

        [Column("value")]
        public decimal value { get; set; }

        [Column("datetime_inclusion")]
        public DateTime dateTimeInclusion { get; set; }
    }
}
