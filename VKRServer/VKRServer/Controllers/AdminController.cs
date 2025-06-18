using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VKRServer.DataBase;
using VKRServer.Models;
using Microsoft.AspNetCore.Authorization;

namespace VKRServer.Controllers
{
    [Authorize(Policy = "AccessAdmin")]
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext Context;

        public AdminController(AppDbContext Context)
        {
            this.Context = Context;
        }

   [HttpPost("GetAttendance")]
    public async Task<IActionResult> GetAttendance([FromBody] string Groop)
    {
        try
        {
            if (Groop != null)
            {
                var AttendanceSimpled = await Context.UserData.Where(z => z.Groop == Groop).Select(z => new
                {
                    FIO = $"{z.LastName} { z.FirstName![0]}." + (z.MiddleName != null ? $"{ z.MiddleName[0]}." : ""),
                    Attendance = z.MarkTable != null ? z.MarkTable.Attendance : null
                })
                .ToListAsync();

                if (AttendanceSimpled.Count > 0)
                {
                    return Ok(AttendanceSimpled.OrderBy(z => z.FIO));
                }
            }
        }
        catch {}
        Console.WriteLine("GroupName Error");
        return BadRequest();
    }


    [HttpPost("AddUser")]
    public async Task<IActionResult> AddUser(Role Role, string? Groop, string? Department)
    {
        try
        {
            Random Random = new Random();
            string LastName   = C.Name[Random.Next(0, C.Name.Length)];
            string FirstName  = C.Name[Random.Next(0, C.Name.Length)][0] + ".";
            string MiddleName = C.Name[Random.Next(0, C.Name.Length)][0] + ".";

            var MaxIDFromDb = await Context.Users.Where(ZOV => ZOV.Role == Role).MaxAsync(ZOV => (int?)ZOV.ID);
            var MaxID = MaxIDFromDb ?? (Role == Role.Admin ? 3 * C.ID : Role == Role.Moder ? 2 * C.ID : C.ID);

            var User = new User
            {
                ID = ++MaxID,
                Role = Role,
                Login = $"{MaxID}@rguk.ru",
                Password = BCrypt.Net.BCrypt.HashPassword("1234"),
            };

            if (Role == Role.User)
            {
                User.UserData = new UserData
                {
                    ID = MaxID,
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MiddleName,
                    Groop = Groop,

                    MarkTable = new MarkTable
                    {
                        ID = MaxID,
                        Attendance = AttendanceGenerator()
                    }
                };
            }
            else if (Role == Role.Moder)
            {
                User.ModerData = new ModerData
                {
                    ID = MaxID,
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MiddleName,
                };
            }
            else if (Role == Role.Admin)
            {
                User.AdminData = new AdminData
                {
                    ID = MaxID,
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MiddleName,
                    Department = Department
                };
            }

            Context.Users.Add(User);
            await Context.SaveChangesAsync();
            return Ok();
        }
        catch
        {
            Console.WriteLine("AddUser Error");
            return BadRequest();
        }
    }

    [HttpPost("AddLesson")]
    public async Task<IActionResult> AddLesson(int N, string Groop, int DayOfWeek, int ModerID, int StartTime, int EndTime, string Name, string Audience)
    {
        try
        {
            TimeTable TimeTable = new TimeTable()
            {
                N = N,
                Groop = Groop,
                DayOfWeek = DayOfWeek,
                ModerID = ModerID,
                StartTime = StartTime,
                EndTime = EndTime,
                Name = Name,
                Audience = Audience
            };

            Context.TimeTable.Add(TimeTable);
            await Context.SaveChangesAsync();
            return Ok();
        }
        catch
        {
            Console.WriteLine("AddLesson Error");
            return BadRequest();
        }
    }

    private string AttendanceGenerator()
    {
        Random Random = new Random();
        StringBuilder Attendance = new StringBuilder();

        for (int i = 0; i < 100 * C.LPD;)
        {
            Attendance.Append(Random.Next(0, 3));

            if (++i % C.LPD == 0)
            {
                Attendance.Append('|');
            }
        }

        return Attendance.ToString();
    }
}
}
