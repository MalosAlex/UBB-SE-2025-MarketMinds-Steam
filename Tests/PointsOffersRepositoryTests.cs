using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories;

namespace Tests
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
        public void Offers_ReturnsHardcodedOffers() 
        {
            var offers = _repository.Offers;

            Assert.That(offers, Is.Not.Null);
            Assert.That(offers.Count, Is.EqualTo(5)); // It has 5 offers

            // Check the first offer
            Assert.That(offers[0].Price, Is.EqualTo(2));
            Assert.That(offers[0].Points, Is.EqualTo(5));


            // Check the second offer
            Assert.That(offers[1].Price, Is.EqualTo(8));
            Assert.That(offers[1].Points, Is.EqualTo(25));


            // Check the third offer
            Assert.That(offers[2].Price, Is.EqualTo(15));
            Assert.That(offers[2].Points, Is.EqualTo(50));

            // Check the fourth offer
            Assert.That(offers[3].Price, Is.EqualTo(20));
            Assert.That(offers[3].Points, Is.EqualTo(100));

            // Check the fifth offer
            Assert.That(offers[4].Price, Is.EqualTo(50));
            Assert.That(offers[4].Points, Is.EqualTo(500));
        }
    }
}