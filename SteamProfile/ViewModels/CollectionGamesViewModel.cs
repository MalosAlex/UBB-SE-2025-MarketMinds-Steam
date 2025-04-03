using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamProfile.Models;
using SteamProfile.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SteamProfile.ViewModels
{
    public partial class CollectionGamesViewModel : ObservableObject
    {
        private readonly CollectionsService _collectionsService;

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

        public CollectionGamesViewModel(CollectionsService collectionsService)
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
                IsAllOwnedGamesCollection = collectionId == 1;
                Debug.WriteLine($"Loading games for collection {collectionId}");
                var games = _collectionsService.GetGamesInCollection(collectionId);
                Games.Clear();
                foreach (var game in games)
                {
                    Games.Add(game);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading games: {ex.Message}");
                ErrorMessage = "Failed to load games";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
} 