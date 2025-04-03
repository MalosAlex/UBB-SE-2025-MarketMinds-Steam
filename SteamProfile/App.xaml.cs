using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using SteamProfile.Data;
using SteamProfile.Repositories;
using SteamProfile.Services;
using SteamProfile.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using SteamProfile.Views;

namespace SteamProfile
{
    public partial class App : Application
    {
        public static readonly AchievementsService AchievementsService;
        public static readonly FeaturesService FeaturesService;
        public static readonly CollectionsService CollectionsService;
        public static readonly WalletService WalletService;
        public static readonly UserService UserService;
        public static readonly FriendsService FriendsService;
        public static readonly OwnedGamesService OwnedGamesService;
        public static readonly AuthenticationService AuthenticationService;
        public static IPasswordResetService PasswordResetService { get; private set; }
        public static readonly SessionService SessionService;
        public static UserProfilesRepository UserProfileRepository { get; private set; }
        public static CollectionsRepository CollectionsRepository { get;  }

        static App()
        {
            var dataLink = DataLink.Instance;
            var navigationService = NavigationService.Instance;

            var achievementsRepository = new AchievementsRepository(dataLink);
            var featuresRepository = new FeaturesRepository(dataLink);
            var usersRepository = new UsersRepository(dataLink);
            var userProfilesRepository = new UserProfilesRepository(dataLink);
            var collectionsRepository = new CollectionsRepository(dataLink);
            var walletRepository = new WalletRepository(dataLink);
            var friendshipsRepository = new FriendshipsRepository(dataLink);
            var ownedGamesRepossitory = new OwnedGamesRepository(dataLink);
            var sessionRepository = new SessionRepository(dataLink);
            var passwordResetRepo = new PasswordResetRepository(dataLink);
            UserProfileRepository = new UserProfilesRepository(dataLink);
            CollectionsRepository = new CollectionsRepository(dataLink);


            // Initialize all services
            AchievementsService = new AchievementsService(achievementsRepository);
            CollectionsService = new CollectionsService(collectionsRepository);
            AuthenticationService = new AuthenticationService(usersRepository);
            SessionService = new SessionService(sessionRepository, usersRepository);
            UserService = new UserService(usersRepository, SessionService);
            WalletService = new WalletService(walletRepository, UserService);
            FriendsService = new FriendsService(friendshipsRepository, UserService);
            OwnedGamesService = new OwnedGamesService(ownedGamesRepossitory);
            PasswordResetService = new PasswordResetService(passwordResetRepo, UserService);
            FeaturesService = new FeaturesService(featuresRepository, UserService);

            InitializeAchievements();
        }

        private static void InitializeAchievements()
        {
            AchievementsService.InitializeAchievements();
        }

        private Window m_window;

        public App()
        {
            this.InitializeComponent();

        }

        public Window MainWindow { get; set; }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            //NavigationService.Instance.Initialize(m_window.Content as Frame); // Ensure the frame is passed
            m_window.Activate();
        }

    }
}
