using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResLoanDto
    {
        public string Id { get; set; }
        public string BorrowId { get; set; }
        public decimal BalanceBorrower { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
