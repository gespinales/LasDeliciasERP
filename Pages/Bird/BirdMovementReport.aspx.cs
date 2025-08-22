using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using LasDeliciasERP.AccesoADatos;

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
            ddlBatch.DataTextField = "BatchDate";
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
            ddlBirdType.DataTextField = "Breed";
            ddlBirdType.DataValueField = "Id";
            ddlBirdType.DataBind();
            ddlBirdType.Items.Insert(0, new ListItem("-- Todos los Tipos de Aves --", ""));

            // Tipo de gráfico
            ddlChartType.SelectedIndex = 0; // Por defecto barras
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            // Obtener todos los movimientos
            var movements = dalMovement.GetAll();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(ddlBatch.SelectedValue))
                movements = movements.Where(m => m.BatchId == int.Parse(ddlBatch.SelectedValue)).ToList();

            if (!string.IsNullOrEmpty(ddlBarn.SelectedValue))
                movements = movements.Where(m => m.BarnId == int.Parse(ddlBarn.SelectedValue)).ToList();

            if (!string.IsNullOrEmpty(ddlBirdType.SelectedValue))
                movements = movements.Where(m => m.BirdTypeId == int.Parse(ddlBirdType.SelectedValue)).ToList();

            if (!string.IsNullOrEmpty(txtStartDate.Text) && DateTime.TryParse(txtStartDate.Text, out DateTime startDate))
                movements = movements.Where(m => m.MovementDate >= startDate).ToList();

            if (!string.IsNullOrEmpty(txtEndDate.Text) && DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
                movements = movements.Where(m => m.MovementDate <= endDate).ToList();

            // GridView
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

            // Datos para la gráfica
            int entradas = movements.Where(m => m.MovementType == "Entrada").Sum(m => m.Quantity);
            int salidas = movements.Where(m => m.MovementType == "Salida").Sum(m => m.Quantity);

            // Tipo de gráfico seleccionado
            string chartType = ddlChartType.SelectedValue;

            // Script dinámico seguro
            string chartScript = $@"
        <script>
            document.addEventListener('DOMContentLoaded', function() {{
                var canvas = document.getElementById('movementChart');
                if (!canvas) return; // previene errores si no existe el canvas
                var ctx = canvas.getContext('2d');
                
                if (window.movementChart instanceof Chart) {{
                    window.movementChart.destroy();
                }}
                
                window.movementChart = new Chart(ctx, {{
                    type: '{chartType}',
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
                        maintainAspectRatio: false,
                        plugins: {{
                            legend: {{ position: 'top' }},
                            title: {{ display: true, text: 'Movimientos de Aves' }}
                        }}
                    }}
                }});
            }});
        </script>";

            ltChartData.Text = chartScript;
        }


        protected void gvMovements_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMovements.PageIndex = e.NewPageIndex;
            LoadReport();
        }
    }
}
