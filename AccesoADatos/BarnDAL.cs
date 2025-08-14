using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;

namespace LasDeliciasERP.AccesoADatos
{
    public class BarnDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<Barn> GetAll()
        {
            List<Barn> list = new List<Barn>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM Barn ORDER BY Name ASC";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Barn
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Length = reader.GetDecimal("Length"),
                            Width = reader.GetDecimal("Width"),
                            Height = reader.GetDecimal("Height"),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes")
                        });
                    }
                }
            }

            return list;
        }

        public Barn GetById(int id)
        {
            Barn barn = null;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM Barn WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        barn = new Barn
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Length = reader.GetDecimal("Length"),
                            Width = reader.GetDecimal("Width"),
                            Height = reader.GetDecimal("Height"),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes")
                        };
                    }
                }
            }

            return barn;
        }

        public void Insert(Barn barn)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"INSERT INTO Barn (Name, Length, Width, Height, Notes)
                                 VALUES (@Name, @Length, @Width, @Height, @Notes)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", barn.Name);
                cmd.Parameters.AddWithValue("@Length", barn.Length);
                cmd.Parameters.AddWithValue("@Width", barn.Width);
                cmd.Parameters.AddWithValue("@Height", barn.Height);
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(barn.Notes) ? (object)DBNull.Value : barn.Notes);

                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Barn barn)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"UPDATE Barn
                                 SET Name = @Name,
                                     Length = @Length,
                                     Width = @Width,
                                     Height = @Height,
                                     Notes = @Notes
                                 WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", barn.Name);
                cmd.Parameters.AddWithValue("@Length", barn.Length);
                cmd.Parameters.AddWithValue("@Width", barn.Width);
                cmd.Parameters.AddWithValue("@Height", barn.Height);
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(barn.Notes) ? (object)DBNull.Value : barn.Notes);
                cmd.Parameters.AddWithValue("@Id", barn.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM Barn WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                cmd.ExecuteNonQuery();
            }
        }
    }
}