package com.example.vkr.ui.home.moder;

import javax.crypto.Mac;
import android.os.Handler;
import java.nio.ByteOrder;
import java.nio.ByteBuffer;
import java.math.BigInteger;
import java.util.Collections;
import android.graphics.Color;
import android.graphics.Bitmap;
import android.os.Looper;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.ViewModel;
import com.example.vkr.ui.API.App;
import com.google.zxing.BarcodeFormat;
import com.google.zxing.EncodeHintType;
import javax.crypto.spec.SecretKeySpec;
import java.security.InvalidKeyException;
import com.google.zxing.common.BitMatrix;
import com.google.zxing.MultiFormatWriter;
import androidx.lifecycle.MutableLiveData;
import java.security.NoSuchAlgorithmException;

public class HomeViewModelModer extends ViewModel
{
    private static final String Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private final Handler Handler = new Handler(Looper.getMainLooper());
    private final BigInteger Key = new BigInteger(App.GetSecretKey());
    private final MutableLiveData<Bitmap> QRBitmap;
    private final MutableLiveData<Long> Timer;
    private static final int TimeOut = 31000;
    private static final int Delay = 1000;
    private static final int Size = 200;
    private static final int Error = 10;

    public HomeViewModelModer()
    {
        QRBitmap = new MutableLiveData<>();
        Timer = new MutableLiveData<>();
        Handler.post(TimerUpdater);
        Handler.post(QRUpdater);
    }

    private final Runnable QRUpdater = new Runnable()
    {
        @Override
        public void run()
        {
            QRBitmap.setValue(GenQR());
            Handler.postDelayed(this, TimeOut + Error - (System.currentTimeMillis() % TimeOut));
        }
    };

    private final Runnable TimerUpdater = new Runnable()
    {
        @Override
        public void run()
        {
            Timer.setValue((TimeOut - (System.currentTimeMillis() % TimeOut)) / Delay);
            long nextExecutionTime = (Delay + Error - (System.currentTimeMillis() % Delay));
            Handler.postDelayed(this, nextExecutionTime);
        }
    };

    private Bitmap GenQR()
    {
        try
        {
            String code = GenCode();
            BitMatrix bitMatrix = new MultiFormatWriter().encode(
                    code,
                    BarcodeFormat.QR_CODE,
                    Size,
                    Size,
                    Collections.singletonMap(EncodeHintType.MARGIN, 0)
            );

            int[] pixels = new int[Size * Size];

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    pixels[x + y * Size] = bitMatrix.get(x, y) ? Color.BLACK : Color.WHITE;
                }
            }

            return Bitmap.createBitmap(pixels, Size, Size, Bitmap.Config.RGB_565);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public String GenCode() throws NoSuchAlgorithmException, InvalidKeyException
    {
        long UnixTime = System.currentTimeMillis();

        Mac HMAC = Mac.getInstance("HMACSHA256");
        SecretKeySpec secretKeySpec = new SecretKeySpec(Reverse(Key.toByteArray()), "HMACSHA256");
        HMAC.init(secretKeySpec);

        byte[] hash = HMAC.doFinal(ByteBuffer.allocate(8).order(ByteOrder.LITTLE_ENDIAN).putLong((UnixTime - UnixTime % TimeOut) / 1000).array());
        return DecimalToCC(new BigInteger(Reverse(hash)));
    }

    private String DecimalToCC(BigInteger HashInt)
    {
        HashInt = HashInt.abs();
        StringBuilder HashString = new StringBuilder();

        for (int i = 0; i < 5; i++)
        {
            int sym = HashInt.mod(BigInteger.valueOf(Alphabet.length())).intValue();
            HashString.insert(0, Alphabet.charAt(sym));
            BigInteger newHashInt = HashInt.divide(BigInteger.valueOf(Alphabet.length()));

            while (newHashInt.mod(BigInteger.valueOf(Alphabet.length())).intValue() == sym && !newHashInt.equals(BigInteger.ZERO))
            {
                HashInt = newHashInt;
                newHashInt = HashInt.divide(BigInteger.valueOf(Alphabet.length()));
            }
            
            HashInt = newHashInt;

            if (HashInt.equals(BigInteger.ZERO) && i < 4)
            {
                break;
            }
        }

        return HashString.toString();
    }

    private byte[] Reverse(byte[] Array)
    {
        int size = Array.length;
        byte[] NewArray = new byte[size];

        for (int i = 0; i < size; i++)
        {
            NewArray[i] = Array[size - 1 - i];
        }

        return NewArray;
    }

    public LiveData<Bitmap> GetQR()
    {
        return QRBitmap;
    }

    public LiveData<Long> GetTimer()
    {
        return Timer;
    }

    @Override
    protected void onCleared()
    {
        super.onCleared();
        Handler.removeCallbacks(QRUpdater);
        Handler.removeCallbacks(TimerUpdater);
    }
}
