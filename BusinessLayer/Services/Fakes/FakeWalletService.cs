using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.fakes
{
    public class FakeWalletService : IWalletService
    {
        private decimal _balance = 100m;
        private int _points = 0;
        public List<string> Actions = new();
        

        public void AddMoney(decimal amount)
        {
            _balance += amount;
        }

        public void AddPoints(int points)
        {
            _points += points;
        }

        public decimal GetBalance()
        {
            return _balance;
        }

        public int GetPoints()
        {
            return _points;
        }

        public void CreateWallet(int userId)
        {
        }

        public void PurchasePoints(PointsOffer offer)
        {
            if (_balance < offer.Price)
                throw new InvalidOperationException("Insufficient funds");

            _balance -= offer.Price;
            _points += offer.Points;
        }
    }
}
