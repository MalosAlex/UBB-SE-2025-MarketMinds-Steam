using System;
using NUnit.Framework;
using Moq;
using BusinessLayer.Services;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;

namespace Tests
{
    [TestFixture]
    public class WalletServiceTests
    {
        private Mock<IWalletRepository> _mockWalletRepository;
        private Mock<IUserService> _mockUserService;
        private WalletService _walletService;
        private User _currentUser;
        private const int USER_ID = 1;
        private const int WALLET_ID = 5;

        [SetUp]
        public void SetUp()
        {
            // Set up mocks
            _mockWalletRepository = new Mock<IWalletRepository>();
            _mockUserService = new Mock<IUserService>();

            // Create a test user
            _currentUser = new User { UserId = USER_ID, Username = "testuser" };

            // Set up UserService to return the test user
            _mockUserService.Setup(u => u.GetCurrentUser()).Returns(_currentUser);

            // Set up WalletRepository to return a wallet ID for the test user
            _mockWalletRepository.Setup(w => w.GetWalletIdByUserId(USER_ID)).Returns(WALLET_ID);

            // Create service with mocked dependencies
            _walletService = new WalletService(_mockWalletRepository.Object, _mockUserService.Object);
        }

        [Test]
        public void Constructor_NullWalletRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new WalletService(null, _mockUserService.Object));
        }

        [Test]
        public void Constructor_NullUserService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new WalletService(_mockWalletRepository.Object, null));
        }

        [Test]
        public void AddMoney_ValidAmount_CallsRepositoryMethod()
        {
            // Arrange
            decimal amount = 50m;

            // Act
            _walletService.AddMoney(amount);

            // Assert
            _mockWalletRepository.Verify(r => r.AddMoneyToWallet(amount, USER_ID), Times.Once);
        }

        [Test]
        public void AddPoints_ValidPoints_CallsRepositoryMethod()
        {
            // Arrange
            int points = 100;

            // Act
            _walletService.AddPoints(points);

            // Assert
            _mockWalletRepository.Verify(r => r.AddPointsToWallet(points, USER_ID), Times.Once);
        }

        [Test]
        public void GetBalance_ReturnsMoney()
        {
            // Arrange
            decimal expectedBalance = 150.50m;
            _mockWalletRepository.Setup(r => r.GetMoneyFromWallet(WALLET_ID)).Returns(expectedBalance);

            // Act
            decimal result = _walletService.GetBalance();

            // Assert
            Assert.That(result, Is.EqualTo(expectedBalance));
        }

        [Test]
        public void GetPoints_ReturnsPoints()
        {
            // Arrange
            int expectedPoints = 250;
            _mockWalletRepository.Setup(r => r.GetPointsFromWallet(WALLET_ID)).Returns(expectedPoints);

            // Act
            int result = _walletService.GetPoints();

            // Assert
            Assert.That(result, Is.EqualTo(expectedPoints));
        }

        [Test]
        public void CreateWallet_ValidUserId_CallsRepositoryMethod()
        {
            // Arrange
            int userId = 10;

            // Act
            _walletService.CreateWallet(userId);

            // Assert
            _mockWalletRepository.Verify(r => r.AddNewWallet(userId), Times.Once);
        }

        [Test]
        public void PurchasePoints_NullOffer_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _walletService.PurchasePoints(null));
        }

        [Test]
        public void PurchasePoints_InsufficientBalance_ThrowsInvalidOperationException()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            _mockWalletRepository.Setup(r => r.GetMoneyFromWallet(WALLET_ID)).Returns(50m); // Only 50 in wallet

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _walletService.PurchasePoints(offer));
        }

        [Test]
        public void PurchasePoints_InsufficientBalance_ExceptionMessageContainsInsufficientFunds()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            _mockWalletRepository.Setup(r => r.GetMoneyFromWallet(WALLET_ID)).Returns(50m); // Only 50 in wallet

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _walletService.PurchasePoints(offer));
            Assert.That(ex.Message, Is.EqualTo("Insufficient funds"));
        }

        [Test]
        public void PurchasePoints_SufficientBalance_CallsRepositoryMethod()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            _mockWalletRepository.Setup(r => r.GetMoneyFromWallet(WALLET_ID)).Returns(150m); // 150 in wallet

            // Act
            _walletService.PurchasePoints(offer);

            // Assert
            _mockWalletRepository.Verify(r => r.PurchasePoints(offer, USER_ID), Times.Once);
        }

        [Test]
        public void TryPurchasePoints_NullOffer_ReturnsFalse()
        {
            // Act
            bool result = _walletService.TryPurchasePoints(null);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryPurchasePoints_InsufficientBalance_ReturnsFalse()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            _mockWalletRepository.Setup(r => r.GetMoneyFromWallet(WALLET_ID)).Returns(50m); // Only 50 in wallet

            // Act
            bool result = _walletService.TryPurchasePoints(offer);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryPurchasePoints_ExceptionThrown_ReturnsFalse()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            _mockWalletRepository.Setup(r => r.GetMoneyFromWallet(WALLET_ID)).Returns(150m); // 150 in wallet
            _mockWalletRepository.Setup(r => r.PurchasePoints(offer, USER_ID)).Throws(new Exception("Test exception"));

            // Act
            bool result = _walletService.TryPurchasePoints(offer);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryPurchasePoints_SufficientBalance_ReturnsTrue()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            _mockWalletRepository.Setup(r => r.GetMoneyFromWallet(WALLET_ID)).Returns(150m); // 150 in wallet

            // Act
            bool result = _walletService.TryPurchasePoints(offer);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}