using DAL.DTO.Req;
using DAL.DTO.Res.Services.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.DTO.Res.Services
{
    public class RepaymentServices : IRepaymentServices
    {
        private readonly PeerLendingContext _peerLendingContext;

        public RepaymentServices(PeerLendingContext peerLendingContext)
        {
            _peerLendingContext = peerLendingContext;
        }

        public async Task<string> AddNewRepayment(ReqRepaymentDto payment)
        {
            var newPayment = new TrnRepayment
            {
                LoanId = payment.LoanId,
                Amount = payment.Amount,
                RepaidAmount = payment.RepaidAmount,
                BalanceAmount = payment.BalanceAmount,
                RepaidStatus = payment.RepaidStatus,
                PaidAt = payment.PaidAt
            };

            await _peerLendingContext.AddAsync(newPayment);
            await _peerLendingContext.SaveChangesAsync();

            return newPayment.Id;
        }

        public async Task<List<ResRepaymentDto>> GetRepaymentById(string borrowerId)
        {
            var RepaymentList = await _peerLendingContext.TrnRepayment
            //.Include(l => l.Loan.User)
            .Where(l => l.Loan.BorrowId == borrowerId)
            .Select(payment => new ResRepaymentDto
            {
                Id = payment.Id,
                LoanId = payment.LoanId,
                Amount = payment.Amount,
                RepaidAmount = payment.RepaidAmount,
                BalanceAmount = payment.BalanceAmount,
                RepaidStatus = payment.RepaidStatus,
                PaidAt = payment.PaidAt,
            }).ToListAsync();
            return RepaymentList;
        }

        private decimal PaymentForAMonth(decimal amount)
        {
            return amount / 12;
        }

        public async Task<ResUpdateAmountRepayment> UpdateAmountRepayment(string id, decimal amount)
        {
            var data = await _peerLendingContext.TrnRepayment.FindAsync(id);
            if (data != null)
            {
                data.RepaidAmount += amount; 
                data.BalanceAmount = data.Amount - data.RepaidAmount;
            }

            await _peerLendingContext.SaveChangesAsync();

            return new ResUpdateAmountRepayment
            {
                RepaidAmount = data.RepaidAmount,
                BalanceAmount = data.BalanceAmount
            };
        }

        public async Task<string> UpdateStatusRepayment(string id)
        {
            var data = await _peerLendingContext.TrnRepayment.FindAsync(id);
            if (data != null)
            {
                data.RepaidStatus = "done";
            }

            await _peerLendingContext.SaveChangesAsync();

            return data.RepaidStatus;
        }
    }
}
