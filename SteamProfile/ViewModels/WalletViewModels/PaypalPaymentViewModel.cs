﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using SteamProfile.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamProfile.ViewModels
{
    public partial class PaypalPaymentViewModel : ObservableObject
    {
        [ObservableProperty]
        private int amount;

        [ObservableProperty]
        private WalletViewModel walletViewModel;

        [ObservableProperty]
        private User user;

        [ObservableProperty]
        private string amountText;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private bool isEmailValid;

        [ObservableProperty]
        private bool isPasswordValid;

        [ObservableProperty]
        private bool showErrorMessage;

        [ObservableProperty]
        private string statusMessage;

        [ObservableProperty]
        private Visibility statusMessageVisibility = Visibility.Collapsed;

        public Visibility ErrorMessageVisibility => ShowErrorMessage ? Visibility.Visible : Visibility.Collapsed;

        public bool AreAllFieldsValid => IsEmailValid && IsPasswordValid;

        public PaypalPaymentViewModel()
        {
            ShowErrorMessage = false;
        }

        public void Initialize(int amount, User user, WalletViewModel walletViewModel)
        {
            Amount = amount;
            User = user;
            WalletViewModel = walletViewModel;
            AmountText = "Sum: " + Amount.ToString();
        }

        public void Initialize(Dictionary<string, object> parameters)
        {
            Amount = parameters.ContainsKey("sum") ? (int)parameters["sum"] : 0;
            WalletViewModel = parameters.ContainsKey("viewModel") ? (WalletViewModel)parameters["viewModel"] : null;
            User = parameters.ContainsKey("user") ? (User)parameters["user"] : null;
            AmountText = "Sum: " + Amount.ToString();
        }

        partial void OnShowErrorMessageChanged(bool oldValue, bool newValue)
        {
            OnPropertyChanged(nameof(ErrorMessageVisibility));
        }

        public void ValidateEmail(string email)
        {
            IsEmailValid = !string.IsNullOrEmpty(email) && Regex.IsMatch(email, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
            ShowErrorMessage = !IsEmailValid;
        }

        public void ValidatePassword(string password)
        {
            IsPasswordValid = !string.IsNullOrEmpty(password) && Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            ShowErrorMessage = !IsPasswordValid;
        }

        public async Task<bool> ProcessPaymentAsync()
        {
            if (!AreAllFieldsValid)
            {
                ShowErrorMessage = true;
                return false;
            }

            ShowErrorMessage = false;
            StatusMessageVisibility = Visibility.Visible;
            StatusMessage = "Processing...";
            await Task.Delay(1000);

            WalletViewModel?.AddFunds(Amount);
            StatusMessage = "Payment was performed successfully";
            await Task.Delay(5000);

            return true;
        }
    }
}
