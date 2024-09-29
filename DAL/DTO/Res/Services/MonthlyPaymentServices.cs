using DAL.DTO.Req;
using DAL.DTO.Res.Services.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Services
{
    public class MonthlyPaymentServices : IMonthlyPaymentServices
    {
        private readonly PeerLendingContext _peerLendingContext;
        public MonthlyPaymentServices(PeerLendingContext peerLendingContext)
        {
            _peerLendingContext = peerLendingContext;
        }

        public async Task<string> AddMonthlyPayment(ReqMonthlyPaymentDto monthlyPaymentDto)
        {
            var newPayment = new TrnMonthlyRepayment
            {
                LoanId = monthlyPaymentDto.LoanId,
                Amount = monthlyPaymentDto.Amount,
                Month = monthlyPaymentDto.Month,
                Year = monthlyPaymentDto.Year,
                PaidAt = monthlyPaymentDto.PaidAt
            };

            await _peerLendingContext.AddAsync(newPayment);
            await _peerLendingContext.SaveChangesAsync();

            return newPayment.Id;
        }
    }
}
