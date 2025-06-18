package com.example.vkr.ui.notifications.user;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

public class NotificationsViewModelUser extends ViewModel {

    private final MutableLiveData<String> mText;

    public NotificationsViewModelUser() {
        mText = new MutableLiveData<>();
        mText.setValue("ZOV");
    }

    public LiveData<String> getText() {
        return mText;
    }
}