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
            dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public List<Collection> GetAllCollections(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                var dataTable = dataLink.ExecuteReader("GetAllCollectionsForUser", parameters);

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new List<Collection>();
                }

                var collections = MapDataTableToCollections(dataTable);
                return collections;
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while retrieving collections.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving collections.", ex);
            }
        }

        public List<Collection> GetLastThreeCollectionsForUser(int userId)
        {
            try
            {
                var allCollections = GetAllCollections(userId);
                var lastThreeCollections = allCollections
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(3)
                    .ToList();

                return lastThreeCollections;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving the last three collections.", ex);
            }
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collectionId", collectionId),
                    new SqlParameter("@user_id", userId)
                };

                var dataTable = dataLink.ExecuteReader("GetCollectionById", parameters);

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return null;
                }

                var collection = MapDataRowToCollection(dataTable.Rows[0]);
                return collection;
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while retrieving collection by ID.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving collection by ID.", ex);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId)
                };

                var dataTable = dataLink.ExecuteReader("GetGamesInCollection", parameters);

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new List<OwnedGame>();
                }

                var games = dataTable.AsEnumerable().Select(row =>
                {
                    // Create OwnedGame using the new constructor
                    var game = new OwnedGame(
                        Convert.ToInt32(row["user_id"]),
                        row["title"].ToString(),
                        row["description"]?.ToString(),
                        row["cover_picture"]?.ToString());
                    game.GameId = Convert.ToInt32(row["game_id"]);
                    return game;
                }).ToList();

                return games;
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while retrieving games in collection.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving games in collection.", ex);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId, int userId)
        {
            try
            {
                if (collectionId == 1)
                {
                    var parameters = new SqlParameter[]
                    {
                        new SqlParameter("@user_id", userId)
                    };
                    var dataTable = dataLink.ExecuteReader("GetAllGamesForUser", parameters);

                    if (dataTable == null || dataTable.Rows.Count == 0)
                    {
                        return new List<OwnedGame>();
                    }

                    var games = dataTable.AsEnumerable().Select(row =>
                    {
                        var game = new OwnedGame(
                            Convert.ToInt32(row["user_id"]),
                            row["title"].ToString(),
                            row["description"]?.ToString(),
                            row["cover_picture"]?.ToString());
                        game.GameId = Convert.ToInt32(row["game_id"]);
                        return game;
                    }).ToList();

                    return games;
                }
                else
                {
                    return GetGamesInCollection(collectionId);
                }
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while retrieving games in collection.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving games in collection.", ex);
            }
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@game_id", gameId)
                };

                dataLink.ExecuteNonQuery("AddGameToCollection", parameters);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while adding game to collection.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while adding game to collection.", ex);
            }
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@game_id", gameId)
                };
                dataLink.ExecuteNonQuery("RemoveGameFromCollection", parameters);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while removing game from collection.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while removing game from collection.", ex);
            }
        }

        public void MakeCollectionPrivateForUser(string userId, string collectionId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@collection_id", collectionId)
                };

                dataLink.ExecuteReader("MakeCollectionPrivate", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to make collection {collectionId} private for user {userId}.", ex);
            }
        }

        public void MakeCollectionPublicForUser(string userId, string collectionId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@collection_id", collectionId)
                };

                dataLink.ExecuteReader("MakeCollectionPublic", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to make collection {collectionId} public for user {userId}.", ex);
            }
        }

        public void RemoveCollectionForUser(string userId, string collectionId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@collection_id", collectionId)
                };

                dataLink.ExecuteReader("DeleteCollection", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to remove collection {collectionId} for user {userId}.", ex);
            }
        }

        public void SaveCollection(string userId, Collection collection)
        {
            try
            {
                if (collection.CollectionId == 0)
                {
                    var parameters = new SqlParameter[]
                    {
                        new SqlParameter("@user_id", userId),
                        new SqlParameter("@name", collection.Name),
                        new SqlParameter("@cover_picture", collection.CoverPicture),
                        new SqlParameter("@is_public", collection.IsPublic),
                        new SqlParameter("@created_at", collection.CreatedAt.ToDateTime(TimeOnly.MinValue))
                    };

                    dataLink.ExecuteReader("CreateCollection", parameters);
                }
                else
                {
                    var parameters = new SqlParameter[]
                    {
                        new SqlParameter("@collection_id", collection.CollectionId),
                        new SqlParameter("@user_id", userId),
                        new SqlParameter("@name", collection.Name),
                        new SqlParameter("@cover_picture", collection.CoverPicture),
                        new SqlParameter("@is_public", collection.IsPublic),
                        new SqlParameter("@created_at", collection.CreatedAt.ToDateTime(TimeOnly.MinValue))
                    };

                    dataLink.ExecuteReader("UpdateCollection", parameters);
                }
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to save collection for user {userId}.", ex);
            }
        }

        public void DeleteCollection(int collectionId, int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@user_id", userId)
                };

                dataLink.ExecuteNonQuery("DeleteCollection", parameters);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while deleting collection.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while deleting collection.", ex);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@name", name),
                    new SqlParameter("@cover_picture", coverPicture),
                    new SqlParameter("@is_public", isPublic),
                    new SqlParameter("@created_at", createdAt.ToDateTime(TimeOnly.MinValue))
                };

                dataLink.ExecuteNonQuery("CreateCollection", parameters);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while creating collection.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while creating collection.", ex);
            }
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@name", name),
                    new SqlParameter("@cover_picture", coverPicture),
                    new SqlParameter("@is_public", isPublic),
                    new SqlParameter("@created_at", DateOnly.FromDateTime(DateTime.Now).ToDateTime(TimeOnly.MinValue))
                };

                dataLink.ExecuteReader("UpdateCollection", parameters);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while updating collection.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while updating collection.", ex);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                var dataTable = dataLink.ExecuteReader("GetPublicCollectionsForUser", parameters);

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new List<Collection>();
                }

                var collections = MapDataTableToCollections(dataTable);
                return collections;
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while retrieving public collections.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving public collections.", ex);
            }
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@user_id", userId)
                };

                var dataTable = dataLink.ExecuteReader("GetGamesNotInCollection", parameters);

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new List<OwnedGame>();
                }

                var games = dataTable.AsEnumerable().Select(row =>
                {
                    var game = new OwnedGame(
                        Convert.ToInt32(row["user_id"]),
                        row["title"].ToString(),
                        row["description"]?.ToString(),
                        row["cover_picture"]?.ToString());
                    game.GameId = Convert.ToInt32(row["game_id"]);
                    return game;
                }).ToList();

                return games;
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while getting games not in collection.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while getting games not in collection.", ex);
            }
        }

        private static List<Collection> MapDataTableToCollections(DataTable dataTable)
        {
            var collections = dataTable.AsEnumerable().Select(row =>
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
            return collections;
        }

        private static Collection MapDataRowToCollection(DataRow row)
        {
            var collection = new Collection(
                userId: Convert.ToInt32(row["user_id"]),
                name: row["name"].ToString(),
                createdAt: DateOnly.FromDateTime(Convert.ToDateTime(row["created_at"])),
                coverPicture: row["cover_picture"]?.ToString(),
                isPublic: Convert.ToBoolean(row["is_public"]));
            collection.CollectionId = Convert.ToInt32(row["collection_id"]);
            return collection;
        }
    }
}
