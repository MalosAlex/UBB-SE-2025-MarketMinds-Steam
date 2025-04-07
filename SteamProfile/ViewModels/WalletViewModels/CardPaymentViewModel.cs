using Microsoft.UI.Xaml;
using BusinessLayer.Models;
using BusinessLayer.Validators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SteamProfile.ViewModels
{
    public partial class CardPaymentViewModel : ObservableObject
    {
        // Private fields
        private int amount;
        private WalletViewModel walletViewModel;
        private User user;

        // Observable properties using MVVM Toolkit's source generators
        [ObservableProperty]
        private string amountText;

        [ObservableProperty]
        private bool isNameValid;

        [ObservableProperty]
        private bool isCardNumberValid;

        [ObservableProperty]
        private bool isCvvValid;

        [ObservableProperty]
        private bool isDateValid;

        [ObservableProperty]
        private bool showErrorMessage;

        [ObservableProperty]
        private string statusMessage;

        [ObservableProperty]
        private Visibility statusMessageVisibility;

        // Computed properties - these must be manually maintained
        public Visibility ErrorMessageVisibility => ShowErrorMessage ? Visibility.Visible : Visibility.Collapsed;

        public bool AreAllFieldsValid => IsNameValid && IsCardNumberValid && IsCvvValid && IsDateValid;

        // Initialization
        public CardPaymentViewModel()
        {
            statusMessageVisibility = Visibility.Collapsed;
            showErrorMessage = false;
        }

        public void Initialize(Dictionary<string, object> parameters)
        {
            // Extract parameters
            amount = parameters.ContainsKey("sum") ? (int)parameters["sum"] : 0;
            walletViewModel = parameters.ContainsKey("viewModel") ? (WalletViewModel)parameters["viewModel"] : null;
            user = parameters.ContainsKey("user") ? (User)parameters["user"] : null;

            // Update UI bindings
            AmountText = "Sum: " + amount.ToString();
        }

        // Validation Methods - Business logic moved to PaymentValidator
        public void ValidateName(string name)
        {
            IsNameValid = PaymentValidator.IsCardNameValid(name);
            UpdateErrorMessageVisibility();
        }

        public void ValidateCardNumber(string cardNumber)
        {
            IsCardNumberValid = PaymentValidator.IsCardNumberValid(cardNumber);
            UpdateErrorMessageVisibility();
        }

        public void ValidateCVV(string cvv)
        {
            IsCvvValid = PaymentValidator.IsCvvValid(cvv);
            UpdateErrorMessageVisibility();
        }

        public void ValidateExpirationDate(string expirationDate)
        {
            IsDateValid = PaymentValidator.IsExpirationDateValid(expirationDate);
            UpdateErrorMessageVisibility();
        }

        // Manually update error message visibility when validation properties change
        partial void OnIsNameValidChanged(bool value) => UpdateErrorMessageVisibility();
        partial void OnIsCardNumberValidChanged(bool value) => UpdateErrorMessageVisibility();
        partial void OnIsCvvValidChanged(bool value) => UpdateErrorMessageVisibility();
        partial void OnIsDateValidChanged(bool value) => UpdateErrorMessageVisibility();

        private void UpdateErrorMessageVisibility()
        {
            ShowErrorMessage = !IsNameValid || !IsCardNumberValid || !IsCvvValid || !IsDateValid;
            OnPropertyChanged(nameof(ErrorMessageVisibility));
        }

        // Payment Processing
        public async Task<bool> ProcessPaymentAsync()
        {
            if (!AreAllFieldsValid)
            {
                ShowErrorMessage = true;
                return false;
            }

            // Update UI to show processing
            StatusMessageVisibility = Visibility.Visible;
            StatusMessage = "Processing...";

            await Task.Delay(1000);

            // Update wallet balance via the WalletViewModel
            if (walletViewModel != null)
            {
                walletViewModel.AddFunds(amount);
            }

            StatusMessage = "Payment was performed successfully";
            await Task.Delay(5000);

            return true;
        }
    }
}