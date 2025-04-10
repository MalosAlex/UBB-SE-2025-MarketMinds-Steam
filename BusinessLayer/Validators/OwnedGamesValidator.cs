using System.ComponentModel.DataAnnotations;
using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class OwnedGameValidator
    {
        // Validation constants
        private const int MinUserId = 1;
        private const int MaxTitleLength = 100;
        private const int MaxCoverPictureLength = 255;

        // Validation error message constants
        private const string ErrInvalidUserId = "User ID must be greater than 0";
        private const string ErrInvalidTitle = "Title cannot be empty or longer than 100 characters";
        private const string ErrInvalidCoverPicture = "Cover picture URL cannot exceed 255 characters";

        public static bool IsUserIdValid(int userId)
        {
            return userId >= MinUserId;
        }

        public static bool IsTitleValid(string title)
        {
            return !string.IsNullOrWhiteSpace(title) && title.Length <= MaxTitleLength;
        }

        public static bool IsCoverPictureValid(string? coverPicture)
        {
            return coverPicture == null || coverPicture.Length <= MaxCoverPictureLength;
        }

        public static void ValidateOwnedGame(OwnedGame ownedGame)
        {
            if (!IsUserIdValid(ownedGame.UserId))
            {
                throw new ValidationException(ErrInvalidUserId);
            }

            if (!IsTitleValid(ownedGame.Title))
            {
                throw new ValidationException(ErrInvalidTitle);
            }

            if (!IsCoverPictureValid(ownedGame.CoverPicture))
            {
                throw new ValidationException(ErrInvalidCoverPicture);
            }
        }
    }
}