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
                if (!string.IsNullOrEmpty(Groop))
                {
                    var AttendanceSimpled = await Context.UserData.Where(z => z.Groop == Groop).Select(z => new
                    {
                        FIO = $"{z.LastName} {z.FirstName![0]}." + (z.MiddleName != null ? $"{z.MiddleName[0]}." : ""),
                        Attendance = z.MarkTable
                    }).ToListAsync();

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
        public async Task<IActionResult> AddUser(Role Role)
        {
            try
            {
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
                        FirstName = "Петр",
                        LastName = "Ягин",
                        MiddleName = "Петрович",
                        Groop = "ИТ-112",

                        MarkTable = new MarkTable
                        {
                            ID = MaxID
                        }
                    };
                }
                else if (Role == Role.Moder)
                {
                    User.ModerData = new ModerData
                    {
                        ID = MaxID,
                        FirstName = "Петр",
                        LastName = "Петров",
                        MiddleName = "Петрович"
                    };
                }
                else if (Role == Role.Admin)
                {
                    User.AdminData = new AdminData
                    {
                        ID = MaxID,
                        FirstName = "Петр",
                        LastName = "Петров",
                        MiddleName = "Петрович",
                        Department = "ITCT"
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
    }
}