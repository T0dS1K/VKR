using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Numerics;
using VKRServer.BackgroundFunc;
using VKRServer.DataBase;
using VKRServer.Models;

namespace VKRServer.Controllers
{
    [Authorize(Policy = "AccessModer")]
    [ApiController]
    [Route("[controller]")]

    public class ModerController : ControllerBase
    {
        private readonly AppDbContext Context;
        private readonly ITempKeyGenerator ITempKeyGenerator;

        public ModerController(AppDbContext Context, ITempKeyGenerator ITempKeyGenerator)
        {
            this.Context = Context;
            this.ITempKeyGenerator = ITempKeyGenerator;
        }

        [HttpPatch("SetKey")]
        public async Task<IActionResult> GetKey([FromBody] PDH PDH)
        {
            try
            {
                if (PDH.A != null && PDH.p != null)
                {
                    int ID = int.Parse(User.FindFirst("ID")?.Value!);

                    BigInteger A = BigInteger.Parse(PDH.A);
                    BigInteger p = BigInteger.Parse(PDH.p);
                    BigInteger b = Crypto.GenNum(256);
                    BigInteger B = Crypto.PowWithMod(7, b, p);
                    BigInteger K = Crypto.PowWithMod(A, b, p);

                    await Context.ModerData.Where(z => z.ID == ID).ExecuteUpdateAsync(z => z.SetProperty(z => z.Key, K));
                    ITempKeyGenerator.AddSecretData(ID, K);
                    return Ok(B.ToString());
                }
            }
            catch {}
            Console.WriteLine("SetKey Error");
            return BadRequest();
        }
    }
}
