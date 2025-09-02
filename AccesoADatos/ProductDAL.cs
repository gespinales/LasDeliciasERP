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

        public List<dynamic> GetAllEggsWithUnitAndPrice()
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
            p.UnitTypeId,
            ut.Name AS UnitName,
            p.EggSizeId,
            es.Name AS EggSizeName,
            p.EggTypeId,
            et.Name AS EggTypeName,
            pp.Price
        FROM Products p
        JOIN UnitTypes ut ON p.UnitTypeId = ut.Id
        JOIN EggSize es ON p.EggSizeId = es.Id
        JOIN EggType et ON p.EggTypeId = et.Id
        JOIN ProductPrices pp 
            ON pp.ProductId = p.Id 
            AND (pp.EndDate IS NULL OR pp.EndDate > NOW())
        ORDER BY p.Name, et.Name, ut.Name, es.Name";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["EggTypeName"].ToString(),
                            Notes = reader["Notes"]?.ToString(),
                            UnitTypeId = reader["UnitTypeId"] != DBNull.Value ? Convert.ToInt32(reader["UnitTypeId"]) : 0,
                            UnitName = reader["UnitName"]?.ToString(),
                            EggSizeId = reader["EggSizeId"] != DBNull.Value ? Convert.ToInt32(reader["EggSizeId"]) : 0,
                            EggSizeName = reader["EggSizeName"]?.ToString(),
                            EggTypeId = reader["EggTypeId"] != DBNull.Value ? Convert.ToInt32(reader["EggTypeId"]) : 0,
                            EggTypeName = reader["EggTypeName"]?.ToString(),
                            Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0m
                        });
                    }
                }
            }
            return list;
        }

        public List<dynamic> GetAllProductsWithUnit()
        {
            var list = new List<dynamic>();

            string sql = @"
        SELECT 
            p.Id, 
            p.Name, 
            p.UnitTypeId,
            ut.Name AS UnitName
        FROM Products p
        JOIN UnitTypes ut ON p.UnitTypeId = ut.Id
        WHERE p.ProductTypeId != 1
        ORDER BY p.Name, ut.Name;";

            using (var conn = new MySqlConnection(connString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            UnitTypeId = reader.GetInt32("UnitTypeId"),
                            UnitName = reader.GetString("UnitName")
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

        public List<Product> GetAllEggProductsUnit()
        {
            var list = new List<Product>();
            using (var conn = new MySqlConnection(connString))
            {
                conn.Open();
                string sql = @"
                SELECT p.Id, p.Name, p.UnitTypeId, p.EggTypeId, p.EggSizeId, p.Notes,
                       et.Name AS EggTypeName,
                       es.Name AS EggSizeName,
                       CONCAT(et.Name, ' - ', es.Name) AS DisplayName
                FROM Products p
                JOIN EggType et ON p.EggTypeId = et.Id
                JOIN EggSize es ON p.EggSizeId = es.Id
                WHERE p.EggTypeId IS NOT NULL
                  AND p.UnitTypeId = 3
                  AND p.ProductTypeId = 1  -- 1 = Huevo
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
    }
}
