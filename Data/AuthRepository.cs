using System.Threading.Tasks;
using rpg.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace rpg.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        public AuthRepository(DataContext context, IConfiguration config)
        {
            _config = config;
            _context = context;

        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            ServiceResponse<string> res = new ServiceResponse<string>();
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
            if (user == null)
            {
                res.Success = false;
                res.Message = "User not found";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                res.Success = false;
                res.Message = "Password not found";

            }
            else
            {
                res.Data = CreateToken(user);
            }
            return res;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            ServiceResponse<int> res = new ServiceResponse<int>();
            if (await UserExist(user.Username))
            {
                res.Success = false;
                res.Message = "User already exists";
                return res;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            res.Data = user.Id;
            return res;
        }

        public async Task<bool> UserExist(string username)
        {
            if (await _context.Users.AnyAsync
                    (u => u.Username.ToLower()
                        == username.ToLower()))
            {
                return true;
            }
            return false;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                if (computedHash.Length != passwordHash.Length)
                {
                    return false;
                }

                for (var i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)
            );
            SigningCredentials cred = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha512
            );

            SecurityTokenDescriptor desc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cred
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(desc);

            return handler.WriteToken(token);
        }
    }
}