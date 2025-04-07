using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly IDataLink dataLink;

        public WalletRepository(IDataLink datalink)
        {
            this.dataLink = datalink ?? throw new ArgumentNullException(nameof(datalink));
        }
        public Wallet GetWallet(int walletId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@wallet_id", walletId)
                };
                var dataTable = dataLink.ExecuteReader("GetWalletById", parameters);
                return MapDataRowToWallet(dataTable.Rows[0]);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to retrieve wallet with ID {walletId} from the database.", ex);
            }
        }

        public int GetWalletIdByUserId(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                     new SqlParameter("@user_id", userId)
                };
                var dataTable = dataLink.ExecuteReader("GetWalletIdByUserId", parameters);
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
            dataLink.ExecuteReader("AddMoney", parameters);
        }

        public void AddPointsToWallet(int amount, int walletId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@amount",  amount),
                new SqlParameter("@userId", walletId)
            };
            dataLink.ExecuteReader("AddPoints", parameters);
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
                new SqlParameter("@numberOfPoints", offer.Points),
                new SqlParameter("@userId", walletId)
            };
            dataLink.ExecuteReader("BuyPoints", parameters);
        }
        public void AddNewWallet(int walletid)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", walletid)
                };
                var datatable = dataLink.ExecuteReader("CreateWallet", parameters);
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
                var datatable = dataLink.ExecuteReader("RemoveWallet", parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}