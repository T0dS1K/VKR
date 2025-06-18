package com.example.vkr.ui.API;

import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.PATCH;
import retrofit2.http.POST;
import retrofit2.http.Body;

public interface ApiService
{
    @POST("AuthN/SignIn")
    Call<TokenData> SignIn(@Body Data Data);

    @POST("AuthN/JWTUpdate")
    Call<TokenData> JWTUpdate(@Body String Token);

    @GET("AuthN/Check")
    Call<Void> Check(@Header("Authorization") String Token);

    @POST("Moder/SetKey")
    Call<String> SetKey(@Header("Authorization") String Token, @Body PDH PDH);

    @POST("User/SetMark")
    Call<Void> SetMark(@Header("Authorization") String Token, @Body String Mark);
}