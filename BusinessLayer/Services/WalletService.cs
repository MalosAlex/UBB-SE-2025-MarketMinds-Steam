using BusinessLayer.Services.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Repositories;

namespace BusinessLayer.Services
{
    public class WalletService : IWalletService
    {
        private readonly WalletRepository walletRepository;
        private readonly IUserService userService;

        public WalletService(WalletRepository walletRepository, IUserService userService)
        {
            walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            userService = userService;
        }

        public void AddMoney(decimal amount)
        {
            walletRepository.AddMoneyToWallet(amount, userService.GetCurrentUser().UserId);
        }

        public void AddPoints(int points)
        {
            walletRepository.AddPointsToWallet(points, userService.GetCurrentUser().UserId);
        }

        public decimal GetBalance()
        {
            return walletRepository.GetMoneyFromWallet(walletRepository.GetWalletIdByUserId(userService.GetCurrentUser().UserId));
        }

        public int GetPoints()
        {
            int userId = userService.GetCurrentUser().UserId;
            int walletId = walletRepository.GetWalletIdByUserId(userId);
            return walletRepository.GetPointsFromWallet(walletId);
        }

        public void CreateWallet(int userId)
        {
            walletRepository.AddNewWallet(userId);
        }
        public void PurchasePoints(PointsOffer offer)
        {
            if (offer == null)
            {
                throw new ArgumentNullException(nameof(offer));
            }

            // Check if user has enough balance
            if (GetBalance() < offer.Price)
            {
                throw new InvalidOperationException("Insufficient funds");
            }

            walletRepository.PurchasePoints(offer, userService.GetCurrentUser().UserId);
        }
    }
}