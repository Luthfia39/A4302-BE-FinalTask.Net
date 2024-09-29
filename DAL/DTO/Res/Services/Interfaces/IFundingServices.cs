using DAL.DTO.Req;
using DAL.Models;

namespace DAL.DTO.Res.Services.Interfaces
{
    public interface IFundingServices
    {
        Task<string> AddNewFunding(ReqFundingDto funding);
        Task<List<ResGetHistoryLoan>> GetHistoryLoansByLenderId(string id);
        Task<string> FundingLoan(string loansId, string lenderId);

        Task<TrnFunding> GetFundingByLoanId(string loanId);

        // //payment process
        //Task<string> ProcessPayment(string loanId, decimal amountOfPayment);
    }
}
