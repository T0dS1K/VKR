package com.example.vkr.ui.dashboard.moder;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

public class DashboardViewModelModer extends ViewModel {

    private final MutableLiveData<String> mText;

    public DashboardViewModelModer() {
        mText = new MutableLiveData<>();
        mText.setValue("ZOV");
    }

    public LiveData<String> getText() {
        return mText;
    }
}