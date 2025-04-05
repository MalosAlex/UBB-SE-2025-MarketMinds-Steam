using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Validators;
using System.ComponentModel.DataAnnotations;
using System;

namespace Tests
{
    [TestFixture]
    public class ValidatorTests
    {
        // --- CollectionValidator Tests ---

        [Test]
        public void ValidateCollection_ValidCollection_DoesNotThrow()
        {
            var collection = new Collection(
                userId: 1,
                name: "My Collection",
                createdAt: DateOnly.FromDateTime(DateTime.Today),
                coverPicture: "http://example.com/pic.jpg",
                isPublic: true
            );

            Assert.That(() => CollectionValidator.ValidateCollection(collection), Throws.Nothing);
        }

        [Test]
        public void ValidateCollection_InvalidUserId_ThrowsException()
        {
            var collection = new Collection(
                userId: 0,
                name: "My Collection",
                createdAt: DateOnly.FromDateTime(DateTime.Today)
            );

            Assert.That(() => CollectionValidator.ValidateCollection(collection), 
                Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ValidateCollection_TooLongName_ThrowsException()
        {
            var collection = new Collection(
                userId: 1,
                name: new string('x', 101),
                createdAt: DateOnly.FromDateTime(DateTime.Today)
            );

            Assert.That(() => CollectionValidator.ValidateCollection(collection), 
                Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ValidateCollection_TooLongCoverPicture_ThrowsException()
        {
            var collection = new Collection(
                userId: 1,
                name: "Valid",
                createdAt: DateOnly.FromDateTime(DateTime.Today),
                coverPicture: new string('x', 256)
            );

            Assert.That(() => CollectionValidator.ValidateCollection(collection), 
                Throws.TypeOf<ValidationException>());
        }

        // --- Individual method tests for CollectionValidator ---
        
        [Test]
        public void IsUserIdValid_PositiveId_ReturnsTrue()
        {
            Assert.That(CollectionValidator.IsUserIdValid(5), Is.True);
        }

        [Test]
        public void IsUserIdValid_ZeroId_ReturnsFalse()
        {
            Assert.That(CollectionValidator.IsUserIdValid(0), Is.False);
        }

        [Test]
        public void IsUserIdValid_NegativeId_ReturnsFalse()
        {
            Assert.That(CollectionValidator.IsUserIdValid(-1), Is.False);
        }

        [Test]
        public void IsNameValid_ValidName_ReturnsTrue()
        {
            Assert.That(CollectionValidator.IsNameValid("Test Collection"), Is.True);
        }

        [Test]
        public void IsNameValid_EmptyName_ReturnsFalse()
        {
            Assert.That(CollectionValidator.IsNameValid(""), Is.False);
        }

        [Test]
        public void IsNameValid_NullName_ReturnsFalse()
        {
            Assert.That(CollectionValidator.IsNameValid(null), Is.False);
        }

        [Test]
        public void IsNameValid_WhitespaceName_ReturnsFalse()
        {
            Assert.That(CollectionValidator.IsNameValid("   "), Is.False);
        }

        [Test]
        public void IsCoverPictureValid_NullCoverPicture_ReturnsTrue()
        {
            Assert.That(CollectionValidator.IsCoverPictureValid(null), Is.True);
        }

        [Test]
        public void IsCoverPictureValid_ValidCoverPicture_ReturnsTrue()
        {
            Assert.That(CollectionValidator.IsCoverPictureValid("http://example.com/image.jpg"), Is.True);
        }

        [Test]
        public void IsCoverPictureValid_TooLongCoverPicture_ReturnsFalse()
        {
            Assert.That(CollectionValidator.IsCoverPictureValid(new string('x', 256)), Is.False);
        }

        // --- FriendshipValidator Tests ---

        [Test]
        public void ValidateFriendship_ValidFriendship_DoesNotThrow()
        {
            var friendship = new Friendship(
                friendshipId: 1,
                userId: 1,
                friendId: 2,
                friendUsername: "FriendUser",
                friendProfilePicture: "http://example.com/pfp.jpg"
            );

            Assert.That(() => FriendshipValidator.ValidateFriendship(friendship), Throws.Nothing);
        }

        [Test]
        public void ValidateFriendship_InvalidUserId_ThrowsException()
        {
            var friendship = new Friendship(
                friendshipId: 1,
                userId: 0,
                friendId: 2
            );

            Assert.That(() => FriendshipValidator.ValidateFriendship(friendship), 
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ValidateFriendship_InvalidFriendId_ThrowsException()
        {
            var friendship = new Friendship(
                friendshipId: 1,
                userId: 1,
                friendId: 0
            );

            Assert.That(() => FriendshipValidator.ValidateFriendship(friendship), 
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ValidateFriendship_SelfFriendship_ThrowsException()
        {
            var friendship = new Friendship(
                friendshipId: 1,
                userId: 1,
                friendId: 1
            );

            Assert.That(() => FriendshipValidator.ValidateFriendship(friendship), 
                Throws.TypeOf<InvalidOperationException>());
        }

        // --- Individual method tests for FriendshipValidator ---

        [Test]
        public void IsFriendIdValid_PositiveId_ReturnsTrue()
        {
            Assert.That(FriendshipValidator.IsFriendIdValid(5), Is.True);
        }

        [Test]
        public void IsFriendIdValid_ZeroId_ReturnsFalse()
        {
            Assert.That(FriendshipValidator.IsFriendIdValid(0), Is.False);
        }

        [Test]
        public void IsFriendIdValid_NegativeId_ReturnsFalse()
        {
            Assert.That(FriendshipValidator.IsFriendIdValid(-1), Is.False);
        }

        [Test]
        public void IsNotSelfFriendship_DifferentIds_ReturnsTrue()
        {
            Assert.That(FriendshipValidator.IsNotSelfFriendship(1, 2), Is.True);
        }

        [Test]
        public void IsNotSelfFriendship_SameIds_ReturnsFalse()
        {
            Assert.That(FriendshipValidator.IsNotSelfFriendship(1, 1), Is.False);
        }

        // --- OwnedGameValidator Tests ---

        [Test]
        public void ValidateOwnedGame_ValidGame_DoesNotThrow()
        {
            var game = new OwnedGame(
                userId: 1,
                title: "Elden Ring",
                description: "Awesome RPG",
                coverPicture: null
            );

            Assert.That(() => OwnedGameValidator.ValidateOwnedGame(game), Throws.Nothing);
        }

        [Test]
        public void ValidateOwnedGame_InvalidUserId_ThrowsException()
        {
            var game = new OwnedGame(
                userId: 0,
                title: "Valid Game",
                description: "Test Game"
            );

            Assert.That(() => OwnedGameValidator.ValidateOwnedGame(game), 
                Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ValidateOwnedGame_InvalidTitle_ThrowsException()
        {
            var game = new OwnedGame(
                userId: 1,
                title: "",
                description: "Test Game"
            );

            Assert.That(() => OwnedGameValidator.ValidateOwnedGame(game), 
                Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ValidateOwnedGame_TooLongCoverPicture_ThrowsException()
        {
            var game = new OwnedGame(
                userId: 1,
                title: "Valid Game",
                description: "Great Game",
                coverPicture: new string('x', 256)
            );

            Assert.That(() => OwnedGameValidator.ValidateOwnedGame(game), 
                Throws.TypeOf<ValidationException>());
        }

        // --- Individual method tests for OwnedGameValidator ---

        [Test]
        public void IsUserIdValid_OwnedGame_PositiveId_ReturnsTrue()
        {
            Assert.That(OwnedGameValidator.IsUserIdValid(5), Is.True);
        }

        [Test]
        public void IsUserIdValid_OwnedGame_ZeroId_ReturnsFalse()
        {
            Assert.That(OwnedGameValidator.IsUserIdValid(0), Is.False);
        }

        [Test]
        public void IsUserIdValid_OwnedGame_NegativeId_ReturnsFalse()
        {
            Assert.That(OwnedGameValidator.IsUserIdValid(-1), Is.False);
        }

        [Test]
        public void IsTitleValid_ValidTitle_ReturnsTrue()
        {
            Assert.That(OwnedGameValidator.IsTitleValid("Game Title"), Is.True);
        }

        [Test]
        public void IsTitleValid_EmptyTitle_ReturnsFalse()
        {
            Assert.That(OwnedGameValidator.IsTitleValid(""), Is.False);
        }

        [Test]
        public void IsTitleValid_NullTitle_ReturnsFalse()
        {
            Assert.That(OwnedGameValidator.IsTitleValid(null), Is.False);
        }

        [Test]
        public void IsTitleValid_WhitespaceTitle_ReturnsFalse()
        {
            Assert.That(OwnedGameValidator.IsTitleValid("   "), Is.False);
        }

        [Test]
        public void IsTitleValid_TooLongTitle_ReturnsFalse()
        {
            Assert.That(OwnedGameValidator.IsTitleValid(new string('x', 101)), Is.False);
        }

        [Test]
        public void IsCoverPictureValid_OwnedGame_NullCoverPicture_ReturnsTrue()
        {
            Assert.That(OwnedGameValidator.IsCoverPictureValid(null), Is.True);
        }

        [Test]
        public void IsCoverPictureValid_OwnedGame_ValidCoverPicture_ReturnsTrue()
        {
            Assert.That(OwnedGameValidator.IsCoverPictureValid("http://example.com/game.jpg"), Is.True);
        }

        [Test]
        public void IsCoverPictureValid_OwnedGame_TooLongCoverPicture_ReturnsFalse()
        {
            Assert.That(OwnedGameValidator.IsCoverPictureValid(new string('x', 256)), Is.False);
        }

        // --- UserValidator Tests ---

        [Test]
        public void ValidateUser_ValidUser_DoesNotThrow()
        {
            var user = new User
            {
                Password = "ValidPass1!",
                Email = "test@example.com"
            };

            Assert.That(() => UserValidator.ValidateUser(user), Throws.Nothing);
        }

        [Test]
        public void ValidateUser_InvalidPassword_ThrowsException()
        {
            var user = new User
            {
                Password = "short",
                Email = "test@example.com"
            };

            Assert.That(() => UserValidator.ValidateUser(user), 
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ValidateUser_InvalidEmail_ThrowsException()
        {
            var user = new User
            {
                Password = "StrongPass1!",
                Email = "invalidemail@"
            };

            Assert.That(() => UserValidator.ValidateUser(user), 
                Throws.TypeOf<InvalidOperationException>());
        }

        // --- Individual method tests for UserValidator ---

        [Test]
        public void IsPasswordValid_ValidPassword_ReturnsTrue()
        {
            Assert.That(UserValidator.IsPasswordValid("ValidPass1!"), Is.True);
        }

        [Test]
        public void IsPasswordValid_TooShortPassword_ReturnsFalse()
        {
            Assert.That(UserValidator.IsPasswordValid("Sh0rt!"), Is.False);
        }

        [Test]
        public void IsPasswordValid_NoUpperCase_ReturnsFalse()
        {
            Assert.That(UserValidator.IsPasswordValid("validpass1!"), Is.False);
        }

        [Test]
        public void IsPasswordValid_NoLowerCase_ReturnsFalse()
        {
            Assert.That(UserValidator.IsPasswordValid("VALIDPASS1!"), Is.False);
        }

        [Test]
        public void IsPasswordValid_NoDigit_ReturnsFalse()
        {
            Assert.That(UserValidator.IsPasswordValid("ValidPass!"), Is.False);
        }

        [Test]
        public void IsPasswordValid_NoSpecialChar_ReturnsFalse()
        {
            Assert.That(UserValidator.IsPasswordValid("ValidPass1"), Is.False);
        }

        [Test]
        public void IsPasswordValid_NullPassword_ReturnsFalse()
        {
            Assert.That(UserValidator.IsPasswordValid(null), Is.False);
        }

        [Test]
        public void IsPasswordValid_EmptyPassword_ReturnsFalse()
        {
            Assert.That(UserValidator.IsPasswordValid(""), Is.False);
        }

        [Test]
        public void IsPasswordValid_WhitespacePassword_ReturnsFalse()
        {
            Assert.That(UserValidator.IsPasswordValid("   "), Is.False);
        }

        [Test]
        public void IsEmailValid_ValidEmail_ReturnsTrue()
        {
            Assert.That(UserValidator.IsEmailValid("test@example.com"), Is.True);
        }

        [Test]
        public void IsEmailValid_NoAtSign_ReturnsFalse()
        {
            Assert.That(UserValidator.IsEmailValid("testexample.com"), Is.False);
        }

        [Test]
        public void IsEmailValid_NoDomain_ReturnsFalse()
        {
            Assert.That(UserValidator.IsEmailValid("test@"), Is.False);
        }

        [Test]
        public void IsEmailValid_NoTLD_ReturnsFalse()
        {
            Assert.That(UserValidator.IsEmailValid("test@example"), Is.False);
        }

        [Test]
        public void IsEmailValid_NullEmail_ReturnsFalse()
        {
            Assert.That(UserValidator.IsEmailValid(null), Is.False);
        }

        [Test]
        public void IsEmailValid_EmptyEmail_ReturnsFalse()
        {
            Assert.That(UserValidator.IsEmailValid(""), Is.False);
        }

        [Test]
        public void IsEmailValid_WhitespaceEmail_ReturnsFalse()
        {
            Assert.That(UserValidator.IsEmailValid("   "), Is.False);
        }
    }
}