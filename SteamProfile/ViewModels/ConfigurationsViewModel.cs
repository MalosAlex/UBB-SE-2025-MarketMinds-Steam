﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Services;
using SteamProfile.Views;
using SteamProfile.Views.ConfigurationsView;
using System;

namespace SteamProfile.ViewModels
{
    public partial class ConfigurationsViewModel : ObservableObject
    {
        private readonly Frame _frame;
        private readonly UserService _userService;

        public ConfigurationsViewModel(Frame frame, UserService userService)
        {
            _frame = frame;
            _userService = userService ?? throw new NullReferenceException(nameof(userService));
        }

        [RelayCommand]
        private void NavigateToFeatures()
        {
            _frame.Navigate(typeof(FeaturesPage));
        }

        [RelayCommand]
        private void NavigateToProfile()
        {
            _frame.Navigate(typeof(ProfilePage),_userService.GetCurrentUser().UserId);
        }
        [RelayCommand]
        private void NavigateToProfileSettings()
        {
            _frame.Navigate(typeof(ModifyProfilePage));
        }
        [RelayCommand]

        private void NavigateToAccountSettings()
        {
            _frame.Navigate(typeof(AccountSettingsPage));
        }
        
    }
}
