using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using BusinessLayer.Data;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Exceptions;
using Microsoft.Data.SqlClient;
using System.Runtime.Serialization;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class WalletRepositoryTests
    {
        private Mock<IDataLink> _mockDataLink;
        private WalletRepository _repository;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _mockDataLink = new Mock<IDataLink>();
            _repository = new WalletRepository(_mockDataLink.Object);
        }

        [Test]
        public void Constructor_NullDataLink_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new WalletRepository(null));
        }

        #region GetWallet Tests

        [Test]
        public void GetWallet_ValidWalletId_ReturnsWalletWithCorrectId()
        {
            // Arrange
            int walletId = 1;
            var dataTable = CreateWalletDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletById", It.IsAny<SqlParameter[]>()))
                         .Returns(dataTable);

            // Act
            Wallet result = _repository.GetWallet(walletId);

            // Assert
            Assert.That(result.WalletId, Is.EqualTo(1));
        }

        [Test]
        public void GetWallet_ValidWalletId_ReturnsWalletWithCorrectUserId()
        {
            // Arrange
            int walletId = 1;
            var dataTable = CreateWalletDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletById", It.IsAny<SqlParameter[]>()))
                         .Returns(dataTable);

            // Act
            Wallet result = _repository.GetWallet(walletId);

            // Assert
            Assert.That(result.UserId, Is.EqualTo(1));
        }

        [Test]
        public void GetWallet_ValidWalletId_ReturnsWalletWithCorrectBalance()
        {
            // Arrange
            int walletId = 1;
            var dataTable = CreateWalletDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletById", It.IsAny<SqlParameter[]>()))
                         .Returns(dataTable);

            // Act
            Wallet result = _repository.GetWallet(walletId);

            // Assert
            Assert.That(result.Balance, Is.EqualTo(100.50m));
        }

        [Test]
        public void GetWallet_ValidWalletId_ReturnsWalletWithCorrectPoints()
        {
            // Arrange
            int walletId = 1;
            var dataTable = CreateWalletDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletById", It.IsAny<SqlParameter[]>()))
                         .Returns(dataTable);

            // Act
            Wallet result = _repository.GetWallet(walletId);

            // Assert
            Assert.That(result.Points, Is.EqualTo(200));
        }

        [Test]
        public void GetWallet_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletById", It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => _repository.GetWallet(walletId));
        }

        [Test]
        public void GetWallet_DatabaseException_ExceptionMessageContainsFailedToRetrieve()
        {
            // Arrange
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletById", It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetWallet(walletId));
            Assert.That(ex.Message, Does.Contain("Failed to retrieve wallet"));
        }

        #endregion

        #region GetWalletIdByUserId Tests

        [Test]
        public void GetWalletIdByUserId_ValidUserId_ReturnsWalletId()
        {
            // Arrange
            int userId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("wallet_id", typeof(int));
            dataTable.Rows.Add(5); // Wallet ID 5 for user 1

            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletIdByUserId", It.IsAny<SqlParameter[]>()))
                         .Returns(dataTable);

            // Act
            int result = _repository.GetWalletIdByUserId(userId);

            // Assert
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void GetWalletIdByUserId_NoWalletFound_ThrowsRepositoryException()
        {
            // Arrange
            int userId = 999;
            var emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("wallet_id", typeof(int));

            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletIdByUserId", It.IsAny<SqlParameter[]>()))
                         .Returns(emptyDataTable);

            // Act & Assert
            Assert.Throws<RepositoryException>(() => _repository.GetWalletIdByUserId(userId));
        }

        [Test]
        public void GetWalletIdByUserId_NoWalletFound_ExceptionMessageContainsNoWalletFound()
        {
            // Arrange
            int userId = 999;
            var emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("wallet_id", typeof(int));

            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletIdByUserId", It.IsAny<SqlParameter[]>()))
                         .Returns(emptyDataTable);

            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetWalletIdByUserId(userId));
            Assert.That(ex.Message, Does.Contain("No wallet found for user ID"));
        }

        [Test]
        public void GetWalletIdByUserId_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            int userId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletIdByUserId", It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => _repository.GetWalletIdByUserId(userId));
        }

        [Test]
        public void GetWalletIdByUserId_DatabaseException_ExceptionMessageContainsFailedToRetrieve()
        {
            // Arrange
            int userId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletIdByUserId", It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetWalletIdByUserId(userId));
            Assert.That(ex.Message, Does.Contain("Failed to retrieve wallet ID"));
        }

        #endregion

        #region AddMoneyToWallet Tests

        [Test]
        public void AddMoneyToWallet_ValidParameters_CallsExecuteReader()
        {
            // Arrange
            decimal amount = 50.25m;
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("AddMoney", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.AddMoneyToWallet(amount, walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("AddMoney", It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void AddMoneyToWallet_ValidParameters_CallsExecuteReaderWithCorrectAmountParameter()
        {
            // Arrange
            decimal amount = 50.25m;
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("AddMoney", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.AddMoneyToWallet(amount, walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("AddMoney",
                It.Is<SqlParameter[]>(p => decimal.Parse(p[0].Value.ToString()) == amount)),
                Times.Once);
        }

        [Test]
        public void AddMoneyToWallet_ValidParameters_CallsExecuteReaderWithCorrectWalletIdParameter()
        {
            // Arrange
            decimal amount = 50.25m;
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("AddMoney", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.AddMoneyToWallet(amount, walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("AddMoney",
                It.Is<SqlParameter[]>(p => int.Parse(p[1].Value.ToString()) == walletId)),
                Times.Once);
        }

        #endregion

        #region AddPointsToWallet Tests

        [Test]
        public void AddPointsToWallet_ValidParameters_CallsExecuteReader()
        {
            // Arrange
            int amount = 100;
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("AddPoints", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.AddPointsToWallet(amount, walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("AddPoints", It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void AddPointsToWallet_ValidParameters_CallsExecuteReaderWithCorrectAmountParameter()
        {
            // Arrange
            int amount = 100;
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("AddPoints", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.AddPointsToWallet(amount, walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("AddPoints",
                It.Is<SqlParameter[]>(p => int.Parse(p[0].Value.ToString()) == amount)),
                Times.Once);
        }

        [Test]
        public void AddPointsToWallet_ValidParameters_CallsExecuteReaderWithCorrectWalletIdParameter()
        {
            // Arrange
            int amount = 100;
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("AddPoints", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.AddPointsToWallet(amount, walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("AddPoints",
                It.Is<SqlParameter[]>(p => int.Parse(p[1].Value.ToString()) == walletId)),
                Times.Once);
        }

        #endregion

        #region GetMoneyFromWallet Tests

        [Test]
        public void GetMoneyFromWallet_ValidWalletId_ReturnsBalance()
        {
            // Arrange
            int walletId = 1;
            var dataTable = CreateWalletDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletById", It.IsAny<SqlParameter[]>()))
                         .Returns(dataTable);

            // Act
            decimal result = _repository.GetMoneyFromWallet(walletId);

            // Assert
            Assert.That(result, Is.EqualTo(100.50m));
        }

        #endregion

        #region GetPointsFromWallet Tests

        [Test]
        public void GetPointsFromWallet_ValidWalletId_ReturnsPoints()
        {
            // Arrange
            int walletId = 1;
            var dataTable = CreateWalletDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetWalletById", It.IsAny<SqlParameter[]>()))
                         .Returns(dataTable);

            // Act
            int result = _repository.GetPointsFromWallet(walletId);

            // Assert
            Assert.That(result, Is.EqualTo(200));
        }

        #endregion

        #region PurchasePoints Tests

        [Test]
        public void PurchasePoints_ValidOffer_CallsExecuteReader()
        {
            // Arrange
            var offer = new PointsOffer(10, 100); // price 10, points 100
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("BuyPoints", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.PurchasePoints(offer, walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("BuyPoints", It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void PurchasePoints_ValidParameters_CallsExecuteReaderWithCorrectPriceParameter()
        {
            // Arrange
            var offer = new PointsOffer(10, 100); // price 10, points 100
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("BuyPoints", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.PurchasePoints(offer, walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("BuyPoints",
                It.Is<SqlParameter[]>(p => int.Parse(p[0].Value.ToString()) == offer.Price)),
                Times.Once);
        }

        [Test]
        public void PurchasePoints_ValidParameters_CallsExecuteReaderWithCorrectPointsParameter()
        {
            // Arrange
            var offer = new PointsOffer(10, 100); // price 10, points 100
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("BuyPoints", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.PurchasePoints(offer, walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("BuyPoints",
                It.Is<SqlParameter[]>(p => int.Parse(p[1].Value.ToString()) == offer.Points)),
                Times.Once);
        }

        [Test]
        public void PurchasePoints_ValidParameters_CallsExecuteReaderWithCorrectWalletIdParameter()
        {
            // Arrange
            var offer = new PointsOffer(10, 100); // price 10, points 100
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("BuyPoints", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.PurchasePoints(offer, walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("BuyPoints",
                It.Is<SqlParameter[]>(p => int.Parse(p[2].Value.ToString()) == walletId)),
                Times.Once);
        }

        #endregion

        #region AddNewWallet Tests

        [Test]
        public void AddNewWallet_ValidParameters_CallsExecuteReader()
        {
            // Arrange
            int userId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("CreateWallet", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.AddNewWallet(userId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("CreateWallet", It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void AddNewWallet_ValidParameters_CallsExecuteReaderWithCorrectUserIdParameter()
        {
            // Arrange
            int userId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("CreateWallet", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.AddNewWallet(userId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("CreateWallet",
                It.Is<SqlParameter[]>(p => int.Parse(p[0].Value.ToString()) == userId)),
                Times.Once);
        }

        [Test]
        public void AddNewWallet_ExceptionInDataLink_CatchesException()
        {
            // Arrange
            int userId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("CreateWallet", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test exception"));

            // Act & Assert
            Assert.DoesNotThrow(() => _repository.AddNewWallet(userId));
        }

        #endregion

        #region RemoveWallet Tests

        [Test]
        public void RemoveWallet_ValidParameters_CallsExecuteReader()
        {
            // Arrange
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("RemoveWallet", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.RemoveWallet(walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("RemoveWallet", It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void RemoveWallet_ValidParameters_CallsExecuteReaderWithCorrectWalletIdParameter()
        {
            // Arrange
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("RemoveWallet", It.IsAny<SqlParameter[]>()));

            // Act
            _repository.RemoveWallet(walletId);

            // Assert
            _mockDataLink.Verify(dl => dl.ExecuteReader("RemoveWallet",
                It.Is<SqlParameter[]>(p => int.Parse(p[0].Value.ToString()) == walletId)),
                Times.Once);
        }

        [Test]
        public void RemoveWallet_ExceptionInDataLink_CatchesException()
        {
            // Arrange
            int walletId = 1;
            _mockDataLink.Setup(dl => dl.ExecuteReader("RemoveWallet", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test exception"));

            // Act & Assert
            Assert.DoesNotThrow(() => _repository.RemoveWallet(walletId));
        }

        #endregion

        #region Helper Methods

        private DataTable CreateWalletDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("wallet_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("points", typeof(int));
            dataTable.Columns.Add("money_for_games", typeof(decimal));

            // Added a sample row
            dataTable.Rows.Add(1, 1, 200, 100.50m);

            return dataTable;
        }

        #endregion
    }
}