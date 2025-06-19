package com.example.vkr.ui.API;

import android.content.Context;
import androidx.annotation.NonNull;
import androidx.security.crypto.EncryptedSharedPreferences;
import androidx.security.crypto.MasterKey;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class JWTManager
{
    private static final String FILE_NAME = "SecuredData";
    private static final String MASTER_KEY = "MasterKey";
    private static final String SECRET_KEY = "SecretKey";
    private static final String JWT_ACCESS = "jwtAccess";
    private static final String JWT_REFRESH = "jwtRefresh";
    private EncryptedSharedPreferences ESP;

    public JWTManager(Context context)
    {
        try
        {
            MasterKey masterKey = new MasterKey.Builder(context, MASTER_KEY).setKeyScheme(MasterKey.KeyScheme.AES256_GCM).build();

            ESP = (EncryptedSharedPreferences) EncryptedSharedPreferences.create(
                    context,
                    FILE_NAME,
                    masterKey,
                    EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
                    EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
            );
        }
        catch (Exception ignored) {}
    }

    public void SaveTokens(TokenData TokenData)
    {
        if (ESP != null)
        {
            ESP.edit().putString(JWT_ACCESS, TokenData.jwtAccess).apply();
            ESP.edit().putString(JWT_REFRESH, TokenData.jwtRefresh).apply();
        }
    }

    public void SaveSecretKey(String SecretKey)
    {
        if (ESP != null)
        {
            ESP.edit().putString(SECRET_KEY, SecretKey).apply();
        }
    }

   public void SetBearer(CallBack CallBack)
    {
        String Access = GetJWT(JWT_ACCESS);

        if (Access == null)
        {
            CallBack.onFailure();
            return;
        }

        String Bearer = "Bearer " + Access;
        com.example.vkr.ui.API.ApiService ApiService = App.GetApiService();
        Call<Void> CheckCall = ApiService.Check(Bearer);

        CheckCall.enqueue(new Callback<Void>()
        {
            @Override
            public void onResponse(@NonNull Call<Void> Call, @NonNull Response<Void> Response)
            {
                if (Response.code() == 200)
                {
                    App.SetBearer(Bearer);
                    CallBack.onSuccess();
                }
                else if (Response.code() == 401)
                {
                    JWTUpdate(new CallBack()
                    {
                        @Override
                        public void onSuccess()
                        {
                            SetBearer(CallBack);
                        }

                        @Override
                        public void onFailure()
                        {
                            CallBack.onFailure();
                        }
                    });
                }
                else
                {
                    CallBack.onFailure();
                }
            }

            @Override
            public void onFailure(@NonNull Call<Void> Call, @NonNull Throwable T)
            {
                CallBack.onSuccess();;
            }
        });
    }

    private void JWTUpdate(CallBack CallBack)
    {
        String Refresh = GetJWT(JWT_REFRESH);

        if (Refresh == null)
        {
            CallBack.onFailure();
            return;
        }

        com.example.vkr.ui.API.ApiService ApiService = App.GetApiService();
        Call<TokenData> JWTUpdateCall = ApiService.JWTUpdate(Refresh);

        JWTUpdateCall.enqueue(new Callback<TokenData>()
        {
            @Override
            public void onResponse(@NonNull Call<TokenData> Call, @NonNull Response<TokenData> Response)
            {
                if (Response.isSuccessful())
                {
                    SaveTokens(Response.body());
                    CallBack.onSuccess();
                }
                else
                {
                    ESP.edit().clear().apply();
                    CallBack.onFailure();
                }
            }

            @Override
            public void onFailure(@NonNull Call<TokenData> Call, @NonNull Throwable T)
            {
                CallBack.onFailure();
            }
        });
    }
    
    public String GetJWT(String Type)
    {
        if (ESP != null)
        {
            return ESP.getString(Type, null);
        }

        return null;
    }

    public void SetSecretKey()
    {
        if (ESP != null)
        {
            App.SetSecretKey(ESP.getString(SECRET_KEY, null));
        }
    }
}
