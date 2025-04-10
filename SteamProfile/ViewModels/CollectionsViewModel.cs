using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

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
        #region Constants
        // Error message constants
        private const string ErrNoCollectionsFound = "No collections found.";
        private const string ErrLoadCollections = "Error loading collections. Please try again.";
        private const string ErrDeleteCollection = "Error deleting collection. Please try again.";
        private const string ErrViewCollection = "Error viewing collection. Please try again.";
        private const string ErrAddGameToCollection = "Error adding game to collection. Please try again.";
        private const string ErrRemoveGameFromCollection = "Error removing game from collection. Please try again.";
        private const string ErrCreateCollection = "Error creating collection. Please try again.";
        private const string ErrUpdateCollection = "Error updating collection. Please try again.";
        #endregion

        private readonly ICollectionsService collectionsService;
        private readonly IUserService userService;
        private int userIdentifier;
        private ObservableCollection<Collection> collections;

        [ObservableProperty]
        private Collection selectedCollection;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        public ObservableCollection<Collection> Collections
        {
            get => collections;
            set
            {
                if (collections != value)
                {
                    collections = value;
                    OnPropertyChanged();
                }
            }
        }

        public CollectionsViewModel(ICollectionsService collectionsService, IUserService userService)
        {
            this.collectionsService = collectionsService ?? throw new ArgumentNullException(nameof(collectionsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            collections = new ObservableCollection<Collection>();
        }

        [RelayCommand]
        public void LoadCollections()
        {
            try
            {
                userIdentifier = userService.GetCurrentUser().UserId;
                IsLoading = true;
                ErrorMessage = string.Empty;

                var collections = collectionsService.GetAllCollections(userIdentifier);

                if (collections == null || collections.Count == 0)
                {
                    ErrorMessage = ErrNoCollectionsFound;
                    Collections.Clear();
                    return;
                }

                Collections.Clear();
                foreach (var collection in collections)
                {
                    Collections.Add(collection);
                }
            }
            catch (Exception)
            {
                ErrorMessage = ErrLoadCollections;
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
                collectionsService.DeleteCollection(collectionId, userIdentifier);
                LoadCollections(); // Reload collections after deletion
            }
            catch (Exception)
            {
                ErrorMessage = ErrDeleteCollection;
            }
        }

        [RelayCommand]
        private void ViewCollection(Collection collection)
        {
            try
            {
                if (collection == null)
                {
                    return;
                }

                SelectedCollection = collection;
                // TODO: Navigate to collection details page
            }
            catch (Exception)
            {
                ErrorMessage = ErrViewCollection;
            }
        }

        [RelayCommand]
        private void AddGameToCollection(int gameId)
        {
            try
            {
                if (SelectedCollection == null)
                {
                    return;
                }

                collectionsService.AddGameToCollection(SelectedCollection.CollectionId, gameId, userIdentifier);
                LoadCollections(); // Reload collections to update the UI
            }
            catch (Exception)
            {
                ErrorMessage = ErrAddGameToCollection;
            }
        }

        [RelayCommand]
        private void RemoveGameFromCollection(int gameId)
        {
            try
            {
                if (SelectedCollection == null)
                {
                    return;
                }

                collectionsService.RemoveGameFromCollection(SelectedCollection.CollectionId, gameId);
                LoadCollections(); // Reload collections to update the UI
            }
            catch (Exception)
            {
                ErrorMessage = ErrRemoveGameFromCollection;
            }
        }

        [RelayCommand]
        private void CreateCollection(CreateCollectionParams parameters)
        {
            try
            {
                collectionsService.CreateCollection(
                    userIdentifier,
                    parameters.Name,
                    parameters.CoverPicture,
                    parameters.IsPublic,
                    parameters.CreatedAt);
                LoadCollections(); // Reload collections after creation
            }
            catch (Exception)
            {
                ErrorMessage = ErrCreateCollection;
            }
        }

        [RelayCommand]
        private void UpdateCollection(UpdateCollectionParams parameters)
        {
            try
            {
                collectionsService.UpdateCollection(
                    parameters.CollectionId,
                    userIdentifier,
                    parameters.Name,
                    parameters.CoverPicture,
                    parameters.IsPublic);
                LoadCollections(); // Reload collections after update
            }
            catch (Exception)
            {
                ErrorMessage = ErrUpdateCollection;
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            return collectionsService.GetPublicCollectionsForUser(userId);
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            return collectionsService.GetCollectionById(collectionId, userId);
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            collectionsService.RemoveGameFromCollection(collectionId, gameId);
        }
    }
}
