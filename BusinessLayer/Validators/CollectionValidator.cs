using BusinessLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Validators
{
    public static class CollectionValidator
    {
        public static bool IsUserIdValid(int userId)
        {
            return userId > 0;
        }

        public static bool IsNameValid(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length <= 100;
        }

        public static bool IsCoverPictureValid(string? coverPicture)
        {
            return coverPicture == null || coverPicture.Length <= 255;
        }

        public static void ValidateCollection(Collection collection)
        {
            if (!IsUserIdValid(collection.UserId))
                throw new ValidationException("User ID must be greater than 0");

            if (!IsNameValid(collection.Name))
                throw new ValidationException("Name cannot be empty or longer than 100 characters");

            if (!IsCoverPictureValid(collection.CoverPicture))
                throw new ValidationException("Cover picture URL cannot exceed 255 characters");
        }
    }
}