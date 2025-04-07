using Microsoft.UI.Xaml;
using BusinessLayer.Models;
using BusinessLayer.Validators;
using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SteamProfile.ViewModels
{
    public partial class AddMoneyViewModel : ObservableObject
    {
        private readonly WalletViewModel walletViewModel;
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
            // Business logic moved to PaymentValidator
            ShowErrorMessage = false;
            IsInputValid = false;

            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            IsInputValid = PaymentValidator.IsMonetaryAmountValid(input, MAXIMUM_AMOUNT);
            ShowErrorMessage = !IsInputValid;
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