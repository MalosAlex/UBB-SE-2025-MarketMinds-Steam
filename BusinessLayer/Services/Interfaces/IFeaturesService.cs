using System.Collections.Generic;
using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IFeaturesService
    {
        Dictionary<string, List<Feature>> GetFeaturesByCategories();
        bool EquipFeature(int userId, int featureId);
        (bool, string) UnequipFeature(int userId, int featureId);
        List<Feature> GetUserEquippedFeatures(int userId);
        bool IsFeaturePurchased(int userId, int featureId);
        IUserService UserService { get; }
    }
}