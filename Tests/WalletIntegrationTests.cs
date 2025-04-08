using System;
using NUnit.Framework;
using BusinessLayer.Services;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Services.Fakes;
using BusinessLayer.Exceptions;

namespace Tests.Integration
{
    [TestFixture]
    public class WalletIntegrationTests
    {
        private FakeWalletRepository _walletRepository;
        private UserService _service;
        private WalletService _walletService;
        private User _testUser;
        private const int USER_ID = 1;

        [SetUp]
        public void SetUp()
        {
            _walletRepository = new FakeWalletRepository();
            _service = new UserService(new FakeUsersRepository(), new FakeSessionService());

            // Configure the fake user service to return the test user
            _testUser = new User
            {
                UserId = USER_ID,
                Username = "testuser",
                Email = "test@mail.com"
            };

            // Create the WalletService with fake dependencies
            _walletService = new WalletService(_walletRepository, _service);
        }

        [Test]
        public void AddMoney_IncreasesBalance()
        {
            // Arrange
            decimal initialBalance = _walletService.GetBalance();
            decimal amountToAdd = 50.25m;

            // Act
            _walletService.AddMoney(amountToAdd);
            decimal newBalance = _walletService.GetBalance();

            // Assert
            Assert.That(newBalance, Is.EqualTo(initialBalance + amountToAdd));
        }

        [Test]
        public void AddPoints_IncreasesPoints()
        {
            // Arrange
            int initialPoints = _walletService.GetPoints();
            int pointsToAdd = 100;

            // Act
            _walletService.AddPoints(pointsToAdd);
            int newPoints = _walletService.GetPoints();

            // Assert
            Assert.That(newPoints, Is.EqualTo(initialPoints + pointsToAdd));
        }

        [Test]
        public void PurchasePoints_DecreasesBalance()
        {
            // Arrange
            decimal initialBalance = _walletService.GetBalance();
            var offer = new PointsOffer(50, 200); // $50 for 200 points

            // Act
            _walletService.PurchasePoints(offer);
            decimal newBalance = _walletService.GetBalance();

            // Assert
            Assert.That(newBalance, Is.EqualTo(initialBalance - offer.Price));
        }

        [Test]
        public void PurchasePoints_IncreasesPoints()
        {
            // Arrange
            int initialPoints = _walletService.GetPoints();
            var offer = new PointsOffer(50, 200); // $50 for 200 points

            // Act
            _walletService.PurchasePoints(offer);
            int newPoints = _walletService.GetPoints();

            // Assert
            Assert.That(newPoints, Is.EqualTo(initialPoints + offer.Points));
        }

        [Test]
        public void PurchasePoints_InsufficientFunds_ThrowsException()
        {
            // Arrange
            var offer = new PointsOffer(200, 500); // $200 for 500 points (more than the test user has)

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _walletService.PurchasePoints(offer));
        }

