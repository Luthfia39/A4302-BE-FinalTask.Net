using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.DTO.Res.Services;
using DAL.DTO.Res.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/loan/[action]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanServices _loanServices;

        public LoanController(ILoanServices loanServices)
        {
            _loanServices = loanServices;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewLoan(ReqLoanDto loan)
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

                var res = await _loanServices.CreateLoan(loan);

                return Ok(new ResBaseDto<String>
                {
                    Success = true,
                    Message = " Borrow succesfully!",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Email already used")
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLoan([FromRoute] string id, [FromBody] ReqUpdateLoanDto loan)
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

                var res = await _loanServices.UpdateLoan(id, loan);

                return Ok(new ResBaseDto<String>
                {
                    Success = true,
                    Message = "Update status succesfully!",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Email already used")
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
        public async Task<IActionResult> GetAllLoans(string status)
        {
            try
            {
                var res = await _loanServices.LoanList(status);
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
        public async Task<IActionResult> GetLoansByBorrowerId(string borrowerId)
        {
            try
            {
                var res = await _loanServices.GetLoansByBorrowerId(borrowerId);
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
        public async Task<IActionResult> GetLoanById(string id)
        {
            try
            {
                var res = await _loanServices.GetLoansById(id);
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

        [HttpPost]
        public async Task<IActionResult> ProcessLoanPayment(string loanId, decimal amountOfPayment)
        {
            if (string.IsNullOrEmpty(loanId))
            {
                return BadRequest(new ResBaseDto<string>
                {
                    Success = false,
                    Message = "Loan ID tidak boleh kosong.",
                    Data = null
                });
            }

            if (amountOfPayment <= 0)
            {
                return BadRequest(new ResBaseDto<string>
                {
                    Success = false,
                    Message = "Jumlah pembayaran harus lebih besar dari nol.",
                    Data = null
                });
            }

            try
            {
                var res = await _loanServices.ProcessPayment(loanId, amountOfPayment);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success process the payment!",
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

    }
}
