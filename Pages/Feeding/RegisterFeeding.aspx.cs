using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Feeding
{
    public partial class RegisterFeeding : System.Web.UI.Page
    {
        private decimal dailyConsumption;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadInfo();
        }

        private void LoadInfo()
        {
            FoodDAL dal = new FoodDAL();
            FeedingInfo info = dal.GetFeedingInfo();
            dailyConsumption = info.DailyConsumption;

            string lastDateText = info.LastFeedingDate.HasValue
                ? info.LastFeedingDate.Value.ToString("dd/MM/yyyy")
                : "No se ha registrado alimentación aún.";

            lblInfo.Text = $"📊 Hoy se deberían descontar <b>{dailyConsumption:N2} lbs</b> de alimento.<br/>" +
                           $"📅 Última alimentación registrada: <b>{lastDateText}</b>";

            if (info.LastFeedingDate.HasValue && info.LastFeedingDate.Value.Date == DateTime.Today)
            {
                btnRegistrar.Enabled = false;
                lblMensaje.Text = "✅ Ya se registró la alimentación del día de hoy.";
                lblMensaje.CssClass = "text-success fw-bold";
            }

            // Cargar historial solo del producto 26
            List<FeedingRecord> history = dal.GetHistory(30);
            gvHistorial.DataSource = history;
            gvHistorial.DataBind();
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                FoodDAL dal = new FoodDAL();
                FeedingInfo info = dal.GetFeedingInfo();
                dailyConsumption = info.DailyConsumption;

                bool success = dal.RegisterDailyFeeding(dailyConsumption);

                if (success)
                {
                    lblMensaje.Text = $"✅ Alimentación registrada. Se descontaron {dailyConsumption:N2} lbs del inventario.";
                    lblMensaje.CssClass = "text-success fw-bold";
                }
                else
                {
                    lblMensaje.Text = "⚠️ Ya se registró la alimentación del día de hoy.";
                    lblMensaje.CssClass = "text-warning fw-bold";
                }

                LoadInfo();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "❌ Error al registrar alimentación: " + ex.Message;
                lblMensaje.CssClass = "text-danger fw-bold";
            }
        }

        protected void gvHistorial_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHistorial.PageIndex = e.NewPageIndex;
            LoadInfo(); // Recarga los datos con el nuevo PageIndex
        }
    }
}
