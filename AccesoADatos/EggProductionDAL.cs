using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using LasDeliciasERP.Models;

namespace LasDeliciasERP.AccesoADatos
{
    public class EggProductionDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        // Obtiene todas las producciones sin detalles
        public List<EggProduction> GetAll()
        {
            var list = new List<EggProduction>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();

                string query = @"SELECT ep.Id, ep.ProductionDate, ep.Notes, ep.BarnId,
                                b.Name AS BarnName,
                                d.ProductId, d.Quantity,
                                et.Name AS EggTypeName,
                                es.Name AS Size, es.MinWeight, es.MaxWeight
                         FROM EggProduction ep
                         JOIN Barn b ON ep.BarnId = b.Id
                         LEFT JOIN EggProductionDetail d ON ep.Id = d.EggProductionId
                         LEFT JOIN Products p ON d.ProductId = p.Id
                         LEFT JOIN EggType et ON p.EggTypeId = et.Id
                         LEFT JOIN EggSize es ON p.EggSizeId = es.Id
                         ORDER BY ep.ProductionDate DESC, ep.Id";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    int currentId = -1;
                    EggProduction current = null;

                    while (reader.Read())
                    {
                        int epId = reader.GetInt32("Id");

                        if (current == null || epId != currentId)
                        {
                            current = new EggProduction
                            {
                                Id = epId,
                                ProductionDate = reader.GetDateTime("ProductionDate"),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes"),
                                BarnName = reader.GetString("BarnName"),
                                EggTypeName = reader.IsDBNull(reader.GetOrdinal("EggTypeName")) ? "Sin tipo" : reader.GetString("EggTypeName"),
                                QuantityS = 0,
                                QuantityM = 0,
                                QuantityL = 0,
                                QuantityXL = 0,
                                TotalQuantity = 0,
                                TotalWeight = 0,
                                AverageWeight = 0
                            };

                            list.Add(current);
                            currentId = epId;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                        {
                            int qty = reader.GetInt32("Quantity");
                            string size = reader.IsDBNull(reader.GetOrdinal("Size")) ? "" : reader.GetString("Size");
                            double minW = reader.IsDBNull(reader.GetOrdinal("MinWeight")) ? 0 : reader.GetDouble("MinWeight");
                            double maxW = reader.IsDBNull(reader.GetOrdinal("MaxWeight")) ? 0 : reader.GetDouble("MaxWeight");

                            double avgWeight = (minW + maxW) / 2.0;

                            switch (size)
                            {
                                case "S": current.QuantityS += qty; break;
                                case "M": current.QuantityM += qty; break;
                                case "L": current.QuantityL += qty; break;
                                case "XL": current.QuantityXL += qty; break;
                            }

                            current.TotalQuantity += qty;
                            current.TotalWeight += qty * avgWeight;
                            current.AverageWeight = current.TotalQuantity > 0
                                ? current.TotalWeight / current.TotalQuantity
                                : 0;
                        }
                    }
                }
            }

