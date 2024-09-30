using DAL.DTO.Req;
using DAL.DTO.Res.Services.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DAL.DTO.Res.Services
{
    public class LoanServices : ILoanServices
    {
        private readonly PeerLendingContext _peerLendingContext;
        private readonly IUserServices _userSevices;
        private readonly IRepaymentServices _repaymentServices;
        public LoanServices(PeerLendingContext peerLendingContext, IUserServices userSevices, IRepaymentServices repaymentServices)
        {
            _peerLendingContext = peerLendingContext;
            _userSevices = userSevices;
            _repaymentServices = repaymentServices;
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

        public async Task<List<ResListLoanDto>> GetLoansByBorrowerId(string borrowwerId)
        {
            var loans = await _peerLendingContext.MstLoans
            .Include(l => l.User)
            .Where(l => l.BorrowId == borrowwerId)
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

        public async Task<ResLoanDto> GetLoansById(string id)
        {
            var data = await _peerLendingContext.MstLoans
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (data == null)
                throw new Exception("Data not found");

            return new ResLoanDto
            {
                Id = data.Id,
                BorrowId = data.BorrowId,
                BalanceBorrower = Convert.ToDecimal(data.User.Balance),
                Amount = data.Amount,
                InterestRate = data.InterestRate,
                Duration = data.Duration,
                Status = data.Status,
                CreatedAt = data.CreatedAt,
                UpdatedAt = data.UpdatedAt
            };
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

        public async Task<string> ProcessPayment(string loanId, decimal amountOfPayment)
        {
            // Logging untuk debugging
            Console.WriteLine($"Processing payment for LoanId: {loanId}, Amount: {amountOfPayment}");

            // get repayment
            var paymentData = await _peerLendingContext.TrnRepayment
                .Where(d => d.LoanId == loanId)
                .OrderByDescending(d => d.PaidAt)
                .FirstOrDefaultAsync();
            Console.WriteLine($"Payment data : {paymentData.LoanId}");

            if (paymentData == null)
            {
                Console.WriteLine("Payment data not found.");
                return "Data pembayaran tidak ditemukan";
            }

            var borrowerData = await GetLoansById(loanId);
            if (borrowerData == null)
            {
                Console.WriteLine("Borrower data not found.");
                return "Data peminjam tidak ditemukan";
            }
            Console.WriteLine($"borrower ditemukan dengan jumlah uang : {borrowerData.BalanceBorrower}");

            var lenderFundingData = await _peerLendingContext.TrnFundings
                .Where(d => d.LoanId == loanId)
                .OrderByDescending(d => d.FundedAt)
                .FirstOrDefaultAsync();

            if (lenderFundingData == null)
            {
                Console.WriteLine("Lender funding data not found.");
                return "Data pendanaan pemberi pinjaman tidak ditemukan";
            }
            Console.WriteLine("lender funding ditemukan");

            var lenderData = await _userSevices.GetUserById(lenderFundingData.LenderId);
            if (lenderData == null)
            {
                Console.WriteLine("Lender data not found.");
                return "Data pemberi pinjaman tidak ditemukan";
            }
            Console.WriteLine("lender data ditemukan");

            if (paymentData.Amount == paymentData.RepaidAmount)
            {
                Console.WriteLine("pinjaman lunas");
                await _repaymentServices.UpdateStatusRepayment(paymentData.Id);

                await UpdateLoan(loanId, new ReqUpdateLoanDto
                {
                    Status = "repaid"
                });
            } 
            else if (paymentData.Amount == paymentData.RepaidAmount + amountOfPayment)
            {
                var newLenderBalance = Convert.ToDecimal(lenderData.Balance + amountOfPayment);
                Console.WriteLine($"new lender balance : {newLenderBalance}");
                var newBorrowerBalance = Convert.ToDecimal(borrowerData.BalanceBorrower - amountOfPayment);
                Console.WriteLine($"new borrower balance : {newBorrowerBalance}");

                // simpan balance lender
                var reqUpdateBalanceLender = new ReqUpdateBalanceDto { Balance = newLenderBalance };
                await _userSevices.UpdateBalance(lenderData.Id, reqUpdateBalanceLender);
                Console.WriteLine("sudah update lender");

                // simpan balance borrower
                var reqUpdateBalanceBorrower = new ReqUpdateBalanceDto { Balance = newBorrowerBalance };
                await _userSevices.UpdateBalance(borrowerData.BorrowId, reqUpdateBalanceBorrower);
                Console.WriteLine("sudah update borrower");

                // update repayment data
                await _repaymentServices.UpdateAmountRepayment(paymentData.Id, amountOfPayment);
                Console.WriteLine("update repayment amount");

                Console.WriteLine("pinjaman lunas");
                await _repaymentServices.UpdateStatusRepayment(paymentData.Id);

                await UpdateLoan(loanId, new ReqUpdateLoanDto
                {
                    Status = "repaid"
                });
            }
            else
            {
                Console.WriteLine("pinjaman belom lunas");
                // ubah balance
                var newLenderBalance = Convert.ToDecimal(lenderData.Balance + amountOfPayment);
                Console.WriteLine($"new lender balance : {newLenderBalance}");
                var newBorrowerBalance = Convert.ToDecimal(borrowerData.BalanceBorrower - amountOfPayment);
                Console.WriteLine($"new borrower balance : {newBorrowerBalance}");

                // simpan balance lender
                var reqUpdateBalanceLender = new ReqUpdateBalanceDto { Balance = newLenderBalance };
                await _userSevices.UpdateBalance(lenderData.Id, reqUpdateBalanceLender);
                Console.WriteLine("sudah update lender");

                // simpan balance borrower
                var reqUpdateBalanceBorrower = new ReqUpdateBalanceDto { Balance = newBorrowerBalance };
                await _userSevices.UpdateBalance(borrowerData.BorrowId, reqUpdateBalanceBorrower);
                Console.WriteLine("sudah update borrower");

                // update repayment data
                await _repaymentServices.UpdateAmountRepayment(paymentData.Id, amountOfPayment);
                Console.WriteLine("update repayment amount");
            }

            await _peerLendingContext.SaveChangesAsync();

            return "Success";
        }

    }
}
