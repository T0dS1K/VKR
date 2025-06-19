package com.example.vkr;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.widget.EditText;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import com.example.vkr.ui.API.ApiService;
import com.example.vkr.ui.API.App;
import com.example.vkr.ui.API.Crypto;
import com.example.vkr.ui.API.Data;
import com.example.vkr.ui.API.JWTManager;
import com.example.vkr.ui.API.PDH;
import com.example.vkr.ui.API.TokenData;
import java.math.BigInteger;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

ublic class AuthNActivity extends AppCompatActivity
{
    private ApiService ApiService;
    private JWTManager JWTManager;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_authn);
        ApiService = App.GetApiService();
        JWTManager = new JWTManager(this);

        findViewById(R.id.SingIn).setOnClickListener(z ->
        {
            String Login = ((EditText) findViewById(R.id.LoginBox)).getText().toString();
            String Password = ((EditText) findViewById(R.id.PasswordBox)).getText().toString();

            if (Login.length() >= 4 && Password.length() >= 4)
            {
                SignInCall(new Data(Login, Password));
            }
            else
            {
                Toast.makeText(this, "СЛИШКОМ КОРОТКИЙ ЛОГИН ИЛИ ПАРОЛЬ", Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void SignInCall(Data Data)
    {
        Call<TokenData> SignInCall = ApiService.SignIn(Data);
        SignInCall.enqueue(new Callback<TokenData>()
        {
            @Override
            public void onResponse(@NonNull Call<TokenData> Call, @NonNull Response<TokenData> Response)
            {
                if (Response.isSuccessful() && Response.body() != null)
                {
                    TokenData TokenData = Response.body();
                    String Role = GetRole(Response.body().jwtAccess);
                    SharedPreferences Prefs = getSharedPreferences("AppPrefs", MODE_PRIVATE);
                    Prefs.edit().putString("Role", Role).apply();

                    if (Role.equals("User"))
                    {
                        StartMainActivity();
                    }
                    else if (Role.equals("Moder"))
                    {
                        BigInteger a = Crypto.GenNum(32);
                        BigInteger p = Crypto.GenNum(32);
                        BigInteger A = Crypto.PowWithMod(BigInteger.valueOf(7), a, p);

                        Call<String> SetKeyCall = ApiService.SetKey("Bearer " + TokenData.jwtAccess, new PDH(A.toString(), p.toString()));
                        SetKeyCall.enqueue(new Callback<String>()
                        {
                            @Override
                            public void onResponse(@NonNull Call<String> Call, @NonNull Response<String> Response)
                            {
                                if (Response.isSuccessful() && Response.body() != null)
                                {
                                    JWTManager.SaveTokens(TokenData);
                                    JWTManager.SaveSecretKey(Crypto.PowWithMod(new BigInteger(Response.body()), a, p).toString());
                                    StartMainActivity();
                                }
                            }

                            @Override
                            public void onFailure(@NonNull Call<String> Call, @NonNull Throwable T) {}
                        });
                    }
                }
                else
                {
                    Toast.makeText(AuthNActivity.this, "НЕВЕРНЫЙ ЛОГИН ИЛИ ПАРОЛЬ", Toast.LENGTH_LONG).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<TokenData> Call, @NonNull Throwable T)
            {
                Toast.makeText(AuthNActivity.this, "СОЕДИНЕНИЕ НЕ УСТАНОВЛЕНО", Toast.LENGTH_LONG).show();
            }
        });
    }

    private void StartMainActivity()
    {
        startActivity(new Intent(this, MainActivity.class));
        finish();
    }

    private String GetRole(String Token)
    {
        String[] parts = Token.split("\\.");
        String payloadJson = new String(Base64.getUrlDecoder().decode(parts[1]), StandardCharsets.UTF_8);
        return payloadJson.split("\"Role\":\"")[1].split("\"")[0];
    }
}
