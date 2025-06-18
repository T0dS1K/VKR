package com.example.vkr.ui.home.user;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

public class HomeViewModelUser extends ViewModel
{
    private final MutableLiveData<String> scannedText = new MutableLiveData<>();
    private boolean bool = true;

    public LiveData<String> getScannedText()
    {
        return scannedText;
    }

    public void HandleScanResult(String text)
    {
        if (!text.equals(scannedText.getValue()) && bool)
        {
            bool = false;
            scannedText.setValue(text);
        }
    }

    public void ResetScanner()
    {
        bool = true;
        scannedText.setValue(null);
    }
}