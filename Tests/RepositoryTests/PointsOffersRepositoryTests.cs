using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class PointsOffersRepositoryTests
    {
        private PointsOffersRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new PointsOffersRepository();
        }

        [Test]
        public void Offers_ReturnsNonNullList()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers, Is.Not.Null);
        }

        [Test]
        public void Offers_ReturnsCorrectNumberOfOffers()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers.Count, Is.EqualTo(5));
        }

        [Test]
        public void Offers_FirstOffer_HasCorrectPrice()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers[0].Price, Is.EqualTo(2));
        }

        [Test]
        public void Offers_FirstOffer_HasCorrectPoints()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers[0].Points, Is.EqualTo(5));
        }

        [Test]
        public void Offers_SecondOffer_HasCorrectPrice()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers[1].Price, Is.EqualTo(8));
        }

        [Test]
        public void Offers_SecondOffer_HasCorrectPoints()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers[1].Points, Is.EqualTo(25));
        }

        [Test]
        public void Offers_ThirdOffer_HasCorrectPrice()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers[2].Price, Is.EqualTo(15));
        }

        [Test]
        public void Offers_ThirdOffer_HasCorrectPoints()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers[2].Points, Is.EqualTo(50));
        }

        [Test]
        public void Offers_FourthOffer_HasCorrectPrice()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers[3].Price, Is.EqualTo(20));
        }

        [Test]
        public void Offers_FourthOffer_HasCorrectPoints()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers[3].Points, Is.EqualTo(100));
        }

        [Test]
        public void Offers_FifthOffer_HasCorrectPrice()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers[4].Price, Is.EqualTo(50));
        }

        [Test]
        public void Offers_FifthOffer_HasCorrectPoints()
        {
            // Act
            var offers = _repository.Offers;

            // Assert
            Assert.That(offers[4].Points, Is.EqualTo(500));
        }
    }
}