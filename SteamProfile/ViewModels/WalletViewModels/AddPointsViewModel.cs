using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SteamProfile.ViewModels
{
    public partial class AddPointsViewModel : ObservableObject
    {
        private readonly WalletViewModel _walletViewModel;
        private readonly PointsOffersRepository _offersRepository;
        private readonly Frame _navigationFrame;

        [ObservableProperty]
        private int userPoints;

        [ObservableProperty]
        private bool isProcessing;

        public ObservableCollection<PointsOffer> PointsOffers { get; }
        public ICommand PurchasePointsCommand { get; }

        public AddPointsViewModel(WalletViewModel walletViewModel, PointsOffersRepository offersRepository, Frame navigationFrame)
        {
            _walletViewModel = walletViewModel ?? throw new ArgumentNullException(nameof(walletViewModel));
            _offersRepository = offersRepository ?? throw new ArgumentNullException(nameof(offersRepository));
            _navigationFrame = navigationFrame ?? throw new ArgumentNullException(nameof(navigationFrame));

            PointsOffers = new ObservableCollection<PointsOffer>(_offersRepository.Offers);
            UserPoints = _walletViewModel.Points;
            IsProcessing = false;

            PurchasePointsCommand = new RelayCommand<PointsOffer>(BuyPoints, CanBuyPoints);
        }

        private bool CanBuyPoints(PointsOffer offer) => offer != null && !IsProcessing;

        private async void BuyPoints(PointsOffer offer)
        {
            if (offer == null)
                return;

            IsProcessing = true;

            try
            {
                bool success = await _walletViewModel.AddPoints(offer);
                if (success)
                {
                    UserPoints = _walletViewModel.Points;
                    _walletViewModel.RefreshWalletData();
                    await ShowSuccessMessageAsync(offer);
                    NavigateBack();
                }
                else
                {
                    await ShowErrorMessageAsync("Insufficient funds to purchase these points.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorMessageAsync($"An error occurred: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task ShowSuccessMessageAsync(PointsOffer offer)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Points Added",
                Content = $"Successfully added {offer.Points} points to your wallet!",
                CloseButtonText = "OK",
                XamlRoot = _navigationFrame.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private async Task ShowErrorMessageAsync(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = _navigationFrame.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void NavigateBack()
        {
            if (_navigationFrame.CanGoBack)
            {
                _navigationFrame.GoBack();
            }
        }

        public void RefreshPoints()
        {
            UserPoints = _walletViewModel.Points;
        }
    }
}
