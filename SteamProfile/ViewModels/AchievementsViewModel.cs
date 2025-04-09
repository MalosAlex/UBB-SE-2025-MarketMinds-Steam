using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services;
using SteamProfile.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public partial class AchievementsViewModel : ObservableObject
    {
        private static AchievementsViewModel _instance;
        private readonly IAchievementsService _achievementsService;
        private readonly IUserService _userService;

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> _allAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> _friendshipsAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> _ownedGamesAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> _soldGamesAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> _yearsOfActivityAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> _numberOfPostsAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> _numberOfReviewsGivenAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> _numberOfReviewsReceivedAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> _developerAchievements = new ObservableCollection<AchievementWithStatus>();

        public static AchievementsViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AchievementsViewModel(App.AchievementsService, App.UserService);
                }
                return _instance;
            }
        }

        private AchievementsViewModel(IAchievementsService achievementsService, IUserService userService)
        {
            _achievementsService = achievementsService ?? throw new ArgumentNullException(nameof(achievementsService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            BackToProfileCommand = new RelayCommand(BackToProfile);
        }
        [RelayCommand]
        public async Task LoadAchievementsAsync()
        {
            var userId = _userService.GetCurrentUser().UserId;

            // Get grouped achievements (no logic in ViewModel)
            var grouped = await Task.Run(() => _achievementsService.GetGroupedAchievementsForUser(userId));

            // Assign to ObservableCollections
            AllAchievements = new ObservableCollection<AchievementWithStatus>(grouped.AllAchievements);
            FriendshipsAchievements = new ObservableCollection<AchievementWithStatus>(grouped.Friendships);
            OwnedGamesAchievements = new ObservableCollection<AchievementWithStatus>(grouped.OwnedGames);
            SoldGamesAchievements = new ObservableCollection<AchievementWithStatus>(grouped.SoldGames);
            YearsOfActivityAchievements = new ObservableCollection<AchievementWithStatus>(grouped.YearsOfActivity);
            NumberOfPostsAchievements = new ObservableCollection<AchievementWithStatus>(grouped.NumberOfPosts);
            NumberOfReviewsGivenAchievements = new ObservableCollection<AchievementWithStatus>(grouped.NumberOfReviewsGiven);
            NumberOfReviewsReceivedAchievements = new ObservableCollection<AchievementWithStatus>(grouped.NumberOfReviewsReceived);
            DeveloperAchievements = new ObservableCollection<AchievementWithStatus>(grouped.Developer);
        }
        public IRelayCommand BackToProfileCommand { get; }

        private void BackToProfile()
        {
            int currentUserId = _userService.GetCurrentUser().UserId;
            NavigationService.Instance.Navigate(typeof(ProfilePage), currentUserId);
        }


    }
}