            return list;
        }

        // Obtiene una producción específica con detalles
        public EggProduction GetById(int id)
        {
            EggProduction production = null;

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();

                string query = @"
                SELECT ep.Id, ep.ProductionDate, ep.Notes, ep.BarnId,
                       b.Name AS BarnName,
                       d.ProductId, p.Name AS ProductName, d.Quantity
                FROM EggProduction ep
                JOIN Barn b ON ep.BarnId = b.Id
                LEFT JOIN EggProductionDetail d ON ep.Id = d.EggProductionId
                LEFT JOIN Products p ON d.ProductId = p.Id
                WHERE ep.Id = @Id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (production == null)
                            {
                                production = new EggProduction
                                {
                                    Id = reader.GetInt32("Id"),
                                    ProductionDate = reader.GetDateTime("ProductionDate"),
                                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes"),
                                    BarnId = reader.GetInt32("BarnId"),
                                    BarnName = reader.GetString("BarnName"),
                                    Details = new List<EggProductionDetail>()
                                };
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                production.Details.Add(new EggProductionDetail
                                {
                                    ProductId = reader.GetInt32("ProductId"),
                                    ProductName = reader.GetString("ProductName"),
                                    Quantity = reader.GetInt32("Quantity")
                                });
                            }
                        }
                    }
                }
            }

            return production;
        }

        public List<EggProductionDetail> GetDetailsByProductionId(int productionId)
        {
            var list = new List<EggProductionDetail>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                //    string query = @"
                //SELECT epd.*, p.Name AS ProductName
                //FROM EggProductionDetail epd
                //JOIN Products p ON epd.ProductId = p.Id
                //WHERE epd.EggProductionId = @ProductionId";

                string query = @"SELECT ep.Id, ep.ProductionDate, ep.Notes, ep.BarnId,
       b.Name AS BarnName,
       d.ProductId, p.Name AS ProductName, p.UnitTypeId, d.Quantity,
       et.Name AS EggTypeName,
       es.Name AS EggSizeName,
       CONCAT(et.Name, ' - ', es.Name) AS DisplayName
FROM EggProduction ep
JOIN Barn b ON ep.BarnId = b.Id
LEFT JOIN EggProductionDetail d ON ep.Id = d.EggProductionId
LEFT JOIN Products p ON d.ProductId = p.Id
LEFT JOIN EggType et ON p.EggTypeId = et.Id
LEFT JOIN EggSize es ON p.EggSizeId = es.Id
WHERE ep.Id = @ProductionId
ORDER BY et.Name, es.Id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductionId", productionId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new EggProductionDetail
                            {
                                ProductId = reader.GetInt32("ProductId"),
                                ProductName = reader["DisplayName"].ToString(),
                                Quantity = reader.GetInt32("Quantity")
                            });
                        }
                    }
                }
            }

            return list;
        }

        public int Insert(EggProduction production)
        {
            int insertedId = 0;

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string insertHeader = @"INSERT INTO EggProduction (ProductionDate, BarnId, Notes) 
                                                VALUES (@Date, @BarnId, @Notes);
                                                SELECT LAST_INSERT_ID();";
                        var cmdHeader = new MySqlCommand(insertHeader, conn, transaction);
                        cmdHeader.Parameters.AddWithValue("@Date", production.ProductionDate);
                        cmdHeader.Parameters.AddWithValue("@BarnId", production.BarnId);
                        cmdHeader.Parameters.AddWithValue("@Notes", production.Notes ?? "");
                        insertedId = Convert.ToInt32(cmdHeader.ExecuteScalar());

                        foreach (var detail in production.Details)
                        {
                            string insertDetail = @"INSERT INTO EggProductionDetail (EggProductionId, ProductId, Quantity)
                                                    VALUES (@ProductionId, @ProductId, @Quantity)";
                            var cmdDetail = new MySqlCommand(insertDetail, conn, transaction);
                            cmdDetail.Parameters.AddWithValue("@ProductionId", insertedId);
                            cmdDetail.Parameters.AddWithValue("@ProductId", detail.ProductId);
                            cmdDetail.Parameters.AddWithValue("@Quantity", detail.Quantity);
                            cmdDetail.ExecuteNonQuery();

                            // Actualizar inventario por cada producto
                            UpdateInventory(conn, transaction, detail.ProductId);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return insertedId;
        }

        public void Update(EggProduction production)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string updateHeader = @"UPDATE EggProduction SET ProductionDate=@Date, BarnId=@BarnId, Notes=@Notes
                                                WHERE Id=@Id";
                        var cmdHeader = new MySqlCommand(updateHeader, conn, transaction);
                        cmdHeader.Parameters.AddWithValue("@Date", production.ProductionDate);
                        cmdHeader.Parameters.AddWithValue("@BarnId", production.BarnId);
                        cmdHeader.Parameters.AddWithValue("@Notes", production.Notes ?? "");
                        cmdHeader.Parameters.AddWithValue("@Id", production.Id);
                        cmdHeader.ExecuteNonQuery();

                        // Eliminar detalles antiguos
                        string deleteDetails = "DELETE FROM EggProductionDetail WHERE EggProductionId=@ProductionId";
                        var cmdDelete = new MySqlCommand(deleteDetails, conn, transaction);
                        cmdDelete.Parameters.AddWithValue("@ProductionId", production.Id);
                        cmdDelete.ExecuteNonQuery();

                        // Insertar detalles nuevos
                        foreach (var detail in production.Details)
                        {
                            string insertDetail = @"INSERT INTO EggProductionDetail (EggProductionId, ProductId, Quantity)
                                                    VALUES (@ProductionId, @ProductId, @Quantity)";
                            var cmdDetail = new MySqlCommand(insertDetail, conn, transaction);
                            cmdDetail.Parameters.AddWithValue("@ProductionId", production.Id);
                            cmdDetail.Parameters.AddWithValue("@ProductId", detail.ProductId);
                            cmdDetail.Parameters.AddWithValue("@Quantity", detail.Quantity);
                            cmdDetail.ExecuteNonQuery();

                            // Actualizar inventario
                            UpdateInventory(conn, transaction, detail.ProductId);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Delete(int productionId)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Obtener lista de productos afectados para actualizar inventario después
                        var productIds = new List<int>();
                        string selectProducts = "SELECT DISTINCT ProductId FROM EggProductionDetail WHERE EggProductionId=@ProductionId";
                        var cmdSelect = new MySqlCommand(selectProducts, conn, transaction);
                        cmdSelect.Parameters.AddWithValue("@ProductionId", productionId);
                        using (var reader = cmdSelect.ExecuteReader())
                        {
                            while (reader.Read())
                                productIds.Add(reader.GetInt32("ProductId"));
                        }

                        // Borrar detalles
                        string deleteDetails = "DELETE FROM EggProductionDetail WHERE EggProductionId=@ProductionId";
                        var cmdDeleteDetails = new MySqlCommand(deleteDetails, conn, transaction);
                        cmdDeleteDetails.Parameters.AddWithValue("@ProductionId", productionId);
                        cmdDeleteDetails.ExecuteNonQuery();

                        // Borrar cabecera
                        string deleteHeader = "DELETE FROM EggProduction WHERE Id=@Id";
                        var cmdDeleteHeader = new MySqlCommand(deleteHeader, conn, transaction);
                        cmdDeleteHeader.Parameters.AddWithValue("@Id", productionId);
                        cmdDeleteHeader.ExecuteNonQuery();

                        // Actualizar inventario de los productos afectados
                        foreach (var productId in productIds)
                        {
                            UpdateInventory(conn, transaction, productId);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void UpdateInventory(MySqlConnection conn, MySqlTransaction transaction, int productId)
        {
            // Calcular total producido
            string sumQuery = @"SELECT SUM(Quantity) FROM EggProductionDetail 
                                WHERE ProductId=@ProductId";
            var cmdSum = new MySqlCommand(sumQuery, conn, transaction);
            cmdSum.Parameters.AddWithValue("@ProductId", productId);
            int totalQuantity = Convert.ToInt32(cmdSum.ExecuteScalar() ?? 0);

            // Verificar si existe en inventario
            string existsQuery = "SELECT COUNT(*) FROM EggInventory WHERE ProductId=@ProductId";
            var cmdExists = new MySqlCommand(existsQuery, conn, transaction);
            cmdExists.Parameters.AddWithValue("@ProductId", productId);
            int count = Convert.ToInt32(cmdExists.ExecuteScalar());

            if (count > 0)
            {
                string updateQuery = @"UPDATE EggInventory SET Quantity=@Quantity, LastUpdated=@LastUpdated
                                       WHERE ProductId=@ProductId";
                var cmdUpdate = new MySqlCommand(updateQuery, conn, transaction);
                cmdUpdate.Parameters.AddWithValue("@Quantity", totalQuantity);
                cmdUpdate.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                cmdUpdate.Parameters.AddWithValue("@ProductId", productId);
                cmdUpdate.ExecuteNonQuery();
            }
            else
            {
                string insertQuery = @"INSERT INTO EggInventory (ProductId, Quantity, LastUpdated)
                                       VALUES (@ProductId, @Quantity, @LastUpdated)";
                var cmdInsert = new MySqlCommand(insertQuery, conn, transaction);
                cmdInsert.Parameters.AddWithValue("@ProductId", productId);
                cmdInsert.Parameters.AddWithValue("@Quantity", totalQuantity);
                cmdInsert.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                cmdInsert.ExecuteNonQuery();
            }
        }
    }
}
