using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class FriendshipValidator
    {
        // Validation constants
        private const int MinUserId = 1;

        // Validation error message constants
        private const string ErrInvalidUserId = "User ID must be greater than 0.";
        private const string ErrInvalidFriendId = "Friend ID must be greater than 0.";
        private const string ErrSelfFriendship = "User cannot be friends with themselves.";

        public static bool IsUserIdValid(int userId)
        {
            return userId >= MinUserId;
        }

        public static bool IsFriendIdValid(int friendUserId)
        {
            return friendUserId >= MinUserId;
        }

        public static bool IsNotSelfFriendship(int userId, int friendUserId)
        {
            return userId != friendUserId;
        }

        public static void ValidateFriendship(Friendship friendship)
        {
            if (!IsUserIdValid(friendship.UserId))
            {
                throw new InvalidOperationException(ErrInvalidUserId);
            }

            if (!IsFriendIdValid(friendship.FriendId))
            {
                throw new InvalidOperationException(ErrInvalidFriendId);
            }

            if (!IsNotSelfFriendship(friendship.UserId, friendship.FriendId))
            {
                throw new InvalidOperationException(ErrSelfFriendship);
            }
        }
    }
}