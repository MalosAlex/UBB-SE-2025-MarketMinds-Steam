using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User GetUserById(int userId);
        User GetUserByEmail(string email);
        User GetUserByUsername(string username);
        void ValidateUserAndEmail(string email, string username);
        User CreateUser(User user);
        User UpdateUser(User user);
        void DeleteUser(int userId);
        bool AcceptChanges(int userId, string givenPassword);
        void UpdateUserEmail(int userId, string newEmail);
        void UpdateUserPassword(int userId, string newPassword);
        void UpdateUserUsername(int userId, string newUsername);
        User? Login(string emailOrUsername, string password);
        void Logout();
        User GetCurrentUser();
        bool IsUserLoggedIn();
        bool UpdateUserUsername(string username, string currentPassword);
        bool UpdateUserPassword(string password, string currentPassword);
        bool UpdateUserEmail(string email, string currentPassword);
        bool VerifyUserPassword(string password);
    }
}