        [Test]
        public void PurchasePoints_InsufficientFunds_ExceptionHasCorrectMessage()
        {
            // Arrange
            var offer = new PointsOffer(200, 500); // $200 for 500 points (more than the test user has)

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _walletService.PurchasePoints(offer));
            Assert.That(ex.Message, Is.EqualTo("Insufficient funds"));
        }

        [Test]
        public void TryPurchasePoints_SufficientFunds_ReturnsTrue()
        {
            // Arrange
            var offer = new PointsOffer(50, 200); // $50 for 200 points

            // Act
            bool result = _walletService.TryPurchasePoints(offer);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TryPurchasePoints_SufficientFunds_UpdatesBalance()
        {
            // Arrange
            decimal initialBalance = _walletService.GetBalance();
            var offer = new PointsOffer(50, 200); // $50 for 200 points

            // Act
            _walletService.TryPurchasePoints(offer);
            decimal newBalance = _walletService.GetBalance();

            // Assert
            Assert.That(newBalance, Is.EqualTo(initialBalance - offer.Price));
        }

        [Test]
        public void TryPurchasePoints_SufficientFunds_UpdatesPoints()
        {
            // Arrange
            int initialPoints = _walletService.GetPoints();
            var offer = new PointsOffer(50, 200); // $50 for 200 points

            // Act
            _walletService.TryPurchasePoints(offer);
            int newPoints = _walletService.GetPoints();

            // Assert
            Assert.That(newPoints, Is.EqualTo(initialPoints + offer.Points));
        }

        [Test]
        public void TryPurchasePoints_InsufficientFunds_ReturnsFalse()
        {
            // Arrange
            var offer = new PointsOffer(200, 500); // $200 for 500 points (more than the test user has)

            // Act
            bool result = _walletService.TryPurchasePoints(offer);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryPurchasePoints_InsufficientFunds_DoesNotUpdateBalance()
        {
            // Arrange
            decimal initialBalance = _walletService.GetBalance();
            var offer = new PointsOffer(200, 500); // $200 for 500 points (more than the test user has)

            // Act
            _walletService.TryPurchasePoints(offer);
            decimal newBalance = _walletService.GetBalance();

            // Assert
            Assert.That(newBalance, Is.EqualTo(initialBalance));
        }

        [Test]
        public void TryPurchasePoints_InsufficientFunds_DoesNotUpdatePoints()
        {
            // Arrange
            int initialPoints = _walletService.GetPoints();
            var offer = new PointsOffer(200, 500); // $200 for 500 points (more than the test user has)

            // Act
            _walletService.TryPurchasePoints(offer);
            int newPoints = _walletService.GetPoints();

            // Assert
            Assert.That(newPoints, Is.EqualTo(initialPoints));
        }

        [Test]
        public void CreateWallet_NewUserId_CreatesWalletSuccessfully()
        {
            // Arrange
            int newUserId = 999; // A user ID that doesn't exist yet

            // Act - Create the wallet
            _walletService.CreateWallet(newUserId);

            // Assert - Now the wallet should exist
            int walletId = _walletRepository.GetWalletIdByUserId(newUserId);
            Assert.That(walletId, Is.GreaterThan(0));
        }

        [Test]
        public void CreateWallet_ForNewUser_WalletHasCorrectUserId()
        {
            // Arrange
            int newUserId = 999; // A user ID that doesn't exist yet

            // Act - Create the wallet
            _walletService.CreateWallet(newUserId);
            int walletId = _walletRepository.GetWalletIdByUserId(newUserId);
            var wallet = _walletRepository.GetWallet(walletId);

            // Assert
            Assert.That(wallet.UserId, Is.EqualTo(newUserId));
        }

        [Test]
        public void CreateWallet_ForNewUser_WalletHasZeroBalance()
        {
            // Arrange
            int newUserId = 999; // A user ID that doesn't exist yet

            // Act - Create the wallet
            _walletService.CreateWallet(newUserId);
            int walletId = _walletRepository.GetWalletIdByUserId(newUserId);
            var wallet = _walletRepository.GetWallet(walletId);

            // Assert
            Assert.That(wallet.Balance, Is.EqualTo(0));
        }

        [Test]
        public void CreateWallet_ForNewUser_WalletHasZeroPoints()
        {
            // Arrange
            int newUserId = 999; // A user ID that doesn't exist yet

            // Act - Create the wallet
            _walletService.CreateWallet(newUserId);
            int walletId = _walletRepository.GetWalletIdByUserId(newUserId);
            var wallet = _walletRepository.GetWallet(walletId);

            // Assert
            Assert.That(wallet.Points, Is.EqualTo(0));
        }

        [Test]
        public void CompleteUserFlow_AddMoney_IncreasesBalance()
        {
            // Arrange
            decimal initialBalance = _walletService.GetBalance();
            decimal moneyToAdd = 150m;

            // Act - First add money to the wallet
            _walletService.AddMoney(moneyToAdd);
            decimal balanceAfterAddingMoney = _walletService.GetBalance();

            // Assert
            Assert.That(balanceAfterAddingMoney, Is.EqualTo(initialBalance + moneyToAdd));
        }

        [Test]
        public void CompleteUserFlow_PurchasePointsAfterAddingMoney_DecreasesBalance()
        {
            // Arrange
            decimal initialBalance = _walletService.GetBalance();
            decimal moneyToAdd = 150m;
            var offer = new PointsOffer(100, 300); // $100 for 300 points

            // Act - First add money to the wallet
            _walletService.AddMoney(moneyToAdd);
            decimal balanceAfterAddingMoney = _walletService.GetBalance();

            // Then purchase points
            _walletService.PurchasePoints(offer);
            decimal finalBalance = _walletService.GetBalance();

            // Assert
            Assert.That(finalBalance, Is.EqualTo(initialBalance + moneyToAdd - offer.Price));
        }

        [Test]
        public void CompleteUserFlow_PurchasePointsAfterAddingMoney_IncreasesPoints()
        {
            // Arrange
            int initialPoints = _walletService.GetPoints();
            decimal moneyToAdd = 150m;
            var offer = new PointsOffer(100, 300); // $100 for 300 points

            // Act - First add money to the wallet
            _walletService.AddMoney(moneyToAdd);

            // Then purchase points
            _walletService.PurchasePoints(offer);
            int finalPoints = _walletService.GetPoints();

            // Assert
            Assert.That(finalPoints, Is.EqualTo(initialPoints + offer.Points));
        }
    }
}