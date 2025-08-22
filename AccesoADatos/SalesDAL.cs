using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class SalesDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<Sale> GetAllByDate(DateTime? startDate = null, DateTime? endDate = null)
        {
            var list = new List<Sale>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"SELECT * FROM Sales WHERE 1=1";
                if (startDate.HasValue) sql += " AND SaleDate >= @StartDate";
                if (endDate.HasValue) sql += " AND SaleDate <= @EndDate";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (startDate.HasValue) cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    if (endDate.HasValue) cmd.Parameters.AddWithValue("@EndDate", endDate.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Sale
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                Notes = reader["Notes"]?.ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public Sale GetById(int id)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = "SELECT * FROM Sales WHERE Id=@Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Sale
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                Notes = reader["Notes"]?.ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Sale> GetAllSales()
        {
            var list = new List<Sale>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();

                string query = @"SELECT s.Id, s.SaleDate, s.Notes,
                                    c.Name AS CustomerName,
                                    d.Id AS DetailId, d.ProductId, d.Quantity, d.Price,
                                    et.Name AS EggTypeName,
                                    es.Name AS EggSizeName,
                                    CONCAT(et.Name, ' - ', es.Name) AS DisplayName
                             FROM Sales s
                             JOIN Customer c ON s.CustomerId = c.Id
                             LEFT JOIN SaleDetails d ON s.Id = d.SaleId
                             LEFT JOIN Products p ON d.ProductId = p.Id
                             LEFT JOIN EggType et ON p.EggTypeId = et.Id
                             LEFT JOIN EggSize es ON p.EggSizeId = es.Id
                             ORDER BY s.SaleDate DESC, s.Id;";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    int currentId = -1;
                    Sale current = null;

                    while (reader.Read())
                    {
                        int saleId = reader.GetInt32("Id");

                        if (current == null || saleId != currentId)
                        {
                            current = new Sale
                            {
                                Id = saleId,
                                SaleDate = reader.GetDateTime("SaleDate"),
                                CustomerName = reader.GetString("CustomerName"),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes"),
                                Details = new List<SaleDetail>(),
                                TotalQuantity = 0,
                                TotalAmount = 0
                            };
                            list.Add(current);
                            currentId = saleId;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("DetailId")))
                        {
                            var detail = new SaleDetail
                            {
                                Id = reader.GetInt32("DetailId"),
                                SaleId = saleId,
                                ProductId = reader.GetInt32("ProductId"),
                                DisplayName = reader.GetString("DisplayName"),
                                Quantity = reader.GetInt32("Quantity"),
                                Price = reader.GetDecimal("Price")
                            };

                            current.Details.Add(detail);
                            current.TotalQuantity += detail.Quantity;
                            current.TotalAmount += detail.Quantity * detail.Price;
                        }
                    }
                }
            }

            return list;
        }

        public int InsertSale(Sale sale)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert Sale
                        string insertSale = @"INSERT INTO Sales (SaleDate, CustomerId, Notes)
                                          VALUES (@SaleDate, @CustomerId, @Notes);
                                          SELECT LAST_INSERT_ID();";
                        int saleId;
                        using (var cmd = new MySqlCommand(insertSale, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                            cmd.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
                            cmd.Parameters.AddWithValue("@Notes", sale.Notes ?? "");
                            saleId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Insert SaleDetails + Update Inventory
                        foreach (var detail in sale.Details)
                        {
                            string insertDetail = @"INSERT INTO SaleDetails (SaleId, ProductId, Quantity, Price)
                                                VALUES (@SaleId, @ProductId, @Quantity, @Price);";
                            using (var cmd = new MySqlCommand(insertDetail, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@SaleId", saleId);
                                cmd.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                cmd.Parameters.AddWithValue("@Price", detail.Price);
                                cmd.ExecuteNonQuery();
                            }

                            // 🔽 Restar inventario
                            string updateInv = @"UPDATE EggInventory
                                             SET Quantity = Quantity - @Qty, LastUpdated = NOW()
                                             WHERE ProductId = @ProductId;";
                            using (var cmd = new MySqlCommand(updateInv, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Qty", detail.Quantity);
                                cmd.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        return saleId;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public void UpdateSale(Sale sale)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Update Sale
                        string updateSale = @"UPDATE Sales SET SaleDate=@SaleDate, CustomerId=@CustomerId, Notes=@Notes
                                          WHERE Id=@Id;";
                        using (var cmd = new MySqlCommand(updateSale, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                            cmd.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
                            cmd.Parameters.AddWithValue("@Notes", sale.Notes ?? "");
                            cmd.Parameters.AddWithValue("@Id", sale.Id);
                            cmd.ExecuteNonQuery();
                        }

                        // 1. Revertir inventario con los detalles actuales
                        string getOldDetails = @"SELECT ProductId, Quantity FROM SaleDetails WHERE SaleId=@SaleId;";
                        using (var cmd = new MySqlCommand(getOldDetails, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleId", sale.Id);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int productId = reader.GetInt32("ProductId");
                                    int qty = reader.GetInt32("Quantity");

                                    // Devolver al inventario
                                    string revertInv = @"UPDATE EggInventory
                                                     SET Quantity = Quantity + @Qty, LastUpdated = NOW()
                                                     WHERE ProductId=@ProductId;";
                                    using (var cmd2 = new MySqlCommand(revertInv, conn, tran))
                                    {
                                        cmd2.Parameters.AddWithValue("@Qty", qty);
                                        cmd2.Parameters.AddWithValue("@ProductId", productId);
                                        cmd2.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        // 2. Eliminar detalles anteriores
                        string deleteDetails = @"DELETE FROM SaleDetails WHERE SaleId=@SaleId;";
                        using (var cmd = new MySqlCommand(deleteDetails, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleId", sale.Id);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. Insertar los nuevos detalles y actualizar inventario
                        foreach (var detail in sale.Details)
                        {
                            string insertDetail = @"INSERT INTO SaleDetails (SaleId, ProductId, Quantity, Price)
                                                VALUES (@SaleId, @ProductId, @Quantity, @Price);";
                            using (var cmd = new MySqlCommand(insertDetail, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@SaleId", sale.Id);
                                cmd.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                cmd.Parameters.AddWithValue("@Price", detail.Price);
                                cmd.ExecuteNonQuery();
                            }

                            string updateInv = @"UPDATE EggInventory
                                             SET Quantity = Quantity - @Qty, LastUpdated = NOW()
                                             WHERE ProductId = @ProductId;";
                            using (var cmd = new MySqlCommand(updateInv, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Qty", detail.Quantity);
                                cmd.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                cmd.ExecuteNonQuery();
                            }
                        }

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

        public void DeleteSale(int saleId)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Revertir inventario antes de eliminar
                        string getDetails = @"SELECT ProductId, Quantity FROM SaleDetails WHERE SaleId=@SaleId;";
                        using (var cmd = new MySqlCommand(getDetails, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleId", saleId);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int productId = reader.GetInt32("ProductId");
                                    int qty = reader.GetInt32("Quantity");

                                    string revertInv = @"UPDATE EggInventory
                                                     SET Quantity = Quantity + @Qty, LastUpdated = NOW()
                                                     WHERE ProductId=@ProductId;";
                                    using (var cmd2 = new MySqlCommand(revertInv, conn, tran))
                                    {
                                        cmd2.Parameters.AddWithValue("@Qty", qty);
                                        cmd2.Parameters.AddWithValue("@ProductId", productId);
                                        cmd2.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        // 2. Eliminar detalles
                        string deleteDetails = @"DELETE FROM SaleDetails WHERE SaleId=@SaleId;";
                        using (var cmd = new MySqlCommand(deleteDetails, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleId", saleId);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. Eliminar venta
                        string deleteSale = @"DELETE FROM Sales WHERE Id=@Id;";
                        using (var cmd = new MySqlCommand(deleteSale, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Id", saleId);
                            cmd.ExecuteNonQuery();
                        }

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
