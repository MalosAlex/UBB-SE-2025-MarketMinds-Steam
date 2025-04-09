using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using BusinessLayer.Models;
using SteamProfile.ViewModels;

namespace SteamProfile.Views.WalletViews
{
    /// <summary>
    /// Payment page for PayPal transactions
    /// </summary>
    public sealed partial class PaypalPaymentPage : Page
    {
        public PaypalPaymentViewModel ViewModel { get; } = new PaypalPaymentViewModel();

        public PaypalPaymentPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Dictionary<string, object> parameters)
            {
                ViewModel.Initialize(parameters);
            }
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ValidateEmail(EmailTextBox.Text);
        }

        private void PasswordText_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.ValidatePassword(PasswordText.Password);
        }

        private async void AddMoneyButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = await ViewModel.ProcessPaymentAsync();
            if (success)
            {
                Frame.GoBack();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}