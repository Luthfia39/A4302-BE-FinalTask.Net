using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqRepaymentDto
    {
        public string LoanId { get; set; }
        public decimal Amount { get; set; }
        public decimal RepaidAmount { get; set; } = 0;
        public decimal BalanceAmount { get; set; }
        public string RepaidStatus { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    }
}
