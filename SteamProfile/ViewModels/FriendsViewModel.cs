using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;
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
        private readonly IFriendsService _friendsService;
        private readonly IUserService _userService;


        [ObservableProperty]
        private ObservableCollection<Friendship> _friendships = new ObservableCollection<Friendship>();

        [ObservableProperty]
        private Friendship _selectedFriendship;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        public FriendsViewModel(IFriendsService friendsService, IUserService userService)
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
            catch (ServiceException serviceException)
            {
                Debug.WriteLine($"Service error: {serviceException.Message}");
                Debug.WriteLine($"Inner exception: {serviceException.InnerException?.Message}");
                ErrorMessage = $"Error loading friends: {serviceException.Message}";
                if (serviceException.InnerException != null)
                {
                    ErrorMessage += $"\nDetails: {serviceException.InnerException.Message}";
                }
            }
            catch (Exception generalException)
            {
                Debug.WriteLine($"Unexpected error: {generalException.Message}");
                Debug.WriteLine($"Stack trace: {generalException.StackTrace}");
                ErrorMessage = $"Unexpected error loading friends: {generalException.Message}";
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
            catch (ServiceException serviceException)
            {
                Debug.WriteLine($"Service error: {serviceException.Message}");
                Debug.WriteLine($"Inner exception: {serviceException.InnerException?.Message}");
                Debug.WriteLine($"Stack trace: {serviceException.StackTrace}");
                ErrorMessage = $"Error removing friend: {serviceException.Message}";
                if (serviceException.InnerException != null)
                {
                    ErrorMessage += $"\nDetails: {serviceException.InnerException.Message}";
                }
            }
            catch (Exception generalException)
            {
                Debug.WriteLine($"Unexpected error: {generalException.Message}");
                Debug.WriteLine($"Stack trace: {generalException.StackTrace}");
                Debug.WriteLine($"Inner exception: {generalException.InnerException?.Message}");
                ErrorMessage = $"Unexpected error removing friend: {generalException.Message}";
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
    }
}
