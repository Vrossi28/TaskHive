using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskHive.Infrastructure.Repositories;
using TaskHive.Core.Entities;

namespace TaskHive.Application.Services.Security
{
    public class SecurityHelper
    {
        private const int SaltSize = 16;
        private const int Iterations = 10000;
        private readonly AccountRepository _accountRepository;

        public SecurityHelper(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public string GenerateJwtToken(IConfiguration _configuration, Account account)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, account.Email),
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Role, account.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration[SecurityConstants.JwtSecret]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration[SecurityConstants.JwtIssuer],
                _configuration[SecurityConstants.JwtAudience],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRandomToken()
        {
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            if (_accountRepository.ResetTokenAlreadyExists(token)) GenerateRandomToken();

            return token;
        }

        public string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            new Random().NextBytes(salt);

            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: 256 / 8
            );

            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hash = Convert.FromBase64String(parts[1]);

            byte[] newHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: 256 / 8
            );

            return hash.SequenceEqual(newHash);
        }
    }
}
