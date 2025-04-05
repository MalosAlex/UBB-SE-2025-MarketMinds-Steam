using BusinessLayer.Data;
using BusinessLayer.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using BusinessLayer.Repositories.Interfaces;
using System.Linq;

namespace BusinessLayer.Repositories
{
    public class CollectionsRepository : ICollectionsRepository
    {
        private readonly IDataLink _dataLink;

        public CollectionsRepository(IDataLink dataLink)
        {
            _dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public List<Collection> GetAllCollections(int userId)
        {
            try
            {
                Debug.WriteLine($"Repository: Getting collections for user {userId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                Debug.WriteLine("Repository: Executing GetAllCollectionsForUser stored procedure");
                var dataTable = _dataLink.ExecuteReader("GetAllCollectionsForUser", parameters);
                Debug.WriteLine($"Repository: Got {dataTable?.Rows?.Count ?? 0} rows from database");

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    Debug.WriteLine("Repository: No collections found for user");
                    return new List<Collection>();
                }

                var collections = MapDataTableToCollections(dataTable);
                Debug.WriteLine($"Repository: Mapped {collections.Count} collections");
                return collections;
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Repository: SQL Error: {ex.Message}");
                Debug.WriteLine($"Repository: Error Number: {ex.Number}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while retrieving collections.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Repository: Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
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

                Debug.WriteLine($"Repository: Retrieved last 3 collections for user {userId}");
                return lastThreeCollections;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Repository: Unexpected Error while getting last three collections: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while retrieving the last three collections.", ex);
            }
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            try
            {
                Debug.WriteLine($"Repository: Getting collection with ID {collectionId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collectionId", collectionId),
                    new SqlParameter("@user_id", userId)
                };

                Debug.WriteLine("Repository: Executing GetCollectionById stored procedure");
                var dataTable = _dataLink.ExecuteReader("GetCollectionById", parameters);

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    Debug.WriteLine($"Repository: No collection found with ID {collectionId}");
                    return null;
                }

                var collection = MapDataRowToCollection(dataTable.Rows[0]);
                Debug.WriteLine($"Repository: Successfully retrieved collection {collectionId}");
                return collection;
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Repository: SQL Error: {ex.Message}");
                Debug.WriteLine($"Repository: Error Number: {ex.Number}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while retrieving collection by ID.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Repository: Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while retrieving collection by ID.", ex);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            try
            {
                Debug.WriteLine($"Repository: Getting games for collection {collectionId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId)
                };

                Debug.WriteLine("Repository: Executing GetGamesInCollection stored procedure");
                var dataTable = _dataLink.ExecuteReader("GetGamesInCollection", parameters);
                Debug.WriteLine($"Repository: Got {dataTable?.Rows?.Count ?? 0} rows from database");

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    Debug.WriteLine("Repository: No games found in collection");
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

                Debug.WriteLine($"Repository: Mapped {games.Count} games");
                return games;
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Repository: SQL Error: {ex.Message}");
                Debug.WriteLine($"Repository: Error Number: {ex.Number}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while retrieving games in collection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Repository: Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while retrieving games in collection.", ex);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId, int userId)
        {
            try
            {
                Debug.WriteLine($"Repository: Getting games for collection {collectionId}");

                if (collectionId == 1)
                {
                    var parameters = new SqlParameter[]
                    {
                        new SqlParameter("@user_id", userId)
                    };
                    Debug.WriteLine("Repository: Executing GetAllGamesForUser stored procedure");
                    var dataTable = _dataLink.ExecuteReader("GetAllGamesForUser", parameters);
                    Debug.WriteLine($"Repository: Got {dataTable?.Rows?.Count ?? 0} rows from database");

                    if (dataTable == null || dataTable.Rows.Count == 0)
                    {
                        Debug.WriteLine("Repository: No games found for user");
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

                    Debug.WriteLine($"Repository: Mapped {games.Count} games");
                    return games;
                }
                else
                {
                    return GetGamesInCollection(collectionId);
                }
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Repository: SQL Error: {ex.Message}");
                Debug.WriteLine($"Repository: Error Number: {ex.Number}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while retrieving games in collection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Repository: Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while retrieving games in collection.", ex);
            }
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            try
            {
                Debug.WriteLine($"Repository: Adding game {gameId} to collection {collectionId} for user {userId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@game_id", gameId)
                };

                Debug.WriteLine("Repository: Executing AddGameToCollection stored procedure");
                _dataLink.ExecuteNonQuery("AddGameToCollection", parameters);
                Debug.WriteLine("Repository: Successfully added game to collection");
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Repository: SQL Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while adding game to collection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Repository: Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
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
                _dataLink.ExecuteNonQuery("RemoveGameFromCollection", parameters);
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
                throw new RepositoryException("Database error while removing game from collection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
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

                _dataLink.ExecuteReader("MakeCollectionPrivate", parameters);
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

                _dataLink.ExecuteReader("MakeCollectionPublic", parameters);
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

                _dataLink.ExecuteReader("DeleteCollection", parameters);
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

                    _dataLink.ExecuteReader("CreateCollection", parameters);
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

                    _dataLink.ExecuteReader("UpdateCollection", parameters);
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
                Debug.WriteLine($"Attempting to delete collection {collectionId} for user {userId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@user_id", userId)
                };

                _dataLink.ExecuteNonQuery("DeleteCollection", parameters);
                Debug.WriteLine($"Successfully deleted collection {collectionId}");
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error in DeleteCollection: {ex.Message}");
                throw new RepositoryException("Database error while deleting collection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in DeleteCollection: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while deleting collection.", ex);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                Debug.WriteLine($"Repository: Creating collection for user {userId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@name", name),
                    new SqlParameter("@cover_picture", coverPicture),
                    new SqlParameter("@is_public", isPublic),
                    new SqlParameter("@created_at", createdAt.ToDateTime(TimeOnly.MinValue))
                };

                Debug.WriteLine("Repository: Executing CreateCollection stored procedure");
                _dataLink.ExecuteNonQuery("CreateCollection", parameters);
                Debug.WriteLine("Repository: Successfully created collection");
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Repository: SQL Error: {ex.Message}");
                Debug.WriteLine($"Repository: Error Number: {ex.Number}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while creating collection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Repository: Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while creating collection.", ex);
            }
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            try
            {
                Debug.WriteLine($"Repository: Updating collection {collectionId} for user {userId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@name", name),
                    new SqlParameter("@cover_picture", coverPicture),
                    new SqlParameter("@is_public", isPublic),
                    new SqlParameter("@created_at", DateOnly.FromDateTime(DateTime.Now).ToDateTime(TimeOnly.MinValue))
                };

                Debug.WriteLine("Repository: Executing UpdateCollection stored procedure");
                _dataLink.ExecuteReader("UpdateCollection", parameters);
                Debug.WriteLine("Repository: Successfully updated collection");
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Repository: SQL Error: {ex.Message}");
                Debug.WriteLine($"Repository: Error Number: {ex.Number}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while updating collection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Repository: Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while updating collection.", ex);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            try
            {
                Debug.WriteLine($"Repository: Getting public collections for user {userId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                Debug.WriteLine("Repository: Executing GetPublicCollectionsForUser stored procedure");
                var dataTable = _dataLink.ExecuteReader("GetPublicCollectionsForUser", parameters);
                Debug.WriteLine($"Repository: Got {dataTable?.Rows?.Count ?? 0} rows from database");

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    Debug.WriteLine("Repository: No public collections found for user");
                    return new List<Collection>();
                }

                var collections = MapDataTableToCollections(dataTable);
                Debug.WriteLine($"Repository: Mapped {collections.Count} public collections");
                return collections;
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Repository: SQL Error: {ex.Message}");
                Debug.WriteLine($"Repository: Error Number: {ex.Number}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while retrieving public collections.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Repository: Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while retrieving public collections.", ex);
            }
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            try
            {
                Debug.WriteLine($"Repository: Getting games not in collection {collectionId} for user {userId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@collection_id", collectionId),
                    new SqlParameter("@user_id", userId)
                };

                Debug.WriteLine("Repository: Executing GetGamesNotInCollection stored procedure");
                var dataTable = _dataLink.ExecuteReader("GetGamesNotInCollection", parameters);
                Debug.WriteLine($"Repository: Got {dataTable?.Rows?.Count ?? 0} rows from database");

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    Debug.WriteLine("Repository: No games found outside collection");
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

                Debug.WriteLine($"Repository: Mapped {games.Count} games");
                return games;
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Repository: SQL Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while getting games not in collection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Repository: Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Repository: Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while getting games not in collection.", ex);
            }
        }

