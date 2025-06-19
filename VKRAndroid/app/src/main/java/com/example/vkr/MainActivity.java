package com.example.vkr;

import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.content.SharedPreferences;
import com.example.vkr.ui.API.CallBack;
import com.example.vkr.ui.API.JWTManager;
import com.google.android.material.bottomnavigation.BottomNavigationView;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.navigation.NavController;
import androidx.navigation.Navigation;
import androidx.navigation.ui.AppBarConfiguration;
import androidx.navigation.ui.NavigationUI;
import com.example.vkr.databinding.ActivityMainBinding;

public class MainActivity extends AppCompatActivity
{
    private static final int REQUEST_CAMERA_PERMISSION = 100;
    private ActivityMainBinding binding;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        SharedPreferences SP = getSharedPreferences("AppPrefs", MODE_PRIVATE);
        JWTManager JWTManager = new JWTManager(this);
        String Role = SP.getString("Role", null);

        binding = ActivityMainBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        if ("User".equals(Role))
        {
            SetupNavigation(R.navigation.user_navigation);
            CheckCameraPermission();
        }
        else if ("Moder".equals(Role))
        {
            JWTManager.SetSecretKey();
            SetupNavigation(R.navigation.moder_navigation);
        }
        else
        {
            StartAuthNActivity();
        }

        JWTManager.SetBearer(new CallBack()
        {
            @Override
            public void onSuccess()
            {
                runOnUiThread(() ->{});
            }

            @Override
            public void onFailure()
            {
                runOnUiThread(() ->
                {
                    try
                    {
                        StartAuthNActivity();
                    }
                    catch (Exception e)
                    {
                        finishAffinity();
                    }
                });
            }
        });
    }

    private void StartAuthNActivity()
    {
        startActivity(new Intent(MainActivity.this, AuthNActivity.class));
        finish();
    }

    private void CheckCameraPermission()
    {
        if (ContextCompat.checkSelfPermission(this, android.Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED)
        {
            ActivityCompat.requestPermissions(this, new String[]{android.Manifest.permission.CAMERA}, REQUEST_CAMERA_PERMISSION);
        }
    }

    private void SetupNavigation(int navigation)
    {
        BottomNavigationView navView = binding.navView;
        NavController navController = Navigation.findNavController(this, R.id.nav_host_fragment_activity_main);

        AppBarConfiguration appBarConfiguration = new AppBarConfiguration.Builder(
                R.id.navigation_home,
                R.id.navigation_dashboard,
                R.id.navigation_notifications
        ).build();

        navView.getMenu().clear();
        navController.setGraph(navigation);
        navView.inflateMenu(R.menu.bottom_nav_menu);

        NavigationUI.setupActionBarWithNavController(this, navController, appBarConfiguration);
        NavigationUI.setupWithNavController(navView, navController);
    }
}
