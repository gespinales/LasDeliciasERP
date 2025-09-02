using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class InventoryDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        /// <summary>
        /// Obtiene el inventario completo con la cantidad actual por producto.
        /// </summary>
        public List<Inventory> GetAll()
        {
            var list = new List<Inventory>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    SELECT i.ProductId, p.Name AS ProductName, i.Quantity, i.LastUpdated
                    FROM Inventory i
                    INNER JOIN Products p ON i.ProductId = p.Id
                    ORDER BY p.Name;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Inventory
                        {
                            ProductId = reader.GetInt32("ProductId"),
                            ProductName = reader.GetString("ProductName"),
                            Quantity = reader.GetDecimal("Quantity"),
                            LastUpdated = reader.GetDateTime("LastUpdated")
                        });
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Obtiene un producto específico del inventario.
        /// </summary>
        public Inventory GetById(int productId)
        {
            Inventory item = null;

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    SELECT i.ProductId, p.Name AS ProductName, i.Quantity, i.LastUpdated
                    FROM Inventory i
                    INNER JOIN Products p ON i.ProductId = p.Id
                    WHERE i.ProductId = @ProductId;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new Inventory
                        {
                            ProductId = reader.GetInt32("ProductId"),
                            ProductName = reader.GetString("ProductName"),
                            Quantity = reader.GetDecimal("Quantity"),
                            LastUpdated = reader.GetDateTime("LastUpdated")
                        };
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Obtiene un resumen del inventario mostrando producto, cantidad en libras y última actualización.
        /// </summary>
        public List<Inventory> GetInventorySummary()
        {
            var list = new List<Inventory>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT 
                p.Name AS ProductName,
                i.Quantity,
                ut.Name AS UnitName,
                i.LastUpdated
            FROM Inventory i
            INNER JOIN Products p ON i.ProductId = p.Id
            INNER JOIN UnitTypes ut ON p.UnitTypeId = ut.Id
            ORDER BY p.Name;";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Inventory
                        {
                            ProductName = reader.GetString("ProductName"),
                            Quantity = reader.GetDecimal("Quantity"),
                            UnitName = reader.GetString("UnitName"),
                            LastUpdated = reader.GetDateTime("LastUpdated")
                        });
                    }
                }
            }

            return list;
        }



        /// <summary>
        /// Inserta o actualiza un producto en inventario.
        /// Si el producto ya existe, suma la cantidad.
        /// </summary>
        public void InsertOrUpdate(Inventory item)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    INSERT INTO Inventory (ProductId, Quantity, LastUpdated)
                    VALUES (@ProductId, @Quantity, NOW())
                    ON DUPLICATE KEY UPDATE
                        Quantity = Quantity + @Quantity,
                        LastUpdated = NOW();";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                cmd.Parameters.AddWithValue("@Quantity", item.Quantity);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza la cantidad exacta de un producto en inventario.
        /// </summary>
        public void Update(Inventory item)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    UPDATE Inventory
                    SET Quantity = @Quantity,
                        LastUpdated = NOW()
                    WHERE ProductId = @ProductId;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                cmd.Parameters.AddWithValue("@Quantity", item.Quantity);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina un producto del inventario (opcional, en la práctica no se usa mucho).
        /// </summary>
        public void Delete(int productId)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM Inventory WHERE ProductId = @ProductId;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
