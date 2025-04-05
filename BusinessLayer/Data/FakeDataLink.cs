using BusinessLayer.Exceptions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BusinessLayer.Data
{
    public class FakeDataLink : IDataLink
    {
        public int throwError { get; set; } = 0;
        // Existing in–memory table for friendships (omitted for brevity; assume already defined).
        private readonly List<Dictionary<string, object>> _friendshipsTable;
        private readonly List<Dictionary<string, object>> _usersTable;
        private readonly List<Dictionary<string, object>> _profilesTable;

        // New in–memory table for collections.
        private readonly List<Dictionary<string, object>> _collectionsTable;
        // New in–memory table for games.
        private readonly List<Dictionary<string, object>> _gamesTable;

        public FakeDataLink()
        {
            // Existing seed for friendships.
            _friendshipsTable = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "friendship_id", 1 },
                    { "user_id", 1 },
                    { "friend_id", 2 }
                },
                new Dictionary<string, object>
                {
                    { "friendship_id", 2 },
                    { "user_id", 1 },
                    { "friend_id", 3 }
                }
            };

            // Existing seed for users.
            _usersTable = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "user_id", 1 },{"email", "user@mail.com" }, { "username", "User1" } },
                new Dictionary<string, object> { { "user_id", 2 }, { "email", "alice@mail.com" }, { "username", "Alice" } },
                new Dictionary<string, object> { { "user_id", 3 }, { "email", "bob@mail.com" }, { "username", "Bob" } },
                new Dictionary<string, object> { { "user_id", 4 }, { "email", "charlie@mail.com" }, { "username", "Charlie" } }
            };

            // Existing seed for profiles.
            _profilesTable = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "user_id", 2 }, { "profile_picture", "alice.jpg" } },
                new Dictionary<string, object> { { "user_id", 3 }, { "profile_picture", "bob.jpg" } }
            };

            // Seed some collections for user 1.
            _collectionsTable = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "collection_id", 1 },
                    { "user_id", 1 },
                    { "name", "Collection 1" },
                    { "cover_picture", "pic1.jpg" },
                    { "is_public", true },
                    { "created_at", new DateTime(2025, 1, 1) }
                },
                new Dictionary<string, object>
                {
                    { "collection_id", 2 },
                    { "user_id", 1 },
                    { "name", "Collection 2" },
                    { "cover_picture", "pic2.jpg" },
                    { "is_public", false },
                    { "created_at", new DateTime(2025, 2, 1) }
                },
                new Dictionary<string, object>
                {
                    { "collection_id", 3 },
                    { "user_id", 1 },
                    { "name", "Collection 3" },
                    { "cover_picture", "pic3.jpg" },
                    { "is_public", true },
                    { "created_at", new DateTime(2025, 3, 1) }
                }
            };

            // Seed some games – two games for collection 1 and some games outside.
            _gamesTable = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "game_id", 1 },
                    { "user_id", 1 },
                    { "title", "Game A" },
                    { "description", "Desc A" },
                    { "cover_picture", "coverA.jpg" },
                    { "collection_id", 1 }
                },
                new Dictionary<string, object>
                {
                    { "game_id", 2 },
                    { "user_id", 1 },
                    { "title", "Game B" },
                    { "description", "Desc B" },
                    { "cover_picture", "coverB.jpg" },
                    { "collection_id", 1 }
                },
                // A game that is not in collection 5 (for GetGamesNotInCollection).
                new Dictionary<string, object>
                {
                    { "game_id", 3 },
                    { "user_id", 1 },
                    { "title", "Game C" },
                    { "description", "Desc C" },
                    { "cover_picture", "coverC.jpg" },
                    { "collection_id", 999 } // Not part of any collection
                }
            };
        }

        public T? ExecuteScalar<T>(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {

            if (storedProcedure == "GetFriendshipCountForUser")
            {
                int userId = (int)sqlParameters.First(p => p.ParameterName == "@user_id").Value;
                int count = _friendshipsTable.Count(f => (int)f["user_id"] == userId);
                return (T)Convert.ChangeType(count, typeof(T));
            }
            if (storedProcedure == "GetFriendshipId")
            {
                int userId = (int)sqlParameters.First(p => p.ParameterName == "@user_id").Value;
                int friendId = (int)sqlParameters.First(p => p.ParameterName == "@friend_id").Value;
                var friendship = _friendshipsTable.FirstOrDefault(f => (int)f["user_id"] == userId && (int)f["friend_id"] == friendId);
                if (friendship != null)
                {
                    object value = friendship["friendship_id"];
                    if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var underlyingType = Nullable.GetUnderlyingType(typeof(T));
                        return (T)Convert.ChangeType(value, underlyingType);
                    }
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }
            return default;
        }

        public DataTable ExecuteReader(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {

            DataTable dt = new DataTable();

            #region Friendships/User/Profiles (existing)
            if (storedProcedure == "GetFriendsForUser")
            {
                dt.Columns.Add("friendship_id", typeof(int));
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("friend_id", typeof(int));

                int userId = (int)sqlParameters.First(p => p.ParameterName == "@user_id").Value;
                foreach (var row in _friendshipsTable.Where(r => (int)r["user_id"] == userId))
                {
                    DataRow dr = dt.NewRow();
                    dr["friendship_id"] = row["friendship_id"];
                    dr["user_id"] = row["user_id"];
                    dr["friend_id"] = row["friend_id"];
                    dt.Rows.Add(dr);
                }
            }
            else if (storedProcedure == "GetUserById")
            {
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("username", typeof(string));

                int userId = (int)sqlParameters.First(p => p.ParameterName == "@user_id").Value;
                var user = _usersTable.FirstOrDefault(u => (int)u["user_id"] == userId);
                if (user != null)
                {
                    DataRow dr = dt.NewRow();
                    dr["user_id"] = user["user_id"];
                    dr["username"] = user["username"];
                    dt.Rows.Add(dr);
                }
            }
            else if (storedProcedure == "GetUserProfileByUserId")
            {
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("profile_picture", typeof(string));

                int userId = (int)sqlParameters.First(p => p.ParameterName == "@user_id").Value;
                var profile = _profilesTable.FirstOrDefault(p => (int)p["user_id"] == userId);
                if (profile != null)
                {
                    DataRow dr = dt.NewRow();
                    dr["user_id"] = profile["user_id"];
                    dr["profile_picture"] = profile["profile_picture"];
                    dt.Rows.Add(dr);
                }
            }
            else if (storedProcedure == "GetFriendshipById")
            {
                dt.Columns.Add("friendship_id", typeof(int));
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("friend_id", typeof(int));

                int friendshipId;
                if (sqlParameters.Any(p => p.ParameterName == "@friendshipId"))
                {
                    friendshipId = (int)sqlParameters.First(p => p.ParameterName == "@friendshipId").Value;
                }
                else
                {
                    friendshipId = (int)sqlParameters.First(p => p.ParameterName == "@friendship_id").Value;
                }
                var friendship = _friendshipsTable.FirstOrDefault(f => (int)f["friendship_id"] == friendshipId);
                if (friendship != null)
                {
                    DataRow dr = dt.NewRow();
                    dr["friendship_id"] = friendship["friendship_id"];
                    dr["user_id"] = friendship["user_id"];
                    dr["friend_id"] = friendship["friend_id"];
                    dt.Rows.Add(dr);
                }
            }
            #endregion

            #region Collections Procedures
            else if (storedProcedure == "GetAllCollectionsForUser")
            {
                dt.Columns.Add("collection_id", typeof(int));
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("name", typeof(string));
                dt.Columns.Add("cover_picture", typeof(string));
                dt.Columns.Add("is_public", typeof(bool));
                dt.Columns.Add("created_at", typeof(DateTime));

                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                var rows = _collectionsTable.Where(c => Convert.ToInt32(c["user_id"]) == userId);
                foreach (var row in rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["collection_id"] = row["collection_id"];
                    dr["user_id"] = row["user_id"];
                    dr["name"] = row["name"];
                    dr["cover_picture"] = row["cover_picture"];
                    dr["is_public"] = row["is_public"];
                    dr["created_at"] = row["created_at"];
                    dt.Rows.Add(dr);
                }
            }
            else if (storedProcedure == "GetCollectionById")
            {
                dt.Columns.Add("collection_id", typeof(int));
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("name", typeof(string));
                dt.Columns.Add("cover_picture", typeof(string));
                dt.Columns.Add("is_public", typeof(bool));
                dt.Columns.Add("created_at", typeof(DateTime));

                int collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collectionId").Value);
                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                var row = _collectionsTable.FirstOrDefault(c => Convert.ToInt32(c["collection_id"]) == collectionId && Convert.ToInt32(c["user_id"]) == userId);
                if (row != null)
                {
                    DataRow dr = dt.NewRow();
                    dr["collection_id"] = row["collection_id"];
                    dr["user_id"] = row["user_id"];
                    dr["name"] = row["name"];
                    dr["cover_picture"] = row["cover_picture"];
                    dr["is_public"] = row["is_public"];
                    dr["created_at"] = row["created_at"];
                    dt.Rows.Add(dr);
                }
            }
            else if (storedProcedure == "GetPublicCollectionsForUser")
            {
                dt.Columns.Add("collection_id", typeof(int));
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("name", typeof(string));
                dt.Columns.Add("cover_picture", typeof(string));
                dt.Columns.Add("is_public", typeof(bool));
                dt.Columns.Add("created_at", typeof(DateTime));

                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                var rows = _collectionsTable.Where(c => Convert.ToInt32(c["user_id"]) == userId && Convert.ToBoolean(c["is_public"]) == true);
                foreach (var row in rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["collection_id"] = row["collection_id"];
                    dr["user_id"] = row["user_id"];
                    dr["name"] = row["name"];
                    dr["cover_picture"] = row["cover_picture"];
                    dr["is_public"] = row["is_public"];
                    dr["created_at"] = row["created_at"];
                    dt.Rows.Add(dr);
                }
            }
            else if (storedProcedure == "GetGamesInCollection")
            {
                dt.Columns.Add("game_id", typeof(int));
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("title", typeof(string));
                dt.Columns.Add("description", typeof(string));
                dt.Columns.Add("cover_picture", typeof(string));

                int collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collection_id").Value);
                var rows = _gamesTable.Where(g => Convert.ToInt32(g["collection_id"]) == collectionId);
                foreach (var row in rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["game_id"] = row["game_id"];
                    dr["user_id"] = row["user_id"];
                    dr["title"] = row["title"];
                    dr["description"] = row["description"];
                    dr["cover_picture"] = row["cover_picture"];
                    dt.Rows.Add(dr);
                }
            }
            else if (storedProcedure == "GetAllGamesForUser")
            {
                dt.Columns.Add("game_id", typeof(int));
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("title", typeof(string));
                dt.Columns.Add("description", typeof(string));
                dt.Columns.Add("cover_picture", typeof(string));

                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                var rows = _gamesTable.Where(g => Convert.ToInt32(g["user_id"]) == userId && Convert.ToInt32(g["collection_id"]) == 1);
                foreach (var row in rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["game_id"] = row["game_id"];
                    dr["user_id"] = row["user_id"];
                    dr["title"] = row["title"];
                    dr["description"] = row["description"];
                    dr["cover_picture"] = row["cover_picture"];
                    dt.Rows.Add(dr);
                }
            }
            else if (storedProcedure == "GetGamesNotInCollection")
            {
                dt.Columns.Add("game_id", typeof(int));
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("title", typeof(string));
                dt.Columns.Add("description", typeof(string));
                dt.Columns.Add("cover_picture", typeof(string));

                int collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collection_id").Value);
                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                var rows = _gamesTable.Where(g => Convert.ToInt32(g["user_id"]) == userId && Convert.ToInt32(g["collection_id"]) != collectionId);
                foreach (var row in rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["game_id"] = row["game_id"];
                    dr["user_id"] = row["user_id"];
                    dr["title"] = row["title"];
                    dr["description"] = row["description"];
                    dr["cover_picture"] = row["cover_picture"];
                    dt.Rows.Add(dr);
                }
            }
            // Procedures for making a collection private/public.
            else if (storedProcedure == "MakeCollectionPrivate")
            {
                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                int collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collection_id").Value);
                var collection = _collectionsTable.FirstOrDefault(c => Convert.ToInt32(c["collection_id"]) == collectionId && Convert.ToInt32(c["user_id"]) == userId);
                if (collection != null)
                {
                    collection["is_public"] = false;
                }
                // Return an empty table.
            }
            else if (storedProcedure == "MakeCollectionPublic")
            {
                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                int collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collection_id").Value);
                var collection = _collectionsTable.FirstOrDefault(c => Convert.ToInt32(c["collection_id"]) == collectionId && Convert.ToInt32(c["user_id"]) == userId);
                if (collection != null)
                {
                    collection["is_public"] = true;
                }
            }
            else if (storedProcedure == "CreateCollection")
            {
                // Create new collection. Assume parameters: @user_id, @name, @cover_picture, @is_public, @created_at.
                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                string name = sqlParameters.First(p => p.ParameterName == "@name").Value.ToString();
                string cover = sqlParameters.First(p => p.ParameterName == "@cover_picture").Value.ToString();
                bool isPublic = Convert.ToBoolean(sqlParameters.First(p => p.ParameterName == "@is_public").Value);
                DateTime createdAt = Convert.ToDateTime(sqlParameters.First(p => p.ParameterName == "@created_at").Value);
                int newId = _collectionsTable.Any() ? _collectionsTable.Max(c => Convert.ToInt32(c["collection_id"])) + 1 : 1;
                _collectionsTable.Add(new Dictionary<string, object>
                {
                    { "collection_id", newId },
                    { "user_id", userId },
                    { "name", name },
                    { "cover_picture", cover },
                    { "is_public", isPublic },
                    { "created_at", createdAt }
                });
            }
            else if (storedProcedure == "UpdateCollection")
            {
                // Update existing collection.
                int collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collection_id").Value);
                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                string name = sqlParameters.First(p => p.ParameterName == "@name").Value.ToString();
                string cover = sqlParameters.First(p => p.ParameterName == "@cover_picture").Value.ToString();
                bool isPublic = Convert.ToBoolean(sqlParameters.First(p => p.ParameterName == "@is_public").Value);
                DateTime createdAt = Convert.ToDateTime(sqlParameters.First(p => p.ParameterName == "@created_at").Value);
                var collection = _collectionsTable.FirstOrDefault(c => Convert.ToInt32(c["collection_id"]) == collectionId && Convert.ToInt32(c["user_id"]) == userId);
                if (collection != null)
                {
                    collection["name"] = name;
                    collection["cover_picture"] = cover;
                    collection["is_public"] = isPublic;
                    collection["created_at"] = createdAt;
                }
            }
            else if (storedProcedure == "DeleteCollection")
            {
                int collectionId;
                // Handle both ExecuteReader and ExecuteNonQuery scenarios.
                if (sqlParameters.Any(p => p.ParameterName == "@collection_id"))
                    collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collection_id").Value);
                else
                    collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collectionId").Value);
                // For ExecuteReader branch, we return a dummy table.
                var coll = _collectionsTable.FirstOrDefault(c => Convert.ToInt32(c["collection_id"]) == collectionId);
                if (coll != null)
                {
                    _collectionsTable.Remove(coll);
                }
            }
            #endregion

            #region Users
            else if (storedProcedure == "GetAllUsers")
            {
                if (throwError == 1)
                {
                    throw new DatabaseOperationException("Test exception");
                }
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("email", typeof(string));
                dt.Columns.Add("username", typeof(string));
                foreach (var row in _usersTable)
                {
                    DataRow dr = dt.NewRow();
                    dr["user_id"] = row["user_id"];
                    dr["username"] = row["username"];
                    dr["email"] = row["email"];
                    dt.Rows.Add(dr);
                }
            }
            else if (storedProcedure == "GetUserById")
            {
                if (throwError == 1)
                {
                    throw new DatabaseOperationException("Test exception");
                }
                dt.Columns.Add("user_id", typeof(int));
                dt.Columns.Add("email", typeof(string));
                dt.Columns.Add("username", typeof(string));
                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                var rows = _usersTable.Where(c => Convert.ToInt32(c["user_id"]) == userId);
                foreach (var row in rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["user_id"] = row["user_id"];
                    dr["email"] = row["email"];
                    dr["username"] = row["username"];
                    dt.Rows.Add(dr);     
                }
            }
                #endregion

                return dt;
        }

        public int ExecuteNonQuery(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {

            // For many procedures, we perform the same operations as in ExecuteReader.
            if (storedProcedure == "AddFriend")
            {
                int userId = (int)sqlParameters.First(p => p.ParameterName == "@user_id").Value;
                int friendId = (int)sqlParameters.First(p => p.ParameterName == "@friend_id").Value;
                if (_friendshipsTable.Any(f => (int)f["user_id"] == userId && (int)f["friend_id"] == friendId))
                {
                    return 0;
                }
                int newFriendshipId = _friendshipsTable.Count > 0 ?
                    _friendshipsTable.Max(f => Convert.ToInt32(f["friendship_id"])) + 1 : 1;
                _friendshipsTable.Add(new Dictionary<string, object>
                {
                    { "friendship_id", newFriendshipId },
                    { "user_id", userId },
                    { "friend_id", friendId }
                });
                return 1;
            }
            else if (storedProcedure == "RemoveFriend")
            {
                int friendshipId;
                if (sqlParameters.Any(p => p.ParameterName == "@friendshipId"))
                    friendshipId = (int)sqlParameters.First(p => p.ParameterName == "@friendshipId").Value;
                else
                    friendshipId = (int)sqlParameters.First(p => p.ParameterName == "@friendship_id").Value;
                var friendship = _friendshipsTable.FirstOrDefault(f => (int)f["friendship_id"] == friendshipId);
                if (friendship != null)
                {
                    _friendshipsTable.Remove(friendship);
                    return 1;
                }
                return 0;
            }
            else if (storedProcedure == "AddGameToCollection")
            {
                // For our fake, just return success.
                return 1;
            }
            else if (storedProcedure == "RemoveGameFromCollection")
            {
                // For our fake, just return success.
                return 1;
            }
            else if (storedProcedure == "CreateCollection")
            {
                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                string name = sqlParameters.First(p => p.ParameterName == "@name").Value.ToString();
                string cover = sqlParameters.First(p => p.ParameterName == "@cover_picture").Value.ToString();
                bool isPublic = Convert.ToBoolean(sqlParameters.First(p => p.ParameterName == "@is_public").Value);
                DateTime createdAt = Convert.ToDateTime(sqlParameters.First(p => p.ParameterName == "@created_at").Value);
                int newId = _collectionsTable.Any() ? _collectionsTable.Max(c => Convert.ToInt32(c["collection_id"])) + 1 : 1;
                _collectionsTable.Add(new Dictionary<string, object>
                {
                    { "collection_id", newId },
                    { "user_id", userId },
                    { "name", name },
                    { "cover_picture", cover },
                    { "is_public", isPublic },
                    { "created_at", createdAt }
                });
                return 1;
            }
            else if (storedProcedure == "UpdateCollection")
            {
                int collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collection_id").Value);
                int userId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@user_id").Value);
                string name = sqlParameters.First(p => p.ParameterName == "@name").Value.ToString();
                string cover = sqlParameters.First(p => p.ParameterName == "@cover_picture").Value.ToString();
                bool isPublic = Convert.ToBoolean(sqlParameters.First(p => p.ParameterName == "@is_public").Value);
                DateTime createdAt = Convert.ToDateTime(sqlParameters.First(p => p.ParameterName == "@created_at").Value);
                var collection = _collectionsTable.FirstOrDefault(c => Convert.ToInt32(c["collection_id"]) == collectionId && Convert.ToInt32(c["user_id"]) == userId);
                if (collection != null)
                {
                    collection["name"] = name;
                    collection["cover_picture"] = cover;
                    collection["is_public"] = isPublic;
                    collection["created_at"] = createdAt;
                    return 1;
                }
                return 0;
            }
            else if (storedProcedure == "DeleteCollection")
            {
                int collectionId;
                if (sqlParameters.Any(p => p.ParameterName == "@collection_id"))
                    collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collection_id").Value);
                else
                    collectionId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName == "@collectionId").Value);
                var coll = _collectionsTable.FirstOrDefault(c => Convert.ToInt32(c["collection_id"]) == collectionId);
                if (coll != null)
                {
                    _collectionsTable.Remove(coll);
                    return 1;
                }
                return 0;
            }
            // For procedures that are implemented in ExecuteReader only, return 1.
            return 1;
        }



        public void Dispose()
        {
            // Nothing to dispose in the fake.
        }
    }
}
