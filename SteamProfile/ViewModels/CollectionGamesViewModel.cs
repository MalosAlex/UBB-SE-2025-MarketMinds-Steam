using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusinessLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public partial class CollectionGamesViewModel : ObservableObject
    {
        // Constants to replace magic numbers and strings
        private const int AllOwnedGamesCollectionId = 1;
        private const string FailedToLoadGamesErrorMessage = "Failed to load games";

        private readonly ICollectionsService _collectionsService;

        [ObservableProperty]
        private string _collectionName;

        [ObservableProperty]
        private ObservableCollection<OwnedGame> _games;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isAllOwnedGamesCollection;

        public CollectionGamesViewModel(ICollectionsService collectionsService)
        {
            _collectionsService = collectionsService;
            _games = new ObservableCollection<OwnedGame>();
        }

        public void LoadGames(int collectionId)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                IsAllOwnedGamesCollection = collectionId == AllOwnedGamesCollectionId;
                var gamesInCollection = _collectionsService.GetGamesInCollection(collectionId);
                Games.Clear();
                foreach (var game in gamesInCollection)
                {
                    Games.Add(game);
                }
            }
            catch (Exception exception)
            {
                ErrorMessage = FailedToLoadGamesErrorMessage;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
