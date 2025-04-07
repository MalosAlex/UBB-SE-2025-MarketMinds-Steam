using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;

namespace SteamProfile.ViewModels
{
    public partial class WalletViewModel : ObservableObject
    {
        private readonly IWalletService walletService;
        private readonly IPointsOffersRepository pointsOffersRepository;

        [ObservableProperty]
        private decimal balance;

        [ObservableProperty]
        private int points;

        private int walletId;

        public List<PointsOffer> PointsOffers { get; set; }

        public string BalanceText => $"${Balance:F2}";

        public string PointsText => $"{Points} pts";

        public WalletViewModel(IWalletService walletService)
        {
            this.walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            this.pointsOffersRepository = new PointsOffersRepository();
            PointsOffers = this.pointsOffersRepository.Offers;
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
            Balance = walletService.GetBalance();
            Points = walletService.GetPoints();
        }

        [RelayCommand]
        public void AddFunds(decimal amount)
        {
            if (amount <= 0)
                return;

            walletService.AddMoney(amount);
            RefreshWalletData();
        }

        [RelayCommand]
        public async Task<bool> AddPoints(PointsOffer pointsOffer)
        {
            // Business logic moved to WalletService
            bool success = walletService.TryPurchasePoints(pointsOffer);

            if (success)
            {
                RefreshWalletData();
            }

            return success;
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