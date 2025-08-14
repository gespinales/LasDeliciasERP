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
                string query = @"
                    SELECT ep.*, et.Name AS EggTypeName, b.Name AS BarnName
                    FROM EggProduction ep
                    JOIN EggType et ON ep.EggTypeId = et.Id
                    JOIN Barn b ON ep.BarnId = b.Id
                    ORDER BY ep.Date DESC";

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
                            Notes = reader.GetString("Notes"),
                            EggTypeId = reader.GetInt32("EggTypeId"),
                            EggTypeName = reader.GetString("EggTypeName"),
                            BarnId = reader.GetInt32("BarnId"),
                            BarnName = reader.GetString("BarnName")
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
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes"),
                            EggTypeId = reader.GetInt32("EggTypeId"),
                            BarnId = reader.GetInt32("BarnId")
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
                string query = @"INSERT INTO EggProduction 
                                 (Date, QuantityS, QuantityM, QuantityL, QuantityXL, Notes, EggTypeId, BarnId)
                                 VALUES (@Date, @S, @M, @L, @XL, @Notes, @EggTypeId, @BarnId)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Date", ep.Date);
                cmd.Parameters.AddWithValue("@S", ep.QuantityS);
                cmd.Parameters.AddWithValue("@M", ep.QuantityM);
                cmd.Parameters.AddWithValue("@L", ep.QuantityL);
                cmd.Parameters.AddWithValue("@XL", ep.QuantityXL);
                cmd.Parameters.AddWithValue("@Notes", ep.Notes);
                cmd.Parameters.AddWithValue("@EggTypeId", ep.EggTypeId);
                cmd.Parameters.AddWithValue("@BarnId", ep.BarnId);
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
                        Notes = @Notes,
                        EggTypeId = @EggTypeId,
                        BarnId = @BarnId
                    WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Date", ep.Date);
                cmd.Parameters.AddWithValue("@S", ep.QuantityS);
                cmd.Parameters.AddWithValue("@M", ep.QuantityM);
                cmd.Parameters.AddWithValue("@L", ep.QuantityL);
                cmd.Parameters.AddWithValue("@XL", ep.QuantityXL);
                cmd.Parameters.AddWithValue("@Notes", ep.Notes);
                cmd.Parameters.AddWithValue("@EggTypeId", ep.EggTypeId);
                cmd.Parameters.AddWithValue("@BarnId", ep.BarnId);
                cmd.Parameters.AddWithValue("@Id", ep.Id);

                cmd.ExecuteNonQuery();
            }
        }
    }
}