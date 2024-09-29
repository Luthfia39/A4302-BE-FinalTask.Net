using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResGetHistoryLoan
    {
        public string LoanId { get; set; }
        public string BorrowerName { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
