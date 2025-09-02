using LasDeliciasERP.Models;
using LasDeliciasERP.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class InventoryPurchaseDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;
        ConversionFactor objConversion = new ConversionFactor();
        Maps objMaps = new Maps();

        public InventoryPurchase GetById(int id)
        {
            InventoryPurchase purchase = null;

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
                    SELECT p.Id AS PurchaseId, p.SupplierId, p.PurchaseDate, p.InvoiceNumber, p.TotalAmount, p.Notes,
                           s.Name AS SupplierName,
                           d.Id AS DetailId, d.ProductId, d.Quantity, d.UnitPrice,
                           pr.Name AS ProductName
                    FROM InventoryPurchase p
                    LEFT JOIN Suppliers s ON p.SupplierId = s.Id
                    LEFT JOIN InventoryPurchaseDetail d ON p.Id = d.PurchaseId
                    LEFT JOIN Products pr ON d.ProductId = pr.Id
                    WHERE p.Id = @Id
                    ORDER BY d.Id ASC";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (purchase == null)
                            {
                                purchase = new InventoryPurchase
                                {
                                    Id = reader.GetInt32("PurchaseId"),
                                    SupplierId = reader.GetInt32("SupplierId"),
                                    SupplierName = reader["SupplierName"]?.ToString(),
                                    PurchaseDate = reader.GetDateTime("PurchaseDate"),
                                    InvoiceNumber = reader["InvoiceNumber"]?.ToString(),
                                    TotalAmount = reader.GetDecimal("TotalAmount"),
                                    Notes = reader["Notes"]?.ToString(),
                                    Details = new List<InventoryPurchaseDetail>()
                                };
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("DetailId")))
                            {
                                purchase.Details.Add(new InventoryPurchaseDetail
                                {
                                    Id = reader.GetInt32("DetailId"),
                                    PurchaseId = purchase.Id,
                                    ProductId = reader.GetInt32("ProductId"),
                                    ProductName = reader["ProductName"]?.ToString(),
                                    Quantity = reader.GetDecimal("Quantity"),
                                    UnitPrice = reader.GetDecimal("UnitPrice")
                                });
                            }
                        }
                    }
                }
            }

            return purchase;
        }

        public List<InventoryPurchase> GetAll()
        {
            var purchases = new List<InventoryPurchase>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
                    SELECT p.Id AS PurchaseId, p.SupplierId, p.PurchaseDate, p.InvoiceNumber, p.TotalAmount, p.Notes,
                           s.Name AS SupplierName,
                           d.Id AS DetailId, d.ProductId, d.Quantity, d.UnitPrice,
                           pr.Name AS ProductName
                    FROM InventoryPurchase p
                    LEFT JOIN Suppliers s ON p.SupplierId = s.Id
                    LEFT JOIN InventoryPurchaseDetail d ON p.Id = d.PurchaseId
                    LEFT JOIN Products pr ON d.ProductId = pr.Id
                    ORDER BY p.PurchaseDate DESC, p.Id, d.Id";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    int currentId = -1;
                    InventoryPurchase currentPurchase = null;

                    while (reader.Read())
                    {
                        int purchaseId = reader.GetInt32("PurchaseId");

                        if (currentPurchase == null || purchaseId != currentId)
                        {
                            currentPurchase = new InventoryPurchase
                            {
                                Id = purchaseId,
                                SupplierId = reader.GetInt32("SupplierId"),
                                SupplierName = reader["SupplierName"]?.ToString(),
                                PurchaseDate = reader.GetDateTime("PurchaseDate"),
                                InvoiceNumber = reader["InvoiceNumber"]?.ToString(),
                                TotalAmount = reader.GetDecimal("TotalAmount"),
                                Notes = reader["Notes"]?.ToString(),
                                Details = new List<InventoryPurchaseDetail>()
                            };
                            purchases.Add(currentPurchase);
                            currentId = purchaseId;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("DetailId")))
                        {
                            currentPurchase.Details.Add(new InventoryPurchaseDetail
                            {
                                Id = reader.GetInt32("DetailId"),
                                PurchaseId = purchaseId,
                                ProductId = reader.GetInt32("ProductId"),
                                ProductName = reader["ProductName"]?.ToString(),
                                Quantity = reader.GetDecimal("Quantity"),
                                UnitPrice = reader.GetDecimal("UnitPrice")
                            });
                        }
                    }
                }
            }

            return purchases;
        }

        public int Insert(InventoryPurchase purchase)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Insertar encabezado de compra
                        string insertPurchase = @"
                    INSERT INTO InventoryPurchase 
                        (SupplierId, PurchaseDate, InvoiceNumber, TotalAmount, Notes)
                    VALUES 
                        (@SupplierId, @PurchaseDate, @InvoiceNumber, @TotalAmount, @Notes);
                    SELECT LAST_INSERT_ID();";

                        int purchaseId;
                        using (var cmd = new MySqlCommand(insertPurchase, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SupplierId", purchase.SupplierId);
                            cmd.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                            cmd.Parameters.AddWithValue("@InvoiceNumber", purchase.InvoiceNumber ?? "");
                            cmd.Parameters.AddWithValue("@TotalAmount", purchase.TotalAmount);
                            cmd.Parameters.AddWithValue("@Notes", purchase.Notes ?? "");

                            purchaseId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Insertar detalles y actualizar inventario
                        foreach (var detail in purchase.Details)
                        {
                            // Insertar detalle en DB
                            string insertDetail = @"
                        INSERT INTO InventoryPurchaseDetail
                            (PurchaseId, ProductId, Quantity, UnitPrice)
                        VALUES
                            (@PurchaseId, @ProductId, @Quantity, @UnitPrice);";

                            using (var cmd = new MySqlCommand(insertDetail, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@PurchaseId", purchaseId);
                                cmd.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                cmd.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);
                                cmd.ExecuteNonQuery();
                            }

                            // Convertir cantidad a libras según unidad
                            decimal conversionFactor = objConversion.GetConversionFactor(detail.UnitTypeId); // ej: quintal->100, arroba->25, libra->1
                            decimal quantityInLbs = detail.Quantity * conversionFactor;

                            // Diccionario de mapeo de inventario
                            var productMap = objMaps.GetProductMap();

                            // Determinar el ProductId que realmente existe en inventario
                            int inventoryProductId = productMap.ContainsKey(detail.ProductId)
                                                        ? productMap[detail.ProductId]
                                                        : detail.ProductId;

                            // Actualizar inventario
                            string updateInventory = @"
                        INSERT INTO Inventory (ProductId, Quantity, LastUpdated)
                        VALUES (@ProductId, @Qty, NOW())
                        ON DUPLICATE KEY UPDATE Quantity = Quantity + @Qty, LastUpdated = NOW();";

                            using (var cmd = new MySqlCommand(updateInventory, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@ProductId", inventoryProductId);
                                cmd.Parameters.AddWithValue("@Qty", quantityInLbs);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        return purchaseId;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Update(InventoryPurchase purchase)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();

                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // Update purchase
                        string sqlPurchase = @"
                            UPDATE InventoryPurchase
                            SET SupplierId = @SupplierId,
                                PurchaseDate = @PurchaseDate,
                                InvoiceNumber = @InvoiceNumber,
                                TotalAmount = @TotalAmount,
                                Notes = @Notes
                            WHERE Id = @Id";

                        using (var cmd = new MySqlCommand(sqlPurchase, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@SupplierId", purchase.SupplierId);
                            cmd.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                            cmd.Parameters.AddWithValue("@InvoiceNumber", purchase.InvoiceNumber);
                            cmd.Parameters.AddWithValue("@TotalAmount", purchase.TotalAmount);
                            cmd.Parameters.AddWithValue("@Notes", purchase.Notes);
                            cmd.Parameters.AddWithValue("@Id", purchase.Id);

                            cmd.ExecuteNonQuery();
                        }

                        // Delete old details
                        string sqlDelete = "DELETE FROM InventoryPurchaseDetail WHERE PurchaseId = @PurchaseId";
                        using (var cmd = new MySqlCommand(sqlDelete, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@PurchaseId", purchase.Id);
                            cmd.ExecuteNonQuery();
                        }

                        // Insert new details
                        foreach (var detail in purchase.Details)
                        {
                            string sqlDetail = @"
                                INSERT INTO InventoryPurchaseDetail (PurchaseId, ProductId, Quantity, UnitPrice)
                                VALUES (@PurchaseId, @ProductId, @Quantity, @UnitPrice)";

                            using (var cmd = new MySqlCommand(sqlDetail, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@PurchaseId", purchase.Id);
                                cmd.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                cmd.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
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

                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlDetails = "DELETE FROM InventoryPurchaseDetail WHERE PurchaseId = @Id";
                        using (var cmd = new MySqlCommand(sqlDetails, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }

                        string sqlPurchase = "DELETE FROM InventoryPurchase WHERE Id = @Id";
                        using (var cmd = new MySqlCommand(sqlPurchase, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
