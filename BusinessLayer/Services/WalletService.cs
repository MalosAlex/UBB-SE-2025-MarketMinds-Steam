using BusinessLayer.Services.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository walletRepository;
        private readonly IUserService userService;

        public WalletService(IWalletRepository walletRepository, IUserService userService)
        {
            this.walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
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
        public void PurchasePoints(PointsOffer pointsOffer)
        {
            if (pointsOffer == null)
            {
                throw new ArgumentNullException(nameof(pointsOffer));
            }

            // Check if user has enough balance
            if (GetBalance() < pointsOffer.Price)
            {
                throw new InvalidOperationException("Insufficient funds");
            }

            walletRepository.PurchasePoints(pointsOffer, userService.GetCurrentUser().UserId);
        }

        // Moved from WalletViewModel
        public bool TryPurchasePoints(PointsOffer pointsOffer)
        {
            if (pointsOffer == null)
            {
                return false;
            }
            try
            {
                // Check if user has enough balance to purchase the points
                if (GetBalance() >= pointsOffer.Price)
                {
                    PurchasePoints(pointsOffer);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}