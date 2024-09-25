using DAL.DTO.Req;
using DAL.DTO.Res.Services.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Services
{
    public class LoanServices : ILoanServices
    {
        private readonly PeerLendingContext _peerLendingContext;
        public LoanServices(PeerLendingContext peerLendingContext)
        {
            _peerLendingContext = peerLendingContext;
        }

        public async Task<string> CreateLoan(ReqLoanDto loan)
        {
            var newLoan = new MstLoans
            {
                BorrowId = loan.BorrowerId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
            };

            await _peerLendingContext.AddAsync(newLoan);
            await _peerLendingContext.SaveChangesAsync();

            return newLoan.BorrowId;
        }

        public async Task<List<ResListLoanDto>> LoanList(string status)
        {
                var loans = await _peerLendingContext.MstLoans
                .Include(l => l.User)
                .Where(l => status == null || l.Status == status)
                .OrderByDescending(l => l.CreatedAt)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    BorrowerName = loan.User.Name,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt,
                }).ToListAsync();
                return loans;
        }

        public async Task<String> UpdateLoan(string id, ReqUpdateLoanDto updateLoan)
        {
            var data = await _peerLendingContext.MstLoans.FindAsync(id);
            if (data != null)
            {
                data.Status = updateLoan.Status;
                data.UpdatedAt = DateTime.UtcNow;
            }

            await _peerLendingContext.SaveChangesAsync();

            return data.Status;
        }
    }
}
