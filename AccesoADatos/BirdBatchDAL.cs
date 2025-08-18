using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace LasDeliciasERP.AccesoADatos
{
    public class BirdBatchDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<BirdBatch> GetAll()
        {
            var list = new List<BirdBatch>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"SELECT * FROM BirdBatches";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BirdBatch
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            BarnId = Convert.ToInt32(reader["BarnId"]),
                            BirdTypeId = Convert.ToInt32(reader["BirdTypeId"]),
                            BatchDate = Convert.ToDateTime(reader["BatchDate"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            EstimatedAgeWeeks = Convert.ToInt32(reader["EstimatedAgeWeeks"]),
                            Notes = reader["Notes"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<dynamic> GetAllWithNames()
        {
            var list = new List<dynamic>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
            SELECT bb.Id, bb.BatchDate, bb.Quantity, bb.EstimatedAgeWeeks, bb.Notes,
                   b.Name AS BarnName,
                   CONCAT(bt.Species, ' - ', bt.Breed) AS BirdTypeName
            FROM BirdBatches bb
            INNER JOIN Barn b ON bb.BarnId = b.Id
            INNER JOIN BirdTypes bt ON bb.BirdTypeId = bt.Id";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            BatchDate = Convert.ToDateTime(reader["BatchDate"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            EstimatedAgeWeeks = Convert.ToInt32(reader["EstimatedAgeWeeks"]),
                            Notes = reader["Notes"]?.ToString(),
                            BarnName = reader["BarnName"].ToString(),
                            BirdTypeName = reader["BirdTypeName"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public BirdBatch GetById(int id)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"SELECT * FROM BirdBatches WHERE Id=@Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BirdBatch
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                BarnId = Convert.ToInt32(reader["BarnId"]),
                                BirdTypeId = Convert.ToInt32(reader["BirdTypeId"]),
                                BatchDate = Convert.ToDateTime(reader["BatchDate"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                EstimatedAgeWeeks = Convert.ToInt32(reader["EstimatedAgeWeeks"]),
                                Notes = reader["Notes"]?.ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Insert(BirdBatch batch)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Insertar batch
                        string insertBatch = @"
                            INSERT INTO BirdBatches (BarnId, BirdTypeId, BatchDate, Quantity, EstimatedAgeWeeks, Notes)
                            VALUES (@BarnId, @BirdTypeId, @BatchDate, @Quantity, @EstimatedAgeWeeks, @Notes);
                            SELECT LAST_INSERT_ID();";

                        var cmdBatch = new MySqlCommand(insertBatch, conn, tran);
                        cmdBatch.Parameters.AddWithValue("@BarnId", batch.BarnId);
                        cmdBatch.Parameters.AddWithValue("@BirdTypeId", batch.BirdTypeId);
                        cmdBatch.Parameters.AddWithValue("@BatchDate", batch.BatchDate);
                        cmdBatch.Parameters.AddWithValue("@Quantity", batch.Quantity);
                        cmdBatch.Parameters.AddWithValue("@EstimatedAgeWeeks", batch.EstimatedAgeWeeks);
                        cmdBatch.Parameters.AddWithValue("@Notes", (object)batch.Notes ?? DBNull.Value);

                        int batchId = Convert.ToInt32(cmdBatch.ExecuteScalar());

                        // Insertar movimiento de entrada
                        string insertMovement = @"
                            INSERT INTO BirdMovement (BatchId, MovementType, Quantity, MovementDate, BirdTypeId, BarnId)
                            VALUES (@BatchId, 'Entrada', @Quantity, @MovementDate, @BirdTypeId, @BarnId);";

                        var cmdMovement = new MySqlCommand(insertMovement, conn, tran);
                        cmdMovement.Parameters.AddWithValue("@BatchId", batchId);
                        cmdMovement.Parameters.AddWithValue("@Quantity", batch.Quantity);
                        cmdMovement.Parameters.AddWithValue("@MovementDate", batch.BatchDate);
                        cmdMovement.Parameters.AddWithValue("@BirdTypeId", batch.BirdTypeId);
                        cmdMovement.Parameters.AddWithValue("@BarnId", batch.BarnId);
                        cmdMovement.ExecuteNonQuery();

                        // Actualizar población
                        string updatePopulation = @"
                            INSERT INTO Population (BarnId, BirdTypeId, Quantity, LastUpdated)
                            VALUES (@BarnId, @BirdTypeId, @Quantity, @LastUpdated)
                            ON DUPLICATE KEY UPDATE Quantity = Quantity + @Quantity, LastUpdated=@LastUpdated;";

                        var cmdPopulation = new MySqlCommand(updatePopulation, conn, tran);
                        cmdPopulation.Parameters.AddWithValue("@BarnId", batch.BarnId);
                        cmdPopulation.Parameters.AddWithValue("@BirdTypeId", batch.BirdTypeId);
                        cmdPopulation.Parameters.AddWithValue("@Quantity", batch.Quantity);
                        cmdPopulation.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                        cmdPopulation.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Update(BirdBatch batch)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Obtener cantidad anterior del lote
                        string getOldQuantity = "SELECT Quantity, BarnId, BirdTypeId FROM BirdBatches WHERE Id = @Id";
                        var cmdOld = new MySqlCommand(getOldQuantity, conn, tran);
                        cmdOld.Parameters.AddWithValue("@Id", batch.Id);
                        var reader = cmdOld.ExecuteReader();
                        if (!reader.Read())
                        {
                            reader.Close();
                            throw new Exception("Lote no encontrado");
                        }

                        int oldQuantity = Convert.ToInt32(reader["Quantity"]);
                        int barnId = Convert.ToInt32(reader["BarnId"]);
                        int birdTypeId = Convert.ToInt32(reader["BirdTypeId"]);
                        reader.Close();

                        int diff = batch.Quantity - oldQuantity;

                        // Actualizar lote
                        string updateBatch = @"
                            UPDATE BirdBatches
                            SET Quantity=@Quantity, EstimatedAgeWeeks=@EstimatedAgeWeeks, Notes=@Notes, BatchDate=@BatchDate
                            WHERE Id=@Id";
                        var cmdUpdateBatch = new MySqlCommand(updateBatch, conn, tran);
                        cmdUpdateBatch.Parameters.AddWithValue("@Id", batch.Id);
                        cmdUpdateBatch.Parameters.AddWithValue("@Quantity", batch.Quantity);
                        cmdUpdateBatch.Parameters.AddWithValue("@EstimatedAgeWeeks", batch.EstimatedAgeWeeks);
                        cmdUpdateBatch.Parameters.AddWithValue("@Notes", (object)batch.Notes ?? DBNull.Value);
                        cmdUpdateBatch.Parameters.AddWithValue("@BatchDate", batch.BatchDate);
                        cmdUpdateBatch.ExecuteNonQuery();

                        // Ajustar movimiento de entrada
                        string updateMovement = @"
                            UPDATE BirdMovement
                            SET Quantity=@Quantity, MovementDate=@MovementDate
                            WHERE BatchId=@BatchId AND MovementType='Entrada'";
                        var cmdUpdateMovement = new MySqlCommand(updateMovement, conn, tran);
                        cmdUpdateMovement.Parameters.AddWithValue("@Quantity", batch.Quantity);
                        cmdUpdateMovement.Parameters.AddWithValue("@MovementDate", batch.BatchDate);
                        cmdUpdateMovement.Parameters.AddWithValue("@BatchId", batch.Id);
                        cmdUpdateMovement.ExecuteNonQuery();

                        // Ajustar población
                        string updatePopulation = @"
                            UPDATE Population
                            SET Quantity = Quantity + @Diff, LastUpdated=@LastUpdated
                            WHERE BarnId=@BarnId AND BirdTypeId=@BirdTypeId";
                        var cmdPopulation = new MySqlCommand(updatePopulation, conn, tran);
                        cmdPopulation.Parameters.AddWithValue("@Diff", diff);
                        cmdPopulation.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                        cmdPopulation.Parameters.AddWithValue("@BarnId", barnId);
                        cmdPopulation.Parameters.AddWithValue("@BirdTypeId", birdTypeId);
                        cmdPopulation.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Delete(int batchId)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Obtener info del lote
                        string getBatch = "SELECT Quantity, BarnId, BirdTypeId FROM BirdBatches WHERE Id=@Id";
                        var cmdGet = new MySqlCommand(getBatch, conn, tran);
                        cmdGet.Parameters.AddWithValue("@Id", batchId);
                        var reader = cmdGet.ExecuteReader();
                        if (!reader.Read())
                        {
                            reader.Close();
                            throw new Exception("Lote no encontrado");
                        }

                        int quantity = Convert.ToInt32(reader["Quantity"]);
                        int barnId = Convert.ToInt32(reader["BarnId"]);
                        int birdTypeId = Convert.ToInt32(reader["BirdTypeId"]);
                        reader.Close();

                        // Eliminar movimiento de entrada asociado
                        string deleteMovement = "DELETE FROM BirdMovement WHERE BatchId=@BatchId AND MovementType='Entrada'";
                        var cmdDeleteMovement = new MySqlCommand(deleteMovement, conn, tran);
                        cmdDeleteMovement.Parameters.AddWithValue("@BatchId", batchId);
                        cmdDeleteMovement.ExecuteNonQuery();

                        // Ajustar población
                        string updatePopulation = @"
                            UPDATE Population
                            SET Quantity = Quantity - @Quantity, LastUpdated=@LastUpdated
                            WHERE BarnId=@BarnId AND BirdTypeId=@BirdTypeId";
                        var cmdPopulation = new MySqlCommand(updatePopulation, conn, tran);
                        cmdPopulation.Parameters.AddWithValue("@Quantity", quantity);
                        cmdPopulation.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                        cmdPopulation.Parameters.AddWithValue("@BarnId", barnId);
                        cmdPopulation.Parameters.AddWithValue("@BirdTypeId", birdTypeId);
                        cmdPopulation.ExecuteNonQuery();

                        // Eliminar lote
                        string deleteBatch = "DELETE FROM BirdBatches WHERE Id=@Id";
                        var cmdDeleteBatch = new MySqlCommand(deleteBatch, conn, tran);
                        cmdDeleteBatch.Parameters.AddWithValue("@Id", batchId);
                        cmdDeleteBatch.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
