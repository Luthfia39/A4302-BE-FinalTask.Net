using DAL.DTO.Req;
using DAL.DTO.Res.Services.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Services
{
    public class UserServices : IUserServices
    {
        private readonly PeerLendingContext _context;
        private readonly IConfiguration _configuration;

        public UserServices(PeerLendingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<string> Register(ReqRegisterUserDto register)
        {
            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == register.Email);
            if (isAnyEmail != null) {
                throw new Exception("Email already used");
            }

            var newUser = new MstUser
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = register.Role,
                Balance = register.Balance
            };

            await _context.MstUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Name;
        }

        public async Task<List<ResUserDto>> GetAllUsers()
        {
            return await _context.MstUsers
                .Where(user => user.Role != "Admin")
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance
                }).ToListAsync();
        }

        private string GenerateJwtToken(MstUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };

            var token = new JwtSecurityToken(
                    issuer: jwtSettings["ValidIssuer"],
                    audience: jwtSettings["ValidAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResLoginDto> Login(ReqLoginDto reqLogin)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == reqLogin.Email);
            if (user == null) {
                throw new Exception("Invalid email or password");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
            if (!isPasswordValid) {
                throw new Exception("Invalid email or password");
            }

            var token = GenerateJwtToken(user);

            var loginResponse = new ResLoginDto
            {
                Token = token,
                Role = user.Role
            };
            return loginResponse;
        }

        public async Task<string> AddUser(ReqRegisterUserDto addUser)
        {
            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == addUser.Email);
            if (isAnyEmail != null)
            {
                throw new Exception("Email already used");
            }

            var newUser = new MstUser
            {
                Name = addUser.Name,
                Email = addUser.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(addUser.Password),
                Role = addUser.Role,
                Balance = addUser.Balance
            };

            await _context.MstUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Name;
        }

        public async Task<bool> DeleteUser(string id)
        {
            var user = await _context.MstUsers.FindAsync(id);
            if (user == null)
                return false;

            _context.MstUsers.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<ResUserDto> UpdateUser(string id, ReqUpdateUserDto updateDto)
        {
            var user = await _context.MstUsers.FindAsync(id);
            if (user == null)
                throw new Exception("User not found");

            // Update user properties
            if (!string.IsNullOrEmpty(updateDto.Name))
                user.Name = updateDto.Name;

            if (!string.IsNullOrEmpty(updateDto.Role))
                user.Role = updateDto.Role;

            if (updateDto.Balance != 0) // Assuming 0 means no change
                user.Balance = updateDto.Balance;

            await _context.SaveChangesAsync();

            return new ResUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Balance = user.Balance
            };
        }

        public async Task<ResUserByIdDto> GetUserById(string id)
        {
            var user = await _context.MstUsers.FindAsync(id);
            if (user == null)
                throw new Exception("User not found");
            return new ResUserByIdDto
            {
                Id = user.Id,
                Name = user.Name,
                Role = user.Role,
                Balance = user.Balance
            };
        }
    }

}
