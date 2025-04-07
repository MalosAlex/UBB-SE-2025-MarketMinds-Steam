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

            var feature = featuresRepository.GetFeaturesByType("frame").FirstOrDefault(f => f.FeatureId == featureId);
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
                .Where(f => f.Equipped)
                .ToList();
        }

        public bool IsFeaturePurchased(int userId, int featureId)
        {
            return featuresRepository.IsFeaturePurchased(userId, featureId);
        }

        public List<Feature> GetUserFeatures(int userId)
        {
            return featuresRepository.GetUserFeatures(userId);
        }
    }
}