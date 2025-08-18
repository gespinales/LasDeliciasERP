using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class BirdTypesDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        // Obtener todos los tipos de ave
        public List<BirdTypes> GetAll()
        {
            var list = new List<BirdTypes>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT Id, Species, Breed, Description FROM BirdTypes ORDER BY Species, Breed";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BirdTypes
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Species = reader["Species"].ToString(),
                            Breed = reader["Breed"].ToString(),
                            Description = reader["Description"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        // Obtener un tipo de ave por Id
        public BirdTypes GetById(int id)
        {
            BirdTypes birdType = null;

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT Id, Species, Breed, Description FROM BirdTypes WHERE Id=@Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            birdType = new BirdTypes
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Species = reader["Species"].ToString(),
                                Breed = reader["Breed"].ToString(),
                                Description = reader["Description"].ToString()
                            };
                        }
                    }
                }
            }

            return birdType;
        }

        // Insertar un nuevo tipo de ave
        public int Insert(BirdTypes birdType)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    INSERT INTO BirdTypes (Species, Breed, Description)
                    VALUES (@Species, @Breed, @Description);
                    SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Species", birdType.Species);
                    cmd.Parameters.AddWithValue("@Breed", birdType.Breed);
                    cmd.Parameters.AddWithValue("@Description", (object)birdType.Description ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Actualizar un tipo de ave
        public void Update(BirdTypes birdType)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    UPDATE BirdTypes
                    SET Species=@Species, Breed=@Breed, Description=@Description
                    WHERE Id=@Id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", birdType.Id);
                    cmd.Parameters.AddWithValue("@Species", birdType.Species);
                    cmd.Parameters.AddWithValue("@Breed", birdType.Breed);
                    cmd.Parameters.AddWithValue("@Description", (object)birdType.Description ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Eliminar un tipo de ave
        public void Delete(int id)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM BirdTypes WHERE Id=@Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}