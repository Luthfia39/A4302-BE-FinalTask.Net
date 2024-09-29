using DAL.DTO.Req;
using DAL.DTO.Res.Services.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.DTO.Res.Services
{
    public class FundingServices : IFundingServices
    {
        private readonly PeerLendingContext _peerLendingContext;
        private readonly ILoanServices _loanServices;
        private readonly IUserServices _userSevices;
        private readonly IRepaymentServices _repaymentServices;

        public FundingServices(PeerLendingContext peerLendingContext, 
            ILoanServices loanServices, IUserServices userSevices, IRepaymentServices repaymentServices
            )
        {
            _peerLendingContext = peerLendingContext;
            _loanServices = loanServices;
            _userSevices = userSevices;
            _repaymentServices = repaymentServices;
        }
        public async Task<string> AddNewFunding(ReqFundingDto funding)
        {
            var newLoan = new TrnFunding
            {
                LenderId = funding.LenderId,
                LoanId = funding.LoanId,
                Amount = funding.Amount,
                FundedAt = funding.FundedAt,
            };

            await _peerLendingContext.AddAsync(newLoan);
            await _peerLendingContext.SaveChangesAsync();

            return newLoan.Id;
        }

        public async Task<string> FundingLoan(string loansId, string lenderId)
        {

            // ambil data borrower berdasarkan borrowerId
            var borrowerLoanData = await _loanServices.GetLoansById(loansId);

            // ubah status ke funded
            var reqUpdateLoanDto = new ReqUpdateLoanDto { Status = "Funded" };
            await _loanServices.UpdateLoan(loansId, reqUpdateLoanDto);

            // get data dari mst_user
            var lenderData = await _userSevices.GetUserById(lenderId);
            var borrowerData = await _userSevices.GetUserById(borrowerLoanData.BorrowId);

            // ubah balance
            var newLenderBalance = lenderData.Balance - borrowerLoanData.Amount;
            var newBorrowerBalace = borrowerData.Balance + borrowerLoanData.Amount;

            // simpan balance lender
            var reqUpdateBalanceLender = new ReqUpdateBalanceDto { Balance = (decimal)newLenderBalance };
            await _userSevices.UpdateBalance(lenderData.Id, reqUpdateBalanceLender);

            // simpan balance borrower
            var reqUpdateBalanceBorrower = new ReqUpdateBalanceDto { Balance = (decimal)newBorrowerBalace };
            await _userSevices.UpdateBalance(borrowerData.Id, reqUpdateBalanceBorrower);

            // add ke trn funding
            var reqFunding = new ReqFundingDto
            {
                LoanId = loansId,
                LenderId = lenderId,
                Amount = borrowerLoanData.Amount
            };
            await AddNewFunding(reqFunding);

            // add ke trn repayment
            var reqRepaymentDto = new ReqRepaymentDto
            {
                LoanId = borrowerLoanData.Id,
                Amount = borrowerLoanData.Amount,
                RepaidAmount = 0,
                BalanceAmount = 0,
                RepaidStatus = "on repay"
            };
            await _repaymentServices.AddNewRepayment(reqRepaymentDto);

            await _peerLendingContext.SaveChangesAsync();

            return "Success";
        }

        public async Task<TrnFunding> GetFundingByLoanId(string loanId)
        {
            var dataFunding = await _peerLendingContext.TrnFundings.Where(d => d.LoanId == loanId).SingleOrDefaultAsync();
            if (dataFunding == null)
                throw new Exception("Data not found");
            return dataFunding;
        }

        public async Task<List<ResGetHistoryLoan>> GetHistoryLoansByLenderId(string lenderId)
        {
            var historyLoans = await _peerLendingContext.TrnFundings
            .Include(l => l.Loan.User)
            .Where(l => l.LenderId == lenderId)
            .Select(loan => new ResGetHistoryLoan
            {
                LoanId = loan.Id,
                BorrowerName = loan.Loan.User.Name,
                Amount = loan.Amount,
                InterestRate = loan.Loan.InterestRate,
                Duration = loan.Loan.Duration,
                Status = loan.Loan.Status,
                CreatedAt = loan.Loan.CreatedAt,
                UpdatedAt = loan.Loan.UpdatedAt,
            }).ToListAsync();
            return historyLoans;
        }
    }
}
