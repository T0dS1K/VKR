using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VKRServer.DataBase;
using VKRServer.Models;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace VKRServer.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AuthNController : ControllerBase
    {
        private readonly TokenValidationParameters TokenValidationParameters;
        private readonly IConfiguration Configuration;
        private readonly AppDbContext Context;

        public AuthNController(AppDbContext Context, IConfiguration Configuration, IOptionsMonitor<JwtBearerOptions> JWTOptions)
        {
            this.Context = Context;
            this.Configuration = Configuration;
            TokenValidationParameters = JWTOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
        }

        [Authorize(Policy = "Access")]
        [HttpGet("Сheck")]
        public IActionResult GetResource()
        {
            return Ok();
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] Data Data)
        {
            var DBUser = await Context.Users.SingleOrDefaultAsync(z => z.Login == Data.Login);

            if (DBUser != null)
            {
                if (BCrypt.Net.BCrypt.Verify(Data.Password, DBUser.Password))
                {
                    try
                    {
                        var ID = DBUser.ID.ToString();
                        var Role = DBUser.Role.ToString();
                        var JWTAccess  = CreateToken(ID, Role, true);
                        var JWTRefresh = CreateToken(ID, Role, false);
                        var JWTHashed  = BCrypt.Net.BCrypt.HashPassword(SecureHash(JWTRefresh));

                        await Context.Users.Where(z => z.Login == $"{ID}@rguk.ru").ExecuteUpdateAsync(z => z.SetProperty(z => z.Token, JWTHashed));
                        return Ok(new { JWTAccess, JWTRefresh });
                    }
                    catch
                    {
                        Console.WriteLine("SignIn Error");
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("JWTUpdate")]
        public async Task<IActionResult> UpdateJWT([FromBody] string Refresh)
        {
            try
            {
                if (Refresh != null)
                {
                    var TokenHandler = new JwtSecurityTokenHandler();
                    SecurityToken ValidatedToken;

                    ClaimsPrincipal JWT = TokenHandler.ValidateToken(
                        Refresh,
                        TokenValidationParameters,
                        out ValidatedToken
                    );

                    if (JWT.Claims.FirstOrDefault(z => z.Type == "Type")!.Value == "Refresh")
                    {
                        var ID    = JWT.Claims.FirstOrDefault(z => z.Type == "ID")!.Value;
                        var Role  = JWT.Claims.FirstOrDefault(z => z.Type == "Role")!.Value;
                        var Token = await Context.Users.Where(z => z.Login == $"{ID}@rguk.ru").Select(z => z.Token).SingleOrDefaultAsync();

                        if (BCrypt.Net.BCrypt.Verify(SecureHash(Refresh), Token))
                        {
                            var JWTAccess  = CreateToken(ID, Role, true);
                            var JWTRefresh = CreateToken(ID, Role, false);
                            var JWTHashed  = BCrypt.Net.BCrypt.HashPassword(SecureHash(JWTRefresh));

                            await Context.Users.Where(z => z.Login == $"{ID}@rguk.ru").ExecuteUpdateAsync(z => z.SetProperty(z => z.Token, JWTHashed));
                            return Ok(new { JWTAccess, JWTRefresh });
                        }
                    } 
                }
            }
            catch
            {
                Console.WriteLine("JWTUpdate Error");
            }

            return BadRequest();
        }

        private string CreateToken(string ID, string Role, bool JWT)
        {
            var Claims = new[] { new Claim("ID", ID), new Claim("Role", Role), JWT ? new Claim("Type", "Access") : new Claim("Type", "Refresh") };
            var TokenHandler = new JwtSecurityTokenHandler();
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = JWT ? DateTime.UtcNow.AddMinutes(30) : DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(Configuration["JWT:Key"]!)), SecurityAlgorithms.HmacSha256Signature),
            };

            return TokenHandler.WriteToken(TokenHandler.CreateToken(TokenDescriptor));
        }

        private string SecureHash(string JWT)
        {
            byte[] JWTBytes = Encoding.UTF8.GetBytes(JWT);

            using (var sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(JWTBytes));
            }
        }
    }
}
