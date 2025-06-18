package com.example.vkr.ui.home.moder;

import android.os.Bundle;
import android.view.View;
import android.graphics.Color;
import android.view.ViewGroup;
import android.graphics.Bitmap;
import android.view.LayoutInflater;
import androidx.lifecycle.Observer;
import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;
import com.example.vkr.databinding.FragmentHomeModerBinding;

public class HomeFragmentModer extends Fragment
{
    private FragmentHomeModerBinding binding;

    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        HomeViewModelModer HomeViewModel =
                new ViewModelProvider(this).get(HomeViewModelModer.class);

        binding = FragmentHomeModerBinding.inflate(inflater, container, false);
        View root = binding.getRoot();

        HomeViewModel.GetQR().observe(getViewLifecycleOwner(), new Observer<Bitmap>()
        {
            @Override
            public void onChanged(Bitmap QR)
            {
                if (QR != null)
                {
                    binding.QRImageView.setImageBitmap(QR);
                }
            }
        });

        HomeViewModel.GetTimer().observe(getViewLifecycleOwner(), new Observer<Long>()
        {
            @Override
            public void onChanged(Long Timer)
            {
                if (Timer != null)
                {
                    float maxTimer = 10;
                    float minTimer = 0;

                    float progress = (Timer - minTimer) / (maxTimer - minTimer);
                    progress = Math.max(0f, Math.min(1f, progress));

                    float[] hsv = {0f, 1f, 1 - progress};

                    binding.Timer.setTextColor(Color.HSVToColor(hsv));
                    binding.Timer.setText(String.valueOf(Timer));
                }
            }
        });

        return root;
    }

    @Override
    public void onDestroyView()
    {
        super.onDestroyView();
        binding = null;
    }
}