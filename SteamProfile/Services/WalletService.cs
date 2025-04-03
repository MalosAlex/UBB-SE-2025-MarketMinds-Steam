using SteamProfile.Models;
using SteamProfile.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamProfile.Services
{
    public class WalletService
    {
        private readonly WalletRepository _walletRepository;
        private readonly UserService _userService;

        public WalletService(WalletRepository walletRepository, UserService userService)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _userService = userService;
        }

        internal void AddMoney(decimal amount)
        {
            _walletRepository.AddMoneyToWallet(amount, _userService.GetCurrentUser().UserId);
        }

        internal void AddPoints(int points)
        {
            _walletRepository.AddPointsToWallet(points, _userService.GetCurrentUser().UserId);
        }

        internal decimal GetBalance()
        {
            return _walletRepository.GetMoneyFromWallet(_walletRepository.GetWalletIdByUserId(_userService.GetCurrentUser().UserId));
        }

        internal int GetPoints()
        {
            int userId = _userService.GetCurrentUser().UserId;
            int walletId = _walletRepository.GetWalletIdByUserId(userId);
            return _walletRepository.GetPointsFromWallet(walletId);
        }

        public void CreateWallet(int userId)
        {
            _walletRepository.AddNewWallet(userId);
        }
        public void PurchasePoints(PointsOffer offer)
        {
            if (offer == null)
                throw new ArgumentNullException(nameof(offer));

            // Check if user has enough balance
            if (GetBalance() < offer.Price)
                throw new InvalidOperationException("Insufficient funds");
            _walletRepository.PurchasePoints(offer, _userService.GetCurrentUser().UserId);
        }
    }
}