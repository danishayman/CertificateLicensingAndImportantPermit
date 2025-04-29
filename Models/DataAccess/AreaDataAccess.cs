using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CLIP.Models.DataAccess
{
    public class AreaDataAccess : DataAccessBase
    {
        public List<AreaPlant> GetAllAreas()
        {
            const string sql = @"
                SELECT a.Id, a.AreaName, a.PlantId, p.PlantName
                FROM AreaPlants a
                INNER JOIN Plants p ON a.PlantId = p.Id
                ORDER BY p.PlantName, a.AreaName";

            return ExecuteReader(sql, reader => new AreaPlant
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                AreaName = reader.GetString(reader.GetOrdinal("AreaName")),
                PlantId = reader.GetInt32(reader.GetOrdinal("PlantId")),
                Plant = new Plant
                {
                    Id = reader.GetInt32(reader.GetOrdinal("PlantId")),
                    PlantName = reader.GetString(reader.GetOrdinal("PlantName"))
                }
            });
        }

        public List<AreaPlant> GetAreasByPlant(int plantId)
        {
            const string sql = @"
                SELECT a.Id, a.AreaName, a.PlantId, p.PlantName
                FROM AreaPlants a
                INNER JOIN Plants p ON a.PlantId = p.Id
                WHERE a.PlantId = @PlantId
                ORDER BY a.AreaName";

            var parameters = new Dictionary<string, object>
            {
                { "@PlantId", plantId }
            };

            return ExecuteReader(sql, reader => new AreaPlant
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                AreaName = reader.GetString(reader.GetOrdinal("AreaName")),
                PlantId = reader.GetInt32(reader.GetOrdinal("PlantId")),
                Plant = new Plant
                {
                    Id = reader.GetInt32(reader.GetOrdinal("PlantId")),
                    PlantName = reader.GetString(reader.GetOrdinal("PlantName"))
                }
            }, parameters);
        }

        public AreaPlant GetAreaById(int id)
        {
            const string sql = @"
                SELECT a.Id, a.AreaName, a.PlantId, p.PlantName
                FROM AreaPlants a
                INNER JOIN Plants p ON a.PlantId = p.Id
                WHERE a.Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            return ExecuteSingleReader(sql, reader => new AreaPlant
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                AreaName = reader.GetString(reader.GetOrdinal("AreaName")),
                PlantId = reader.GetInt32(reader.GetOrdinal("PlantId")),
                Plant = new Plant
                {
                    Id = reader.GetInt32(reader.GetOrdinal("PlantId")),
                    PlantName = reader.GetString(reader.GetOrdinal("PlantName"))
                }
            }, parameters);
        }

        public int CreateArea(AreaPlant area)
        {
            const string sql = @"
                INSERT INTO AreaPlants (AreaName, PlantId)
                VALUES (@AreaName, @PlantId);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@AreaName", area.AreaName },
                { "@PlantId", area.PlantId }
            };

            return ExecuteScalar<int>(sql, parameters);
        }

        public void UpdateArea(AreaPlant area)
        {
            const string sql = @"
                UPDATE AreaPlants
                SET AreaName = @AreaName, PlantId = @PlantId
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", area.Id },
                { "@AreaName", area.AreaName },
                { "@PlantId", area.PlantId }
            };

            ExecuteNonQuery(sql, parameters);
        }

        public void DeleteArea(int id)
        {
            const string sql = @"
                DELETE FROM AreaPlants
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            ExecuteNonQuery(sql, parameters);
        }
    }
} 