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

        [HttpPost("SetMark")]
        public async Task<ActionResult> SetMark([FromBody] string ScanTempKey)
        {
            if (ScanTempKey != null)
            {
                try
                {
                    int Index = Crypto.Pow(10, N);
                    int ID = int.Parse(User.FindFirst("ID")?.Value!);
                    int Marker = await Context.MarkTable.Where(z => z.ID == ID).Select(z => z.Mark).SingleOrDefaultAsync();
    
                    if (Marker / Index == 0)
                    {
                        var Groop = await Context.UserData.Where(z => z.ID == ID).Select(z => z.Groop).SingleOrDefaultAsync();
                        var Lessons = await Context.TimeTable.Where(z => z.Groop == Groop && z.DayOfWeek == GetDayOfWeek() && z.N == N).ToListAsync();
    
                        if (Groop != null && Lessons != null)
                        {
                            foreach (var Lesson in Lessons)
                            {
                                if (Lesson.StartTime <= Time && Time <= Lesson.EndTime)
                                {
                                    Marker += Index;
    
                                    if (Lesson.StartTime + C.TimeOfDelay >= Time)
                                    {
                                        Marker += Index;
                                    }
    
                                    if (TempData.Data.TryGetValue(Lesson.ModerID, out string? TempKey))
                                    {
                                        if (TempKey == ScanTempKey)
                                        {
                                            Console.WriteLine($"{ID} код {Marker}");
                                            await Context.MarkTable.Where(z => z.ID == ID).ExecuteUpdateAsync(z => z.SetProperty(z => z.Mark, Marker));
                                            return Ok();
                                        }
                                    }
                                }
                            }
                        } 
                    }
                    else
                    {
                        return NoContent();
                    }
                }
                catch {}
            }
    
            Console.WriteLine("SetMarkController Error");
            return BadRequest();
        }
    
        private int GetLessonN()
        {
            int Length = C.Schedule.Length - 1;
    
            for (int N = 0; N < Length; N++)
            {
                if (Time < C.Schedule[N + 1])
                {
                    return N;
                }
            }
    
            return Length;
        }
    
        private int GetDayOfWeek()
        {
            DateOnly DateOnly = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(C.TimeZone));
            int DaysPass = DateOnly.DayNumber - new DateOnly(2024, 1, 1).DayNumber;
            return (DaysPass % 7 * 2) + (DaysPass / 7 % 2);
        }
    
        private int GetTimeNow()
        {
            return (int)TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(C.TimeZone)).ToTimeSpan().TotalMinutes;
        }
    }
}
