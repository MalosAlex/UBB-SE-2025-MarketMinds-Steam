using System.ComponentModel.DataAnnotations;
using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class CollectionValidator
    {
        // Validation constants
        private const int MinUserId = 1;
        private const int MaxNameLength = 100;
        private const int MaxCoverPictureLength = 255;

        // Validation error message constants
        private const string ErrInvalidUserId = "User ID must be greater than 0";
        private const string ErrInvalidName = "Name cannot be empty or longer than 100 characters";
        private const string ErrInvalidCoverPicture = "Cover picture URL cannot exceed 255 characters";

        public static bool IsUserIdValid(int userId)
        {
            return userId >= MinUserId;
        }

        public static bool IsNameValid(string collectionName)
        {
            return !string.IsNullOrWhiteSpace(collectionName) && collectionName.Length <= MaxNameLength;
        }

        public static bool IsCoverPictureValid(string? coverPicture)
        {
            return coverPicture == null || coverPicture.Length <= MaxCoverPictureLength;
        }

        public static void ValidateCollection(Collection collection)
        {
            if (!IsUserIdValid(collection.UserId))
            {
                throw new ValidationException(ErrInvalidUserId);
            }

            if (!IsNameValid(collection.Name))
            {
                throw new ValidationException(ErrInvalidName);
            }

            if (!IsCoverPictureValid(collection.CoverPicture))
            {
                throw new ValidationException(ErrInvalidCoverPicture);
            }
        }
    }
}