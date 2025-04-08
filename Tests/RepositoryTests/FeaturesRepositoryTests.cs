using System;
using System.Collections.Generic;
using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class FeaturesRepositoryTests
    {
        private Mock<IDataLink> mockDataLink;
        private FeaturesRepository repository;

        [SetUp]
        public void Setup()
        {
            mockDataLink = new Mock<IDataLink>();
            repository = new FeaturesRepository(mockDataLink.Object);
        }

        [Test]
        public void Constructor_WithNullDataLink_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => new FeaturesRepository(null), Throws.ArgumentNullException);
        }

        [Test]
        public void GetAllFeatures_ReturnsCorrectFeatures()
        {
            // Arrange
            int userId = 1;
            var dataTable = CreateFeaturesDataTable();

            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.Is<string>(s => s == "GetAllFeatures"),
                It.Is<SqlParameter[]>(p => p.Length == 1 && (int)p[0].Value == userId)))
                .Returns(dataTable);

            // Act
            var features = repository.GetAllFeatures(userId);

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features.Count, Is.EqualTo(2));
            Assert.That(features[0].FeatureId, Is.EqualTo(1));
            Assert.That(features[0].Name, Is.EqualTo("Gold Frame"));
            Assert.That(features[0].Type, Is.EqualTo("frame"));
            Assert.That(features[0].Value, Is.EqualTo(100));
        }

        [Test]
        public void GetAllFeatures_WhenDatabaseOperationExceptionOccurs_ThrowsDatabaseOperationException()
        {
            // Arrange
            int userId = 1;
            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.That(() => repository.GetAllFeatures(userId), Throws.TypeOf<DatabaseOperationException>());
        }

        [Test]
        public void GetFeaturesByType_ReturnsCorrectFeatures()
        {
            // Arrange
            string type = "frame";
            var dataTable = CreateFeaturesDataTable();

            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.Is<string>(s => s == "GetFeaturesByType"),
                It.Is<SqlParameter[]>(p => p.Length == 1 && (string)p[0].Value == type)))
                .Returns(dataTable);

            // Act
            var features = repository.GetFeaturesByType(type);

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features.Count, Is.EqualTo(2));
            Assert.That(features.TrueForAll(f => f.Type == type), Is.True);
        }

        [Test]
        public void GetFeaturesByType_WhenDatabaseOperationExceptionOccurs_ThrowsDatabaseOperationException()
        {
            // Arrange
            string type = "frame";
            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.That(() => repository.GetFeaturesByType(type), Throws.TypeOf<DatabaseOperationException>());
        }

        [Test]
        public void GetUserFeatures_ReturnsCorrectFeatures()
        {
            // Arrange
            int userId = 1;
            var dataTable = CreateFeaturesDataTable();

            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.Is<string>(s => s == "GetUserFeatures"),
                It.Is<SqlParameter[]>(p => p.Length == 1 && (int)p[0].Value == userId)))
                .Returns(dataTable);

            // Act
            var features = repository.GetUserFeatures(userId);

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetUserFeatures_WhenDatabaseOperationExceptionOccurs_ThrowsDatabaseOperationException()
        {
            // Arrange
            int userId = 1;
            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.That(() => repository.GetUserFeatures(userId), Throws.TypeOf<DatabaseOperationException>());
        }

        [Test]
        public void IsFeaturePurchased_WhenRelationshipExists_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("relationship_id");
            dataTable.Rows.Add(1);

            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.Is<string>(s => s == "GetFeatureUserRelationship"),
                It.Is<SqlParameter[]>(p => p.Length == 2 && (int)p[0].Value == userId && (int)p[1].Value == featureId)))
                .Returns(dataTable);

            // Act
            bool result = repository.IsFeaturePurchased(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsFeaturePurchased_WhenRelationshipDoesNotExist_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("relationship_id");

            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.Is<string>(s => s == "GetFeatureUserRelationship"),
                It.Is<SqlParameter[]>(p => p.Length == 2 && (int)p[0].Value == userId && (int)p[1].Value == featureId)))
                .Returns(dataTable);

            // Act
            bool result = repository.IsFeaturePurchased(userId, featureId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void EquipFeature_WhenFeatureIsPurchased_UpdatesEquipStatusAndReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("relationship_id");
            dataTable.Rows.Add(1);

            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.Is<string>(s => s == "GetFeatureUserRelationship"),
                It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            mockDataLink.Setup(dl => dl.ExecuteNonQuery(
                It.Is<string>(s => s == "UpdateFeatureUserEquipStatus"),
                It.IsAny<SqlParameter[]>()))
                .Verifiable();

            // Act
            bool result = repository.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
            mockDataLink.Verify(dl => dl.ExecuteNonQuery("UpdateFeatureUserEquipStatus", It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void EquipFeature_WhenFeatureIsNotPurchased_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("relationship_id");

            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.Is<string>(s => s == "GetFeatureUserRelationship"),
                It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            bool result = repository.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.False);
            mockDataLink.Verify(dl => dl.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Never);
        }

        [Test]
        public void UnequipFeature_WhenFeatureIsPurchased_UpdatesEquipStatusAndReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("relationship_id");
            dataTable.Rows.Add(1);

            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.Is<string>(s => s == "GetFeatureUserRelationship"),
                It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            mockDataLink.Setup(dl => dl.ExecuteNonQuery(
                It.Is<string>(s => s == "UpdateFeatureUserEquipStatus"),
                It.IsAny<SqlParameter[]>()))
                .Verifiable();

            // Act
            bool result = repository.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
            mockDataLink.Verify(dl => dl.ExecuteNonQuery("UpdateFeatureUserEquipStatus", It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void UnequipFeature_WhenFeatureIsNotPurchased_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("relationship_id");

            mockDataLink.Setup(dl => dl.ExecuteReader(
                It.Is<string>(s => s == "GetFeatureUserRelationship"),
                It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            bool result = repository.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.False);
            mockDataLink.Verify(dl => dl.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Never);
        }

        [Test]
        public void UnequipFeaturesByType_ExecutesCorrectProcedureWithParameters()
        {
            // Arrange
            int userId = 1;
            string featureType = "frame";

            mockDataLink.Setup(dl => dl.ExecuteNonQuery(
                It.Is<string>(s => s == "UnequipFeaturesByType"),
                It.Is<SqlParameter[]>(p => p.Length == 2 && (int)p[0].Value == userId && (string)p[1].Value == featureType)))
                .Verifiable();

            // Act
            bool result = repository.UnequipFeaturesByType(userId, featureType);

            // Assert
            Assert.That(result, Is.True);
            mockDataLink.Verify(dl => dl.ExecuteNonQuery("UnequipFeaturesByType", It.IsAny<SqlParameter[]>()), Times.Once);
        }

        // Helper method to create a data table with sample feature data
        private DataTable CreateFeaturesDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("feature_id", typeof(int));
            dataTable.Columns.Add("name", typeof(string));
            dataTable.Columns.Add("value", typeof(int));
            dataTable.Columns.Add("description", typeof(string));
            dataTable.Columns.Add("type", typeof(string));
            dataTable.Columns.Add("source", typeof(string));
            dataTable.Columns.Add("equipped", typeof(bool));

            dataTable.Rows.Add(1, "Gold Frame", 100, "A premium gold frame", "frame", "store", true);
            dataTable.Rows.Add(2, "Silver Frame", 50, "A nice silver frame", "frame", "store", false);

            return dataTable;
        }
    }
} 