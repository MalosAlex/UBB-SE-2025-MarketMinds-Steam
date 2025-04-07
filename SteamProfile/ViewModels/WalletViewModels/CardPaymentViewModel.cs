using Microsoft.UI.Xaml;
using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        // Validation Methods
        public void ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                IsNameValid = false;
                return;
            }
            IsNameValid = name.Split(' ').Length > 1;
        }

        public void ValidateCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
            {
                IsCardNumberValid = false;
                return;
            }

            // Check for 16-digit card number
            IsCardNumberValid = Regex.IsMatch(cardNumber, @"^\d{16}$");
        }

        public void ValidateCVV(string cvv)
        {
            if (string.IsNullOrEmpty(cvv))
            {
                IsCvvValid = false;
                return;
            }

            // Check for 3-digit CVV
            IsCvvValid = Regex.IsMatch(cvv, @"^\d{3}$");
        }

        public void ValidateExpirationDate(string expirationDate)
        {
            if (string.IsNullOrEmpty(expirationDate))
            {
                IsDateValid = false;
                return;
            }

            // Check MM/YY format
            bool isValidDateFormat = Regex.IsMatch(expirationDate, @"^(0[1-9]|1[0-2])\/\d{2}$");

            if (!isValidDateFormat)
            {
                IsDateValid = false;
                return;
            }

            // Check if date is in the future
            string[] dateParts = expirationDate.Split('/');
            int month = int.Parse(dateParts[0]);
            int year = int.Parse(dateParts[1]);
            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year % 100;

            IsDateValid = (year > currentYear) || (year == currentYear && month >= currentMonth);
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