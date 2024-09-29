using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqFundingDto
    {
        [Required(ErrorMessage = "LoanId is required")]
        public string LoanId { get; set; }

        [Required(ErrorMessage = "LenderId is required")]
        public string LenderId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "FundedAt is required")]
        public DateTime FundedAt { get; set; } = DateTime.UtcNow;
    }
}
