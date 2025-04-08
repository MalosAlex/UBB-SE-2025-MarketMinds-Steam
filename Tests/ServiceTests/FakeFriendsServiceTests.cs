using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Services.Fakes;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class FakeFriendsServiceTests
    {
        private IFriendsService _fakeFriendsService;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Instantiate the fake friends service.
            // (Assuming FakeFriendsService is parameterless or has its own seeded data.)
            _fakeFriendsService = new FakeFriendsService();
        }

        #region GetAllFriendships

        [Test]
        public void GetAllFriendships_ReturnsFriendshipsForCurrentUser()
        {
            // Act: Retrieve all friendships.
            List<Friendship> result = _fakeFriendsService.GetAllFriendships();

            // Assert: Expect at least one friendship to be returned.
            Assert.That(result.Count, Is.GreaterThan(0));
        }

        [Test]
        public void GetAllFriendships_IsSortedByFriendUsername()
        {
            // Act: Retrieve all friendships.
            List<Friendship> result = _fakeFriendsService.GetAllFriendships();

            // Assert: The list should be sorted by FriendUsername in ascending order.
            for (int i = 1; i < result.Count; i++)
            {
                Assert.That(
                    string.Compare(result[i - 1].FriendUsername, result[i].FriendUsername, StringComparison.Ordinal),
                    Is.LessThanOrEqualTo(0));
            }
        }

        #endregion

        #region RemoveFriend

        [Test]
        public void RemoveFriend_RemovesFriendship()
        {
            // Arrange: Get the initial count of friendships.
            List<Friendship> before = _fakeFriendsService.GetAllFriendships();
            int countBefore = before.Count;
            int friendshipId = before.First().FriendshipId;

            // Act: Remove one friendship.
            _fakeFriendsService.RemoveFriend(friendshipId);
            List<Friendship> after = _fakeFriendsService.GetAllFriendships();

            // Assert: Expect the friendship count to have decreased by one.
            Assert.That(after.Count, Is.EqualTo(countBefore - 1));
        }

        [Test]
        public void RemoveFriend_NonExistentFriendship_DoesNothing()
        {
            // Arrange: Get the initial count.
            List<Friendship> before = _fakeFriendsService.GetAllFriendships();
            int countBefore = before.Count;
            int nonExistentFriendshipId = -1; // Assume negative ids are never used

            // Act: Attempt to remove a non-existent friendship.
            _fakeFriendsService.RemoveFriend(nonExistentFriendshipId);
            List<Friendship> after = _fakeFriendsService.GetAllFriendships();

            // Assert: Count should remain the same.
            Assert.That(after.Count, Is.EqualTo(countBefore));
        }

        #endregion

        #region GetFriendshipCount

        [Test]
        public void GetFriendshipCount_ReturnsCorrectCount()
        {
            // Arrange: Use a known user id (assume current user id is seeded as 1).
            int expectedCount = _fakeFriendsService.GetAllFriendships().Count;

            // Act: Get the friendship count.
            int count = _fakeFriendsService.GetFriendshipCount(1);

            // Assert: The count matches the expected count.
            Assert.That(count, Is.EqualTo(expectedCount));
        }

        #endregion

        #region AreUsersFriends

        [Test]
        public void AreUsersFriends_ExistingRelationship_ReturnsTrue()
        {
            // Arrange: Get a known friendship from the fake data.
            Friendship friend = _fakeFriendsService.GetAllFriendships().First();
            int userId1 = friend.UserId;
            int userId2 = friend.FriendId;

            // Act: Check if they are friends.
            bool areFriends = _fakeFriendsService.AreUsersFriends(userId1, userId2);

            // Assert: Expect true.
            Assert.That(areFriends, Is.True);
        }

        [Test]
        public void AreUsersFriends_NonExistingRelationship_ReturnsFalse()
        {
            // Arrange: Use user id 1 and a friend id that is not present.
            int userId1 = 1;
            int nonExistingFriendId = 999;

            // Act: Check friendship status.
            bool areFriends = _fakeFriendsService.AreUsersFriends(userId1, nonExistingFriendId);

            // Assert: Expect false.
            Assert.That(areFriends, Is.False);
        }

        #endregion

        #region GetFriendshipId

        [Test]
        public void GetFriendshipId_ExistingRelationship_ReturnsId()
        {
            // Arrange: Get a known friendship from seeded data.
            Friendship friend = _fakeFriendsService.GetAllFriendships().First();
            int userId1 = friend.UserId;
            int userId2 = friend.FriendId;
            int expectedId = friend.FriendshipId;

            // Act: Retrieve the friendship id.
            int? id = _fakeFriendsService.GetFriendshipId(userId1, userId2);

            // Assert: The returned id equals the expected id.
            Assert.That(id, Is.EqualTo(expectedId));
        }

        [Test]
        public void GetFriendshipId_NonExistingRelationship_ReturnsNull()
        {
            // Arrange: Use a non-existing friendship.
            int userId1 = 1;
            int nonExistingFriendId = 999;

            // Act: Retrieve the friendship id.
            int? id = _fakeFriendsService.GetFriendshipId(userId1, nonExistingFriendId);

            // Assert: Expect null.
            Assert.That(id, Is.Null);
        }

        #endregion

        #region AddFriend

        [Test]
        public void AddFriend_AddsNewFriendship()
        {
            // Arrange: Get the initial count.
            List<Friendship> before = _fakeFriendsService.GetAllFriendships();
            int countBefore = before.Count;

            // Act: Add a new friend (assume friend id 100 is not already a friend).
            _fakeFriendsService.AddFriend(1, 100);
            List<Friendship> after = _fakeFriendsService.GetAllFriendships();

            // Assert: Count increased by one.
            Assert.That(after.Count, Is.EqualTo(countBefore + 1));
        }

        [Test]
        public void AddFriend_DuplicateFriendship_ThrowsException()
        {
            // Arrange: Use an existing friendship from the seeded data.
            Friendship friend = _fakeFriendsService.GetAllFriendships().First();
            int userId = friend.UserId;
            int friendId = friend.FriendId;

            // Act & Assert: Adding the same friendship should throw an exception.
            var ex = Assert.Throws<Exception>(() => _fakeFriendsService.AddFriend(userId, friendId));
            Assert.That(ex.Message, Is.EqualTo("Friendship already exists."));
        }

        #endregion
    }
}
