using SteamProfile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamProfile.Services
{
    internal interface IWalletService
    {
        void CreateWallet(int userId);
        void PurchasePoints(PointsOffer offer);
        decimal GetBalance();
        int GetPoints();
        void AddMoney(decimal amount);
        void AddPoints(int points);
    }
}
