using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamProfile.Models;
using SteamProfile.Repositories;
using SteamProfile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SteamProfile.ViewModels
{
    public partial class WalletViewModel : ObservableObject
    {
        private readonly WalletService _walletService;
        private readonly PointsOffersRepository _pointsOffersRepository;

        [ObservableProperty]
        private decimal _balance;

        [ObservableProperty]
        private int _points;

        private int _walletId;

        public List<PointsOffer> PointsOffers { get; set; }

        public string BalanceText => $"${Balance:F2}";

        public string PointsText => $"{Points} pts";

        public WalletViewModel(WalletService walletService)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _pointsOffersRepository = new PointsOffersRepository();
            PointsOffers = _pointsOffersRepository.Offers;
            RefreshWalletData();
        }

        partial void OnBalanceChanged(decimal value)
        {
            OnPropertyChanged(nameof(BalanceText));
        }

        //partial void OnPointsChanged(decimal value)
        //{
        //    OnPropertyChanged(nameof(PointsText));
        //}

        [RelayCommand]
        public void RefreshWalletData()
        {
            Balance = _walletService.GetBalance();
            Points = _walletService.GetPoints();
        }

        [RelayCommand]
        public void AddFunds(decimal amount)
        {
            if (amount <= 0)
                return;

            _walletService.AddMoney(amount);
            RefreshWalletData();
        }

        [RelayCommand]
        public async Task<bool> AddPoints(PointsOffer offer)
        {
            if (offer == null)
                return false;

            // Check if user has enough balance to purchase the points
            if (Balance >= offer.Price)
            {
                try
                {
                    // Use the service to handle the purchase transaction
                    _walletService.PurchasePoints(offer);
                    // Refresh wallet data after purchase
                    RefreshWalletData();
                    return true;
                }
                catch (Exception)
                {
                    // Return false if any exception occurs during the purchase
                    return false;
                }
            }
            return false;
        }

        //[RelayCommand]
        //public void AddPoints(int points)
        //{
        //    if (points <= 0)
        //        return;

        //    _walletService.AddPoints(points);
        //    RefreshWalletData();
        //}
    }
}