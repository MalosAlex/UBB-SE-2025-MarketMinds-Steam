using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace SteamProfile.ViewModels
{
    public class CreateCollectionParams
    {
        public string Name { get; set; }
        public string CoverPicture { get; set; }
        public bool IsPublic { get; set; }
        public DateOnly CreatedAt { get; set; }
    }

    public class UpdateCollectionParams
    {
        public int CollectionId { get; set; }
        public string Name { get; set; }
        public string CoverPicture { get; set; }
        public bool IsPublic { get; set; }
    }

    public partial class CollectionsViewModel : ObservableObject
    {
        private readonly ICollectionsService _collectionsService;
        private readonly IUserService _userService;
        private int _userId;
        private ObservableCollection<Collection> _collections;

        [ObservableProperty]
        private Collection _selectedCollection;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        public ObservableCollection<Collection> Collections
        {
            get => _collections;
            set
            {
                if (_collections != value)
                {
                    _collections = value;
                    OnPropertyChanged();
                }
            }
        }

        public CollectionsViewModel(ICollectionsService collectionsService, IUserService userService)
        {
            _collectionsService = collectionsService ?? throw new ArgumentNullException(nameof(collectionsService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _collections = new ObservableCollection<Collection>();
        }


        [RelayCommand]
        public void LoadCollections()
        {
            try
            {
                _userId = _userService.GetCurrentUser().UserId;
                IsLoading = true;
                ErrorMessage = string.Empty;
                Debug.WriteLine("Loading collections...");

                var collections = _collectionsService.GetAllCollections(_userId);
                Debug.WriteLine($"Retrieved {collections?.Count ?? 0} collections from service");

                if (collections == null || collections.Count == 0)
                {
                    Debug.WriteLine("Collections list is empty or null");
                    ErrorMessage = "No collections found.";
                    Collections.Clear();
                    return;
                }

                Collections.Clear();
                foreach (var collection in collections)
                {
                    Collections.Add(collection);
                }
                Debug.WriteLine($"Added {collections.Count} collections to ObservableCollection");
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error loading collections: {exception.Message}");
                Debug.WriteLine($"Stack trace: {exception.StackTrace}");
                ErrorMessage = "Error loading collections. Please try again.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void DeleteCollection(int collectionId)
        {
            try
            {
                Debug.WriteLine($"Deleting collection {collectionId}");
                _collectionsService.DeleteCollection(collectionId, _userId);
                LoadCollections(); // Reload collections after deletion
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting collection: {ex.Message}");
                ErrorMessage = "Error deleting collection. Please try again.";
            }
        }

        [RelayCommand]
        private void ViewCollection(Collection collection)
        {
            try
            {
                if (collection == null)
                {
                    Debug.WriteLine("No collection selected");
                    return;
                }

                Debug.WriteLine($"Viewing collection: {collection.Name}");
                SelectedCollection = collection;
                // TODO: Navigate to collection details page
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error viewing collection: {exception.Message}");
                ErrorMessage = "Error viewing collection. Please try again.";
            }
        }

        [RelayCommand]
        private void AddGameToCollection(int gameId)
        {
            try
            {
                if (SelectedCollection == null)
                {
                    Debug.WriteLine("No collection selected");
                    return;
                }

                Debug.WriteLine($"Adding game {gameId} to collection {SelectedCollection.Name}");
                _collectionsService.AddGameToCollection(SelectedCollection.CollectionId, gameId, _userId);
                LoadCollections(); // Reload collections to update the UI
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error adding game to collection: {exception.Message}");
                ErrorMessage = "Error adding game to collection. Please try again.";
            }
        }

        [RelayCommand]
        private void RemoveGameFromCollection(int gameId)
        {
            try
            {
                if (SelectedCollection == null)
                {
                    Debug.WriteLine("No collection selected");
                    return;
                }

                Debug.WriteLine($"Removing game {gameId} from collection {SelectedCollection.Name}");
                _collectionsService.RemoveGameFromCollection(SelectedCollection.CollectionId, gameId);
                LoadCollections(); // Reload collections to update the UI
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error removing game from collection: {exception.Message}");
                ErrorMessage = "Error removing game from collection. Please try again.";
            }
        }

        [RelayCommand]
        private void CreateCollection(CreateCollectionParams parameters)
        {
            try
            {
                Debug.WriteLine($"Creating collection: {parameters.Name}");
                _collectionsService.CreateCollection(
                    _userId,
                    parameters.Name,
                    parameters.CoverPicture,
                    parameters.IsPublic,
                    parameters.CreatedAt
                );
                LoadCollections(); // Reload collections after creation
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error creating collection: {exception.Message}");
                ErrorMessage = "Error creating collection. Please try again.";
            }
        }

        [RelayCommand]
        private void UpdateCollection(UpdateCollectionParams parameters)
        {
            try
            {
                Debug.WriteLine($"Updating collection: {parameters.Name}");
                _collectionsService.UpdateCollection(
                    parameters.CollectionId,
                    _userId,
                    parameters.Name,
                    parameters.CoverPicture,
                    parameters.IsPublic
                );
                LoadCollections(); // Reload collections after update
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating collection: {ex.Message}");
                ErrorMessage = "Error updating collection. Please try again.";
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            return _collectionsService.GetPublicCollectionsForUser(userId);
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            return _collectionsService.GetCollectionById(collectionId, userId);
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            _collectionsService.RemoveGameFromCollection(collectionId, gameId);
        }
    }
}
