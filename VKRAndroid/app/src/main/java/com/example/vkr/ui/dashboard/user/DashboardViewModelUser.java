package com.example.vkr.ui.dashboard.user;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

public class DashboardViewModelUser extends ViewModel {

    private final MutableLiveData<String> mText;

    public DashboardViewModelUser() {
        mText = new MutableLiveData<>();
        mText.setValue("ZOV");
    }

    public LiveData<String> getText() {
        return mText;
    }
}