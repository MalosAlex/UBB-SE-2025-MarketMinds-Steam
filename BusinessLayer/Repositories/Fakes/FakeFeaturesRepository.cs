using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakeFeaturesRepository : IFeaturesRepository
    {
        private readonly List<Feature> features = new();
        private readonly List<Feature> userFeatures = new();

        public FakeFeaturesRepository()
        {
            // Initialize available features
            features.AddRange(new[]
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Value = 100, Description = "A premium gold frame" },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Value = 50, Description = "A nice silver frame" },
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Value = 30, Description = "Express happiness" },
                new Feature { FeatureId = 4, Name = "Sad Emoji", Type = "emoji", Value = 20, Description = "Express sadness" }
            });
            // Initialize purchased features for user 1
            userFeatures.AddRange(new[]
            {
                new Feature { FeatureId = 1, UserId = 1, Name = "Gold Frame", Type = "frame", Value = 100, Equipped = true, Description = "A premium gold frame" },
                new Feature { FeatureId = 3, UserId = 1, Name = "Happy Emoji", Type = "emoji", Value = 30, Equipped = false, Description = "Express happiness" }
            });
        }

        public List<Feature> GetAllFeatures(int userId)
        {
            return features;
        }

        public List<Feature> GetFeaturesByType(string type)
        {
            return features.Where(feature => feature.Type == type).ToList();
        }

        public List<Feature> GetUserFeatures(int userId)
        {
            return userFeatures.Where(feature => feature.UserId == userId).ToList();
        }

        public bool IsFeaturePurchased(int userId, int featureId)
        {
            return userFeatures.Any(feature => feature.UserId == userId && feature.FeatureId == featureId);
        }

        public bool EquipFeature(int userId, int featureId)
        {
            var feature = userFeatures.FirstOrDefault(feature => feature.UserId == userId && feature.FeatureId == featureId);
            if (feature == null)
            {
                return false;
            }
            feature.Equipped = true;
            return true;
        }

        public bool UnequipFeature(int userId, int featureId)
        {
            var feature = userFeatures.FirstOrDefault(feature => feature.UserId == userId && feature.FeatureId == featureId);
            if (feature == null)
            {
                return false;
            }
            feature.Equipped = false;
            return true;
        }

        public bool UnequipFeaturesByType(int userId, string featureType)
        {
            var featuresToUnequip = userFeatures
                .Where(feature => feature.UserId == userId && feature.Type == featureType)
                .ToList();
            foreach (var feature in featuresToUnequip)
            {
                feature.Equipped = false;
            }
            return true;
        }

        public void AddFeature(Feature feature)
        {
            features.Add(feature);
        }

        public void PurchaseFeature(int userId, int featureId)
        {
            if (!IsFeaturePurchased(userId, featureId))
            {
                var featureToPurchase = features.FirstOrDefault(feature => feature.FeatureId == featureId);
                if (featureToPurchase != null)
                {
                    userFeatures.Add(new Feature
                    {
                        FeatureId = featureToPurchase.FeatureId,
                        UserId = userId,
                        Name = featureToPurchase.Name,
                        Type = featureToPurchase.Type,
                        Value = featureToPurchase.Value,
                        Description = featureToPurchase.Description,
                        Equipped = false
                    });
                }
            }
        }
        public bool AddUserFeature(int userId, int featureId)
        {
            if (IsFeaturePurchased(userId, featureId))
            {
                return false;
            }

            var feature = features.FirstOrDefault(feature => feature.FeatureId == featureId);
            if (feature == null)
            {
                return false;
            }

            var userFeature = new Feature
            {
                FeatureId = feature.FeatureId,
                UserId = userId,
                Name = feature.Name,
                Value = feature.Value,
                Description = feature.Description,
                Type = feature.Type,
                Source = feature.Source,
                Equipped = false
            };

            userFeatures.Add(userFeature);
            return true;
        }
    }
}