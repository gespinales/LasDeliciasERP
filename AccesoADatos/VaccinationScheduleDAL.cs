using LasDeliciasERP.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LasDeliciasERP.AccesoADatos
{
    public class VaccinationScheduleDAL
    {
        private string connString = ConfigurationManager.ConnectionStrings["EJDMDConn"].ConnectionString;

        public class VaccinationScheduleWithRecord : VaccinationSchedule
        {
            public DateTime? AppliedDate { get; set; }
            public string AppliedBy { get; set; }
            public decimal QuantityApplied { get; set; }
        }

        public List<VaccinationScheduleWithRecord> GetAllWithLastRecord()
        {
            List<VaccinationScheduleWithRecord> list = new List<VaccinationScheduleWithRecord>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();

                string query = @"
            SELECT 
                vs.Id, vs.BarnId, vs.VaccineId, vs.ScheduledDate, vs.Status, vs.Notes,
                b.Name AS BarnName, v.Name AS VaccineName,
                vr.AppliedDate, vr.AppliedBy, vr.QuantityApplied
            FROM VaccinationSchedule vs
            INNER JOIN Barn b ON vs.BarnId = b.Id
            INNER JOIN Vaccine v ON vs.VaccineId = v.Id
            LEFT JOIN (
                SELECT vr1.*
                FROM VaccinationRecord vr1
                INNER JOIN (
                    SELECT ScheduleId, MAX(AppliedDate) AS LastApplied
                    FROM VaccinationRecord
                    GROUP BY ScheduleId
                ) vr2 ON vr1.ScheduleId = vr2.ScheduleId AND vr1.AppliedDate = vr2.LastApplied
            ) vr ON vs.Id = vr.ScheduleId
            ORDER BY vs.ScheduledDate ASC";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new VaccinationScheduleWithRecord
                        {
                            Id = reader.GetInt32("Id"),
                            BarnId = reader.GetInt32("BarnId"),
                            VaccineId = reader.GetInt32("VaccineId"),
                            ScheduledDate = reader.GetDateTime("ScheduledDate"),
                            Status = reader.GetString("Status"),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes"),
                            BarnName = reader.GetString("BarnName"),
                            VaccineName = reader.GetString("VaccineName"),
                            AppliedDate = reader.IsDBNull(reader.GetOrdinal("AppliedDate"))
                                ? (DateTime?)null
                                : reader.GetDateTime("AppliedDate"),
                            AppliedBy = reader.IsDBNull(reader.GetOrdinal("AppliedBy"))
                                ? ""
                                : reader.GetString("AppliedBy"),
                            QuantityApplied = reader.IsDBNull(reader.GetOrdinal("QuantityApplied"))
                                ? 0
                                : reader.GetDecimal("QuantityApplied")
                        });
                    }
                }
            }

            return list;
        }

        public VaccinationSchedule GetById(int id)
        {
            VaccinationSchedule schedule = null;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT vs.Id, vs.BarnId, vs.VaccineId, vs.ScheduledDate, vs.Status, vs.Notes,
                         b.Name AS BarnName, v.Name AS VaccineName
                         FROM VaccinationSchedule vs
                         INNER JOIN Barn b ON vs.BarnId = b.Id
                         INNER JOIN Vaccine v ON vs.VaccineId = v.Id
                         WHERE vs.Id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        schedule = new VaccinationSchedule
                        {
                            Id = reader.GetInt32("Id"),
                            BarnId = reader.GetInt32("BarnId"),
                            VaccineId = reader.GetInt32("VaccineId"),
                            BarnName = reader.GetString("BarnName"),
                            VaccineName = reader.GetString("VaccineName"),
                            ScheduledDate = reader.GetDateTime("ScheduledDate"),
                            Status = reader.GetString("Status"),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes")
                        };
                    }
                }
            }

            return schedule;
        }


        public void Insert(VaccinationSchedule schedule)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"INSERT INTO VaccinationSchedule 
                                (BarnId, VaccineId, ScheduledDate, Status, Notes)
                                 VALUES (@BarnId, @VaccineId, @ScheduledDate, @Status, @Notes)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BarnId", schedule.BarnId);
                cmd.Parameters.AddWithValue("@VaccineId", schedule.VaccineId);
                cmd.Parameters.AddWithValue("@ScheduledDate", schedule.ScheduledDate);
                cmd.Parameters.AddWithValue("@Status", schedule.Status);
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(schedule.Notes) ? (object)DBNull.Value : schedule.Notes);

                cmd.ExecuteNonQuery();
            }
        }

        public void Update(VaccinationSchedule schedule)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = @"UPDATE VaccinationSchedule
                                 SET BarnId=@BarnId, VaccineId=@VaccineId, ScheduledDate=@ScheduledDate,
                                     Status=@Status, Notes=@Notes
                                 WHERE Id=@Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BarnId", schedule.BarnId);
                cmd.Parameters.AddWithValue("@VaccineId", schedule.VaccineId);
                cmd.Parameters.AddWithValue("@ScheduledDate", schedule.ScheduledDate);
                cmd.Parameters.AddWithValue("@Status", schedule.Status);
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(schedule.Notes) ? (object)DBNull.Value : schedule.Notes);
                cmd.Parameters.AddWithValue("@Id", schedule.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateStatus(int scheduleId, string status)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "UPDATE VaccinationSchedule SET Status = @Status WHERE Id = @Id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Id", scheduleId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM VaccinationSchedule WHERE Id=@Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
