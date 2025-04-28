using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CLIP.Models.DataAccess
{
    public class PlantDataAccess : DataAccessBase
    {
        public List<Plant> GetAllPlants()
        {
            const string sql = @"
                SELECT Id, PlantName
                FROM Plants
                ORDER BY PlantName";

            return ExecuteReader(sql, reader => new Plant
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                PlantName = reader.GetString(reader.GetOrdinal("PlantName"))
            });
        }

        public Plant GetPlantById(int id)
        {
            const string sql = @"
                SELECT Id, PlantName
                FROM Plants
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            return ExecuteSingleReader(sql, reader => new Plant
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                PlantName = reader.GetString(reader.GetOrdinal("PlantName"))
            }, parameters);
        }

        public int CreatePlant(Plant plant)
        {
            const string sql = @"
                INSERT INTO Plants (PlantName)
                VALUES (@PlantName);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@PlantName", plant.PlantName }
            };

            return ExecuteScalar<int>(sql, parameters);
        }

        public void UpdatePlant(Plant plant)
        {
            const string sql = @"
                UPDATE Plants
                SET PlantName = @PlantName
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", plant.Id },
                { "@PlantName", plant.PlantName }
            };

            ExecuteNonQuery(sql, parameters);
        }

        public void DeletePlant(int id)
        {
            const string sql = @"
                DELETE FROM Plants
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };

            ExecuteNonQuery(sql, parameters);
        }
    }
} 