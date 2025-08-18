using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class BirdDeathDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        // Obtener todos los decesos con nombres de galpón y tipo de ave
        public List<BirdDeath> GetAll()
        {
            var list = new List<BirdDeath>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
                    SELECT d.Id, d.BarnId, d.BirdTypeId, d.Quantity, d.DeathDate, d.Reason,
                           b.Name AS BarnName,
                           bt.Breed AS BirdTypeName
                    FROM BirdDeath d
                    INNER JOIN Barn b ON d.BarnId = b.Id
                    INNER JOIN BirdTypes bt ON d.BirdTypeId = bt.Id
                    ORDER BY d.DeathDate DESC";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BirdDeath
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            BarnId = Convert.ToInt32(reader["BarnId"]),
                            BirdTypeId = Convert.ToInt32(reader["BirdTypeId"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            DeathDate = Convert.ToDateTime(reader["DeathDate"]),
                            Reason = reader["Reason"]?.ToString(),
                            BarnName = reader["BarnName"].ToString(),
                            BirdTypeName = reader["BirdTypeName"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public BirdDeath GetById(int id)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
            SELECT Id, BarnId, BirdTypeId, Quantity, DeathDate, Reason
            FROM BirdDeath
            WHERE Id=@Id;";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BirdDeath
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            BarnId = Convert.ToInt32(reader["BarnId"]),
                            BirdTypeId = Convert.ToInt32(reader["BirdTypeId"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            DeathDate = Convert.ToDateTime(reader["DeathDate"]),
                            Reason = reader["Reason"]?.ToString()
                        };
                    }
                }
            }
            return null;
        }

        public void Insert(BirdDeath death)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar deceso
                        string insertDeath = @"
                            INSERT INTO BirdDeath (BarnId, BirdTypeId, Quantity, DeathDate, Reason)
                            VALUES (@BarnId, @BirdTypeId, @Quantity, @DeathDate, @Reason);
                            SELECT LAST_INSERT_ID();";

                        var cmdDeath = new MySqlCommand(insertDeath, conn, tran);
                        cmdDeath.Parameters.AddWithValue("@BarnId", death.BarnId);
                        cmdDeath.Parameters.AddWithValue("@BirdTypeId", death.BirdTypeId);
                        cmdDeath.Parameters.AddWithValue("@Quantity", death.Quantity);
                        cmdDeath.Parameters.AddWithValue("@DeathDate", death.DeathDate);
                        cmdDeath.Parameters.AddWithValue("@Reason", (object)death.Reason ?? DBNull.Value);

                        int deathId = Convert.ToInt32(cmdDeath.ExecuteScalar());

                        // 2. Insertar movimiento de salida
                        string insertMovement = @"
                            INSERT INTO BirdMovement (BatchId, MovementType, Quantity, MovementDate, BirdTypeId, BarnId, Reason)
                            VALUES (NULL, 'Salida', @Quantity, @MovementDate, @BirdTypeId, @BarnId, @Reason);";

                        var cmdMovement = new MySqlCommand(insertMovement, conn, tran);
                        cmdMovement.Parameters.AddWithValue("@Quantity", death.Quantity);
                        cmdMovement.Parameters.AddWithValue("@MovementDate", death.DeathDate);
                        cmdMovement.Parameters.AddWithValue("@BirdTypeId", death.BirdTypeId);
                        cmdMovement.Parameters.AddWithValue("@BarnId", death.BarnId);
                        cmdMovement.Parameters.AddWithValue("@Reason", (object)death.Reason ?? DBNull.Value);
                        cmdMovement.ExecuteNonQuery();

                        // 3. Actualizar población
                        string updatePopulation = @"
                            UPDATE Population
                            SET Quantity = Quantity - @Quantity, LastUpdated=@LastUpdated
                            WHERE BarnId=@BarnId AND BirdTypeId=@BirdTypeId;";

                        var cmdPopulation = new MySqlCommand(updatePopulation, conn, tran);
                        cmdPopulation.Parameters.AddWithValue("@BarnId", death.BarnId);
                        cmdPopulation.Parameters.AddWithValue("@BirdTypeId", death.BirdTypeId);
                        cmdPopulation.Parameters.AddWithValue("@Quantity", death.Quantity);
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

        public void Update(BirdDeath death)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Obtener cantidad anterior
                        string getOldQuantity = "SELECT Quantity FROM BirdDeath WHERE Id=@Id;";
                        var cmdOld = new MySqlCommand(getOldQuantity, conn, tran);
                        cmdOld.Parameters.AddWithValue("@Id", death.Id);
                        int oldQuantity = Convert.ToInt32(cmdOld.ExecuteScalar());

                        int quantityDiff = death.Quantity - oldQuantity; // diferencia entre nueva y vieja cantidad

                        // 2. Actualizar deceso
                        string updateDeath = @"
                    UPDATE BirdDeath
                    SET BarnId=@BarnId, BirdTypeId=@BirdTypeId, Quantity=@Quantity, DeathDate=@DeathDate, Reason=@Reason
                    WHERE Id=@Id;";
                        var cmdDeath = new MySqlCommand(updateDeath, conn, tran);
                        cmdDeath.Parameters.AddWithValue("@BarnId", death.BarnId);
                        cmdDeath.Parameters.AddWithValue("@BirdTypeId", death.BirdTypeId);
                        cmdDeath.Parameters.AddWithValue("@Quantity", death.Quantity);
                        cmdDeath.Parameters.AddWithValue("@DeathDate", death.DeathDate);
                        cmdDeath.Parameters.AddWithValue("@Reason", (object)death.Reason ?? DBNull.Value);
                        cmdDeath.Parameters.AddWithValue("@Id", death.Id);
                        cmdDeath.ExecuteNonQuery();

                        // 3. Actualizar movimiento de salida correspondiente
                        string updateMovement = @"
                    UPDATE BirdMovement
                    SET Quantity=@Quantity, MovementDate=@DeathDate, BirdTypeId=@BirdTypeId, BarnId=@BarnId, Reason=@Reason
                    WHERE MovementType='Salida' AND Reason=@Reason AND Quantity=@OldQuantity;";
                        var cmdMovement = new MySqlCommand(updateMovement, conn, tran);
                        cmdMovement.Parameters.AddWithValue("@Quantity", death.Quantity);
                        cmdMovement.Parameters.AddWithValue("@DeathDate", death.DeathDate);
                        cmdMovement.Parameters.AddWithValue("@BirdTypeId", death.BirdTypeId);
                        cmdMovement.Parameters.AddWithValue("@BarnId", death.BarnId);
                        cmdMovement.Parameters.AddWithValue("@Reason", (object)death.Reason ?? DBNull.Value);
                        cmdMovement.Parameters.AddWithValue("@OldQuantity", oldQuantity);
                        cmdMovement.ExecuteNonQuery();

                        // 4. Ajustar población
                        string updatePopulation = @"
                    UPDATE Population
                    SET Quantity = Quantity - @QuantityDiff, LastUpdated=@LastUpdated
                    WHERE BarnId=@BarnId AND BirdTypeId=@BirdTypeId;";
                        var cmdPopulation = new MySqlCommand(updatePopulation, conn, tran);
                        cmdPopulation.Parameters.AddWithValue("@QuantityDiff", quantityDiff);
                        cmdPopulation.Parameters.AddWithValue("@BarnId", death.BarnId);
                        cmdPopulation.Parameters.AddWithValue("@BirdTypeId", death.BirdTypeId);
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

        public void Delete(int id)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Obtener deceso para ajustar población y movimiento
                        string getDeath = "SELECT BarnId, BirdTypeId, Quantity FROM BirdDeath WHERE Id=@Id;";
                        var cmdGet = new MySqlCommand(getDeath, conn, tran);
                        cmdGet.Parameters.AddWithValue("@Id", id);
                        using (var reader = cmdGet.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new Exception("Deceso no encontrado.");

                            int barnId = Convert.ToInt32(reader["BarnId"]);
                            int birdTypeId = Convert.ToInt32(reader["BirdTypeId"]);
                            int quantity = Convert.ToInt32(reader["Quantity"]);
                            reader.Close();

                            // 2. Eliminar movimiento de salida asociado
                            string deleteMovement = @"
                        DELETE FROM BirdMovement
                        WHERE MovementType='Salida' 
                          AND Quantity=@Quantity 
                          AND BarnId=@BarnId 
                          AND BirdTypeId=@BirdTypeId;";
                            var cmdMove = new MySqlCommand(deleteMovement, conn, tran);
                            cmdMove.Parameters.AddWithValue("@Quantity", quantity);
                            cmdMove.Parameters.AddWithValue("@BarnId", barnId);
                            cmdMove.Parameters.AddWithValue("@BirdTypeId", birdTypeId);
                            cmdMove.ExecuteNonQuery();

                            // 3. Ajustar población
                            string updatePopulation = @"
                        UPDATE Population
                        SET Quantity = Quantity + @Quantity, LastUpdated=@LastUpdated
                        WHERE BarnId=@BarnId AND BirdTypeId=@BirdTypeId;";
                            var cmdPop = new MySqlCommand(updatePopulation, conn, tran);
                            cmdPop.Parameters.AddWithValue("@Quantity", quantity);
                            cmdPop.Parameters.AddWithValue("@BarnId", barnId);
                            cmdPop.Parameters.AddWithValue("@BirdTypeId", birdTypeId);
                            cmdPop.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                            cmdPop.ExecuteNonQuery();

                            // 4. Eliminar deceso
                            string deleteDeath = "DELETE FROM BirdDeath WHERE Id=@Id;";
                            var cmdDeath = new MySqlCommand(deleteDeath, conn, tran);
                            cmdDeath.Parameters.AddWithValue("@Id", id);
                            cmdDeath.ExecuteNonQuery();

                            tran.Commit();
                        }
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
