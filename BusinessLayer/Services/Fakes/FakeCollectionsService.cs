using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Fakes
{
    public class FakeCollectionsService : ICollectionsService
    {
        private List<Collection> collections;
        private List<OwnedGame> games;

        public FakeCollectionsService()
        {
            // Seed some fake collections.
            collections = new List<Collection>();
            collections.Add(new Collection(1, "Collection 1", DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), "cover1.jpg", true) { CollectionId = 1 });
            collections.Add(new Collection(1, "Collection 2", DateOnly.FromDateTime(DateTime.Now.AddDays(-3)), "cover2.jpg", false) { CollectionId = 2 });
            collections.Add(new Collection(1, "Collection 3", DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), "cover3.jpg", true) { CollectionId = 3 });
            collections.Add(new Collection(2, "Other User Collection", DateOnly.FromDateTime(DateTime.Now.AddDays(-2)), "cover4.jpg", true) { CollectionId = 4 });

            // Seed some dummy games (only for collectionId == 1) using the new constructor.
            games = new List<OwnedGame>();
            var gameA = new OwnedGame(1, "Game A", "Desc A", "gameA.jpg");
            gameA.GameId = 1;
            games.Add(gameA);

            var gameB = new OwnedGame(1, "Game B", "Desc B", "gameB.jpg");
            gameB.GameId = 2;
            games.Add(gameB);
        }

        public List<Collection> GetAllCollections(int userId)
        {
            List<Collection> result = new List<Collection>();
            foreach (Collection c in collections)
            {
                if (c.UserId == userId)
                {
                    result.Add(c);
                }
            }
            return result;
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            Collection found = null;
            foreach (Collection c in collections)
            {
                if (c.CollectionId == collectionId && c.UserId == userId)
                {
                    found = c;
                    break;
                }
            }
            if (found != null && found.CollectionId == 1)
            {
                // For collection 1, attach dummy games.
                found.Games = new List<OwnedGame>();
                foreach (OwnedGame g in games)
                {
                    found.Games.Add(g);
                }
            }
            return found;
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            // When user is not specified, return empty list.
            return new List<OwnedGame>();
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId, int userId)
        {
            if (collectionId == 1)
            {
                List<OwnedGame> result = new List<OwnedGame>();
                foreach (OwnedGame g in games)
                {
                    result.Add(g);
                }
                return result;
            }
            return GetGamesInCollection(collectionId);
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            // Fake implementation: No operation.
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            // Fake implementation: No operation.
        }

        public void MakeCollectionPrivateForUser(string userId, string collectionId)
        {
            // Fake implementation: No operation.
        }

        public void MakeCollectionPublicForUser(string userId, string collectionId)
        {
            // Fake implementation: No operation.
        }

        public void RemoveCollectionForUser(string userId, string collectionId)
        {
            int uid = int.Parse(userId);
            int cid = int.Parse(collectionId);
            Collection toRemove = null;
            foreach (Collection c in collections)
            {
                if (c.UserId == uid && c.CollectionId == cid)
                {
                    toRemove = c;
                    break;
                }
            }
            if (toRemove != null)
            {
                collections.Remove(toRemove);
            }
        }

        public void SaveCollection(string userId, Collection collection)
        {
            int uid = int.Parse(userId);
            if (collection.CollectionId == 0)
            {
                int newId = collections.Count > 0 ? collections[collections.Count - 1].CollectionId + 1 : 1;
                collection.CollectionId = newId;
                collection.UserId = uid;
                collections.Add(collection);
            }
            else
            {
                for (int i = 0; i < collections.Count; i++)
                {
                    if (collections[i].CollectionId == collection.CollectionId && collections[i].UserId == uid)
                    {
                        collections[i] = collection;
                        break;
                    }
                }
            }
        }

        public void DeleteCollection(int collectionId, int userId)
        {
            Collection toDelete = null;
            foreach (Collection c in collections)
            {
                if (c.CollectionId == collectionId && c.UserId == userId)
                {
                    toDelete = c;
                    break;
                }
            }
            if (toDelete != null)
            {
                collections.Remove(toDelete);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            int newId = collections.Count > 0 ? collections[collections.Count - 1].CollectionId + 1 : 1;
            Collection newCol = new Collection(userId, name, createdAt, coverPicture, isPublic)
            {
                CollectionId = newId
            };
            collections.Add(newCol);
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            foreach (Collection c in collections)
            {
                if (c.CollectionId == collectionId && c.UserId == userId)
                {
                    c.Name = name;
                    c.CoverPicture = coverPicture;
                    c.IsPublic = isPublic;
                    c.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
                    break;
                }
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            List<Collection> result = new List<Collection>();
            foreach (Collection c in collections)
            {
                if (c.UserId == userId && c.IsPublic)
                {
                    result.Add(c);
                }
            }
            return result;
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            List<OwnedGame> result = new List<OwnedGame>();
            var gameC = new OwnedGame(userId, "Game C", "Desc C", "gameC.jpg");
            gameC.GameId = 3;
            result.Add(gameC);
            return result;
        }

        private int CompareByCreatedAtDescending(Collection a, Collection b)
        {
            return b.CreatedAt.CompareTo(a.CreatedAt);
        }

        public List<Collection> GetLastThreeCollectionsForUser(int userId)
        {
            List<Collection> userCollections = GetAllCollections(userId);
            userCollections.Sort(CompareByCreatedAtDescending);
            int count = userCollections.Count < 3 ? userCollections.Count : 3;
            List<Collection> result = new List<Collection>();
            for (int i = 0; i < count; i++)
            {
                result.Add(userCollections[i]);
            }
            return result;
        }
    }
}
