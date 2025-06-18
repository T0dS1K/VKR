package com.example.vkr.ui.API;

import java.math.BigInteger;
import java.util.Random;

public class Crypto
{
    public static BigInteger PowWithMod(BigInteger G, BigInteger a, BigInteger P)
    {
        BigInteger result = BigInteger.ONE;
        BigInteger value = G.mod(P);

        while (a.compareTo(BigInteger.ZERO) > 0)
        {
            if (a.mod(BigInteger.valueOf(2)).equals(BigInteger.ONE))
            {
                result = result.multiply(value).mod(P);
            }

            value = value.multiply(value).mod(P);
            a = a.divide(BigInteger.valueOf(2));
        }

        return result;
    }

    public static BigInteger GenNum(int byteSize)
    {
        BigInteger num = BigInteger.ONE;
        Random random = new Random();

        for (int i = 0; i < byteSize; i++)
        {
            byte Z = (byte)random.nextInt(256);

            num = num.shiftLeft(8);

            if (i == 0)
            {
                num = num.shiftRight(1);
            }

            num = num.or(BigInteger.valueOf(Z & 0xFF));
        }

        return num;
    }
}
