using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class FoodDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        // Obtener información general de alimentación
        public FeedingInfo GetFeedingInfo()
        {
            FeedingInfo info = new FeedingInfo();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();

                // Total aves (solo birdtypeid 1 y 2)
                string sqlBirds = "SELECT SUM(Quantity) FROM Population WHERE birdtypeid IN (1,2)";
                int totalBirds = 0;
                using (var cmd = new MySqlCommand(sqlBirds, conn))
                {
                    object result = cmd.ExecuteScalar();
                    totalBirds = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                info.DailyConsumption = totalBirds * 0.265m; // promedio en lbs

                // Última fecha de alimentación para producto 26
                string sqlLast = @"
                    SELECT MAX(FeedingDate)
                    FROM FeedingHistory
                    WHERE ProductId = 26";

                using (var cmd = new MySqlCommand(sqlLast, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                        info.LastFeedingDate = Convert.ToDateTime(result);
                }
            }

            return info;
        }

        // Registrar alimentación diaria
        public bool RegisterDailyFeeding(decimal dailyConsumption)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();

                // Verificar si ya se registró hoy
                string sqlCheck = @"
                    SELECT COUNT(*) 
                    FROM FeedingHistory
                    WHERE ProductId = 26
                    AND FeedingDate = CURDATE()";

                using (var cmdCheck = new MySqlCommand(sqlCheck, conn))
                {
                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                    if (count > 0)
                        return false; // Ya se registró hoy
                }

                // Descontar inventario del producto 26
                string sqlUpdate = @"
                    UPDATE Inventory
                    SET Quantity = GREATEST(0, Quantity - @consumption),
                        LastUpdated = NOW()
                    WHERE ProductId = 26";

                using (var cmd = new MySqlCommand(sqlUpdate, conn))
                {
                    cmd.Parameters.AddWithValue("@consumption", dailyConsumption);
                    cmd.ExecuteNonQuery();
                }

                // Guardar en historial
                string sqlInsert = @"
                    INSERT INTO FeedingHistory (ProductId, FeedingDate, Quantity)
                    VALUES (26, CURDATE(), @qty)";

                using (var cmd = new MySqlCommand(sqlInsert, conn))
                {
                    cmd.Parameters.AddWithValue("@qty", dailyConsumption);
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
        }

        // Obtener historial de alimentación
        public List<FeedingRecord> GetHistory(int limit = 30)
        {
            List<FeedingRecord> list = new List<FeedingRecord>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();

                string sql = @"
                    SELECT Id, FeedingDate, Quantity
                    FROM FeedingHistory
                    WHERE ProductId = 26
                    ORDER BY FeedingDate DESC
                    LIMIT @limit";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@limit", limit);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new FeedingRecord
                            {
                                Id = reader.GetInt32("Id"),
                                FeedingDate = reader.GetDateTime("FeedingDate"),
                                Quantity = reader.GetDecimal("Quantity")
                            });
                        }
                    }
                }
            }

            // Orden ascendente para gráficas
            list.Sort((a, b) => a.FeedingDate.CompareTo(b.FeedingDate));

            return list;
        }
    }
}
