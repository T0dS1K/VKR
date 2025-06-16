using System.Collections.Concurrent;
using System.Security.Cryptography;
using VKRServer.DataBase;
using VKRServer.Models;
using System.Numerics;
using System.Text;
using System.Collections.Immutable;

namespace VKRServer.BackgroundFunc
{
    public interface ITempKeyGenerator
    {
        void AddSecretData(int ID, BigInteger Key);
    }

    public class GeneratorTempKeys : IHostedService, IDisposable, ITempKeyGenerator
    {
        private ConcurrentBag<SecretData> SecretData { get; set; } = new ConcurrentBag<SecretData>();
        private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private readonly IServiceScopeFactory ScopeFactory;
        private readonly object LockUpdate = new();
        private readonly TempDataCon TempData;
        private const int TimeOut = C.TimeOut;
        private const int Error = C.Error;
        private const int CC = C.CC;
        private long UnixTime;
        private Timer? Timer;

        public GeneratorTempKeys(IServiceScopeFactory ScopeFactory, TempDataCon TempData)
        {
            this.TempData = TempData;
            this.ScopeFactory = ScopeFactory;
        }

        private void GenerateTempKeys(object state)
        {
            if (!SecretData.Any()) return;

            var NewTempData = new ConcurrentDictionary<int, string>();
            UnixTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            Parallel.ForEach(SecretData, Data =>
            {
                if (Data.Key != 0)
                {
                    NewTempData[Data.ID] = GetTempKey(Data.Key);
                    Console.WriteLine($"{Data.ID}\t{NewTempData[Data.ID]}\t{UnixTime}");
                }
            });

            Console.WriteLine();

            lock (LockUpdate)
            {
                
                TempData.Data = NewTempData.ToImmutableDictionary();
            }

            Timer?.Change(TimeSpan.FromMilliseconds(TimeOut - (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % TimeOut) + Error), Timeout.InfiniteTimeSpan);
        }

        public void AddSecretData(int ID, BigInteger Key)
        {
            lock (LockUpdate)
            {
                var Data = SecretData.FirstOrDefault(x => x.ID == ID);

                if (Data != null)
                {
                    Data.Key = Key;
                }
                else
                {
                    SecretData.Add(new SecretData { ID = ID, Key = Key });
                }
            }
        }

        private string GetTempKey(BigInteger SecretKey)
        {
            using (var HMAC = new HMACSHA256(SecretKey.ToByteArray()))
            {
                byte[] hash = HMAC.ComputeHash(BitConverter.GetBytes((UnixTime - UnixTime % TimeOut) / 1000));
                return DecimalToCC(new BigInteger(hash));
            }
        }

        private string DecimalToCC(BigInteger HashInt)
        {
            HashInt = BigInteger.Abs(HashInt);
            StringBuilder HashString = new StringBuilder();

            for (int i = 0; i < 5; i++)
            {
                HashString.Insert(0, Alphabet[(int)(HashInt % CC)]);

                while (Alphabet[(int)(HashInt % CC)] == HashString[0])
                {
                    HashInt /= CC;
                }
            }
            return HashString.ToString();
        }

        private void LoadSecretData()
        {
            try
            {
                using var Scope = ScopeFactory.CreateScope();
                var Context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var SecretKeys = Context.ModerData.Select(z => new SecretData { ID = z.ID, Key = z.Key }).ToList();

                SecretData.Clear();

                foreach (var Data in SecretKeys)
                {
                    SecretData.Add(Data);
                }
            }
            catch
            {
                Console.WriteLine("LoadSecretData Error");
            }
        }

        public Task StartAsync(CancellationToken CancellationToken)
        {
            LoadSecretData();
            Timer = new Timer(GenerateTempKeys!, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
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
}
