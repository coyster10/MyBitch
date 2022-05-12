using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using C2_Final.Data;
using C2_Final.Models;

namespace C2_Final
{
    public class JwtAuthenticationManager
    {
        private readonly string key;

        private readonly Bailey_FinancialContext _context;

        private readonly IDictionary<string, string> users = new Dictionary<string, string>()
        { {"test", "password"}, {"test1", "pwd"} };

        public JwtAuthenticationManager(string key)
        {
            this.key = key;
        }

        public string Authenticate(string email, string userid, string admin)
        {

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim("userID", userid),
                    new Claim("admin", admin)
                }),

                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}