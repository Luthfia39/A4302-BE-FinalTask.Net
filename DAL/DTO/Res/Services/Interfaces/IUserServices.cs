using DAL.DTO.Req;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Services.Interfaces
{
    public interface IUserServices
    {
        Task<String> Register(ReqRegisterUserDto register);

        Task<List<ResUserDto>> GetAllUsers();

        Task<ResLoginDto> Login(ReqLoginDto reqLogin);

        Task<String> AddUser(ReqRegisterUserDto addUser);

        Task<ResUserDto> UpdateUser(string id, ReqUpdateUserDto updateDto, bool isAdmin);
        Task<bool> DeleteUser(string id);
    }
}
