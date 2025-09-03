using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class FoodProjectionDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public FoodProjection GetProjection(decimal consumoPorAveKg = 0.12m)
        {
            var projection = new FoodProjection();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();

                // Total aves
                string sqlBirds = "SELECT SUM(Quantity) FROM Population where birdtypeid in (1,2)";
                using (var cmd = new MySqlCommand(sqlBirds, conn))
                {
                    object r1 = cmd.ExecuteScalar();
                    projection.TotalBirds = r1 != DBNull.Value ? Convert.ToInt32(r1) : 0;
                }

                // Total alimento disponible (kg)
                string sqlFood = @"
            SELECT SUM(i.Quantity) 
            FROM Inventory i
            INNER JOIN Products p ON p.Id = i.ProductId
            INNER JOIN ProductTypes pt ON pt.Id = p.producttypeid
            WHERE p.Id = 26";

                using (var cmd = new MySqlCommand(sqlFood, conn))
                {
                    object r2 = cmd.ExecuteScalar();
                    projection.TotalFoodKg = r2 != DBNull.Value ? Convert.ToDecimal(r2) : 0;
                }
            }

            // Convertir a libras
            decimal factorLbs = 2.20462m;
            decimal consumoPorAveLbs = consumoPorAveKg * factorLbs;
            decimal totalFoodLbs = projection.TotalFoodKg;

            // Calcular proyección
            projection.DailyConsumptionKg = projection.TotalBirds * consumoPorAveLbs; // ahora en lbs
            projection.TotalFoodKg = totalFoodLbs; // en lbs
            projection.AvailableDays = projection.DailyConsumptionKg > 0
                ? (totalFoodLbs / projection.DailyConsumptionKg)
                : 0;

            return projection;
        }

        public List<FeedingRecord> GetFeedingHistory(int days = 30)
        {
            List<FeedingRecord> history = new List<FeedingRecord>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();

                string sql = @"
                    SELECT fe.FeedingDate, fe.Quantity
                    FROM FeedingHistory fe
                    INNER JOIN Products p ON p.Id = fe.productid
                    WHERE p.Id = 26
                    ORDER BY fe.FeedingDate DESC
                    LIMIT @Days";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Days", days);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new FeedingRecord
                            {
                                FeedingDate = reader.GetDateTime("FeedingDate"),
                                Quantity = reader.GetDecimal("Quantity")
                            });
                        }
                    }
                }
            }

            // Ordenar de manera ascendente por fecha para que la gráfica vaya de izquierda a derecha
            history.Sort((a, b) => a.FeedingDate.CompareTo(b.FeedingDate));

            return history;
        }
    }
}
