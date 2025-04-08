using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class FriendshipValidator
    {
        public static bool IsUserIdValid(int userId)
        {
            return userId > 0;
        }

        public static bool IsFriendIdValid(int friendUserId)
        {
            return friendUserId > 0;
        }

        public static bool IsNotSelfFriendship(int userId, int friendUserId)
        {
            return userId != friendUserId;
        }

        public static void ValidateFriendship(Friendship friendship)
        {
            if (!IsUserIdValid(friendship.UserId))
            {
                throw new InvalidOperationException("User ID must be greater than 0.");
            }

            if (!IsFriendIdValid(friendship.FriendId))
            {
                throw new InvalidOperationException("Friend ID must be greater than 0.");
            }

            if (!IsNotSelfFriendship(friendship.UserId, friendship.FriendId))
            {
                throw new InvalidOperationException("User cannot be friends with themselves.");
            }
        }
    }
}