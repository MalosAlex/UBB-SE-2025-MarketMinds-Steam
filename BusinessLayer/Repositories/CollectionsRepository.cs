using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories
{
    public class CollectionsRepository : ICollectionsRepository
    {
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
                    new SqlParameter("@user_id", userId)
                };

                var resultTable = dataLink.ExecuteReader("GetAllCollectionsForUser", sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<Collection>();
                }

                var collectionsList = MapDataTableToCollections(resultTable);
                return collectionsList;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while retrieving collections.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving collections.", generalException);
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
                    new SqlParameter("@collectionId", collectionId),
                    new SqlParameter("@user_id", userId)
                };

                var resultTable = dataLink.ExecuteReader("GetCollectionById", sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return null;
                }

                var collection = MapDataRowToCollection(resultTable.Rows[0]);
                return collection;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while retrieving collection by ID.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving collection by ID.", generalException);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId)
                };

                var resultTable = dataLink.ExecuteReader("GetGamesInCollection", sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<OwnedGame>();
                }

                var gamesInCollection = resultTable.AsEnumerable().Select(row =>
                {
                    var ownedGame = new OwnedGame(
                        Convert.ToInt32(row["user_id"]),
                        row["title"].ToString(),
                        row["description"]?.ToString(),
                        row["cover_picture"]?.ToString());

                    ownedGame.GameId = Convert.ToInt32(row["game_id"]);
                    return ownedGame;
                }).ToList();

                return gamesInCollection;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while retrieving games in collection.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving games in collection.", generalException);
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
                        new SqlParameter("@user_id", userId)
                    };

                    var resultTable = dataLink.ExecuteReader("GetAllGamesForUser", sqlParameters);

                    if (resultTable == null || resultTable.Rows.Count == 0)
                    {
                        return new List<OwnedGame>();
                    }

                    var userOwnedGames = resultTable.AsEnumerable().Select(row =>
                    {
                        var ownedGame = new OwnedGame(
                            Convert.ToInt32(row["user_id"]),
                            row["title"].ToString(),
                            row["description"]?.ToString(),
                            row["cover_picture"]?.ToString());

                        ownedGame.GameId = Convert.ToInt32(row["game_id"]);
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
                throw new RepositoryException("Database error while retrieving games in collection.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving games in collection.", generalException);
            }
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@game_id", gameId)
                };

                dataLink.ExecuteNonQuery("AddGameToCollection", sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while adding game to collection.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while adding game to collection.", generalException);
            }
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@game_id", gameId)
                };

                dataLink.ExecuteNonQuery("RemoveGameFromCollection", sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while removing game from collection.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while removing game from collection.", generalException);
            }
        }

        public void MakeCollectionPrivateForUser(string userId, string collectionId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@collection_id", collectionId)
                };

                dataLink.ExecuteReader("MakeCollectionPrivate", sqlParameters);
            }
            catch (DatabaseOperationException dbOperationException)
            {
                throw new RepositoryException($"Failed to make collection {collectionId} private for user {userId}.", dbOperationException);
            }
        }

        public void MakeCollectionPublicForUser(string userId, string collectionId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@collection_id", collectionId)
                };

                dataLink.ExecuteReader("MakeCollectionPublic", sqlParameters);
            }
            catch (DatabaseOperationException dbOperationException)
            {
                throw new RepositoryException($"Failed to make collection {collectionId} public for user {userId}.", dbOperationException);
            }
        }

        public void RemoveCollectionForUser(string userId, string collectionId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@collection_id", collectionId)
                };

                dataLink.ExecuteReader("DeleteCollection", sqlParameters);
            }
            catch (DatabaseOperationException dbOperationException)
            {
                throw new RepositoryException($"Failed to remove collection {collectionId} for user {userId}.", dbOperationException);
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
                        new SqlParameter("@user_id", userId),
                        new SqlParameter("@name", collection.Name),
                        new SqlParameter("@cover_picture", collection.CoverPicture),
                        new SqlParameter("@is_public", collection.IsPublic),
                        new SqlParameter("@created_at", collection.CreatedAt.ToDateTime(TimeOnly.MinValue))
                    };

                    dataLink.ExecuteReader("CreateCollection", sqlParameters);
                }
                else
                {
                    var sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@collection_id", collection.CollectionId),
                        new SqlParameter("@user_id", userId),
                        new SqlParameter("@name", collection.Name),
                        new SqlParameter("@cover_picture", collection.CoverPicture),
                        new SqlParameter("@is_public", collection.IsPublic),
                        new SqlParameter("@created_at", collection.CreatedAt.ToDateTime(TimeOnly.MinValue))
                    };

                    dataLink.ExecuteReader("UpdateCollection", sqlParameters);
                }
            }
            catch (DatabaseOperationException dbOperationException)
            {
                throw new RepositoryException($"Failed to save collection for user {userId}.", dbOperationException);
            }
        }

        public void DeleteCollection(int collectionId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@user_id", userId)
                };

                dataLink.ExecuteNonQuery("DeleteCollection", sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while deleting collection.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while deleting collection.", generalException);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@name", name),
                    new SqlParameter("@cover_picture", coverPicture),
                    new SqlParameter("@is_public", isPublic),
                    new SqlParameter("@created_at", createdAt.ToDateTime(TimeOnly.MinValue))
                };

                dataLink.ExecuteNonQuery("CreateCollection", sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while creating collection.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while creating collection.", generalException);
            }
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@name", name),
                    new SqlParameter("@cover_picture", coverPicture),
                    new SqlParameter("@is_public", isPublic),
                    new SqlParameter("@created_at", DateOnly.FromDateTime(DateTime.Now).ToDateTime(TimeOnly.MinValue))
                };

                dataLink.ExecuteReader("UpdateCollection", sqlParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while updating collection.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while updating collection.", generalException);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                var resultTable = dataLink.ExecuteReader("GetPublicCollectionsForUser", sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<Collection>();
                }

                var publicCollections = MapDataTableToCollections(resultTable);
                return publicCollections;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while retrieving public collections.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving public collections.", generalException);
            }
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            try
            {
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@user_id", userId)
                };

                var resultTable = dataLink.ExecuteReader("GetGamesNotInCollection", sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<OwnedGame>();
                }

                var unassignedGames = resultTable.AsEnumerable().Select(row =>
                {
                    var ownedGame = new OwnedGame(
                        Convert.ToInt32(row["user_id"]),
                        row["title"].ToString(),
                        row["description"]?.ToString(),
                        row["cover_picture"]?.ToString());
                    ownedGame.GameId = Convert.ToInt32(row["game_id"]);
                    return ownedGame;
                }).ToList();

                return unassignedGames;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while getting games not in collection.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while getting games not in collection.", generalException);
            }
        }

        private static List<Collection> MapDataTableToCollections(DataTable dataTable)
        {
            var collectionList = dataTable.AsEnumerable().Select(row =>
            {
                var collection = new Collection(
                    userId: Convert.ToInt32(row["user_id"]),
                    name: row["name"].ToString(),
                    createdAt: DateOnly.FromDateTime(Convert.ToDateTime(row["created_at"])),
                    coverPicture: row["cover_picture"]?.ToString(),
                    isPublic: Convert.ToBoolean(row["is_public"]));

                collection.CollectionId = Convert.ToInt32(row["collection_id"]);
                return collection;
            }).ToList();

            return collectionList;
        }

        private static Collection MapDataRowToCollection(DataRow dataRow)
        {
            var collection = new Collection(
                userId: Convert.ToInt32(dataRow["user_id"]),
                name: dataRow["name"].ToString(),
                createdAt: DateOnly.FromDateTime(Convert.ToDateTime(dataRow["created_at"])),
                coverPicture: dataRow["cover_picture"]?.ToString(),
                isPublic: Convert.ToBoolean(dataRow["is_public"]));

            collection.CollectionId = Convert.ToInt32(dataRow["collection_id"]);
            return collection;
        }
    }
}
