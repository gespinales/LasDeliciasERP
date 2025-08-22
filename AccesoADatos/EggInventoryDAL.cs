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

                string query = @"
SELECT
    p.EggTypeId,
    COALESCE(et.Name, 'Sin tipo') AS EggTypeName,
    SUM(CASE WHEN es.Name = 'S'  THEN ei.Quantity ELSE 0 END) AS QuantityS,
    SUM(CASE WHEN es.Name = 'M'  THEN ei.Quantity ELSE 0 END) AS QuantityM,
    SUM(CASE WHEN es.Name = 'L'  THEN ei.Quantity ELSE 0 END) AS QuantityL,
    SUM(CASE WHEN es.Name = 'XL' THEN ei.Quantity ELSE 0 END) AS QuantityXL,
    SUM(ei.Quantity) AS TotalQuantity
FROM EggInventory ei
JOIN Products p   ON p.Id = ei.ProductId
LEFT JOIN EggType et ON et.Id = p.EggTypeId
LEFT JOIN EggSize es ON es.Id = p.EggSizeId
GROUP BY COALESCE(et.Name, 'Sin tipo')
ORDER BY EggTypeName;
";

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