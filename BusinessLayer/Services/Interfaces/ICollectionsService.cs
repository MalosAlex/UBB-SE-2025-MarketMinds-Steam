using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface ICollectionsService
    {
        List<Collection> GetAllCollections(int userId);
        Collection GetCollectionById(int collectionId, int userId);
        List<OwnedGame> GetGamesInCollection(int collectionId);
        void AddGameToCollection(int collectionId, int gameId, int userId);
        void RemoveGameFromCollection(int collectionId, int gameId);
        void DeleteCollection(int collectionId, int userId);
        void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt);
        void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic);
        List<Collection> GetPublicCollectionsForUser(int userId);
        List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId);
    }
}