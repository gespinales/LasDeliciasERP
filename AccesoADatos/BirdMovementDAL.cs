using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;

namespace LasDeliciasERP.AccesoADatos
{
    public class BirdMovementDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<BirdMovement> GetAll()
        {
            var list = new List<BirdMovement>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
                SELECT bm.Id, bm.BatchId, bm.MovementType, bm.Quantity, bm.MovementDate, bm.Reason,
                       bm.BirdTypeId, bt.Species AS BirdTypeName,
                       bm.BarnId, b.Name AS BarnName
                FROM BirdMovement bm
                INNER JOIN BirdTypes bt ON bm.BirdTypeId = bt.Id
                INNER JOIN Barn b ON bm.BarnId = b.Id
                ORDER BY bm.MovementDate DESC";

                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new BirdMovement
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        BatchId = reader["BatchId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["BatchId"]),
                        MovementType = reader["MovementType"].ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        MovementDate = Convert.ToDateTime(reader["MovementDate"]),
                        Reason = reader["Reason"]?.ToString(),
                        BirdTypeId = Convert.ToInt32(reader["BirdTypeId"]),
                        BarnId = Convert.ToInt32(reader["BarnId"]),
                        BirdTypeName = reader["BirdTypeName"].ToString(),
                        BarnName = reader["BarnName"].ToString()
                    });
                }
            }
            return list;
        }

        public List<BirdMovement> GetByBatch(int batchId)
        {
            var list = new List<BirdMovement>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
                SELECT bm.Id, bm.BatchId, bm.MovementType, bm.Quantity, bm.MovementDate, bm.Reason,
                       bm.BirdTypeId, bt.Species AS BirdTypeName,
                       bm.BarnId, b.Name AS BarnName
                FROM BirdMovement bm
                INNER JOIN BirdTypes bt ON bm.BirdTypeId = bt.Id
                INNER JOIN Barn b ON bm.BarnId = b.Id
                WHERE bm.BatchId = @BatchId
                ORDER BY bm.MovementDate DESC";

                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BatchId", batchId);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new BirdMovement
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        BatchId = reader["BatchId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["BatchId"]),
                        MovementType = reader["MovementType"].ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        MovementDate = Convert.ToDateTime(reader["MovementDate"]),
                        Reason = reader["Reason"]?.ToString(),
                        BirdTypeId = Convert.ToInt32(reader["BirdTypeId"]),
                        BarnId = Convert.ToInt32(reader["BarnId"]),
                        BirdTypeName = reader["BirdTypeName"].ToString(),
                        BarnName = reader["BarnName"].ToString()
                    });
                }
            }
            return list;
        }

        public void Insert(BirdMovement movement)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"INSERT INTO BirdMovement (BatchId, MovementType, Quantity, MovementDate, Reason, BirdTypeId, BarnId)
                           VALUES (@BatchId, @MovementType, @Quantity, @MovementDate, @Reason, @BirdTypeId, @BarnId)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BatchId", (object)movement.BatchId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MovementType", movement.MovementType);
                cmd.Parameters.AddWithValue("@Quantity", movement.Quantity);
                cmd.Parameters.AddWithValue("@MovementDate", movement.MovementDate);
                cmd.Parameters.AddWithValue("@Reason", movement.Reason ?? "");
                cmd.Parameters.AddWithValue("@BirdTypeId", movement.BirdTypeId);
                cmd.Parameters.AddWithValue("@BarnId", movement.BarnId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}