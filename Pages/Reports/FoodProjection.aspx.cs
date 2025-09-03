using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LasDeliciasERP.Pages.Reports
{
    public partial class FoodProjection : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProjection();
            }
        }

        private void LoadProjection()
        {
            var dal = new FoodProjectionDAL();

            // Obtener proyección de alimento
            var projection = dal.GetProjection();

            lblAves.Text = $"Aves: {projection.TotalBirds}";
            lblComida.Text = $"Comida disponible: {projection.TotalFoodKg:N2} lbs";
            lblDias.Text = $"Días disponibles: {projection.AvailableDays:N1}";

            // Pasar datos de proyección a Chart.js
            string projectionJson = JsonConvert.SerializeObject(projection);
            ClientScript.RegisterStartupScript(this.GetType(), "projectionData",
                $"var projectionData = {projectionJson};", true);

            // Obtener historial de alimentación (últimos 30 días)
            List<FeedingRecord> history = dal.GetFeedingHistory(30);
            string historyJson = JsonConvert.SerializeObject(history);
            ClientScript.RegisterStartupScript(this.GetType(), "historyData",
                $"var historyData = {historyJson};", true);
        }
    }
}
