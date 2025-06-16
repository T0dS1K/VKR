using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VKRServer.BackgroundFunc;
using VKRServer.DataBase;
using VKRServer.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace VKRServer.Controllers
{
    [Authorize(Policy = "AccessUser")]
    [ApiController]
    [Route("[controller]")]

    public class UserController : ControllerBase
    {
        private readonly AppDbContext Context;
        private readonly TempDataCon TempData;
        private readonly int Time;
        private readonly int N;

        public UserController(AppDbContext Context, TempDataCon TempData)
        {
            this.TempData = TempData;
            this.Context = Context;
            Time = GetTimeNow();
            N = GetLessonN();
        }

        [HttpPatch("SetMark")]
        public async Task<ActionResult> SetMark([FromBody] string ScanTempKey)
        {
            if (ScanTempKey != null)
            {
                try
                {
                    int Index = Crypto.Pow(10, N);
                    int ID = int.Parse(User.FindFirst("ID")?.Value!);
                    int Marker = await Context.MarkTable.Where(z => z.ID == ID).Select(z => z.Mark).SingleOrDefaultAsync();

                    if (Marker / Index % 10 == 0)
                    {
                        var Groop = await Context.UserData.Where(z => z.ID == ID).Select(z => z.Groop).SingleOrDefaultAsync();
                        var DBTime = await Context.TimeTable.FirstOrDefaultAsync(z => z.Groop == Groop && z.DayOfWeek == GetDayOfWeek() && z.N == N);

                        if (Groop != null && DBTime != null)
                        {
                            if (DBTime.StartTime <= Time && Time <= DBTime.EndTime)
                            {
                                Marker += Index;

                                if (DBTime.StartTime + C.TimeOfDelay >= Time)
                                {
                                    Marker += Index;
                                }

                                if (TempData.Data.TryGetValue(DBTime.ModerID, out string? TempKey))
                                {
                                    if (TempKey == ScanTempKey)
                                    {
                                        await Context.MarkTable.Where(z => z.ID == ID).ExecuteUpdateAsync(z => z.SetProperty(z => z.Mark, Marker));
                                        return Ok("Успешно");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return Ok("Вы уже отметились");
                    }

                }
                catch {}
            }

            Console.WriteLine("SetMarkController Error");
            return BadRequest("Ошибка");
        }

        private int GetLessonN()
        {
            for (int N = 0; N < C.Schedule.Length - 1; N++)
            {
                if (Time < C.Schedule[N + 1])
                {
                    return N;
                }
            }

            return C.Schedule.Length - 1;
        }

        private int GetDayOfWeek()
        {
            long Time = (((DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600 * C.TimeZone) / 86400) - 4);
            return (int)((Time % 7 * 2) + (Time / 7 % 2));
        }

        private int GetTimeNow()
        {
            return (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60 + 60 * C.TimeZone) % 1440;
        }
    }
}