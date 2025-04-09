using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;

namespace SteamProfile.Views.WalletViews
{
    public sealed partial class AddMoneyPage : Page
    {
        private AddMoneyViewModel ViewModel { get; set; }

        public AddMoneyPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is WalletViewModel walletViewModel)
            {
                ViewModel = new AddMoneyViewModel(walletViewModel);
                this.DataContext = ViewModel;
            }
        }

        private void SumToBeAddedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ValidateInput(sumToBeAddedTextBox.Text);
        }

        private void UseCardForPayment(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IsInputValid)
            {
                Frame.Navigate(typeof(CardPaymentPage), ViewModel.CreateNavigationParameters());
            }
        }

        private void UsePaypalForPayment(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IsInputValid)
            {
                Frame.Navigate(typeof(PaypalPaymentPage), ViewModel.CreateNavigationParameters());
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}