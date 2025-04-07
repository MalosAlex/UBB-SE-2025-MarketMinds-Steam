using Microsoft.UI.Xaml;
using BusinessLayer.Data;
using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Services;
using SteamProfile.ViewModels;

namespace SteamProfile
{
    public partial class App : Application
    {

        // Services
        public static readonly IAchievementsService AchievementsService;
        public static readonly FeaturesService FeaturesService;
        public static readonly ICollectionsService CollectionsService;
        public static readonly WalletService WalletService;
        public static readonly IUserService UserService;
        public static readonly IFriendsService FriendsService;
        public static readonly IOwnedGamesService OwnedGamesService;
        public static readonly AuthenticationService AuthenticationService;

        // View Models
        public static readonly AddGameToCollectionViewModel AddGameToCollectionViewModel;
        public static readonly CollectionGamesViewModel CollectionGamesViewModel;
        public static readonly CollectionsViewModel CollectionsViewModel;
        public static readonly UsersViewModel UsersViewModel;
        public static readonly FriendsViewModel FriendsViewModel;

        // MUST BE ELIMINATED WHAT EVEN IS THIS !!!!!!!!!!!!!!!!!
        public static PasswordResetService PasswordResetService { get; private set; }
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
            UserProfileRepository = new UserProfilesRepository(dataLink);
            CollectionsRepository = new CollectionsRepository(dataLink);
            var walletRepository = new WalletRepository(dataLink);
            var friendshipsRepository = new FriendshipsRepository(dataLink);
            var ownedGamesRepossitory = new OwnedGamesRepository(dataLink);
            var sessionRepository = new SessionRepository(dataLink);
            var passwordResetRepo = new PasswordResetRepository(dataLink);


            // Initialize all services
            SessionService = new SessionService(sessionRepository, usersRepository);
            UserService = new UserService(usersRepository, SessionService);
            AchievementsService = new AchievementsService(achievementsRepository);
            CollectionsService = new CollectionsService(CollectionsRepository);
            AuthenticationService = new AuthenticationService(usersRepository);
            WalletService = new WalletService(walletRepository, UserService);
            FriendsService = new FriendsService(friendshipsRepository, UserService);
            OwnedGamesService = new OwnedGamesService(ownedGamesRepossitory);
            PasswordResetService = new PasswordResetService(passwordResetRepo, UserService);
            FeaturesService = new FeaturesService(featuresRepository, UserService);

            // Initialize all view models
            UsersViewModel = UsersViewModel.Instance;
            AddGameToCollectionViewModel = new AddGameToCollectionViewModel(CollectionsService);
            FriendsViewModel = new FriendsViewModel(FriendsService, UserService);
            CollectionGamesViewModel = new CollectionGamesViewModel(CollectionsService);
            CollectionsViewModel = new CollectionsViewModel(CollectionsService, UserService);

            // Others
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
