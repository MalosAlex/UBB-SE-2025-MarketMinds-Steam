﻿using System.Data;
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
            catch (DatabaseOperationException ex)
            {
                throw new DatabaseOperationException("Failed to retrieve features.", ex);
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

                foreach (DataRow row in dataTable.Rows)
                {
                    features.Add(new Feature
                    {
                        FeatureId = Convert.ToInt32(row["feature_id"]),
                        Name = row["name"].ToString(),
                        Value = Convert.ToInt32(row["value"]),
                        Description = row["description"].ToString(),
                        Type = row["type"].ToString(),
                        Source = row["source"].ToString()
                    });
                }

                return features;
            }
            catch (DatabaseOperationException ex)
            {
                throw new DatabaseOperationException($"Failed to retrieve features of type {type}.", ex);
            }
        }

        public List<Feature> GetUserFeatures(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId)
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
            catch (DatabaseOperationException ex)
            {
                throw new DatabaseOperationException($"Failed to retrieve features for user {userId}.", ex);
            }
        }

        public bool EquipFeature(int userId, int featureId)
        {
            try
            {
                // Check if the relationship exists
                var checkParameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@featureId", featureId)
                };

                var relationshipTable = dataLink.ExecuteReader("GetFeatureUserRelationship", checkParameters);

                if (relationshipTable.Rows.Count > 0)
                {
                    // Update existing relationship
                    var updateParameters = new SqlParameter[]
                    {
                        new SqlParameter("@userId", userId),
                        new SqlParameter("@featureId", featureId),
                        new SqlParameter("@equipped", 1)
                    };

                    dataLink.ExecuteNonQuery("UpdateFeatureUserEquipStatus", updateParameters);
                    return true;
                }

                return false; // Feature is not purchased
            }
            catch (DatabaseOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to equip feature {featureId} for user {userId}: {ex.Message}");
                return false;
            }
        }

        public bool UnequipFeature(int userId, int featureId)
        {
            try
            {
                // Check if the relationship exists
                var checkParameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@featureId", featureId)
                };

                var relationshipTable = dataLink.ExecuteReader("GetFeatureUserRelationship", checkParameters);

                if (relationshipTable.Rows.Count > 0)
                {
                    var parameters = new SqlParameter[]
                    {
                        new SqlParameter("@userId", userId),
                        new SqlParameter("@featureId", featureId),
                        new SqlParameter("@equipped", 0)
                    };

                    dataLink.ExecuteNonQuery("UpdateFeatureUserEquipStatus", parameters);
                    return true;
                }

                return false; // Feature is not purchased
            }
            catch (DatabaseOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to unequip feature {featureId} for user {userId}: {ex.Message}");
                return false;
            }
        }

        public bool UnequipFeaturesByType(int userId, string featureType)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@featureType", featureType)
                };

                dataLink.ExecuteNonQuery("UnequipFeaturesByType", parameters);
                return true;  // If no exception, consider it successful
            }
            catch (DatabaseOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to unequip features of type {featureType} for user {userId}: {ex.Message}");
                return false;
            }
        }

        public bool IsFeaturePurchased(int userId, int featureId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@featureId", featureId)
                };

                var relationshipTable = dataLink.ExecuteReader("GetFeatureUserRelationship", parameters);
                return relationshipTable.Rows.Count > 0;
            }
            catch (DatabaseOperationException ex)
            {
                throw new DatabaseOperationException($"Failed to check feature purchase status.", ex);
            }
        }
    }
}
