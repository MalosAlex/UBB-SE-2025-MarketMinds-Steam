using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Validators;

namespace BusinessLayer.Services
{
    public class FeaturesService : IFeaturesService
    {
        private readonly IFeaturesRepository featuresRepository;
        private readonly IUserService userService;

        public FeaturesService(IFeaturesRepository featuresRepository, IUserService userService)
        {
            this.featuresRepository = featuresRepository;
            this.userService = userService;
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
            try
            {
                var features = featuresRepository.GetUserFeatures(userId);
                // Validate all features
                foreach (var feature in features)
                {
                    var validation = FeaturesValidator.ValidateFeature(feature);
                    if (!validation.isValid)
                    {
                        throw new ValidationException(validation.errorMessage);
                    }
                }

                return features;
            }
            catch (DatabaseOperationException ex)
            {
                throw new DatabaseOperationException($"Failed to retrieve features for user {userId}.", ex);
            }
        }

        public FeaturesRepository GetRepository()
        {
            return featuresRepository as FeaturesRepository;
        }
    }
}
