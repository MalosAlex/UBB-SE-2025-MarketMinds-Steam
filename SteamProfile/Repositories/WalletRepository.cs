using SteamProfile.Data;
using SteamProfile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.System;

namespace SteamProfile.Repositories
{
    public class WalletRepository
    {
        private readonly DataLink _dataLink;

        public WalletRepository(DataLink datalink)
        {
            _dataLink = datalink ?? throw new ArgumentNullException(nameof(datalink));
        }
        public Wallet GetWallet(int walletId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@wallet_id", walletId)
                };
                var dataTable = _dataLink.ExecuteReader("GetWalletById", parameters);
                return MapDataRowToWallet(dataTable.Rows[0]);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to retrieve wallet with ID {walletId} from the database.", ex);
            }
        }

        // ... existing code ...

        public int GetWalletIdByUserId(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userId)
                };
                var dataTable = _dataLink.ExecuteReader("GetWalletIdByUserId", parameters);
                if (dataTable.Rows.Count > 0)
                {
                    return Convert.ToInt32(dataTable.Rows[0]["wallet_id"]);
                }
                throw new RepositoryException($"No wallet found for user ID {userId}.");
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to retrieve wallet ID for user ID {userId} from the database.", ex);
            }
        }

        // ... existing code ...

        private Wallet MapDataRowToWallet(DataRow dataRow) => new Wallet
        {
            WalletId = Convert.ToInt32(dataRow["wallet_id"]),
            UserId = Convert.ToInt32(dataRow["user_id"]),
            Balance = Convert.ToDecimal(dataRow["money_for_games"]),
            Points = Convert.ToInt32(dataRow["points"]),
        };

        public void AddMoneyToWallet(decimal amount, int waletId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@amount",  amount),
                new SqlParameter("@userId", waletId)
            };
            _dataLink.ExecuteReader("AddMoney", parameters);
        }

        public void AddPointsToWallet(int amount, int walletId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@amount",  amount),
                new SqlParameter("@userId", walletId)
            };
            _dataLink.ExecuteReader("AddPoints", parameters);
        }

        public decimal GetMoneyFromWallet(int walletId)
        {
            return GetWallet(walletId).Balance;
        }

        public int GetPointsFromWallet(int walletId)
        {
            return GetWallet(walletId).Points;
        }

        public void PurchasePoints(PointsOffer offer, int walletId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@price", offer.Price),
                new SqlParameter("@numberOfPoints", offer.Points), // Fixed parameter name from @@numberOfPoints to @points
                new SqlParameter("@userId", walletId)
            };
            _dataLink.ExecuteReader("BuyPoints", parameters);
        }
        public void AddNewWallet(int walletid)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", walletid)
                };
                var datatable = _dataLink.ExecuteReader("CreateWallet", parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void RemoveWallet(int walletId)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", walletId)
                };
                var datatable = _dataLink.ExecuteReader("RemoveWallet", parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}