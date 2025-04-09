﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services;
using SteamProfile.Views;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public partial class AchievementsViewModel : ObservableObject
    {
        private static AchievementsViewModel achievementsViewModelInstance;
        private readonly IAchievementsService achievementsService;
        private readonly IUserService userService;

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> allAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> friendshipsAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> ownedGamesAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> soldGamesAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> yearsOfActivityAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> numberOfPostsAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> numberOfReviewsGivenAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> numberOfReviewsReceivedAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> developerAchievements = new ObservableCollection<AchievementWithStatus>();

        public static AchievementsViewModel Instance
        {
            get
            {
                if (achievementsViewModelInstance == null)
                {
                    achievementsViewModelInstance = new AchievementsViewModel(App.AchievementsService, App.UserService);
                }
                return achievementsViewModelInstance;
            }
        }

        private AchievementsViewModel(IAchievementsService achievementsService, IUserService userService)
        {
            this.achievementsService = achievementsService ?? throw new ArgumentNullException(nameof(achievementsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            BackToProfileCommand = new RelayCommand(BackToProfile);
        }
        [RelayCommand]
        public async Task LoadAchievementsAsync()
        {
            var userId = userService.GetCurrentUser().UserId;

            // Get grouped achievements (no logic in ViewModel)
            var grouped = await Task.Run(() => achievementsService.GetGroupedAchievementsForUser(userId));

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
            int currentUserId = userService.GetCurrentUser().UserId;
            NavigationService.Instance.Navigate(typeof(ProfilePage), currentUserId);
        }
    }
}
