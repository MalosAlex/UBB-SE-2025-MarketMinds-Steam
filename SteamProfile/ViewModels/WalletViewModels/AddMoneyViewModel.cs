using Microsoft.UI.Xaml;
using SteamProfile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SteamProfile.ViewModels
{
    public partial class AddMoneyViewModel : ObservableObject
    {
        private readonly WalletViewModel _walletViewModel;
        private readonly List<char> _digitsAsChar = new() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private const int MAX_AMOUNT = 500;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ErrorMessageVisibility))]
        private bool _showErrorMessage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PaymentButtonsEnabled))]
        private bool _isInputValid;

        [ObservableProperty]
        private string _amountToAdd;

        public ICommand AddFundsCommand { get; }

        public Visibility ErrorMessageVisibility => ShowErrorMessage ? Visibility.Visible : Visibility.Collapsed;

        public bool PaymentButtonsEnabled => IsInputValid;

        public AddMoneyViewModel(WalletViewModel walletViewModel)
        {
            _walletViewModel = walletViewModel ?? throw new ArgumentNullException(nameof(walletViewModel));
            _isInputValid = false;
            _showErrorMessage = false;

            // Initialize the command
            AddFundsCommand = new RelayCommand(ProcessAddFunds, () => IsInputValid);
        }

        // Called when AmountToAdd property changes
        partial void OnAmountToAddChanged(string value)
        {
            ValidateInput(value);
            // Force command to reevaluate CanExecute
            (AddFundsCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }

        public void ValidateInput(string input)
        {
            ShowErrorMessage = false;
            IsInputValid = false;

            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            // Check if input is too long
            if (input.Length > 3)
            {
                ShowErrorMessage = true;
                return;
            }

            // Check if input contains only digits
            if (input.Any(c => !_digitsAsChar.Contains(c)))
            {
                ShowErrorMessage = true;
                return;
            }

            // Check if amount is within limits
            if (int.TryParse(input, out int amount))
            {
                if (amount > MAX_AMOUNT || amount <= 0)
                {
                    ShowErrorMessage = true;
                    return;
                }

                IsInputValid = true;
            }
            else
            {
                ShowErrorMessage = true;
            }
        }

        private void ProcessAddFunds()
        {
            if (!IsInputValid || string.IsNullOrEmpty(AmountToAdd))
                return;

            if (int.TryParse(AmountToAdd, out int amount))
            {
                _walletViewModel.AddFunds(amount);
                AmountToAdd = string.Empty;
                IsInputValid = false;
            }
        }

        public Dictionary<string, object> CreateNavigationParameters()
        {
            return new Dictionary<string, object>
            {
                { "sum", int.Parse(AmountToAdd) },
                { "viewModel", _walletViewModel }
            };
        }
    }
}