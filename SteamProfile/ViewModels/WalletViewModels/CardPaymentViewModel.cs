using Microsoft.UI.Xaml;
using SteamProfile.Models;
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
        private int _amount;
        private WalletViewModel _walletViewModel;
        private User _user;

        // Observable properties using MVVM Toolkit's source generators
        [ObservableProperty]
        private string _amountText;

        [ObservableProperty]
        private bool _isNameValid;

        [ObservableProperty]
        private bool _isCardNumberValid;

        [ObservableProperty]
        private bool _isCVVValid;

        [ObservableProperty]
        private bool _isDateValid;

        [ObservableProperty]
        private bool _showErrorMessage;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private Visibility _statusMessageVisibility;

        // Computed properties - these must be manually maintained
        public Visibility ErrorMessageVisibility => ShowErrorMessage ? Visibility.Visible : Visibility.Collapsed;

        public bool AreAllFieldsValid => IsNameValid && IsCardNumberValid && IsCVVValid && IsDateValid;

        // Initialization
        public CardPaymentViewModel()
        {
            _statusMessageVisibility = Visibility.Collapsed;
            _showErrorMessage = false;
        }

        public void Initialize(Dictionary<string, object> parameters)
        {
            // Extract parameters
            _amount = parameters.ContainsKey("sum") ? (int)parameters["sum"] : 0;
            _walletViewModel = parameters.ContainsKey("viewModel") ? (WalletViewModel)parameters["viewModel"] : null;
            _user = parameters.ContainsKey("user") ? (User)parameters["user"] : null;

            // Update UI bindings
            AmountText = "Sum: " + _amount.ToString();
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

            // Simple check for 16-digit card number
            IsCardNumberValid = Regex.IsMatch(cardNumber, @"^\d{16}$");
        }

        public void ValidateCVV(string cvv)
        {
            if (string.IsNullOrEmpty(cvv))
            {
                IsCVVValid = false;
                return;
            }

            // Simple check for 3-digit CVV
            IsCVVValid = Regex.IsMatch(cvv, @"^\d{3}$");
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
            string[] date = expirationDate.Split('/');
            int month = int.Parse(date[0]);
            int year = int.Parse(date[1]);
            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year % 100;

            IsDateValid = (year > currentYear) || (year == currentYear && month >= currentMonth);
        }

        // Manually update error message visibility when validation properties change
        partial void OnIsNameValidChanged(bool value) => UpdateErrorMessageVisibility();
        partial void OnIsCardNumberValidChanged(bool value) => UpdateErrorMessageVisibility();
        partial void OnIsCVVValidChanged(bool value) => UpdateErrorMessageVisibility();
        partial void OnIsDateValidChanged(bool value) => UpdateErrorMessageVisibility();

        private void UpdateErrorMessageVisibility()
        {
            ShowErrorMessage = !IsNameValid || !IsCardNumberValid || !IsCVVValid || !IsDateValid;
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
            if (_walletViewModel != null)
            {
                _walletViewModel.AddFunds(_amount);
            }

            StatusMessage = "Payment was performed successfully";
            await Task.Delay(5000);

            return true;
        }
    }
}