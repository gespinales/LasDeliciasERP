using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;

namespace LasDeliciasERP.AccesoADatos
{
    public class CustomerDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        // Obtener todos los clientes
        public List<Customer> GetAll()
        {
            List<Customer> list = new List<Customer>();
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM Customer ORDER BY Name ASC";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Customer
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
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

        // Obtener cliente por Id
        public Customer GetById(int id)
        {
            Customer customer = null;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM Customer WHERE Id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString("Phone"),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email"),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? "" : reader.GetString("Address"),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes")
                        };
                    }
                }
            }
            return customer;
        }

        // Insertar nuevo cliente
        public void Insert(Customer customer)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"INSERT INTO Customer (Name, Phone, Email, Address, Notes)
                                 VALUES (@Name, @Phone, @Email, @Address, @Notes)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@Address", customer.Address);
                cmd.Parameters.AddWithValue("@Notes", customer.Notes);
                cmd.ExecuteNonQuery();
            }
        }

        // Actualizar cliente existente
        public void Update(Customer customer)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"UPDATE Customer SET 
                                    Name = @Name, 
                                    Phone = @Phone, 
                                    Email = @Email, 
                                    Address = @Address, 
                                    Notes = @Notes
                                 WHERE Id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@Address", customer.Address);
                cmd.Parameters.AddWithValue("@Notes", customer.Notes);
                cmd.Parameters.AddWithValue("@Id", customer.Id);
                cmd.ExecuteNonQuery();
            }
        }

        // Eliminar cliente por Id
        public void Delete(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM Customer WHERE Id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}