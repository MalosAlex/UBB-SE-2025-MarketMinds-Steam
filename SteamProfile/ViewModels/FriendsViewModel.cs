using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Models;
using SteamProfile.Services;
using SteamProfile.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.System;

namespace SteamProfile.ViewModels
{
    public partial class FriendsViewModel : ObservableObject
    {
        private readonly FriendsService _friendsService;
        private readonly UserService _userService;


        [ObservableProperty]
        private ObservableCollection<Friendship> _friendships = new ObservableCollection<Friendship>();

        [ObservableProperty]
        private Friendship _selectedFriendship;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        public FriendsViewModel(FriendsService friendsService, UserService userService)
        {
            _friendsService = friendsService ?? throw new ArgumentNullException(nameof(friendsService));
            _userService = userService ?? throw new ArgumentNullException(nameof(friendsService));
        }

        [RelayCommand]
        public void LoadFriends()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                Debug.WriteLine("Loading friends for user ID ...");
                var friendships = _friendsService.GetAllFriendships();
                Debug.WriteLine($"Retrieved {friendships.Count} friendships");

                Friendships.Clear();
                foreach (var friendship in friendships)
                {
                    Debug.WriteLine($"Adding friendship: ID={friendship.FriendshipId}, Username={friendship.FriendUsername}");
                    Friendships.Add(friendship);
                }
            }
            catch (ServiceException ex)
            {
                Debug.WriteLine($"Service error: {ex.Message}");
                Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                ErrorMessage = $"Error loading friends: {ex.Message}";
                if (ex.InnerException != null)
                {
                    ErrorMessage += $"\nDetails: {ex.InnerException.Message}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ErrorMessage = $"Unexpected error loading friends: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void RemoveFriend(int friendshipId)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                Debug.WriteLine($"Starting to remove friendship with ID {friendshipId}");
                Debug.WriteLine($"Current number of friends before removal: {Friendships.Count}");
                
                _friendsService.RemoveFriend(friendshipId);
                Debug.WriteLine("Friend removed successfully from database");
                
                // Refresh the friends list
                var updatedFriendships = _friendsService.GetAllFriendships();
                Friendships.Clear();
                foreach (var friendship in updatedFriendships)
                {
                    Friendships.Add(friendship);
                }
                Debug.WriteLine($"Friends list reloaded. New count: {Friendships.Count}");
            }
            catch (ServiceException ex)
            {
                Debug.WriteLine($"Service error: {ex.Message}");
                Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ErrorMessage = $"Error removing friend: {ex.Message}";
                if (ex.InnerException != null)
                {
                    ErrorMessage += $"\nDetails: {ex.InnerException.Message}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                ErrorMessage = $"Unexpected error removing friend: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void SelectFriendship(Friendship friendship)
        {
            SelectedFriendship = friendship;
        }
        public class ServiceException : Exception
        {
            public ServiceException(string message) : base(message) { }
            public ServiceException(string message, Exception innerException)
                : base(message, innerException) { }
        }
    }
}
