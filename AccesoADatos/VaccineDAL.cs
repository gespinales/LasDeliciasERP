using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class VaccineDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<Vaccine> GetAll()
        {
            List<Vaccine> list = new List<Vaccine>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM Vaccine ORDER BY Name ASC";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Vaccine
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                            RecommendedDose = reader.IsDBNull(reader.GetOrdinal("RecommendedDose")) ? "" : reader.GetString("RecommendedDose"),
                            IntervalDays = reader.IsDBNull(reader.GetOrdinal("IntervalDays")) ? 0 : reader.GetInt32("IntervalDays"),
                            AdministrationRoute = reader.IsDBNull(reader.GetOrdinal("AdministrationRoute")) ? "" : reader.GetString("AdministrationRoute")
                        });
                    }
                }
            }

            return list;
        }

        public Vaccine GetById(int id)
        {
            Vaccine vaccine = null;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM Vaccine WHERE Id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        vaccine = new Vaccine
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                            RecommendedDose = reader.IsDBNull(reader.GetOrdinal("RecommendedDose")) ? "" : reader.GetString("RecommendedDose"),
                            IntervalDays = reader.IsDBNull(reader.GetOrdinal("IntervalDays")) ? 0 : reader.GetInt32("IntervalDays"),
                            AdministrationRoute = reader.IsDBNull(reader.GetOrdinal("AdministrationRoute")) ? "" : reader.GetString("AdministrationRoute")
                        };
                    }
                }
            }

            return vaccine;
        }
    }
}
