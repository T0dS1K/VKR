using VKRServer.DataBase;
using Microsoft.EntityFrameworkCore;

public class ConcatenationAttendance : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory ScopeFactory;
    private Timer? Timer;

    public ConcatenationAttendance(IServiceScopeFactory ScopeFactory)
    {
        this.ScopeFactory = ScopeFactory;
    }

    private void ConcatenationProcess(object state)
    {
        try
        {
            using var Scope = ScopeFactory.CreateScope();
            var Context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
            using var Transaction = Context.Database.BeginTransaction();

            try
            {
                var MarkTableData = Context.MarkTable.AsNoTracking().ToList();

                foreach (var Data in MarkTableData)
                {
                    var MarkTable = new MarkTable
                    {
                        ID = Data.ID,
                        Attendance = $"{Data.Attendance}{Data.Mark.ToString()}|",
                        Mark = 0
                    };

                    Context.Entry(MarkTable).State = EntityState.Modified;
                }

                Context.SaveChanges();
                Transaction.Commit();
            }
            catch
            {
                Transaction.Rollback();
                throw;
            }
        }
        catch
        {
            Console.WriteLine("ConcatenationProcess Error");
        }
    }

    public Task StartAsync(CancellationToken CancellationToken)
    {
        var Now = DateTimeOffset.Now;
        var DueTime = now.Date.AddDays(1).AddHours(C.TimeZone) - Now;
        Timer = new Timer(ConcatenationProcess!, DueTime, TimeSpan.Zero, TimeSpan.FromHours(24));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken CancellationToken)
    {
        Timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Timer?.Dispose();
    }
}
