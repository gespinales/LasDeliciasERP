using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class VaccinationRecordDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public List<VaccinationRecord> GetAll()
        {
            List<VaccinationRecord> list = new List<VaccinationRecord>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT vr.Id, vr.ScheduleId, vr.AppliedDate, vr.QuantityApplied, 
                                        vr.AppliedBy, vr.Notes,
                                        b.Name AS BarnName, v.Name AS VaccineName
                                 FROM VaccinationRecord vr
                                 INNER JOIN VaccinationSchedule vs ON vr.ScheduleId = vs.Id
                                 INNER JOIN Barn b ON vs.BarnId = b.Id
                                 INNER JOIN Vaccine v ON vs.VaccineId = v.Id
                                 ORDER BY vr.AppliedDate DESC";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new VaccinationRecord
                        {
                            Id = reader.GetInt32("Id"),
                            ScheduleId = reader.GetInt32("ScheduleId"),
                            AppliedDate = reader.GetDateTime("AppliedDate"),
                            QuantityApplied = reader.IsDBNull(reader.GetOrdinal("QuantityApplied")) ? 0 : reader.GetInt32("QuantityApplied"),
                            AppliedBy = reader.IsDBNull(reader.GetOrdinal("AppliedBy")) ? "" : reader.GetString("AppliedBy"),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes"),
                            BarnName = reader.GetString("BarnName"),
                            VaccineName = reader.GetString("VaccineName")
                        });
                    }
                }
            }
            return list;
        }

        public VaccinationRecord GetByScheduleId(int scheduleId)
        {
            VaccinationRecord record = null;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT vr.Id, vr.ScheduleId, vr.AppliedDate, vr.QuantityApplied, vr.AppliedBy, vr.Notes
                         FROM VaccinationRecord vr
                         WHERE vr.ScheduleId = @ScheduleId
                         LIMIT 1"; 

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            record = new VaccinationRecord
                            {
                                Id = reader.GetInt32("Id"),
                                ScheduleId = reader.GetInt32("ScheduleId"),
                                AppliedDate = reader.GetDateTime("AppliedDate"),
                                QuantityApplied = reader.GetInt32("QuantityApplied"),
                                AppliedBy = reader.IsDBNull(reader.GetOrdinal("AppliedBy")) ? "" : reader.GetString("AppliedBy"),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes")
                            };
                        }
                    }
                }
            }

            return record;
        }

        public void Insert(VaccinationRecord record)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"INSERT INTO VaccinationRecord 
                                (ScheduleId, AppliedDate, QuantityApplied, AppliedBy, Notes)
                                 VALUES (@ScheduleId, @AppliedDate, @QuantityApplied, @AppliedBy, @Notes)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ScheduleId", record.ScheduleId);
                cmd.Parameters.AddWithValue("@AppliedDate", record.AppliedDate);
                cmd.Parameters.AddWithValue("@QuantityApplied", record.QuantityApplied);
                cmd.Parameters.AddWithValue("@AppliedBy", string.IsNullOrEmpty(record.AppliedBy) ? (object)DBNull.Value : record.AppliedBy);
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(record.Notes) ? (object)DBNull.Value : record.Notes);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM VaccinationRecord WHERE Id=@Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
