using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using LasDeliciasERP.Models;

namespace LasDeliciasERP.AccesoADatos
{
    public class EggProductionDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<EggProduction> GetAll()
        {
            List<EggProduction> list = new List<EggProduction>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM EggProduction ORDER BY Date DESC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new EggProduction
                        {
                            Id = reader.GetInt32("Id"),
                            Date = reader.GetDateTime("Date"),
                            QuantityS = reader.GetInt32("QuantityS"),
                            QuantityM = reader.GetInt32("QuantityM"),
                            QuantityL = reader.GetInt32("QuantityL"),
                            QuantityXL = reader.GetInt32("QuantityXL"),
                            Notes = reader.GetString("Notes")
                        });
                    }
                }
            }

            return list;
        }

        public EggProduction GetById(int id)
        {
            EggProduction ep = null;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM EggProduction WHERE Id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ep = new EggProduction
                        {
                            Id = reader.GetInt32("Id"),
                            Date = reader.GetDateTime("Date"),
                            QuantityS = reader.GetInt32("QuantityS"),
                            QuantityM = reader.GetInt32("QuantityM"),
                            QuantityL = reader.GetInt32("QuantityL"),
                            QuantityXL = reader.GetInt32("QuantityXL"),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes")
                        };
                    }
                }
            }
            return ep;
        }

        public void Insert(EggProduction ep)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"INSERT INTO EggProduction (Date, QuantityS, QuantityM, QuantityL, QuantityXL, Notes)
                                 VALUES (@Date, @S, @M, @L, @XL, @Notes)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Date", ep.Date);
                cmd.Parameters.AddWithValue("@S", ep.QuantityS);
                cmd.Parameters.AddWithValue("@M", ep.QuantityM);
                cmd.Parameters.AddWithValue("@L", ep.QuantityL);
                cmd.Parameters.AddWithValue("@XL", ep.QuantityXL);
                cmd.Parameters.AddWithValue("@Notes", ep.Notes);
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(EggProduction ep)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    UPDATE EggProduction SET 
                        Date = @Date,
                        QuantityS = @S,
                        QuantityM = @M,
                        QuantityL = @L,
                        QuantityXL = @XL,
                        Notes = @Notes
                    WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Date", ep.Date);
                cmd.Parameters.AddWithValue("@S", ep.QuantityS);
                cmd.Parameters.AddWithValue("@M", ep.QuantityM);
                cmd.Parameters.AddWithValue("@L", ep.QuantityL);
                cmd.Parameters.AddWithValue("@XL", ep.QuantityXL);
                cmd.Parameters.AddWithValue("@Notes", ep.Notes);
                cmd.Parameters.AddWithValue("@Id", ep.Id);

                cmd.ExecuteNonQuery();
            }
        }
    }
}