using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamProfile.Models
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

        public List<OwnedGame> Games { get; set; } = new List<OwnedGame>();
        public bool IsAllOwnedGamesCollection { get; internal set; } = false;

        public void AddGame(OwnedGame game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            if (!Games.Any(g => g.GameId == game.GameId))
            {
                Games.Add(game);
            }
        }

        public void RemoveGame(OwnedGame game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            Games.RemoveAll(g => g.GameId == game.GameId);
        }

        public void UpdateFrom(Collection other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other.CollectionId != CollectionId)
                throw new InvalidOperationException("Cannot update from a collection with a different ID");

            UserId = other.UserId;
            Name = other.Name;
            CoverPicture = other.CoverPicture;
            IsPublic = other.IsPublic;
            CreatedAt = other.CreatedAt;
        }

        public void Validate()
        {
            if (UserId <= 0)
                throw new ValidationException("User ID must be greater than 0");

            if (string.IsNullOrWhiteSpace(Name))
                throw new ValidationException("Name cannot be empty");

            if (CoverPicture != null && CoverPicture.Length > 255)
                throw new ValidationException("Cover picture URL cannot exceed 255 characters");
        }
    }
}
