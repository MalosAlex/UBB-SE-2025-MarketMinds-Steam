using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Repositories.fakes
{
    public class FakeUserProfilesRepository : IUserProfilesRepository
    {
        public UserProfile? CreateProfile(int userId)
        {
            UserProfile profile = new UserProfile();
            profile.UserId = userId;
            return profile;
        }

        public UserProfile? GetUserProfileByUserId(int userId)
        {
            UserProfile profile = new UserProfile();
            profile.UserId = userId;
            return profile;
        }

        public UserProfile? UpdateProfile(UserProfile profile)
        {
            profile.Bio = "fake";
            return profile;
        }

        public void UpdateProfileBio(int user_id, string bio)
        {
           // throw an error for testing if needed
        }

        public void UpdateProfilePicture(int user_id, string picture)
        {
            // throw an error for testing if needed
        }
    }
}
