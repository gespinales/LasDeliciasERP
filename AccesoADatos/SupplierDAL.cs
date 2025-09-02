using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class SupplierDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<Supplier> GetAll()
        {
            List<Supplier> list = new List<Supplier>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM Suppliers ORDER BY Name ASC";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Supplier
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? "" : reader.GetString("ContactName"),
                            Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString("Phone"),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email"),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? "" : reader.GetString("Address"),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes")
                        });
                    }
                }
            }

            return list;
        }

        public Supplier GetById(int id)
        {
            Supplier supplier = null;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM Suppliers WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        supplier = new Supplier
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? "" : reader.GetString("ContactName"),
                            Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString("Phone"),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email"),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? "" : reader.GetString("Address"),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes")
                        };
                    }
                }
            }

            return supplier;
        }

        public void Insert(Supplier supplier)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"INSERT INTO Suppliers (Name, ContactName, Phone, Email, Address, Notes)
                                 VALUES (@Name, @ContactName, @Phone, @Email, @Address, @Notes)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", supplier.Name);
                cmd.Parameters.AddWithValue("@ContactName", string.IsNullOrEmpty(supplier.ContactName) ? (object)DBNull.Value : supplier.ContactName);
                cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(supplier.Phone) ? (object)DBNull.Value : supplier.Phone);
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(supplier.Email) ? (object)DBNull.Value : supplier.Email);
                cmd.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(supplier.Address) ? (object)DBNull.Value : supplier.Address);
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(supplier.Notes) ? (object)DBNull.Value : supplier.Notes);

                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Supplier supplier)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"UPDATE Suppliers
                                 SET Name = @Name,
                                     ContactName = @ContactName,
                                     Phone = @Phone,
                                     Email = @Email,
                                     Address = @Address,
                                     Notes = @Notes
                                 WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", supplier.Name);
                cmd.Parameters.AddWithValue("@ContactName", string.IsNullOrEmpty(supplier.ContactName) ? (object)DBNull.Value : supplier.ContactName);
                cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(supplier.Phone) ? (object)DBNull.Value : supplier.Phone);
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(supplier.Email) ? (object)DBNull.Value : supplier.Email);
                cmd.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(supplier.Address) ? (object)DBNull.Value : supplier.Address);
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(supplier.Notes) ? (object)DBNull.Value : supplier.Notes);
                cmd.Parameters.AddWithValue("@Id", supplier.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM Suppliers WHERE Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
