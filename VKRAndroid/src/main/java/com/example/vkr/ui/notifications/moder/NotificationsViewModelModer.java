package com.example.vkr.ui.notifications.moder;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

public class NotificationsViewModelModer extends ViewModel {

    private final MutableLiveData<String> mText;

    public NotificationsViewModelModer() {
        mText = new MutableLiveData<>();
        mText.setValue("ZOV");
    }

    public LiveData<String> getText() {
        return mText;
    }
}