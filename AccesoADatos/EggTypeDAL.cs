using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;

namespace LasDeliciasERP.AccesoADatos
{
    public class EggTypeDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<EggType> GetAll()
        {
            var list = new List<EggType>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT Id, Name FROM EggType ORDER BY Name";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new EggType
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name")
                        });
                    }
                }
            }

            return list;
        }

        public EggType GetById(int id)
        {
            EggType type = null;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT Id, Name FROM EggType WHERE Id = @Id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            type = new EggType
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name")
                            };
                        }
                    }
                }
            }

            return type;
        }
    }
}