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

            // Setup fake user service to return the test user

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
        public void PurchasePoints_DecreasesBalance_IncreasesPoints()
        {
            // Arrange
            decimal initialBalance = _walletService.GetBalance();
            int initialPoints = _walletService.GetPoints();
            var offer = new PointsOffer(50, 200); // $50 for 200 points

            // Act
            _walletService.PurchasePoints(offer);
            decimal newBalance = _walletService.GetBalance();
            int newPoints = _walletService.GetPoints();

            // Assert
            Assert.That(newBalance, Is.EqualTo(initialBalance - offer.Price));
            Assert.That(newPoints, Is.EqualTo(initialPoints + offer.Points));
        }

        [Test]
        public void PurchasePoints_InsufficientFunds_ThrowsException()
        {
            // Arrange
            var offer = new PointsOffer(200, 500); // $200 for 500 points (more than our test user has)

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _walletService.PurchasePoints(offer));
            Assert.That(ex.Message, Is.EqualTo("Insufficient funds"));
        }

        [Test]
        public void TryPurchasePoints_SufficientFunds_ReturnsTrue_UpdatesValues()
        {
            // Arrange
            decimal initialBalance = _walletService.GetBalance();
            int initialPoints = _walletService.GetPoints();
            var offer = new PointsOffer(50, 200); // $50 for 200 points

            // Act
            bool result = _walletService.TryPurchasePoints(offer);
            decimal newBalance = _walletService.GetBalance();
            int newPoints = _walletService.GetPoints();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(newBalance, Is.EqualTo(initialBalance - offer.Price));
            Assert.That(newPoints, Is.EqualTo(initialPoints + offer.Points));
        }

        [Test]
        public void TryPurchasePoints_InsufficientFunds_ReturnsFalse_DoesNotUpdateValues()
        {
            // Arrange
            decimal initialBalance = _walletService.GetBalance();
            int initialPoints = _walletService.GetPoints();
            var offer = new PointsOffer(200, 500); // $200 for 500 points (more than our test user has)

            // Act
            bool result = _walletService.TryPurchasePoints(offer);
            decimal newBalance = _walletService.GetBalance();
            int newPoints = _walletService.GetPoints();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(newBalance, Is.EqualTo(initialBalance));
            Assert.That(newPoints, Is.EqualTo(initialPoints));
        }

        [Test]
        public void CreateWallet_ForNewUser_CreatesNewWallet()
        {
            // Arrange
            int newUserId = 999; // A user ID that doesn't exist yet

            // Act & Assert - First verify the wallet doesn't exist
            Assert.Throws<RepositoryException>(() => _walletRepository.GetWalletIdByUserId(newUserId));

            // Act - Create the wallet
            _walletService.CreateWallet(newUserId);

            // Assert - Now the wallet should exist
            int walletId = _walletRepository.GetWalletIdByUserId(newUserId);
            var wallet = _walletRepository.GetWallet(walletId);

            Assert.That(wallet, Is.Not.Null);
            Assert.That(wallet.UserId, Is.EqualTo(newUserId));
            Assert.That(wallet.Balance, Is.EqualTo(0));
            Assert.That(wallet.Points, Is.EqualTo(0));
        }

        [Test]
        public void CompleteUserFlow_AddMoneyThenPurchasePoints()
        {
            // Arrange
            decimal initialBalance = _walletService.GetBalance();
            int initialPoints = _walletService.GetPoints();

            decimal moneyToAdd = 150m;
            var offer = new PointsOffer(100, 300); // $100 for 300 points

            // Act - First add money to the wallet
            _walletService.AddMoney(moneyToAdd);
            decimal balanceAfterAddingMoney = _walletService.GetBalance();

            // Then purchase points
            _walletService.PurchasePoints(offer);
            decimal finalBalance = _walletService.GetBalance();
            int finalPoints = _walletService.GetPoints();

            // Assert
            Assert.That(balanceAfterAddingMoney, Is.EqualTo(initialBalance + moneyToAdd));
            Assert.That(finalBalance, Is.EqualTo(initialBalance + moneyToAdd - offer.Price));
            Assert.That(finalPoints, Is.EqualTo(initialPoints + offer.Points));
        }
    }
}