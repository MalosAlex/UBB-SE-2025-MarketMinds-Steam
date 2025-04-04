using BusinessLayer.Services.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class WalletService : IWalletService
    {
        private readonly WalletRepository _walletRepository;
        private readonly UserService _userService;

        public WalletService(WalletRepository walletRepository, UserService userService)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _userService = userService;
        }

        public void AddMoney(decimal amount)
        {
            _walletRepository.AddMoneyToWallet(amount, _userService.GetCurrentUser().UserId);
        }

        public void AddPoints(int points)
        {
            _walletRepository.AddPointsToWallet(points, _userService.GetCurrentUser().UserId);
        }

        public decimal GetBalance()
        {
            return _walletRepository.GetMoneyFromWallet(_walletRepository.GetWalletIdByUserId(_userService.GetCurrentUser().UserId));
        }

        public int GetPoints()
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