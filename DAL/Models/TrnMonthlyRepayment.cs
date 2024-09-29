using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("trn_monthly_repayment")]
    public class TrnMonthlyRepayment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Loan")]
        [Column("loan_id")]
        public string LoanId { get; set; }

        [Required]
        [Column("month")]
        public string Month { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("year")]
        public string Year { get; set; }

        [Required]
        [Column("paid_at")]
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    }
}