        private static List<Collection> MapDataTableToCollections(DataTable dataTable)
        {
            try
            {
                Debug.WriteLine("Starting to map DataTable to Collections");
                var collections = dataTable.AsEnumerable().Select(row =>
                {
                    var collection = new Collection(
                        userId: Convert.ToInt32(row["user_id"]),
                        name: row["name"].ToString(),
                        createdAt: DateOnly.FromDateTime(Convert.ToDateTime(row["created_at"])),
                        coverPicture: row["cover_picture"]?.ToString(),
                        isPublic: Convert.ToBoolean(row["is_public"])
                    );
                    collection.CollectionId = Convert.ToInt32(row["collection_id"]);
                    return collection;
                }).ToList();
                Debug.WriteLine($"Successfully mapped {collections.Count} collections");
                return collections;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error mapping DataTable: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        private static Collection MapDataRowToCollection(DataRow row)
        {
            var collection = new Collection(
                userId: Convert.ToInt32(row["user_id"]),
                name: row["name"].ToString(),
                createdAt: DateOnly.FromDateTime(Convert.ToDateTime(row["created_at"])),
                coverPicture: row["cover_picture"]?.ToString(),
                isPublic: Convert.ToBoolean(row["is_public"])
            );
            collection.CollectionId = Convert.ToInt32(row["collection_id"]);
            return collection;
        }
    }
}
