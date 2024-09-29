using DAL.DTO.Req;

namespace DAL.DTO.Res.Services.Interfaces
{
    public interface IRepaymentServices
    {
        // add
        Task<string> AddNewRepayment(ReqRepaymentDto payment);

        // update balance
        Task<ResUpdateAmountRepayment> UpdateAmountRepayment(String id, decimal amount);

        // update status
        Task<string> UpdateStatusRepayment(String id);

        // get repayment detail by id
        Task<List<ResRepaymentDto>> GetRepaymentById(string id);
    }
}
