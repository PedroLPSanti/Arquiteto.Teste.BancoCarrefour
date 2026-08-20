using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Carrefour.API.BusinessIntelligence.Enums;
using Carrefour.API.BusinessIntelligence.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Carrefour.API.BusinessIntelligence.DTOs
{
    public class DailyConsolidatedDTO
    {
        public DailyConsolidatedDTO() { }

        public DailyConsolidatedDTO(DailyConsolidated dailyConsolidated)
        {
            this.valueTotal = dailyConsolidated.valueCredit - dailyConsolidated.valueDebit;
            this.valueDebit = dailyConsolidated.valueDebit;
            this.valueCredit = dailyConsolidated.valueCredit;
            this.debitQuantity = dailyConsolidated.debitQuantity;
            this.creditQuantity = dailyConsolidated.creditQuantity;
            this.consolidatedDate = dailyConsolidated.consolidatedDate;
        }

        public decimal valueTotal { get; set; }
        public decimal valueDebit { get; set; }
        public decimal valueCredit { get; set; }
        public int debitQuantity { get; set; }
        public int creditQuantity { get; set; }
        public DateOnly consolidatedDate { get; set; }
    }
}
