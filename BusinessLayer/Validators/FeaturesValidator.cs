using System;
using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class FeaturesValidator
    {
        // === Constants ===
        private const int MinValidIdentifier = 1;
        private const int MinFeatureValue = 0;

        // Error messages
        private const string NullFeatureMessage = "Feature cannot be null.";
        private const string EmptyFeatureNameMessage = "Feature name cannot be empty.";
        private const string EmptyFeatureTypeMessage = "Feature type cannot be empty.";
        private const string NegativeFeatureValueMessage = "Feature value cannot be negative.";
        private const string InvalidFeatureTypeMessage = "Invalid feature type.";
        private const string InvalidUserIdentifierMessage = "Invalid user ID.";
        private const string InvalidFeatureIdentifierMessage = "Invalid feature ID.";
        private const string FeatureNotPurchasedMessage = "Feature is not purchased by the user.";

        // Valid types
        private static readonly string[] ValidFeatureTypes = { "frame", "emoji", "background", "pet", "hat" };

        // === Validation Methods ===
        public static (bool isValid, string errorMessage) ValidateFeature(Feature feature)
        {
            if (feature == null)
            {
                return (false, NullFeatureMessage);
            }

            if (string.IsNullOrWhiteSpace(feature.Name))
            {
                return (false, EmptyFeatureNameMessage);
            }

            if (string.IsNullOrWhiteSpace(feature.Type))
            {
                return (false, EmptyFeatureTypeMessage);
            }

            if (feature.Value < MinFeatureValue)
            {
                return (false, NegativeFeatureValueMessage);
            }

            return (true, string.Empty);
        }

        public static (bool isValid, string errorMessage) ValidateFeatureType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return (false, EmptyFeatureTypeMessage);
            }

            if (!Array.Exists(ValidFeatureTypes, t => t.Equals(type, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, InvalidFeatureTypeMessage);
            }

            return (true, string.Empty);
        }

        public static (bool isValid, string errorMessage) ValidateFeatureEquip(int userId, int featureId, bool isPurchased)
        {
            if (userId < MinValidIdentifier)
            {
                return (false, InvalidUserIdentifierMessage);
            }

            if (featureId < MinValidIdentifier)
            {
                return (false, InvalidFeatureIdentifierMessage);
            }

            if (!isPurchased)
            {
                return (false, FeatureNotPurchasedMessage);
            }

            return (true, string.Empty);
        }
    }
}
