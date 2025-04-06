using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Validators;

namespace BusinessLayer.Services
{
    public class FeaturesService
    {
        private readonly FeaturesRepository featuresRepository;
        public IUserService UserService { get; }

        public FeaturesService(FeaturesRepository featuresRepository, IUserService userService)
        {
            this.featuresRepository = featuresRepository ?? throw new ArgumentNullException(nameof(featuresRepository));
            UserService = userService;
        }

        public Dictionary<string, List<Feature>> GetFeaturesByCategories()
        {
            try
            {
                var userId = UserService.GetCurrentUser().UserId;
                var allFeatures = featuresRepository.GetAllFeatures(userId);
                // Validate all features
                foreach (var feature in allFeatures)
                {
                    var validation = FeaturesValidator.ValidateFeature(feature);
                    if (!validation.isValid)
                    {
                        throw new ValidationException(validation.errorMessage);
                    }
                }

                return allFeatures.GroupBy(f => f.Type)
                                 .ToDictionary(g => g.Key, g => g.ToList());
            }
            catch (DatabaseOperationException ex)
            {
                throw new DatabaseOperationException("Failed to retrieve features by categories.", ex);
            }
        }

        public bool EquipFeature(int userId, int featureId)
        {
            try
            {
                Debug.WriteLine($"FeaturesService.EquipFeature: User {userId}, Feature {featureId}");

                // First check if the feature is purchased
                if (!IsFeaturePurchased(userId, featureId))
                {
                    Debug.WriteLine("Feature is not purchased by this user");
                    return false;
                }

                // Get the feature type to ensure we unequip other features of the same type
                var features = featuresRepository.GetAllFeatures(userId);
                var featureToEquip = features.FirstOrDefault(f => f.FeatureId == featureId);

                if (featureToEquip == null)
                {
                    Debug.WriteLine("Feature not found");
                    return false;
                }

                Debug.WriteLine($"Unequipping features of type: {featureToEquip.Type}");
                // Unequip any existing features of the same type
                bool unequipResult = UnequipFeaturesByType(userId, featureToEquip.Type);
                Debug.WriteLine($"Unequip result: {unequipResult}");

                // Now equip the selected feature
                bool equipResult = featuresRepository.EquipFeature(userId, featureId);
                Debug.WriteLine($"Equip result: {equipResult}");

                return equipResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error equipping feature: {ex.Message}");
                return false;
            }
        }

        public (bool success, string message) UnequipFeature(int userId, int featureId)
        {
            try
            {
                if (featuresRepository.UnequipFeature(userId, featureId))
                {
                    return (true, "Feature unequipped successfully.");
                }
                else
                {
                    return (false, "Failed to unequip feature.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error unequipping feature: {ex.Message}");
                return (false, "An error occurred while unequipping the feature.");
            }
        }

        private bool UnequipFeaturesByType(int userId, string featureType)
        {
            try
            {
                return featuresRepository.UnequipFeaturesByType(userId, featureType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error unequipping features by type: {ex.Message}");
                return false;
            }
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

        public bool IsFeaturePurchased(int userId, int featureId)
        {
            try
            {
                return featuresRepository.IsFeaturePurchased(userId, featureId);
            }
            catch (DatabaseOperationException ex)
            {
                Debug.WriteLine($"Error checking feature purchase: {ex.Message}");
                return false;
            }
        }

        public List<Feature> GetUserEquippedFeatures(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting equipped features for user {userId}");
                var features = featuresRepository.GetUserFeatures(userId);
                var equippedFeatures = features.Where(f => f.Equipped).ToList();
                Debug.WriteLine($"Found {equippedFeatures.Count} equipped features");
                return equippedFeatures;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting equipped features: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return new List<Feature>();
            }
        }

        public FeaturesRepository GetRepository()
        {
            return featuresRepository;
        }
    }
}
