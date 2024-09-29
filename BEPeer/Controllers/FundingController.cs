using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.DTO.Res.Services;
using DAL.DTO.Res.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/funding/[action]")]
    [ApiController]
    public class FundingController : ControllerBase
    {
        private readonly IFundingServices _fundingServices;

        public FundingController(IFundingServices fundingServices)
        {
            _fundingServices = fundingServices;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewFunding(ReqFundingDto fundingDto)
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

                var res = await _fundingServices.AddNewFunding(fundingDto);

                return Ok(new ResBaseDto<String>
                {
                    Success = true,
                    Message = "Add new funding succesfully!",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "LoanId is required")
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

        [HttpGet]
        public async Task<IActionResult> GetHistoryLoans(string lenderId)
        {
            try
            {
                var res = await _fundingServices.GetHistoryLoansByLenderId(lenderId);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success load loan!",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFundingByLoanId(string loanId)
        {
            try
            {
                var res = await _fundingServices.GetFundingByLoanId(loanId);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success retrieve the funding data!",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessFunding(string loanId, string lenderId)
        {
            try
            {
                var res = await _fundingServices.FundingLoan(loanId, lenderId);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success process the loan!",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> ProcessLoanPayment(string loanId, decimal amountOfPayment)
        //{
        //    try
        //    {
        //        var res = await _fundingServices.ProcessPayment(loanId, amountOfPayment);
        //        return Ok(new ResBaseDto<object>
        //        {
        //            Success = true,
        //            Message = "Success process the payment!",
        //            Data = res
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
        //        {
        //            Success = false,
        //            Message = ex.Message,
        //            Data = null
        //        });
        //    }
        //}
    }
}
