using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;

namespace LasDeliciasERP.AccesoADatos
{
    public class UnitTypeDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<UnitType> GetAll()
        {
            var list = new List<UnitType>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT Id, Name, Abbreviation FROM UnitTypes ORDER BY Name";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new UnitType
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Abbreviation = reader["Abbreviation"]?.ToString()
                        });
                    }
                }
            }

            return list;
        }
    }
}