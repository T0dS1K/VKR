using System.Numerics;

namespace VKRServer
{
    public class Crypto
    {
        public static int Pow(int G, int a)
        {
            int Result = 1;

            while (a > 0)
            {
                if (a % 2 == 1)
                {
                    Result *= G;
                }

                G *= G;
                a /= 2;
            }

            return Result;
        }

        public static BigInteger PowWithMod(BigInteger G, BigInteger a, BigInteger P)
        {
            BigInteger Result = 1;
            BigInteger Value = G % P;

            while (a > 0)
            {
                if (a % 2 == 1)
                {
                    Result = Result * Value % P;
                }

                Value = Value * Value % P;

                a /= 2;
            }

            return Result;
        }

        public static BigInteger GenNum(int ByteSize)
        {
            BigInteger Num = 1;

            for (int i = 0; i < ByteSize; i++)
            {
                Random Random = new Random();
                byte ez = (byte)Random.Next(0, 256);

                Num = Num << 8;

                if (i == 0)
                {
                    Num = Num >>> 1;
                }

                Num |= ez;
            }

            return Num;
        }
    }
}
