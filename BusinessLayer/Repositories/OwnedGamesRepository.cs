using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories
{
    public class OwnedGamesRepository : IOwnedGamesRepository
    {
        private readonly IDataLink dataLink;

        public OwnedGamesRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                var ownedGamesDataTable = dataLink.ExecuteReader("GetAllOwnedGames", sqlParameters);
                var ownedGamesList = MapDataTableToOwnedGames(ownedGamesDataTable);

                return ownedGamesList;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while retrieving owned games.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving owned games.", generalException);
            }
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@gameId", gameId),
                    new SqlParameter("@userId", userId)
                };

                var ownedGameDataTable = dataLink.ExecuteReader("GetOwnedGameById", sqlParameters);

                if (ownedGameDataTable.Rows.Count == 0)
                {
                    return null;
                }

                var ownedGame = MapDataRowToOwnedGame(ownedGameDataTable.Rows[0]);
                return ownedGame;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while retrieving owned game by ID.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving owned game by ID.", generalException);
            }
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@game_id", gameId),
                    new SqlParameter("@user_id", userId)
                };

                dataLink.ExecuteNonQuery("RemoveOwnedGame", sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while removing owned game.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while removing owned game.", generalException);
            }
        }

        private static List<OwnedGame> MapDataTableToOwnedGames(DataTable dataTable)
        {
            var ownedGamesList = dataTable.AsEnumerable().Select(row =>
            {
                var ownedGame = new OwnedGame(
                    Convert.ToInt32(row["user_id"]),
                    row["title"].ToString(),
                    row["description"]?.ToString(),
                    row["cover_picture"]?.ToString());

                ownedGame.GameId = Convert.ToInt32(row["game_id"]);

                return ownedGame;
            }).ToList();

            return ownedGamesList;
        }

        private static OwnedGame MapDataRowToOwnedGame(DataRow dataRow)
        {
            var ownedGame = new OwnedGame(
                Convert.ToInt32(dataRow["user_id"]),
                dataRow["title"].ToString(),
                dataRow["description"]?.ToString(),
                dataRow["cover_picture"]?.ToString());

            ownedGame.GameId = Convert.ToInt32(dataRow["game_id"]);

            return ownedGame;
        }
    }
}
