using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class OwnedGame
    {
        // Constants for validation
        private const int TitleMaxLength = 100;
        private const int TitleMinLength = 1;
        private const int CoverPictureMaxLength = 255;

        private const string TitleLengthError = "Title must be between 1 and 100 characters";
        private const string CoverPictureLengthError = "Cover picture URL cannot exceed 255 characters";

        public int GameId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength, ErrorMessage = TitleLengthError)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [StringLength(CoverPictureMaxLength, ErrorMessage = CoverPictureLengthError)]
        public string? CoverPicture { get; set; }

        public OwnedGame(int userId, string title, string description, string? coverPicture = null)
        {
            UserId = userId;
            Title = title;
            Description = description;
            CoverPicture = coverPicture;
        }
    }
}