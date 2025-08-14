using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;

namespace LasDeliciasERP.AccesoADatos
{
    public class EggInventoryDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<EggInventory> GetAll()
        {
            List<EggInventory> list = new List<EggInventory>();
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM EggInventory ORDER BY EggTypeId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new EggInventory
                        {
                            EggTypeId = reader.GetInt32("EggTypeId"),
                            QuantityS = reader.GetInt32("QuantityS"),
                            QuantityM = reader.GetInt32("QuantityM"),
                            QuantityL = reader.GetInt32("QuantityL"),
                            QuantityXL = reader.GetInt32("QuantityXL")
                        });
                    }
                }
            }
            return list;
        }
    }
}