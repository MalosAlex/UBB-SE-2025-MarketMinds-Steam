using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class Friendship
    {
        [Required]
        public int FriendshipId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int FriendId { get; set; }

        public string FriendUsername { get; set; }
        public string FriendProfilePicture { get; set; }

        public void Validate()
        {
            if (UserId <= 0)
                throw new InvalidOperationException("User ID must be greater than 0.");

            if (FriendId <= 0)
                throw new InvalidOperationException("Friend ID must be greater than 0.");

            if (UserId == FriendId)
                throw new InvalidOperationException("User cannot be friends with themselves.");
        }
    }
}