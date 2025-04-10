using System;
using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class FeaturesValidator
    {
        // === Constants ===

        // Numeric constants
        private const int MinValidId = 1;
        private const int MinFeatureValue = 0;

        // Error messages
        private const string ErrorFeatureNull = "Feature cannot be null.";
        private const string ErrorFeatureNameEmpty = "Feature name cannot be empty.";
        private const string ErrorFeatureTypeEmpty = "Feature type cannot be empty.";
        private const string ErrorFeatureValueNegative = "Feature value cannot be negative.";
        private const string ErrorInvalidFeatureType = "Invalid feature type.";
        private const string ErrorInvalidUserId = "Invalid user ID.";
        private const string ErrorInvalidFeatureId = "Invalid feature ID.";
        private const string ErrorFeatureNotPurchased = "Feature is not purchased by the user.";

        // Valid types
        private static readonly string[] ValidFeatureTypes = { "frame", "emoji", "background", "pet", "hat" };

        // === Validation Methods ===
        public static (bool isValid, string errorMessage) ValidateFeature(Feature feature)
        {
            if (feature == null)
            {
                return (false, ErrorFeatureNull);
            }

            if (string.IsNullOrWhiteSpace(feature.Name))
            {
                return (false, ErrorFeatureNameEmpty);
            }

            if (string.IsNullOrWhiteSpace(feature.Type))
            {
                return (false, ErrorFeatureTypeEmpty);
            }

            if (feature.Value < MinFeatureValue)
            {
                return (false, ErrorFeatureValueNegative);
            }

            return (true, string.Empty);
        }

        public static (bool isValid, string errorMessage) ValidateFeatureType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return (false, ErrorFeatureTypeEmpty);
            }

            if (!Array.Exists(ValidFeatureTypes, currentType => currentType.Equals(type, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, ErrorInvalidFeatureType);
            }

            return (true, string.Empty);
        }

        public static (bool isValid, string errorMessage) ValidateFeatureEquip(int userId, int featureId, bool isPurchased)
        {
            if (userId < MinValidId)
            {
                return (false, ErrorInvalidUserId);
            }

            if (featureId < MinValidId)
            {
                return (false, ErrorInvalidFeatureId);
            }

            if (!isPurchased)
            {
                return (false, ErrorFeatureNotPurchased);
            }

            return (true, string.Empty);
        }
    }
}
