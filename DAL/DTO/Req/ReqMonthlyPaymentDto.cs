using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqMonthlyPaymentDto
    {
        public string LoanId { get; set; }
        public string Month { get; set; }
        public decimal Amount { get; set; }
        public string Year { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
