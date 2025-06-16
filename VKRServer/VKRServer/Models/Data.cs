using System.Collections.Immutable;
using System.Numerics;

namespace VKRServer.Models
{
    public class Data
    {
        public string? Login { get; set; }
        public string? Password { get; set; }
    }

    public class PDH
    {
        public string? A { get; set; }
        public string? p { get; set; }
    }

    public class SecretData
    {
        public int ID { get; set; }
        public BigInteger Key { get; set; }
    }

    public class Refresh
    {
        public string? Token { get; set; }
    }

    public class TempDataCon
    {
        public ImmutableDictionary<int, string> Data { get; set; } = ImmutableDictionary<int, string>.Empty;
    }
}
