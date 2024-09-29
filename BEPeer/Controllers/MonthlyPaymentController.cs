using DAL.DTO.Req;
using DAL.DTO.Res.Services;
using DAL.DTO.Res;
using DAL.DTO.Res.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/MonthlyPayment/[action]")]
    [ApiController]
    public class MonthlyPaymentController : ControllerBase
    {
        private readonly IMonthlyPaymentServices _monthlyPayment;
        public MonthlyPaymentController(IMonthlyPaymentServices monthlyPayment)
        {
            _monthlyPayment = monthlyPayment;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPayment(ReqMonthlyPaymentDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Message = x.Value.Errors.Select(equals => equals.ErrorMessage).ToList()
                        }).ToList();

                    var errorMessage = new StringBuilder("Validation errors occured!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }

                var res = await _monthlyPayment.AddMonthlyPayment(paymentDto);

                return Ok(new ResBaseDto<String>
                {
                    Success = true,
                    Message = "payment successfully!",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "LoanId is not valid")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
