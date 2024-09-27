using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.DTO.Res.Services;
using DAL.DTO.Res.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal.Postgres;
using System.Security.Claims;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost]
        public async Task<IActionResult> Register(ReqRegisterUserDto register)
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

                var res = await _userServices.Register(register);

                return Ok(new ResBaseDto<String>
                {
                    Success = true,
                    Message = "user registered!",
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
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userServices.GetAllUsers();

                return Ok(new ResBaseDto<List<ResUserDto>>
                {
                    Success = true,
                    Message = "List of users",
                    Data = users,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<ResUserDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(ReqLoginDto loginDto)
        {
            try
            {
                var response = await _userServices.Login(loginDto);
                return Ok(
                    new ResBaseDto<object>
                    {
                        Success = true,
                        Message = "User login success",
                        Data = response
                    }
                );
            }
            catch (Exception ex)
            {
                
                if (ex.Message == "Invalid email or password")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<ResUserDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser(ReqRegisterUserDto register)
        {
            var currentUser = HttpContext.User;
            if (!currentUser.IsInRole("Admin"))
            {
                return Forbid("You do not have permission to add users.");
            }

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

                var res = await _userServices.Register(register);

                return Ok(new ResBaseDto<String>
                {
                    Success = true,
                    Message = "user registered!",
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

        [HttpDelete("{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _userServices.DeleteUser(id);
                if (result)
                    return Ok(new ResBaseDto<object>
                    {
                        Success = true,
                        Message = "User deleted successfully",
                        Data = null
                    });
                else
                    return NotFound(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Data = null
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

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] ReqUpdateUserDto updateDto)
        {
            try
            {
                var updatedUser = await _userServices.UpdateUser(id, updateDto);
                return Ok(new ResBaseDto<ResUserDto>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = updatedUser
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

        [HttpGet]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var users = await _userServices.GetUserById(id);

                return Ok(new ResBaseDto<ResUserByIdDto>
                {
                    Success = true,
                    Message = "Succesfully get user by id!",
                    Data = users,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<ResUserByIdDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
