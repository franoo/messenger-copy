using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Authentication
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, UserDTO user);
        int? ValidateToken(string key, string token);
    }
    public class TokenService: ITokenService
    {
        private const double EXPIRY_DURATION_HOURS = 1;

        public string BuildToken(string key, string issuer, UserDTO user)
        {
            //var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new[] {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier,
            Guid.NewGuid().ToString()),
            new Claim("id", user.Id.ToString())
        };
            Console.Write(user.Id);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.UtcNow.AddHours(EXPIRY_DURATION_HOURS), signingCredentials: credentials);
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        public int? IsTokenValid(string key,  string token)
        {
            var mySecret = Encoding.UTF8.GetBytes(key);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                return userId;
            }
            catch
            {
                return null;
            }

        }

        public int? ValidateToken(string key, string token)
        {
            return IsTokenValid(key, token);
        }
    }
}
