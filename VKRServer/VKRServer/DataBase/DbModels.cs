using System.Numerics;

namespace VKRServer.DataBase
{
    public enum Role
    {
        User,
        Moder,
        Admin
    }

    public class User
    {
        public int ID { get; set; }
        public Role Role { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; }
        public UserData? UserData { get; set; }
        public ModerData? ModerData { get; set; }
        public AdminData? AdminData { get; set; }
    }

    public abstract class BaseData
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
    }

    public class UserData : BaseData
    {
        public string? Groop { get; set; }
        public User? User { get; set; }
        public MarkTable? MarkTable { get; set; }
    }

    public class ModerData : BaseData
    {
        public BigInteger Key { get; set; }
        public User? User { get; set; }
    }

    public class AdminData : BaseData
    {
        public string? Department { get; set; }
        public User? User { get; set; }
    }

    public class TimeTable
    {
        public int ID { get; set; }
        public int N { get; set; }
        public string? Groop { get; set; }
        public int DayOfWeek { get; set; }
        public int ModerID { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string? Name { get; set; }
        public string? Audience { get; set; }
    }

    public class MarkTable
    {
        public int ID { get; set; }
        public int Mark { get; set; }
        public string? Attendance { get; set; }
        public UserData? UserData { get; set; }
    }
}