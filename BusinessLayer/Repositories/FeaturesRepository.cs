using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class FeaturesRepository : IFeaturesRepository
    {
        private readonly IDataLink dataLink;

        public FeaturesRepository(IDataLink datalink)
        {
            dataLink = datalink ?? throw new ArgumentNullException(nameof(datalink));
        }

        public List<Feature> GetAllFeatures(int userId)
        {
            try
            {
                var features = new List<Feature>();
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@userId", userId)
                };

                var dataTable = dataLink.ExecuteReader("GetAllFeatures", parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    features.Add(new Feature
                    {
                        FeatureId = Convert.ToInt32(row["feature_id"]),
                        Name = row["name"].ToString(),
                        Value = Convert.ToInt32(row["value"]),
                        Description = row["description"].ToString(),
                        Type = row["type"].ToString(),
                        Source = row["source"].ToString(),
                        Equipped = Convert.ToBoolean(row["equipped"])
                    });
                }

                return features;
            }
            catch (DatabaseOperationException exception)
            {
                throw new DatabaseOperationException("Failed to retrieve features.", exception);
            }
        }

        public List<Feature> GetFeaturesByType(string type)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@type", type)
                };

                var features = new List<Feature>();
                var dataTable = dataLink.ExecuteReader("GetFeaturesByType", parameters);

                const string featureIdentifierString = "feature_id";
                const string nameString = "name";
                const string valueString = "value";
                const string descriptionString = "description";
                const string typeString = "type";
                const string sourceString = "source";

                foreach (DataRow row in dataTable.Rows)
                {
                    features.Add(new Feature
                    {
                        FeatureId = Convert.ToInt32(row[featureIdentifierString]),
                        Name = row[nameString].ToString(),
                        Value = Convert.ToInt32(row[valueString]),
                        Description = row[descriptionString].ToString(),
                        Type = row[typeString].ToString(),
                        Source = row[sourceString].ToString()
                    });
                }

                return features;
            }
            catch (DatabaseOperationException exception)
            {
                throw new DatabaseOperationException($"Failed to retrieve features of type {type}.", exception);
            }
        }

        public List<Feature> GetUserFeatures(int userIdentifier)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userIdentifier", userIdentifier)
                };

                var features = new List<Feature>();
                var dataTable = dataLink.ExecuteReader("GetUserFeatures", parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    features.Add(new Feature
                    {
                        FeatureId = Convert.ToInt32(row["feature_id"]),
                        Name = row["name"].ToString(),
                        Value = Convert.ToInt32(row["value"]),
                        Description = row["description"].ToString(),
                        Type = row["type"].ToString(),
                        Source = row["source"].ToString(),
                        Equipped = Convert.ToBoolean(row["equipped"])
                    });
                }

                return features;
            }
            catch (DatabaseOperationException exception)
            {
                throw new DatabaseOperationException($"Failed to retrieve features for user {userIdentifier}.", exception);
            }
        }

        public bool EquipFeature(int userIdentifier, int featureIdentifier)
        {
            try
            {
                // Check if the relationship exists
                var checkParameters = new SqlParameter[]
                {
                    new SqlParameter("@userIdentifier", userIdentifier),
                    new SqlParameter("@featureIdentifier", featureIdentifier)
                };

                var relationshipTable = dataLink.ExecuteReader("GetFeatureUserRelationship", checkParameters);

                if (relationshipTable.Rows.Count > 0)
                {
                    // Update existing relationship
                    var updateParameters = new SqlParameter[]
                    {
                        new SqlParameter("@userId", userIdentifier),
                        new SqlParameter("@featureId", featureIdentifier),
                        new SqlParameter("@equipped", 1)
                    };

                    dataLink.ExecuteNonQuery("UpdateFeatureUserEquipStatus", updateParameters);
                    return true;
                }

                return false; // Feature is not purchased
            }
            catch (DatabaseOperationException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to equip feature {featureIdentifier} for user {userIdentifier}: {exception.Message}");
                return false;
            }
        }

        public bool UnequipFeature(int userIdentifier, int featureIdentifier)
        {
            try
            {
                // Check if the relationship exists
                var checkParameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userIdentifier),
                    new SqlParameter("@featureId", featureIdentifier)
                };

                var relationshipTable = dataLink.ExecuteReader("GetFeatureUserRelationship", checkParameters);

                if (relationshipTable.Rows.Count > 0)
                {
                    var parameters = new SqlParameter[]
                    {
                        new SqlParameter("@userId", userIdentifier),
                        new SqlParameter("@featureId", featureIdentifier),
                        new SqlParameter("@equipped", 0)
                    };

                    dataLink.ExecuteNonQuery("UpdateFeatureUserEquipStatus", parameters);
                    return true;
                }

                return false; // Feature is not purchased
            }
            catch (DatabaseOperationException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to unequip feature {featureIdentifier} for user {userId}: {exception.Message}");
                return false;
            }
        }

        public bool UnequipFeaturesByType(int userIdentifier, string featureType)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userIdentifier),
                    new SqlParameter("@featureType", featureType)
                };

                dataLink.ExecuteNonQuery("UnequipFeaturesByType", parameters);
                return true;
            }
            catch (DatabaseOperationException)
            {
                return false;
            }
        }

        public bool IsFeaturePurchased(int userIdentifier, int featureIdentifier)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userIdentifier),
                    new SqlParameter("@featureId", featureIdentifier)
                };

                var relationshipTable = dataLink.ExecuteReader("GetFeatureUserRelationship", parameters);
                return relationshipTable.Rows.Count > 0;
            }
            catch (DatabaseOperationException exception)
            {
                throw new DatabaseOperationException($"Failed to check feature purchase status.", exception);
            }
        }

        public bool AddUserFeature(int userIdentifier, int featureIdentifier)
        {
            try
            {
                // First check if the user already has this feature
                if (IsFeaturePurchased(userIdentifier, featureIdentifier))
                {
                    return false;
                }

                // Add the feature to the user's purchased features
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userIdentifier),
                    new SqlParameter("@featureId", featureId),
                    new SqlParameter("@equipped", false) // New features are unequipped by default
                };

                dataLink.ExecuteNonQuery("AddUserFeature", parameters);
                return true;
            }
            catch (DatabaseOperationException)
            {
                return false;
            }
        }
    }
}
