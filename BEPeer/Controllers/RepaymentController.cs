using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.DTO.Res.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/repayment/[action]")]
    [ApiController]
    public class RepaymentController : ControllerBase
    {
        private readonly IRepaymentServices _repaymentServices;

        public RepaymentController(IRepaymentServices repaymentServices)
        {
            _repaymentServices = repaymentServices;
        }

        // add
        [HttpPost]
        public async Task<IActionResult> AddNewRepayment(ReqRepaymentDto repaymentDto)
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

                var res = await _repaymentServices.AddNewRepayment(repaymentDto);

                return Ok(new ResBaseDto<String>
                {
                    Success = true,
                    Message = "Add new repayment succesfully!",
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

        // get repayment by borrowerId
        [HttpGet]
        public async Task<IActionResult> GetRepaymentById(string borrowerId)
        {
            try
            {
                var res = await _repaymentServices.GetRepaymentById(borrowerId);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success load repayment!",
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

        // update amount
        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateAmount([FromRoute] string id, [FromBody] decimal amount)
        {
            try
            {
                var updatedAmountRepayment = await _repaymentServices.UpdateAmountRepayment(id, amount);
                return Ok(new ResBaseDto<ResUpdateAmountRepayment>
                {
                    Success = true,
                    Message = "User repaid amount updated successfully",
                    Data = updatedAmountRepayment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // update status
        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateStatus([FromRoute] string id)
        {
            try
            {
                var updatedStatusRepayment = await _repaymentServices.UpdateStatusRepayment(id);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User repaid amount updated successfully",
                    Data = updatedStatusRepayment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
