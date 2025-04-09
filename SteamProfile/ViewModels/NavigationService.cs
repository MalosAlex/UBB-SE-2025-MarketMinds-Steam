using Microsoft.UI.Xaml.Controls;
using System;

namespace SteamProfile.ViewModels
{
    public class NavigationService
    {
        private Frame _frame;
        private static NavigationService _instance;

        public static NavigationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NavigationService();
                }
                return _instance;
            }
        }

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public bool Navigate(Type pageType)
        {
            if (_frame != null)
            {
                return _frame.Navigate(pageType);
            }
                return false;

            return _frame.Navigate(pageType);
        }

        public bool Navigate(Type pageType, object parameter)
        {
            if (_frame != null)
        {
                return _frame.Navigate(pageType, parameter);
            }
                return false;

            return _frame.Navigate(pageType, parameter);
        }

        public bool GoBack()
        {
            if (_frame != null && _frame.CanGoBack)
        {
            if (_frame == null || !_frame.CanGoBack)
                return false;

            _frame.GoBack();
            return true;
        }
            return false;
        }
    }
} 