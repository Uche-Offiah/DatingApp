using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController( DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto details){

            if (await UsernameExist(details.username)) return BadRequest("UserName already Exists");

            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = details.username,
                Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(details.password)),
                PasswordSalt = hmac.Key
            };

            _context.Add(user);
             await _context.SaveChangesAsync();

            return new UserDto{
                username = user.UserName,
                token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto login){

            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == login.username);
            if(user == null) return Unauthorized("Invalid UserName");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.password));
            for (int i = 0; i < computedHash.Length; i++)
            {
               if(computedHash[i] != user.Password[i]) return Unauthorized("Invalid Password"); 
            }

            return new UserDto{
                username = user.UserName,
                token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UsernameExist(string username){

           return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}