using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Fakes
{
    public class FakeFeaturesService : IFeaturesService
    {
        private readonly IFeaturesRepository featuresRepository;
        private readonly IUserService userService;

        public FakeFeaturesService()
        {
            featuresRepository = new FakeFeaturesRepository();
            userService = new FakeUserService();
        }

        public IUserService UserService => userService;

        public Dictionary<string, List<Feature>> GetFeaturesByCategories()
        {
            var categories = new Dictionary<string, List<Feature>>();
            var allFeatures = featuresRepository.GetAllFeatures(userService.GetCurrentUser().UserId);

            foreach (var feature in allFeatures)
            {
                if (!categories.ContainsKey(feature.Type))
                {
                    categories[feature.Type] = new List<Feature>();
                }
                categories[feature.Type].Add(feature);
            }

            return categories;
        }

        public bool EquipFeature(int userId, int featureId)
        {
            if (!featuresRepository.IsFeaturePurchased(userId, featureId))
            {
                return false;
            }

            var feature = featuresRepository.GetFeaturesByType("frame").FirstOrDefault(feature => feature.FeatureId == featureId);
            if (feature != null)
            {
                featuresRepository.UnequipFeaturesByType(userId, "frame");
            }

            return featuresRepository.EquipFeature(userId, featureId);
        }

        public (bool, string) UnequipFeature(int userId, int featureId)
        {
            if (!featuresRepository.IsFeaturePurchased(userId, featureId))
            {
                return (false, "Feature not purchased");
            }

            bool success = featuresRepository.UnequipFeature(userId, featureId);
            return (success, success ? "Feature unequipped successfully" : "Failed to unequip feature");
        }

        public List<Feature> GetUserEquippedFeatures(int userId)
        {
            return featuresRepository.GetUserFeatures(userId)
                .Where(feature => feature.Equipped)
                .ToList();
        }

        public bool IsFeaturePurchased(int userId, int featureId)
        {
            return featuresRepository.IsFeaturePurchased(userId, featureId);
        }

        public (bool success, string message) PurchaseFeature(int userId, int featureId)
        {
            // Fake implementation for testing purposes
            return (true, "Feature purchased successfully in test environment");
        }

        public (string profilePicturePath, string bioText, List<Feature> equippedFeatures) GetFeaturePreviewData(int userId, int featureId)
        {
            // Return fake data for testing
            var equippedFeatures = GetUserEquippedFeatures(userId);
            return ("ms-appx:///Assets/default-profile.png", "This is a test bio", equippedFeatures);
        }

        public List<Feature> GetUserFeatures(int userId)
        {
            return featuresRepository.GetUserFeatures(userId);
        }
    }
}