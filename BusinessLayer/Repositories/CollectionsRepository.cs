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
    public class CollectionsRepository : ICollectionsRepository
    {
        // SQL Parameter Names
        private const string ParamUserId = "@user_id";
        private const string ParamCollectionIdCamel = "@collectionId";   // used in GetCollectionById
        private const string ParamCollectionIdUnderscore = "@collection_id"; // used in other methods
        private const string ParamGameId = "@game_id";
        private const string ParamName = "@name";
        private const string ParamCoverPicture = "@cover_picture";
        private const string ParamIsPublic = "@is_public";
        private const string ParamCreatedAt = "@created_at";

        // Stored Procedure Names
        private const string SP_GetAllCollectionsForUser = "GetAllCollectionsForUser";
        private const string SP_GetCollectionById = "GetCollectionById";
        private const string SP_GetGamesInCollection = "GetGamesInCollection";
        private const string SP_GetAllGamesForUser = "GetAllGamesForUser";
        private const string SP_AddGameToCollection = "AddGameToCollection";
        private const string SP_RemoveGameFromCollection = "RemoveGameFromCollection";
        private const string SP_MakeCollectionPrivate = "MakeCollectionPrivate";
        private const string SP_MakeCollectionPublic = "MakeCollectionPublic";
        private const string SP_DeleteCollection = "DeleteCollection";
        private const string SP_CreateCollection = "CreateCollection";
        private const string SP_UpdateCollection = "UpdateCollection";
        private const string SP_GetPublicCollectionsForUser = "GetPublicCollectionsForUser";
        private const string SP_GetGamesNotInCollection = "GetGamesNotInCollection";

        // Error messages
        private const string Error_GetCollections_Db = "Database error while retrieving collections.";
        private const string Error_GetCollections_Unexpected = "An unexpected error occurred while retrieving collections.";
        private const string Error_GetCollectionById_Db = "Database error while retrieving collection by ID.";
        private const string Error_GetCollectionById_Unexpected = "An unexpected error occurred while retrieving collection by ID.";
        private const string Error_GetGamesInCollection_Db = "Database error while retrieving games in collection.";
        private const string Error_GetGamesInCollection_Unexpected = "An unexpected error occurred while retrieving games in collection.";
        private const string Error_AddGameToCollection_Db = "Database error while adding game to collection.";
        private const string Error_AddGameToCollection_Unexpected = "An unexpected error occurred while adding game to collection.";
        private const string Error_RemoveGameFromCollection_Db = "Database error while removing game from collection.";
        private const string Error_RemoveGameFromCollection_Unexpected = "An unexpected error occurred while removing game from collection.";
        private const string Error_MakeCollectionPrivate = "Failed to make collection {0} private for user {1}.";
        private const string Error_MakeCollectionPublic = "Failed to make collection {0} public for user {1}.";
        private const string Error_RemoveCollection = "Failed to remove collection {0} for user {1}.";
        private const string Error_SaveCollection = "Failed to save collection for user {0}.";
        private const string Error_DeleteCollection_Db = "Database error while deleting collection.";
        private const string Error_DeleteCollection_Unexpected = "An unexpected error occurred while deleting collection.";
        private const string Error_CreateCollection_Db = "Database error while creating collection.";
        private const string Error_CreateCollection_Unexpected = "An unexpected error occurred while creating collection.";
        private const string Error_UpdateCollection_Db = "Database error while updating collection.";
        private const string Error_UpdateCollection_Unexpected = "An unexpected error occurred while updating collection.";
        private const string Error_GetPublicCollections_Db = "Database error while retrieving public collections.";
        private const string Error_GetPublicCollections_Unexpected = "An unexpected error occurred while retrieving public collections.";
        private const string Error_GetGamesNotInCollection_Db = "Database error while getting games not in collection.";
        private const string Error_GetGamesNotInCollection_Unexpected = "An unexpected error occurred while getting games not in collection.";

        // Column Names in DataRows
        private const string ColUserId = "user_id";
        private const string ColName = "name";
        private const string ColCreatedAt = "created_at";
        private const string ColCoverPicture = "cover_picture";
        private const string ColIsPublic = "is_public";
        private const string ColCollectionId = "collection_id";
        private const string ColTitle = "title";
        private const string ColDescription = "description";
        private const string ColGameId = "game_id";

        private readonly IDataLink dataLink;

        public CollectionsRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public List<Collection> GetAllCollections(int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, userId)
                };

                var resultTable = dataLink.ExecuteReader(SP_GetAllCollectionsForUser, sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<Collection>();
                }

                var collectionsList = MapDataTableToCollections(resultTable);
                return collectionsList;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_GetCollections_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetCollections_Unexpected, generalException);
            }
        }

        public List<Collection> GetLastThreeCollectionsForUser(int userId)
        {
            try
            {
                var allUserCollections = GetAllCollections(userId);

                var lastThreeCollections = allUserCollections
                    .OrderByDescending(collection => collection.CreatedAt)
                    .Take(3)
                    .ToList();

                return lastThreeCollections;
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving the last three collections.", generalException);
            }
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamCollectionIdCamel, collectionId),
                    new SqlParameter(ParamUserId, userId)
                };

                var resultTable = dataLink.ExecuteReader(SP_GetCollectionById, sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return null;
                }

                var collection = MapDataRowToCollection(resultTable.Rows[0]);
                return collection;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_GetCollectionById_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetCollectionById_Unexpected, generalException);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamCollectionIdUnderscore, collectionId)
                };

                var resultTable = dataLink.ExecuteReader(SP_GetGamesInCollection, sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<OwnedGame>();
                }

                var gamesInCollection = resultTable.AsEnumerable().Select(row =>
                {
                    var ownedGame = new OwnedGame(
                        Convert.ToInt32(row[ColUserId]),
                        row[ColTitle].ToString(),
                        row[ColDescription]?.ToString(),
                        row[ColCoverPicture]?.ToString());

                    ownedGame.GameId = Convert.ToInt32(row[ColGameId]);
                    return ownedGame;
                }).ToList();

                return gamesInCollection;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_GetGamesInCollection_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetGamesInCollection_Unexpected, generalException);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId, int userId)
        {
            try
            {
                if (collectionId == 1)
                {
                    var sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter(ParamUserId, userId)
                    };

                    var resultTable = dataLink.ExecuteReader(SP_GetAllGamesForUser, sqlParameters);

                    if (resultTable == null || resultTable.Rows.Count == 0)
                    {
                        return new List<OwnedGame>();
                    }

                    var userOwnedGames = resultTable.AsEnumerable().Select(row =>
                    {
                        var ownedGame = new OwnedGame(
                            Convert.ToInt32(row[ColUserId]),
                            row[ColTitle].ToString(),
                            row[ColDescription]?.ToString(),
                            row[ColCoverPicture]?.ToString());

                        ownedGame.GameId = Convert.ToInt32(row[ColGameId]);
                        return ownedGame;
                    }).ToList();

                    return userOwnedGames;
                }
                else
                {
                    return GetGamesInCollection(collectionId);
                }
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_GetGamesInCollection_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetGamesInCollection_Unexpected, generalException);
            }
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamCollectionIdUnderscore, collectionId),
                    new SqlParameter(ParamGameId, gameId)
                };

                dataLink.ExecuteNonQuery(SP_AddGameToCollection, sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_AddGameToCollection_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_AddGameToCollection_Unexpected, generalException);
            }
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamCollectionIdUnderscore, collectionId),
                    new SqlParameter(ParamGameId, gameId)
                };

                dataLink.ExecuteNonQuery(SP_RemoveGameFromCollection, sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_RemoveGameFromCollection_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_RemoveGameFromCollection_Unexpected, generalException);
            }
        }

        public void MakeCollectionPrivateForUser(string userId, string collectionId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, userId),
                    new SqlParameter(ParamCollectionIdUnderscore, collectionId)
                };

                dataLink.ExecuteReader(SP_MakeCollectionPrivate, sqlParameters);
            }
            catch (DatabaseOperationException dbOperationException)
            {
                throw new RepositoryException(string.Format(Error_MakeCollectionPrivate, collectionId, userId), dbOperationException);
            }
        }

        public void MakeCollectionPublicForUser(string userId, string collectionId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, userId),
                    new SqlParameter(ParamCollectionIdUnderscore, collectionId)
                };

                dataLink.ExecuteReader(SP_MakeCollectionPublic, sqlParameters);
            }
            catch (DatabaseOperationException dbOperationException)
            {
                throw new RepositoryException(string.Format(Error_MakeCollectionPublic, collectionId, userId), dbOperationException);
            }
        }

        public void RemoveCollectionForUser(string userId, string collectionId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, userId),
                    new SqlParameter(ParamCollectionIdUnderscore, collectionId)
                };

                dataLink.ExecuteReader(SP_DeleteCollection, sqlParameters);
            }
            catch (DatabaseOperationException dbOperationException)
            {
                throw new RepositoryException(string.Format(Error_RemoveCollection, collectionId, userId), dbOperationException);
            }
        }

        public void SaveCollection(string userId, Collection collection)
        {
            try
            {
                if (collection.CollectionId == 0)
                {
                    var sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter(ParamUserId, userId),
                        new SqlParameter(ParamName, collection.Name),
                        new SqlParameter(ParamCoverPicture, collection.CoverPicture),
                        new SqlParameter(ParamIsPublic, collection.IsPublic),
                        new SqlParameter(ParamCreatedAt, collection.CreatedAt.ToDateTime(TimeOnly.MinValue))
                    };

                    dataLink.ExecuteReader(SP_CreateCollection, sqlParameters);
                }
                else
                {
                    var sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter(ParamCollectionIdUnderscore, collection.CollectionId),
                        new SqlParameter(ParamUserId, userId),
                        new SqlParameter(ParamName, collection.Name),
                        new SqlParameter(ParamCoverPicture, collection.CoverPicture),
                        new SqlParameter(ParamIsPublic, collection.IsPublic),
                        new SqlParameter(ParamCreatedAt, collection.CreatedAt.ToDateTime(TimeOnly.MinValue))
                    };

                    dataLink.ExecuteReader(SP_UpdateCollection, sqlParameters);
                }
            }
            catch (DatabaseOperationException dbOperationException)
            {
                throw new RepositoryException(string.Format(Error_SaveCollection, userId), dbOperationException);
            }
        }

        public void DeleteCollection(int collectionId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamCollectionIdUnderscore, collectionId),
                    new SqlParameter(ParamUserId, userId)
                };

                dataLink.ExecuteNonQuery(SP_DeleteCollection, sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_DeleteCollection_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_DeleteCollection_Unexpected, generalException);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, userId),
                    new SqlParameter(ParamName, name),
                    new SqlParameter(ParamCoverPicture, coverPicture),
                    new SqlParameter(ParamIsPublic, isPublic),
                    new SqlParameter(ParamCreatedAt, createdAt.ToDateTime(TimeOnly.MinValue))
                };

                dataLink.ExecuteNonQuery(SP_CreateCollection, sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_CreateCollection_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_CreateCollection_Unexpected, generalException);
            }
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamCollectionIdUnderscore, collectionId),
                    new SqlParameter(ParamUserId, userId),
                    new SqlParameter(ParamName, name),
                    new SqlParameter(ParamCoverPicture, coverPicture),
                    new SqlParameter(ParamIsPublic, isPublic),
                    new SqlParameter(ParamCreatedAt, DateOnly.FromDateTime(DateTime.Now).ToDateTime(TimeOnly.MinValue))
                };

                dataLink.ExecuteReader(SP_UpdateCollection, sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_UpdateCollection_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_UpdateCollection_Unexpected, generalException);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, userId)
                };

                var resultTable = dataLink.ExecuteReader(SP_GetPublicCollectionsForUser, sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<Collection>();
                }

                var publicCollections = MapDataTableToCollections(resultTable);
                return publicCollections;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_GetPublicCollections_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetPublicCollections_Unexpected, generalException);
            }
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamCollectionIdUnderscore, collectionId),
                    new SqlParameter(ParamUserId, userId)
                };

                var resultTable = dataLink.ExecuteReader(SP_GetGamesNotInCollection, sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<OwnedGame>();
                }

                var unassignedGames = resultTable.AsEnumerable().Select(row =>
                {
                    var ownedGame = new OwnedGame(
                        Convert.ToInt32(row[ColUserId]),
                        row[ColTitle].ToString(),
                        row[ColDescription]?.ToString(),
                        row[ColCoverPicture]?.ToString());
                    ownedGame.GameId = Convert.ToInt32(row[ColGameId]);
                    return ownedGame;
                }).ToList();

                return unassignedGames;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_GetGamesNotInCollection_Db, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetGamesNotInCollection_Unexpected, generalException);
            }
        }

        private static List<Collection> MapDataTableToCollections(DataTable dataTable)
        {
            var collectionList = dataTable.AsEnumerable().Select(row =>
            {
                var collection = new Collection(
                    userId: Convert.ToInt32(row[ColUserId]),
                    name: row[ColName].ToString(),
                    createdAt: DateOnly.FromDateTime(Convert.ToDateTime(row[ColCreatedAt])),
                    coverPicture: row[ColCoverPicture]?.ToString(),
                    isPublic: Convert.ToBoolean(row[ColIsPublic]));

                collection.CollectionId = Convert.ToInt32(row[ColCollectionId]);
                return collection;
            }).ToList();

            return collectionList;
        }

        private static Collection MapDataRowToCollection(DataRow dataRow)
        {
            var collection = new Collection(
                userId: Convert.ToInt32(dataRow[ColUserId]),
                name: dataRow[ColName].ToString(),
                createdAt: DateOnly.FromDateTime(Convert.ToDateTime(dataRow[ColCreatedAt])),
                coverPicture: dataRow[ColCoverPicture]?.ToString(),
                isPublic: Convert.ToBoolean(dataRow[ColIsPublic]));

            collection.CollectionId = Convert.ToInt32(dataRow[ColCollectionId]);
            return collection;
        }
    }
}
