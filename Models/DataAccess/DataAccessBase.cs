using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CLIP.Models.DataAccess
{
    public class DataAccessBase
    {
        protected string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
        }

        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        protected void ExecuteNonQuery(string commandText, Dictionary<string, object> parameters = null)
        {
            using (var connection = CreateConnection())
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        protected T ExecuteScalar<T>(string commandText, Dictionary<string, object> parameters = null)
        {
            using (var connection = CreateConnection())
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
        }

        protected List<T> ExecuteReader<T>(string commandText, Func<IDataReader, T> mapFunc, Dictionary<string, object> parameters = null)
        {
            var items = new List<T>();

            using (var connection = CreateConnection())
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(mapFunc(reader));
                        }
                    }
                }
            }

            return items;
        }

        protected T ExecuteSingleReader<T>(string commandText, Func<IDataReader, T> mapFunc, Dictionary<string, object> parameters = null)
        {
            using (var connection = CreateConnection())
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return mapFunc(reader);
                        }
                        return default(T);
                    }
                }
            }
        }
    }
} 