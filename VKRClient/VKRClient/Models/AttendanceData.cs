namespace VKRClient.Models
{
    public class AttendanceData
    {
        public string? FIO { get; set; }
        public string? Monday { get; set; }
        public string? Tuesday { get; set; }
        public string? Wednesday { get; set; }
        public string? Thursday { get; set; }
        public string? Friday { get; set; }
        public string? Saturday { get; set; }

        public AttendanceData(string FIO, string Monday, string Tuesday, string Wednesday, string Thursday, string Friday, string Saturday)
        {
            this.FIO = FIO;
            this.Monday = Monday;
            this.Tuesday = Tuesday;
            this.Wednesday = Wednesday;
            this.Thursday = Thursday;
            this.Friday = Friday;
            this.Saturday = Saturday;
        }
    }
}
