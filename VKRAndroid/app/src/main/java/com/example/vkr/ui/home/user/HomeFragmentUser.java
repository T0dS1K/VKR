package com.example.vkr.ui.home.user;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;

import com.example.vkr.ui.API.App;
import com.example.vkr.R;
import com.journeyapps.barcodescanner.BarcodeResult;
import com.journeyapps.barcodescanner.DecoratedBarcodeView;
import com.journeyapps.barcodescanner.BarcodeCallback;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class HomeFragmentUser extends Fragment
{
    private DecoratedBarcodeView barcodeView;
    private FrameLayout scannerContainer;
    private HomeViewModelUser viewModel;
    private TextView resultTextView;
    private Button clearButton;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState)
    {
        return inflater.inflate(R.layout.fragment_home, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState)
    {
        super.onViewCreated(view, savedInstanceState);
        viewModel = new ViewModelProvider(this).get(HomeViewModelUser.class);

        scannerContainer = view.findViewById(R.id.scannerContainer);
        resultTextView = view.findViewById(R.id.resultTextView);
        clearButton = view.findViewById(R.id.clearButton);

        ScannerQR();
        SetMark();
        ClearButton();
    }

    private void ScannerQR()
    {
        barcodeView = new DecoratedBarcodeView(requireContext());
        barcodeView.setStatusText(null);
        barcodeView.getViewFinder().setVisibility(View.GONE);
        scannerContainer.addView(barcodeView);

        barcodeView.decodeContinuous(new BarcodeCallback()
        {
            @Override
            public void barcodeResult(BarcodeResult result)
            {
                if (result.getText() != null)
                {
                    requireActivity().runOnUiThread(() ->
                    {
                        viewModel.HandleScanResult(result.getText());
                    });
                }
            }
        });
    }

    private void SetMark()
    {
        viewModel.getScannedText().observe(getViewLifecycleOwner(), Code ->
        {
            if (Code != null)
            {
                if (ValidQR(Code))
                {
                    resultTextView.setText("...");

                    com.example.vkr.ui.API.ApiService ApiService = App.GetApiService();
                    Call<Void> SetMarkCall = ApiService.SetMark(App.GetBearer(), Code);

                    SetMarkCall.enqueue(new Callback<Void>()
                    {
                        @Override
                        public void onResponse(Call<Void> Call, Response<Void> Response)
                        {
                            if (Response.code() == 200)
                            {
                                resultTextView.setText("УСПЕШНО");
                            }
                            else if (Response.code() == 204)
                            {
                                resultTextView.setText("ОТМЕТКА АКТИВНА");
                            }
                            else
                            {
                                resultTextView.setText("ОШИБКА");
                            }
                        }

                        @Override
                        public void onFailure(Call<Void> Call, Throwable T)
                        {
                            resultTextView.setText("НЕ В СЕТИ");
                        }
                    });

                }
                else
                {
                    resultTextView.setText("НЕВЕРНЫЙ КОД");
                }
            }
        });
    }

    private void ClearButton()
    {
        clearButton.setOnClickListener(z ->
        {
            resultTextView.setText("");
            viewModel.ResetScanner();
        });
    }

    private boolean ValidQR(String Code)
    {
        return Code.matches("[0-9A-Z]{5}");
    }

    @Override
    public void onResume()
    {
        super.onResume();
        barcodeView.resume();
    }

    @Override
    public void onPause()
    {
        super.onPause();
        barcodeView.pause();
    }
}