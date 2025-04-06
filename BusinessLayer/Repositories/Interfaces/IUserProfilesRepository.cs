using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IUserProfilesRepository
    {
        UserProfile? GetUserProfileByUserId(int userId);
        UserProfile? UpdateProfile(UserProfile profile);
        UserProfile? CreateProfile(int userId);
        void UpdateProfileBio(int user_id, string bio);
        void UpdateProfilePicture(int user_id, string picture);

    }
}
