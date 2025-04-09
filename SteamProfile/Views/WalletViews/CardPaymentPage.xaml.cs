using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;
using System.Collections.Generic;

namespace SteamProfile.Views.WalletViews
{
    public sealed partial class CardPaymentPage : Page
    {
        private CardPaymentViewModel ViewModel { get; set; }

        public CardPaymentPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Dictionary<string, object> parameters)
            {
                // Create and initialize the ViewModel
                ViewModel = new CardPaymentViewModel();
                this.DataContext = ViewModel;

                // Initialize ViewModel with navigation parameters
                ViewModel.Initialize(parameters);

            }
        }

        private void ValidName(object sender, TextChangedEventArgs e)
        {
            ViewModel.ValidateName(OwnerNameTextBox.Text);
        }

        private void ValidNumber(object sender, TextChangedEventArgs e)
        {
            ViewModel.ValidateCardNumber(CardNumberTextBox.Text);
        }

        private void ValidCVV(object sender, TextChangedEventArgs e)
        {
            ViewModel.ValidateCVV(CVVTextBox.Text);
        }

        private void ValidDate(object sender, TextChangedEventArgs e)
        {
            ViewModel.ValidateExpirationDate(ExpirationDateTextBox.Text);
        }

        private async void AddMoneyToAccount(object sender, RoutedEventArgs e)
        {
            if (await ViewModel.ProcessPaymentAsync())
            {
                Frame.GoBack();
            }
        }

        private void CancelPayment(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}