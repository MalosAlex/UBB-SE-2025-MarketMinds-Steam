using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;
using System;
using SteamProfile.Views.WalletViews;
using SteamProfile.Services;

namespace SteamProfile.Views
{
    public sealed partial class WalletPage : Page
    {
        private WalletViewModel ViewModel { get; }

        public WalletPage()
        {
            this.InitializeComponent();
            ViewModel = new WalletViewModel(App.WalletService);
            this.DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Refresh wallet data when navigating to this page
            ViewModel.RefreshWalletData();
        }

        private void AddMoneyButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddMoneyPage), ViewModel);
        }

        private void AddPointsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddPointsPage), ViewModel);
        }
        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }


    }
}