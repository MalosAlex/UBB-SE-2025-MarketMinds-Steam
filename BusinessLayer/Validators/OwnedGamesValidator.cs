using System.ComponentModel.DataAnnotations;
using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class OwnedGameValidator
    {
        public static bool IsUserIdValid(int userId)
        {
            return userId > 0;
        }

        public static bool IsTitleValid(string title)
        {
            return !string.IsNullOrWhiteSpace(title) && title.Length <= 100;
        }

        public static bool IsCoverPictureValid(string? coverPicture)
        {
            return coverPicture == null || coverPicture.Length <= 255;
        }

        public static void ValidateOwnedGame(OwnedGame ownedGame)
        {
            if (!IsUserIdValid(ownedGame.UserId))
            {
                throw new ValidationException("User ID must be greater than 0");
            }

            if (!IsTitleValid(ownedGame.Title))
            {
                throw new ValidationException("Title cannot be empty or longer than 100 characters");
            }

            if (!IsCoverPictureValid(ownedGame.CoverPicture))
            {
                throw new ValidationException("Cover picture URL cannot exceed 255 characters");
            }
        }
    }
}