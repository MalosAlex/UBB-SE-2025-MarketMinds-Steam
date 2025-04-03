using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamProfile.Models
{
    public class OwnedGame
    {
        public int GameId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Cover picture URL cannot exceed 255 characters")]
        public string? CoverPicture { get; set; }

        public void UpdateFrom(OwnedGame other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other.GameId != GameId)
                throw new InvalidOperationException("Cannot update from a game with a different ID");

            UserId = other.UserId;
            Title = other.Title;
            Description = other.Description;
            CoverPicture = other.CoverPicture;
        }

        public void Validate()
        {
            if (UserId <= 0)
                throw new ValidationException("User ID must be greater than 0");

            if (string.IsNullOrWhiteSpace(Title))
                throw new ValidationException("Title cannot be empty");

            if (CoverPicture != null && CoverPicture.Length > 255)
                throw new ValidationException("Cover picture URL cannot exceed 255 characters");
        }
    }
}
