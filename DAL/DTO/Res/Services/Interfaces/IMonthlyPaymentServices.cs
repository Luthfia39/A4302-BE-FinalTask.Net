using DAL.DTO.Req;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Services.Interfaces
{
    public interface IMonthlyPaymentServices
    {
        Task<string> AddMonthlyPayment(ReqMonthlyPaymentDto monthlyPaymentDto);
    }
}
