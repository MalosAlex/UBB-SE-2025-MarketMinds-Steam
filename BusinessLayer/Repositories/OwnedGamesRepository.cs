using BusinessLayer.Data;
using BusinessLayer.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories
{
    public class OwnedGamesRepository: IOwnedGamesRepository
    {
        private readonly IDataLink _dataLink;

        public OwnedGamesRepository(IDataLink dataLink) 
        {
            _dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting owned games for user {userId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                Debug.WriteLine("Executing GetAllOwnedGames stored procedure");
                var dataTable = _dataLink.ExecuteReader("GetAllOwnedGames", parameters);
                Debug.WriteLine($"Got {dataTable.Rows.Count} rows from database");

                var games = MapDataTableToOwnedGames(dataTable);
                Debug.WriteLine($"Mapped {games.Count} owned games");
                return games;
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
                Debug.WriteLine($"Error Number: {ex.Number}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while retrieving owned games.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while retrieving owned games.", ex);
            }
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@gameId", gameId),
                    new SqlParameter("@userId", userId)
                };
                var dataTable = _dataLink.ExecuteReader("GetOwnedGameById", parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToOwnedGame(dataTable.Rows[0]) : null;
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while retrieving owned game by ID.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving owned game by ID.", ex);
            }
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@game_id", gameId),
                    new SqlParameter("@user_id", userId)
                };
                _dataLink.ExecuteNonQuery("RemoveOwnedGame", parameters);
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
                throw new RepositoryException("Database error while removing owned game.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while removing owned game.", ex);
            }
        }

        private static List<OwnedGame> MapDataTableToOwnedGames(DataTable dataTable)
        {
            try
            {
                Debug.WriteLine("Starting to map DataTable to OwnedGames");
                var games = dataTable.AsEnumerable().Select(row =>
                {
                    // Create a new OwnedGame using the new constructor
                    var game = new OwnedGame(
                        Convert.ToInt32(row["user_id"]),
                        row["title"].ToString(),
                        row["description"]?.ToString(),
                        row["cover_picture"]?.ToString());
                    
                    // Set the GameId separately
                    game.GameId = Convert.ToInt32(row["game_id"]);
                    
                    Debug.WriteLine($"Loaded game with GameId: {game.GameId} and cover picture: {game.CoverPicture}");
                    return game;
                }).ToList();
                Debug.WriteLine($"Successfully mapped {games.Count} owned games");
                return games;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error mapping DataTable: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        private static OwnedGame MapDataRowToOwnedGame(DataRow row)
        {
            // Create a new OwnedGame using the new constructor
            var game = new OwnedGame(
                Convert.ToInt32(row["user_id"]),
                row["title"].ToString(),
                row["description"]?.ToString(),
                row["cover_picture"]?.ToString());
            
            // Set the GameId separately
            game.GameId = Convert.ToInt32(row["game_id"]);
            return game;
        }
    }
}
