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

        public List<Sale> GetAll(DateTime? startDate = null, DateTime? endDate = null)
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

        public List<SaleDetail> GetDetailsBySaleId(int saleId)
        {
            var list = new List<SaleDetail>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
            SELECT sd.Id, sd.SaleId, sd.ProductId, sd.Quantity, sd.Price
            FROM SaleDetails sd
            WHERE sd.SaleId=@SaleId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SaleId", saleId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SaleDetail
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SaleId = Convert.ToInt32(reader["SaleId"]),
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                Quantity = Convert.ToDecimal(reader["Quantity"]),
                                Price = Convert.ToDecimal(reader["Price"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        //public void Insert(Sale sale, List<SaleDetail> details)
        //{
        //    using (var conn = new MySqlConnection(connString))
        //    {
        //        conn.Open();
        //        using (var tran = conn.BeginTransaction())
        //        {
        //            try
        //            {
        //                // 1️ Insertar venta
        //                string insertSale = @"INSERT INTO Sales (SaleDate, CustomerId, Notes) 
        //                              VALUES (@SaleDate, @CustomerId, @Notes); 
        //                              SELECT LAST_INSERT_ID();";
        //                var cmdSale = new MySqlCommand(insertSale, conn, tran);
        //                cmdSale.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
        //                cmdSale.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
        //                cmdSale.Parameters.AddWithValue("@Notes", (object)sale.Notes ?? DBNull.Value);
        //                int saleId = Convert.ToInt32(cmdSale.ExecuteScalar());

        //                // 2️ Insertar detalles y ajustar inventario si aplica
        //                foreach (var d in details)
        //                {
        //                    // Obtener precio vigente al momento de la venta
        //                    decimal price = GetPriceAtDate(d.ProductId, sale.SaleDate, conn, tran);

        //                    // Insertar detalle de venta
        //                    string insertDetail = @"INSERT INTO SaleDetails (SaleId, ProductId, Quantity, Price) 
        //                                    VALUES (@SaleId, @ProductId, @Quantity, @Price)";
        //                    var cmdDetail = new MySqlCommand(insertDetail, conn, tran);
        //                    cmdDetail.Parameters.AddWithValue("@SaleId", saleId);
        //                    cmdDetail.Parameters.AddWithValue("@ProductId", d.ProductId);
        //                    cmdDetail.Parameters.AddWithValue("@Quantity", d.Quantity);
        //                    cmdDetail.Parameters.AddWithValue("@Price", price);
        //                    cmdDetail.ExecuteNonQuery();

        //                    // Ajustar inventario si es producto tipo huevo
        //                    if (IsEggProduct(d.ProductId, conn, tran))
        //                    {
        //                        // Obtener EggTypeId del producto
        //                        int eggTypeId = GetEggTypeIdForProduct(d.ProductId, conn, tran);

        //                        // Obtener columna correspondiente al tamaño del huevo
        //                        string sizeColumn = GetEggSizeColumn(d.Size); // "S", "M", "L" o "XL"

        //                        string updateInventory = $@"
        //                    UPDATE EggInventory
        //                    SET {sizeColumn} = {sizeColumn} - @Quantity, LastUpdated = @LastUpdated
        //                    WHERE EggTypeId = @EggTypeId";

        //                        var cmdInv = new MySqlCommand(updateInventory, conn, tran);
        //                        cmdInv.Parameters.AddWithValue("@Quantity", d.Quantity);
        //                        cmdInv.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
        //                        cmdInv.Parameters.AddWithValue("@EggTypeId", eggTypeId);
        //                        cmdInv.ExecuteNonQuery();
        //                    }
        //                }

        //                // 3️ Confirmar transacción
        //                tran.Commit();
        //            }
        //            catch
        //            {
        //                tran.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}

        //private int GetEggTypeIdForProduct(int productId, MySqlConnection conn, MySqlTransaction tran)
        //{
        //    string query = @"SELECT EggTypeId 
        //             FROM Products 
        //             WHERE Id = @ProductId 
        //               AND IsEgg = 1 
        //             LIMIT 1";

        //    using (var cmd = new MySqlCommand(query, conn, tran))
        //    {
        //        cmd.Parameters.AddWithValue("@ProductId", productId);
        //        object result = cmd.ExecuteScalar();
        //        if (result != null && int.TryParse(result.ToString(), out int eggTypeId))
        //            return eggTypeId;
        //        else
        //            throw new Exception($"No se encontró EggTypeId para el ProductId {productId} o el producto no es huevo.");
        //    }
        //}



        //// Función auxiliar para mapear tamaño de huevo a columna
        //private string GetEggSizeColumn(string size)
        //{
        //    return size switch
        //    {
        //        "S" => "QuantityS",
        //        "M" => "QuantityM",
        //        "L" => "QuantityL",
        //        "XL" => "QuantityXL",
        //        _ => throw new ArgumentException("Tamaño de huevo inválido")
        //    };
        //}

        private decimal GetPriceAtDate(int productId, DateTime saleDate, MySqlConnection conn, MySqlTransaction tran)
        {
            string sql = @"SELECT Price FROM ProductPrices 
                           WHERE ProductId=@ProductId AND StartDate <= @SaleDate 
                             AND (EndDate IS NULL OR EndDate >= @SaleDate)
                           ORDER BY StartDate DESC LIMIT 1";
            var cmd = new MySqlCommand(sql, conn, tran);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@SaleDate", saleDate);
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToDecimal(result) : 0m;
        }

        private bool IsEggProduct(int productId, MySqlConnection conn, MySqlTransaction tran)
        {
            string sql = "SELECT COUNT(*) FROM Products WHERE Id=@Id AND EggTypeId IS NOT NULL";
            var cmd = new MySqlCommand(sql, conn, tran);
            cmd.Parameters.AddWithValue("@Id", productId);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public void Update(Sale sale, List<SaleDetail> details)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Obtener detalles antiguos para ajustar inventario
                        var oldDetails = GetDetailsBySaleId(sale.Id);

                        foreach (var old in oldDetails)
                        {
                            var product = new ProductDAL().GetById(old.ProductId);
                            if (product.EggTypeId.HasValue)
                            {
                                string updateStock = @"
                            UPDATE EggInventory
                            SET Quantity = Quantity + @Quantity
                            WHERE ProductId=@ProductId";
                                var cmdStock = new MySqlCommand(updateStock, conn, tran);
                                cmdStock.Parameters.AddWithValue("@Quantity", old.Quantity);
                                cmdStock.Parameters.AddWithValue("@ProductId", old.ProductId);
                                cmdStock.ExecuteNonQuery();
                            }
                        }

                        // 2. Actualizar venta
                        string updateSale = @"
                    UPDATE Sales
                    SET CustomerId=@CustomerId, SaleDate=@SaleDate, Notes=@Notes
                    WHERE Id=@Id";
                        var cmdSale = new MySqlCommand(updateSale, conn, tran);
                        cmdSale.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
                        cmdSale.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                        cmdSale.Parameters.AddWithValue("@Notes", (object)(sale.Notes?.Substring(0, Math.Min(sale.Notes.Length, 250))) ?? DBNull.Value);
                        cmdSale.Parameters.AddWithValue("@Id", sale.Id);
                        cmdSale.ExecuteNonQuery();

                        // 3. Eliminar detalles antiguos
                        string deleteDetails = "DELETE FROM SaleDetails WHERE SaleId=@SaleId";
                        var cmdDelete = new MySqlCommand(deleteDetails, conn, tran);
                        cmdDelete.Parameters.AddWithValue("@SaleId", sale.Id);
                        cmdDelete.ExecuteNonQuery();

                        // 4. Insertar nuevos detalles y ajustar inventario de huevos
                        foreach (var detail in details)
                        {
                            string insertDetail = @"
                        INSERT INTO SaleDetails (SaleId, ProductId, Quantity, Price)
                        VALUES (@SaleId, @ProductId, @Quantity, @Price)";
                            var cmdDetail = new MySqlCommand(insertDetail, conn, tran);
                            cmdDetail.Parameters.AddWithValue("@SaleId", sale.Id);
                            cmdDetail.Parameters.AddWithValue("@ProductId", detail.ProductId);
                            cmdDetail.Parameters.AddWithValue("@Quantity", detail.Quantity);
                            cmdDetail.Parameters.AddWithValue("@Price", detail.Price);
                            cmdDetail.ExecuteNonQuery();

                            var product = new ProductDAL().GetById(detail.ProductId);
                            if (product.EggTypeId.HasValue)
                            {
                                string updateStock = @"
                            UPDATE EggInventory
                            SET Quantity = Quantity - @Quantity
                            WHERE ProductId=@ProductId";
                                var cmdStock = new MySqlCommand(updateStock, conn, tran);
                                cmdStock.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                cmdStock.Parameters.AddWithValue("@ProductId", detail.ProductId);
                                cmdStock.ExecuteNonQuery();
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


        public void Delete(int saleId)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Ajustar inventario de huevos antes de eliminar detalles
                        string sqlDetails = "SELECT ProductId, Quantity FROM SaleDetails WHERE SaleId=@SaleId";
                        var cmdDetails = new MySqlCommand(sqlDetails, conn, tran);
                        cmdDetails.Parameters.AddWithValue("@SaleId", saleId);
                        using (var reader = cmdDetails.ExecuteReader())
                        {
                            var eggAdjustments = new List<(int productId, decimal qty)>();
                            while (reader.Read())
                            {
                                int pid = Convert.ToInt32(reader["ProductId"]);
                                decimal qty = Convert.ToDecimal(reader["Quantity"]);
                                if (IsEggProduct(pid, conn, tran))
                                    eggAdjustments.Add((pid, qty));
                            }
                            reader.Close();

                            foreach (var adj in eggAdjustments)
                            {
                                string updateInventory = @"
                                    UPDATE EggInventory
                                    SET Quantity = Quantity + @Quantity, LastUpdated=@LastUpdated
                                    WHERE ProductId=@ProductId";
                                var cmdInv = new MySqlCommand(updateInventory, conn, tran);
                                cmdInv.Parameters.AddWithValue("@Quantity", adj.qty);
                                cmdInv.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                                cmdInv.Parameters.AddWithValue("@ProductId", adj.productId);
                                cmdInv.ExecuteNonQuery();
                            }
                        }

                        // Eliminar detalles
                        string deleteDetails = "DELETE FROM SaleDetails WHERE SaleId=@SaleId";
                        var cmdDelDetails = new MySqlCommand(deleteDetails, conn, tran);
                        cmdDelDetails.Parameters.AddWithValue("@SaleId", saleId);
                        cmdDelDetails.ExecuteNonQuery();

                        // Eliminar venta
                        string deleteSale = "DELETE FROM Sales WHERE Id=@Id";
                        var cmdDelSale = new MySqlCommand(deleteSale, conn, tran);
                        cmdDelSale.Parameters.AddWithValue("@Id", saleId);
                        cmdDelSale.ExecuteNonQuery();

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
