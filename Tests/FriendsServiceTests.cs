using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Services;
using BusinessLayer.Services.fakes;
using BusinessLayer.Services.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class FriendsServiceTests
    {
        private IFriendsService _friendsService;
        private FakeFriendshipsRepository _fakeFriendshipsRepository;
        private IUserService _fakeUserService;

        [SetUp]
        public void SetUp()
        {
            _fakeFriendshipsRepository = new FakeFriendshipsRepository();
            _fakeUserService = new FakeUserService();
            _friendsService = new FriendsService(_fakeFriendshipsRepository, _fakeUserService);
        }

        [Test]
        public void GetAllFriendships_ReturnsFriendshipsForCurrentUser()
        {
            List<Friendship> result = _friendsService.GetAllFriendships();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(2));
            foreach (Friendship f in result)
            {
                Assert.That(f.UserId, Is.EqualTo(1));
            }
        }

        [Test]
        public void RemoveFriend_RemovesFriendship()
        {
            List<Friendship> before = _fakeFriendshipsRepository.GetAllFriendships(1);
            int countBefore = before.Count;
            Friendship toRemove = before[0];
            _friendsService.RemoveFriend(toRemove.FriendshipId);
            List<Friendship> after = _fakeFriendshipsRepository.GetAllFriendships(1);
            int countAfter = after.Count;
            Assert.That(countAfter, Is.EqualTo(countBefore - 1));
        }

        [Test]
        public void GetFriendshipCount_ReturnsCorrectCount()
        {
            int count1 = _friendsService.GetFriendshipCount(1);
            List<Friendship> all = _fakeFriendshipsRepository.GetAllFriendships(1);
            int expected = all.Count;
            Assert.That(count1, Is.EqualTo(expected));
        }

        [Test]
        public void AreUsersFriends_ReturnsTrueWhenFriends()
        {
            bool result = _friendsService.AreUsersFriends(1, 2);
            Assert.That(result, Is.True);
        }

        [Test]
        public void AreUsersFriends_ReturnsFalseWhenNotFriends()
        {
            bool result = _friendsService.AreUsersFriends(1, 999);
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetFriendshipId_ExistingRelationship_ReturnsId()
        {
            int? id = _friendsService.GetFriendshipId(1, 2);
            Assert.That(id, Is.Not.Null);
            Assert.That(id, Is.EqualTo(1));
        }

        [Test]
        public void GetFriendshipId_NonExistingRelationship_ReturnsNull()
        {
            int? id = _friendsService.GetFriendshipId(1, 999);
            Assert.That(id, Is.Null);
        }

        [Test]
        public void AddFriend_AddsNewFriendship()
        {
            List<Friendship> before = _fakeFriendshipsRepository.GetAllFriendships(1);
            int countBefore = before.Count;
            _friendsService.AddFriend(1, 4);
            List<Friendship> after = _fakeFriendshipsRepository.GetAllFriendships(1);
            int countAfter = after.Count;
            Assert.That(countAfter, Is.EqualTo(countBefore + 1));
        }
    }
}
