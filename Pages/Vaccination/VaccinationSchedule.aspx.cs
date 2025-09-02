using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Vaccination
{
    public partial class VaccinationSchedule : System.Web.UI.Page
    {
        private readonly BarnDAL dalBarn = new BarnDAL();
        private readonly VaccineDAL dalVaccine = new VaccineDAL();
        private readonly VaccinationScheduleDAL dalSchedule = new VaccinationScheduleDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBarns();
                LoadVaccines();

                string action = Request.QueryString["action"];
                string id = Request.QueryString["id"];

                if (!string.IsNullOrEmpty(action))
                {
                    hfAction.Value = action.ToLower();
                    hfId.Value = id ?? "0";
                }

                if ((hfAction.Value == "update" || hfAction.Value == "delete") && hfId.Value != "0")
                {
                    LoadSchedule(int.Parse(hfId.Value));

                    if (hfAction.Value == "delete")
                    {
                        ddlBarn.Enabled = false;
                        ddlVaccine.Enabled = false;
                        txtScheduledDate.ReadOnly = true;
                        txtNotes.ReadOnly = true;
                        btnSave.Text = "Eliminar";
                        formTitle.InnerText = "Eliminar Programación de Vacuna";
                    }
                    else
                    {
                        btnSave.Text = "Modificar";
                        formTitle.InnerText = "Modificar Programación de Vacuna";
                    }
                }
            }
        }

        private void LoadBarns()
        {
            ddlBarn.DataSource = dalBarn.GetAll();
            ddlBarn.DataTextField = "Name";
            ddlBarn.DataValueField = "Id";
            ddlBarn.DataBind();
            ddlBarn.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        }

        private void LoadVaccines()
        {
            ddlVaccine.DataSource = dalVaccine.GetAll();
            ddlVaccine.DataTextField = "Name";
            ddlVaccine.DataValueField = "Id";
            ddlVaccine.DataBind();
            ddlVaccine.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        }

        private void LoadSchedule(int scheduleId)
        {
            var schedule = dalSchedule.GetById(scheduleId);
            if (schedule != null)
            {
                ddlBarn.SelectedValue = schedule.BarnId.ToString();
                ddlVaccine.SelectedValue = schedule.VaccineId.ToString();
                txtScheduledDate.Text = schedule.ScheduledDate.ToString("yyyy-MM-dd");
                txtNotes.Text = schedule.Notes;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int barnId = int.Parse(ddlBarn.SelectedValue);
            int vaccineId = int.Parse(ddlVaccine.SelectedValue);
            DateTime scheduledDate = DateTime.Parse(txtScheduledDate.Text);

            Models.VaccinationSchedule schedule = new Models.VaccinationSchedule
            {
                BarnId = barnId,
                VaccineId = vaccineId,
                ScheduledDate = scheduledDate,
                Status = "Programado",
                Notes = txtNotes.Text
            };

            int scheduleId = string.IsNullOrEmpty(hfId.Value) ? 0 : int.Parse(hfId.Value);

            if (hfAction.Value == "save")
            {
                dalSchedule.Insert(schedule);
            }
            else if (hfAction.Value == "update")
            {
                schedule.Id = scheduleId;
                dalSchedule.Update(schedule);
            }
            else if (hfAction.Value == "delete")
            {
                dalSchedule.Delete(scheduleId);
            }

            Response.Redirect("VaccinationScheduleList.aspx");
        }
    }
}
