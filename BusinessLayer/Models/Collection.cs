using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class Collection
    {
        public int CollectionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Cover picture URL cannot exceed 255 characters")]
        public string? CoverPicture { get; set; }

        public bool IsPublic { get; set; }

        [Required]
        public DateOnly CreatedAt { get; set; }

        public List<OwnedGame> Games { get; set; } = new();
        public bool IsAllOwnedGamesCollection { get; }

        public Collection(int userId, string name, DateOnly createdAt, string? coverPicture = null, bool isPublic = false)
        {
            UserId = userId;
            Name = name;
            CreatedAt = createdAt;
            CoverPicture = coverPicture;
            IsPublic = isPublic;
            IsAllOwnedGamesCollection = false;
        }
    }
}