package com.example.vkr.ui.API;

import android.app.Application;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class App extends Application
{
    private static final String BASE_URL = "http://192.168.0.225:8000/";
    private static ApiService ApiService;
    private static Retrofit Retrofit;
    private static String SecretKey;
    private static String Bearer;

    @Override
    public void onCreate()
    {
        super.onCreate();
        InitializeRetrofit();
    }

    private void InitializeRetrofit()
    {
        Retrofit = new Retrofit.Builder().baseUrl(BASE_URL).addConverterFactory(GsonConverterFactory.create()).build();
        ApiService = Retrofit.create(ApiService.class);
    }

    public static ApiService GetApiService()
    {
        if (ApiService == null)
        {
            throw new IllegalStateException();
        }
        return ApiService;
    }

    public static Retrofit GetRetrofit()
    {
        if (Retrofit == null)
        {
            throw new IllegalStateException();
        }

        return Retrofit;
    }

    public static void SetBearer(String Token)
    {
        App.Bearer = Bearer;
    }

    public static String GetBearer()
    {
        return Bearer;
    }

    public static void SetSecretKey(String Key)
    {
        SecretKey = Key;
    }

    public static String GetSecretKey()
    {
        return SecretKey;
    }
}
