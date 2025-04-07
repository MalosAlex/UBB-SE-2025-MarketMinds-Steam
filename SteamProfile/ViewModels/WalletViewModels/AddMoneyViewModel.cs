using Microsoft.UI.Xaml;
using BusinessLayer.Models;
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
        private readonly WalletViewModel walletViewModel;
        private readonly List<char> allowedDigits = new() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private const int MAXIMUM_AMOUNT = 500;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ErrorMessageVisibility))]
        private bool showErrorMessage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PaymentButtonsEnabled))]
        private bool isInputValid;

        [ObservableProperty]
        private string amountToAdd;

        public ICommand AddFundsCommand { get; }

        public Visibility ErrorMessageVisibility => ShowErrorMessage ? Visibility.Visible : Visibility.Collapsed;

        public bool PaymentButtonsEnabled => IsInputValid;

        public AddMoneyViewModel(WalletViewModel walletViewModel)
        {
            this.walletViewModel = walletViewModel ?? throw new ArgumentNullException(nameof(walletViewModel));
            isInputValid = false;
            showErrorMessage = false;

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
            if (input.Any(character => !allowedDigits.Contains(character)))
            {
                ShowErrorMessage = true;
                return;
            }

            // Check if amount is within limits
            if (int.TryParse(input, out int amount))
            {
                if (amount > MAXIMUM_AMOUNT || amount <= 0)
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
                walletViewModel.AddFunds(amount);
                AmountToAdd = string.Empty;
                IsInputValid = false;
            }
        }

        public Dictionary<string, object> CreateNavigationParameters()
        {
            return new Dictionary<string, object>
            {
                { "sum", int.Parse(AmountToAdd) },
                { "viewModel", walletViewModel }
            };
        }
    }
}