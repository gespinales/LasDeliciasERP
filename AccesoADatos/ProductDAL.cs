using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace LasDeliciasERP.AccesoADatos
{
    public class ProductDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<Product> GetAll()
        {
            var list = new List<Product>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"SELECT * FROM Products";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Product
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            UnitTypeId = Convert.ToInt32(reader["UnitTypeId"]),
                            Notes = reader["Notes"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<dynamic> GetAllWithUnitAndPrice()
        {
            var list = new List<dynamic>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
            SELECT 
                p.Id, 
                p.Name, 
                p.Notes, 
                ut.Name AS UnitName,
                pp.Price
            FROM Products p
            LEFT JOIN UnitTypes ut ON p.UnitTypeId = ut.Id
            LEFT JOIN ProductPrices pp 
                ON pp.ProductId = p.Id 
                AND (pp.EndDate IS NULL OR pp.EndDate > NOW())
            ORDER BY p.Name";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            UnitName = reader["UnitName"]?.ToString(),
                            Notes = reader["Notes"]?.ToString(),
                            Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0m
                        });
                    }
                }
            }
            return list;
        }

        public List<UnitType> GetByProductId(int productId)
        {
            var list = new List<UnitType>();

            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();

                string query = @"
                SELECT u.Id, u.Name
                FROM ProductUnits pu
                INNER JOIN UnitTypes u ON pu.UnitTypeId = u.Id
                WHERE pu.ProductId = @ProductId;";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var unit = new UnitType
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name")
                            };
                            list.Add(unit);
                        }
                    }
                }
            }

            return list;
        }

        public Product GetById(int id)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"SELECT * FROM Products WHERE Id=@Id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                UnitTypeId = Convert.ToInt32(reader["UnitTypeId"]),
                                Notes = reader["Notes"]?.ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Product GetEggproductById(int id)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();

                string sql = @"
                SELECT p.Id, p.Name, p.UnitTypeId, p.EggTypeId, p.Notes,
                       et.Name AS EggTypeName,
                       es.Name AS EggSizeName,
                       CONCAT(et.Name, ' - ', es.Name) AS DisplayName
                FROM Products p
                JOIN EggType et ON p.EggTypeId = et.Id
                JOIN EggSize es ON p.EggSizeId = es.Id
                WHERE p.EggTypeId IS NOT NULL
                  AND p.Id=@Id
                ORDER BY et.Name, es.Id";

                //    string sql = @"
                //SELECT ep.Id, ep.ProductionDate, ep.Notes, ep.BarnId,
                //    b.Name AS BarnName,
                //    d.ProductId, p.Name AS ProductName, p.UnitTypeId, d.Quantity,
                //    et.Name AS EggTypeName,
                //    es.Name AS EggSizeName,
                //    CONCAT(et.Name, ' - ', es.Name) AS DisplayName
                //FROM EggProduction ep
                //JOIN Barn b ON ep.BarnId = b.Id
                //LEFT JOIN EggProductionDetail d ON ep.Id = d.EggProductionId
                //LEFT JOIN Products p ON d.ProductId = p.Id
                //LEFT JOIN EggType et ON p.EggTypeId = et.Id
                //LEFT JOIN EggSize es ON p.EggSizeId = es.Id
                //WHERE ep.Id = @Id
                //ORDER BY et.Name, es.Id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["DisplayName"].ToString(),
                                UnitTypeId = Convert.ToInt32(reader["UnitTypeId"]),
                                Notes = reader["Notes"]?.ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Product> GetAllEggProducts()
        {
            var list = new List<Product>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"SELECT * FROM Products WHERE EggTypeId IS NOT NULL ORDER BY Name";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Product
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            UnitTypeId = Convert.ToInt32(reader["UnitTypeId"]),
                            EggTypeId = reader["EggTypeId"] != DBNull.Value ? Convert.ToInt32(reader["EggTypeId"]) : 0,
                            Notes = reader["Notes"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<Product> GetAllEggProductsUnit()
        {
            var list = new List<Product>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
            SELECT p.Id, p.Name, p.UnitTypeId, p.EggTypeId, p.Notes,
                   et.Name AS EggTypeName,
                   es.Name AS EggSizeName,
                   CONCAT(et.Name, ' - ', es.Name) AS DisplayName
            FROM Products p
            JOIN EggType et ON p.EggTypeId = et.Id
            JOIN EggSize es ON p.EggSizeId = es.Id
            WHERE p.EggTypeId IS NOT NULL
              AND p.UnitTypeId = 3
            ORDER BY et.Name, es.Id";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Product
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["DisplayName"].ToString(),
                            UnitTypeId = Convert.ToInt32(reader["UnitTypeId"]),
                            EggTypeId = reader["EggTypeId"] != DBNull.Value ? Convert.ToInt32(reader["EggTypeId"]) : 0,
                            Notes = reader["Notes"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        public int? GetProductIdByEggTypeAndSize(int eggTypeId, int eggSizeId)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
            SELECT Id
            FROM Products
            WHERE EggTypeId = @EggTypeId
              AND EggSizeId = @EggSizeId
            LIMIT 1";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EggTypeId", eggTypeId);
                    cmd.Parameters.AddWithValue("@EggSizeId", eggSizeId);

                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        return Convert.ToInt32(result);
                    else
                        return null;
                }
            }
        }

        public void Insert(Product product)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"INSERT INTO Products (Name, UnitTypeId, Notes)
                               VALUES (@Name, @UnitTypeId, @Notes)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@UnitTypeId", product.UnitTypeId);
                    cmd.Parameters.AddWithValue("@Notes", (object)product.Notes ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Product product)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"UPDATE Products 
                               SET Name=@Name, UnitTypeId=@UnitTypeId, Notes=@Notes
                               WHERE Id=@Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", product.Id);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@UnitTypeId", product.UnitTypeId);
                    cmd.Parameters.AddWithValue("@Notes", (object)product.Notes ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"DELETE FROM Products WHERE Id=@Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
