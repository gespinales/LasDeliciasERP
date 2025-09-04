using LasDeliciasERP.AccesoADatos;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.UI;

namespace LasDeliciasERP.Pages.Reports
{
    public partial class EggLayingEfficiency : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadReport();
        }

        private void LoadReport(string startDate = null, string endDate = null)
        {
            var dal = new EggProductionDAL();
            var report = dal.GetEfficiencyReport(startDate, endDate);

            var reportForJs = report.Select(r => new
            {
                r.BarnName,
                ProductionDate = r.ProductionDate.ToString("yyyy-MM-ddTHH:mm:ss"), // Formato ISO completo
                r.EfficiencyPercentage
            }).ToList();

            string json = JsonConvert.SerializeObject(reportForJs);
            ClientScript.RegisterStartupScript(this.GetType(), "efficiencyData",
                $"var efficiencyData = {json};", true);
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string start = txtStartDate.Text.Trim();
            string end = txtEndDate.Text.Trim();
            LoadReport(start, end);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtStartDate.Text = string.Empty;
            txtEndDate.Text = string.Empty;
            LoadReport();
        }
    }
}
