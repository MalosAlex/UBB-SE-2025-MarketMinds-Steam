using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
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

        
        public OwnedGame(int userId, string title, string description, string? coverPicture = null)
        {
            UserId = userId;
            Title = title;
            Description = description;
            CoverPicture = coverPicture;
        }
    }
}