using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamProfile.Models;
using SteamProfile.Services;
using System;
using System.Collections.ObjectModel;

namespace SteamProfile.ViewModels { 

    public partial class UsersViewModel : ObservableObject
    {
        // Singleton instance to ensure only one ViewModel exists for the Users page
        private static UsersViewModel _instance;
        private readonly UserService _userService;
        private static Random _random = new Random();

        // ObservableCollection automatically notifies the UI when items are added/removed
        // The [ObservableProperty] attribute generates the property with INotifyPropertyChanged implementation
        [ObservableProperty]
        private ObservableCollection<User> _users;

        // SelectedUser property for tracking the currently selected user in the DataGrid
        // The [ObservableProperty] attribute ensures UI updates when selection changes
        [ObservableProperty]
        private User _selectedUser;

        public static UsersViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UsersViewModel(App.UserService);
                }
                return _instance;
            }
        }

        private UsersViewModel(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _users = new ObservableCollection<User>();
            LoadUsers();
        }

        public void LoadUsers()
        {
            var users = _userService.GetAllUsers();
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        [RelayCommand]
        private void AddRandomUser()
        {
            // Create a new random user
            var randomUser = new User
            {
                Username = $"RandomUser_{_random.Next(1000)}",
                Email = $"random{_random.Next(1000)}@example.com",
                Password = "RandomPassword123",
               // Description = "This is a random test user",
                IsDeveloper = _random.Next(2) == 1,
                CreatedAt = DateTime.Now
            };

            // First save to database through the service
            var createdUser = _userService.CreateUser(randomUser);
            // Only update UI if database operation was successful

            if (createdUser != null)
            {
                Users.Add(createdUser);
            }
        }
    }
} 