using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using BusinessLayer.Models;
using BusinessLayer.Validators;
using System;
using System.Collections.Generic;
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

        public Visibility ErrorMessageVisibility
        {
            get { return ShowErrorMessage ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool AreAllFieldsValid
        {
            get { return IsEmailValid && IsPasswordValid; }
        }

        public PaypalPaymentViewModel()
        {
            ShowErrorMessage = false;
        }

        public void Initialize(int paymentAmount, User currentUser, WalletViewModel currentWalletViewModel)
        {
            Amount = paymentAmount;
            User = currentUser;
            WalletViewModel = currentWalletViewModel;
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

        public void ValidateEmail(string emailAddress)
        {
            // Business logic moved to PaymentValidator
            IsEmailValid = PaymentValidator.IsEmailValid(emailAddress);
            ShowErrorMessage = !IsEmailValid;
        }

        public void ValidatePassword(string password)
        {
            // Business logic moved to PaymentValidator
            IsPasswordValid = PaymentValidator.IsPasswordValid(password);
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