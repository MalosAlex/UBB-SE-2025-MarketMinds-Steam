using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories
{
    public class OwnedGamesRepository : IOwnedGamesRepository
    {
        // SQL Parameter Names
        private const string ParamUserId = "@user_id";
        private const string ParamGameIdCamel = "@gameId";
        private const string ParamUserIdCamel = "@userId";
        private const string ParamGameIdUnderscore = "@game_id";

        // Stored Procedure Names
        private const string SP_GetAllOwnedGames = "GetAllOwnedGames";
        private const string SP_GetOwnedGameById = "GetOwnedGameById";
        private const string SP_RemoveOwnedGame = "RemoveOwnedGame";

        // Error messages
        private const string Err_GetOwnedGamesDb = "Database error while retrieving owned games.";
        private const string Err_GetOwnedGamesUnexpected = "An unexpected error occurred while retrieving owned games.";
        private const string Err_GetOwnedGameByIdDb = "Database error while retrieving owned game by ID.";
        private const string Err_GetOwnedGameByIdUnexpected = "An unexpected error occurred while retrieving owned game by ID.";
        private const string Err_RemoveOwnedGameDb = "Database error while removing owned game.";
        private const string Err_RemoveOwnedGameUnexpected = "An unexpected error occurred while removing owned game.";

        // Column Names
        private const string ColUserId = "user_id";
        private const string ColTitle = "title";
        private const string ColDescription = "description";
        private const string ColCoverPicture = "cover_picture";
        private const string ColGameId = "game_id";

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
                    new SqlParameter(ParamUserId, userId)
                };

                var ownedGamesDataTable = dataLink.ExecuteReader(SP_GetAllOwnedGames, sqlParameters);
                var ownedGamesList = MapDataTableToOwnedGames(ownedGamesDataTable);

                return ownedGamesList;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Err_GetOwnedGamesDb, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Err_GetOwnedGamesUnexpected, generalException);
            }
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamGameIdCamel, gameId),
                    new SqlParameter(ParamUserIdCamel, userId)
                };

                var ownedGameDataTable = dataLink.ExecuteReader(SP_GetOwnedGameById, sqlParameters);

                if (ownedGameDataTable.Rows.Count == 0)
                {
                    return null;
                }

                var ownedGame = MapDataRowToOwnedGame(ownedGameDataTable.Rows[0]);
                return ownedGame;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Err_GetOwnedGameByIdDb, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Err_GetOwnedGameByIdUnexpected, generalException);
            }
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamGameIdUnderscore, gameId),
                    new SqlParameter(ParamUserId, userId)
                };

                dataLink.ExecuteNonQuery(SP_RemoveOwnedGame, sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Err_RemoveOwnedGameDb, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Err_RemoveOwnedGameUnexpected, generalException);
            }
        }

        private static List<OwnedGame> MapDataTableToOwnedGames(DataTable dataTable)
        {
            var ownedGamesList = dataTable.AsEnumerable().Select(row =>
            {
                var ownedGame = new OwnedGame(
                    Convert.ToInt32(row[ColUserId]),
                    row[ColTitle].ToString(),
                    row[ColDescription]?.ToString(),
                    row[ColCoverPicture]?.ToString());

                ownedGame.GameId = Convert.ToInt32(row[ColGameId]);
                return ownedGame;
            }).ToList();

            return ownedGamesList;
        }

        private static OwnedGame MapDataRowToOwnedGame(DataRow dataRow)
        {
            var ownedGame = new OwnedGame(
                Convert.ToInt32(dataRow[ColUserId]),
                dataRow[ColTitle].ToString(),
                dataRow[ColDescription]?.ToString(),
                dataRow[ColCoverPicture]?.ToString());

            ownedGame.GameId = Convert.ToInt32(dataRow[ColGameId]);
            return ownedGame;
        }
    }
}
