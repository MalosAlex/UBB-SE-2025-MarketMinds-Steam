using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Fakes
{
    public class FakeWalletService : IWalletService
    {
        private decimal balance = 100m;
        private int points = 0;
        public List<string> Actions = new();

        public void AddMoney(decimal amount)
        {
            balance += amount;
        }

        public void AddPoints(int points)
        {
            points += points;
        }

        public decimal GetBalance()
        {
            return balance;
        }

        public int GetPoints()
        {
            return points;
        }

        public void CreateWallet(int userId)
        {
        }

        public void PurchasePoints(PointsOffer offer)
        {
            if (balance < offer.Price)
            {
                throw new InvalidOperationException("Insufficient funds");
            }

            balance -= offer.Price;
            points += offer.Points;
        }
        public bool TryPurchasePoints(PointsOffer pointsOffer)
        {
            if (pointsOffer == null)
            {
                return false;
            }

            try
            {
                if (balance >= pointsOffer.Price)
                {
                    PurchasePoints(pointsOffer);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
