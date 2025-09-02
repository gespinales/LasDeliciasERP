using LasDeliciasERP.Models;
using LasDeliciasERP.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LasDeliciasERP.AccesoADatos
{
    public class SalesDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;
        ConversionFactor objConversion = new ConversionFactor();
        Maps objMaps = new Maps();

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

        public List<Sale> GetAllSales()
        {
            var list = new List<Sale>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();

                string query = @"SELECT s.Id, s.SaleDate, s.Notes,
                                    c.Name AS CustomerName,
                                    d.Id AS DetailId, d.ProductId, d.Quantity, d.Price, d.SalePrice,
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
                                TotalAmount = 0,
                                TotalSaleAmount = 0
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
                                Price = reader.GetDecimal("Price"),
                                SalePrice = reader.GetDecimal("SalePrice")
                            };

                            current.Details.Add(detail);
                            current.TotalQuantity += detail.Quantity;
                            current.TotalAmount += detail.Quantity * detail.Price;
                            current.TotalSaleAmount += detail.Quantity * detail.SalePrice;
                        }
                    }
                }
            }

            return list;
        }

        public Sale GetSaleById(int saleId)
        {
            Sale sale = null;

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();

                string query = @"
    SELECT 
        s.Id, s.SaleDate, s.Notes, s.CustomerId,
        c.Name AS CustomerName,
        d.Id AS DetailId, d.ProductId, d.Quantity, d.Price, d.SalePrice,
        p.Name AS ProductName,
        p.UnitTypeId,
        ut.Name AS UnitName,
        es.Name AS EggSizeName,
        et.Name AS EggTypeName
    FROM Sales s
    JOIN Customer c ON s.CustomerId = c.Id
    LEFT JOIN SaleDetails d ON s.Id = d.SaleId
    LEFT JOIN Products p ON d.ProductId = p.Id
    LEFT JOIN UnitTypes ut ON p.UnitTypeId = ut.Id
    LEFT JOIN EggSize es ON p.EggSizeId = es.Id
    LEFT JOIN EggType et ON p.EggTypeId = et.Id
    WHERE s.Id = @SaleId;
";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SaleId", saleId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (sale == null)
                            {
                                sale = new Sale
                                {
                                    Id = reader.GetInt32("Id"),
                                    SaleDate = reader.GetDateTime("SaleDate"),
                                    CustomerId = reader.GetInt32("CustomerId"),
                                    CustomerName = reader.GetString("CustomerName"),
                                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes"),
                                    Details = new List<SaleDetail>()
                                };
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("DetailId")))
                            {
                                var detail = new SaleDetail
                                {
                                    Id = reader.GetInt32("DetailId"),
                                    ProductId = reader.GetInt32("ProductId"),
                                    ProductName = reader.GetString("EggTypeName"),
                                    UnitTypeId = reader.GetInt32("UnitTypeId"),
                                    UnitName = reader.GetString("UnitName"),
                                    EggSizeName = reader.GetString("EggSizeName"),
                                    Quantity = reader.GetDecimal("Quantity"),
                                    Price = reader.GetDecimal("Price"),
                                    SalePrice = reader.GetDecimal("SalePrice")
                                };

                                sale.Details.Add(detail);
                            }
                        }
                    }
                }
            }

            return sale;
        }

        public int Insert(Sale sale)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Insertar venta principal
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

                        // Insertar detalles + actualizar inventario
                        foreach (var detail in sale.Details)
                        {
                            // Insertar detalle en DB
                            string insertDetail = @"
                        INSERT INTO SaleDetails 
                            (SaleId, ProductId, Quantity, Price, SalePrice)
                        VALUES 
                            (@SaleId, @ProductId, @Quantity, @Price, @SalePrice);";

                            using (var cmd = new MySqlCommand(insertDetail, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@SaleId", saleId);
                                cmd.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                cmd.Parameters.AddWithValue("@Price", detail.Price);
                                cmd.Parameters.AddWithValue("@SalePrice", detail.SalePrice);
                                cmd.ExecuteNonQuery();
                            }

                            // Calcular cantidad real a descontar
                            // Aquí asumimos que la conversión depende del ProductId
                            int conversionFactor = objConversion.GetConversionFactor(detail.UnitTypeId);
                            decimal qtyToDiscount = detail.Quantity * conversionFactor;

                            // Diccionario de mapeo de inventario
                            var eggProductMap = objMaps.GetEggProductMap();

                            // Determinar el ProductId que realmente existe en inventario
                            int inventoryProductId = eggProductMap.ContainsKey(detail.ProductId)
                                                        ? eggProductMap[detail.ProductId]
                                                        : detail.ProductId;

                            // Actualizar inventario
                            string updateInv = @"
                        UPDATE EggInventory
                        SET Quantity = Quantity - @Qty, LastUpdated = NOW()
                        WHERE ProductId = @ProductId;";

                            using (var cmd = new MySqlCommand(updateInv, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Qty", qtyToDiscount);
                                cmd.Parameters.AddWithValue("@ProductId", inventoryProductId);
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

        public void Update(Sale sale)
        {
            var eggProductMap = objMaps.GetEggProductMap();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Actualizar encabezado de la venta
                        string updateSale = @"
                    UPDATE Sales 
                    SET SaleDate=@SaleDate, CustomerId=@CustomerId, Notes=@Notes
                    WHERE Id=@Id;";
                        using (var cmd = new MySqlCommand(updateSale, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                            cmd.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
                            cmd.Parameters.AddWithValue("@Notes", sale.Notes ?? "");
                            cmd.Parameters.AddWithValue("@Id", sale.Id);
                            cmd.ExecuteNonQuery();
                        }

                        // Obtener detalles antiguos
                        string getOldDetails = @"SELECT ProductId, Quantity FROM SaleDetails WHERE SaleId=@SaleId;";
                        var oldDetails = new List<(int ProductId, decimal Quantity)>();
                        using (var cmd = new MySqlCommand(getOldDetails, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleId", sale.Id);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    oldDetails.Add((
                                        reader.GetInt32("ProductId"),
                                        reader.GetDecimal("Quantity")
                                    ));
                                }
                            }
                        }

                        // Revertir inventario de detalles antiguos
                        foreach (var d in oldDetails)
                        {
                            // Usar unitTypeId desde el objeto SaleDetail en memoria
                            var detailInMemory = sale.Details.FirstOrDefault(x => x.ProductId == d.ProductId);
                            int conversionFactor = detailInMemory != null ? objConversion.GetConversionFactor(detailInMemory.UnitTypeId) : 1;
                            decimal qtyToReturn = d.Quantity * conversionFactor;

                            int inventoryProductId = eggProductMap.ContainsKey(d.ProductId) ? eggProductMap[d.ProductId] : d.ProductId;

                            string revertInv = @"
                        UPDATE EggInventory
                        SET Quantity = Quantity + @Qty, LastUpdated = NOW()
                        WHERE ProductId = @ProductId;";
                            using (var cmd = new MySqlCommand(revertInv, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Qty", qtyToReturn);
                                cmd.Parameters.AddWithValue("@ProductId", inventoryProductId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Eliminar detalles antiguos
                        string deleteDetails = @"DELETE FROM SaleDetails WHERE SaleId=@SaleId;";
                        using (var cmd = new MySqlCommand(deleteDetails, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleId", sale.Id);
                            cmd.ExecuteNonQuery();
                        }

                        // Insertar nuevos detalles y actualizar inventario
                        foreach (var detail in sale.Details)
                        {
                            // Insertar detalle (sin UnitTypeId)
                            string insertDetail = @"
                        INSERT INTO SaleDetails (SaleId, ProductId, Quantity, Price, SalePrice)
                        VALUES (@SaleId, @ProductId, @Quantity, @Price, @SalePrice);";
                            using (var cmd = new MySqlCommand(insertDetail, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@SaleId", sale.Id);
                                cmd.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                cmd.Parameters.AddWithValue("@Price", detail.Price);
                                cmd.Parameters.AddWithValue("@SalePrice", detail.SalePrice);
                                cmd.ExecuteNonQuery();
                            }

                            // Actualizar inventario
                            int conversionFactor = objConversion.GetConversionFactor(detail.UnitTypeId);
                            decimal qtyToDeduct = detail.Quantity * conversionFactor;
                            int inventoryProductId = eggProductMap.ContainsKey(detail.ProductId) ? eggProductMap[detail.ProductId] : detail.ProductId;

                            string updateInv = @"
                        UPDATE EggInventory
                        SET Quantity = Quantity - @Qty, LastUpdated = NOW()
                        WHERE ProductId = @ProductId;";
                            using (var cmd = new MySqlCommand(updateInv, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Qty", qtyToDeduct);
                                cmd.Parameters.AddWithValue("@ProductId", inventoryProductId);
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

        public void Delete(int saleId, List<SaleDetail> saleDetailsInMemory)
        {
            var eggProductMap = objMaps.GetEggProductMap();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Revertir inventario según los detalles que vienen en memoria                      
                        foreach (var detail in saleDetailsInMemory)
                        {
                            int conversionFactor = objConversion.GetConversionFactor(detail.UnitTypeId);
                            decimal qtyToReturn = detail.Quantity * conversionFactor;

                            int inventoryProductId = eggProductMap.ContainsKey(detail.ProductId)
                                                        ? eggProductMap[detail.ProductId]
                                                        : detail.ProductId;

                            string revertInv = @"
                        UPDATE EggInventory
                        SET Quantity = Quantity + @Qty, LastUpdated = NOW()
                        WHERE ProductId = @ProductId;";
                            using (var cmd = new MySqlCommand(revertInv, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Qty", qtyToReturn);
                                cmd.Parameters.AddWithValue("@ProductId", inventoryProductId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Eliminar detalles de la venta
                        string deleteDetails = @"DELETE FROM SaleDetails WHERE SaleId=@SaleId;";
                        using (var cmd = new MySqlCommand(deleteDetails, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleId", saleId);
                            cmd.ExecuteNonQuery();
                        }

                        // Eliminar la venta
                        string deleteSale = @"DELETE FROM Sales WHERE Id=@SaleId;";
                        using (var cmd = new MySqlCommand(deleteSale, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@SaleId", saleId);
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
