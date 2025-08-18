using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using LasDeliciasERP.AccesoADatos;
using System.Collections.Generic;

namespace LasDeliciasERP.Pages.Bird
{
    public partial class BirdMovementReport : Page
    {
        private readonly BirdMovementDAL dalMovement = new BirdMovementDAL();
        private readonly BirdBatchDAL dalBatch = new BirdBatchDAL();
        private readonly BarnDAL dalBarn = new BarnDAL();
        private readonly BirdTypesDAL dalBirdType = new BirdTypesDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFilters();
                LoadReport();
            }
        }

        private void LoadFilters()
        {
            // Lotes
            ddlBatch.DataSource = dalBatch.GetAll();
            ddlBatch.DataTextField = "BatchDate"; // Puedes personalizar: Fecha o Fecha + ID
            ddlBatch.DataValueField = "Id";
            ddlBatch.DataBind();
            ddlBatch.Items.Insert(0, new ListItem("-- Todos los Lotes --", ""));

            // Galpones
            ddlBarn.DataSource = dalBarn.GetAll();
            ddlBarn.DataTextField = "Name";
            ddlBarn.DataValueField = "Id";
            ddlBarn.DataBind();
            ddlBarn.Items.Insert(0, new ListItem("-- Todos los Galpones --", ""));

            // Tipos de Aves
            ddlBirdType.DataSource = dalBirdType.GetAll();
            ddlBirdType.DataTextField = "Breed"; // Mostrar raza
            ddlBirdType.DataValueField = "Id";
            ddlBirdType.DataBind();
            ddlBirdType.Items.Insert(0, new ListItem("-- Todos los Tipos de Aves --", ""));
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            // Obtener todos los movimientos
            var movements = dalMovement.GetAll(); // Devuelve lista completa de movimientos

            // Aplicar filtros
            if (!string.IsNullOrEmpty(ddlBatch.SelectedValue))
                movements = movements.Where(m => m.BatchId == int.Parse(ddlBatch.SelectedValue)).ToList();

            if (!string.IsNullOrEmpty(ddlBarn.SelectedValue))
                movements = movements.Where(m => m.BarnId == int.Parse(ddlBarn.SelectedValue)).ToList();

            if (!string.IsNullOrEmpty(ddlBirdType.SelectedValue))
                movements = movements.Where(m => m.BirdTypeId == int.Parse(ddlBirdType.SelectedValue)).ToList();

            if (!string.IsNullOrEmpty(txtStartDate.Text))
            {
                if (DateTime.TryParse(txtStartDate.Text, out DateTime startDate))
                    movements = movements.Where(m => m.MovementDate >= startDate).ToList();
            }

            if (!string.IsNullOrEmpty(txtEndDate.Text))
            {
                if (DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
                    movements = movements.Where(m => m.MovementDate <= endDate).ToList();
            }

            // Bind a GridView
            gvMovements.DataSource = movements.Select(m => new
            {
                m.Id,
                m.MovementType,
                m.Quantity,
                m.MovementDate,
                m.Reason,
                m.BirdTypeName,
                m.BarnName
            }).ToList();
            gvMovements.DataBind();

            // Preparar datos para Chart.js
            int entradas = movements.Where(m => m.MovementType == "Entrada").Sum(m => m.Quantity);
            int salidas = movements.Where(m => m.MovementType == "Salida").Sum(m => m.Quantity);

            string chartData = $@"
                <script>
                    var ctx = document.getElementById('movementChart').getContext('2d');
                    if (window.movementChart instanceof Chart) {{
                        window.movementChart.destroy();
                    }}
                    window.movementChart = new Chart(ctx, {{
                        type: 'bar',
                        data: {{
                            labels: ['Entradas', 'Salidas'],
                            datasets: [{{
                                label: 'Cantidad',
                                data: [{entradas}, {salidas}],
                                backgroundColor: ['#198754', '#dc3545']
                            }}]
                        }},
                        options: {{
                            responsive: true,
                            plugins: {{
                                legend: {{ display: false }}
                            }}
                        }}
                    }});
                </script>";

            ltChartData.Text = chartData;
        }

        protected void gvMovements_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMovements.PageIndex = e.NewPageIndex;
            LoadReport();
        }
    }
}
