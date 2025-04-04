using BusinessLayer.Data;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BusinessLayer.Services
{
    public class FeaturesService
    {
        private readonly FeaturesRepository _featuresRepository;
        public UserService UserService { get; }

        public FeaturesService(FeaturesRepository featuresRepository, UserService userService)
        {
            _featuresRepository = featuresRepository ?? throw new ArgumentNullException(nameof(featuresRepository));
            UserService = userService;
        }

        public Dictionary<string, List<Feature>> GetFeaturesByCategories()
        {
            try
            {
                var userId = UserService.GetCurrentUser().UserId;
                var allFeatures = _featuresRepository.GetAllFeatures(userId);
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
                var features = _featuresRepository.GetAllFeatures(userId);
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
                bool equipResult = _featuresRepository.EquipFeature(userId, featureId);
                Debug.WriteLine($"Equip result: {equipResult}");
                
                return equipResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error equipping feature: {ex.Message}");
                return false;
            }
        }

        public bool UnequipFeature(int userId, int featureId)
        {
            try
            {
                return _featuresRepository.UnequipFeature(userId, featureId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error unequipping feature: {ex.Message}");
                return false;
            }
        }

        private bool UnequipFeaturesByType(int userId, string featureType)
        {
            try
            {
                return _featuresRepository.UnequipFeaturesByType(userId, featureType);
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
                return _featuresRepository.GetUserFeatures(userId);
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
                return _featuresRepository.IsFeaturePurchased(userId, featureId);
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
                var features = _featuresRepository.GetUserFeatures(userId);
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
    }

}
