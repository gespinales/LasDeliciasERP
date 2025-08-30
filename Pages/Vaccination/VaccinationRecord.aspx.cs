using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Vaccination
{
    public partial class VaccinationRecord : System.Web.UI.Page
    {
        private VaccinationScheduleDAL scheduleDal = new VaccinationScheduleDAL();
        private VaccinationRecordDAL recordDal = new VaccinationRecordDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string action = Request.QueryString["action"];
                hfAction.Value = action ?? "apply";

                if (Request.QueryString["scheduleId"] != null)
                {
                    int scheduleId;
                    if (int.TryParse(Request.QueryString["scheduleId"], out scheduleId))
                    {
                        hfScheduleId.Value = scheduleId.ToString();

                        // Cargar datos de la programación
                        var schedule = scheduleDal.GetById(scheduleId);
                        if (schedule != null)
                        {
                            lblBarn.Text = schedule.BarnName;
                            lblVaccine.Text = schedule.VaccineName;
                            lblScheduledDate.Text = schedule.ScheduledDate.ToString("dd/MM/yyyy");
                            lblStatus.Text = schedule.Status;
                            txtNotes.Text = schedule.Notes; // si quieres mostrar notas de programación
                        }

                        // Cargar datos de aplicación si existen
                        var record = recordDal.GetByScheduleId(scheduleId);
                        if (record != null)
                        {
                            // Mostrar datos de la aplicación
                            txtAppliedDate.Text = record.AppliedDate.ToString("yyyy-MM-dd");
                            txtQuantityApplied.Text = record.QuantityApplied.ToString();
                            txtAppliedBy.Text = record.AppliedBy;
                            txtNotes.Text = record.Notes;

                            // Bloquear campos porque ya se aplicó
                            txtAppliedDate.Enabled = false;
                            txtQuantityApplied.Enabled = false;
                            txtAppliedBy.Enabled = false;
                            txtNotes.Enabled = false;
                            btnApply.Enabled = false;

                            lblStatus.Text = "Aplicada";
                        }
                        else
                        {
                            // Campos disponibles para aplicar vacuna
                            txtAppliedDate.Enabled = true;
                            txtQuantityApplied.Enabled = true;
                            txtAppliedBy.Enabled = true;
                            txtNotes.Enabled = true;
                            btnApply.Enabled = true;
                        }
                    }
                }
            }
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    Models.VaccinationRecord record = new Models.VaccinationRecord
                    {
                        ScheduleId = int.Parse(hfScheduleId.Value),
                        AppliedDate = DateTime.Parse(txtAppliedDate.Text),
                        QuantityApplied = int.Parse(txtQuantityApplied.Text),
                        Notes = string.IsNullOrEmpty(txtNotes.Text) ? null : txtNotes.Text,
                        AppliedBy = string.IsNullOrEmpty(txtAppliedBy.Text) ? null : txtAppliedBy.Text,
                    };

                    recordDal.Insert(record);

                    // Actualizamos el estado de la programación
                    scheduleDal.UpdateStatus(record.ScheduleId, "Aplicada");

                    Response.Redirect("VaccinationScheduleList.aspx");
                }
                catch (Exception ex)
                {
                    lblError.Text = "Error al guardar: " + ex.Message;
                    lblError.CssClass = "text-danger fw-bold";
                }
            }
        }
    }
}
