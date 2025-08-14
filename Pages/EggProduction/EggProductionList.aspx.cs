using LasDeliciasERP.AccesoADatos; 
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.EggProduction
{
    public partial class EggProductionList : System.Web.UI.Page
    {
        //objeto para el acceso a los datos
        EggProductionDAL dalEggProduction = new EggProductionDAL();
        private List<Models.EggProduction> allProductions;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private List<Models.EggProduction> GetData()
        {
            //data de prueba, este iría en lugar de dalEggProduction.GetAll();
            return new List<Models.EggProduction>
            {
                new Models.EggProduction { Id=1, Date=DateTime.Today, QuantityS=5, QuantityM=10, QuantityL=8, QuantityXL=2, Notes="Normal" },
                new Models.EggProduction { Id=2, Date=DateTime.Today.AddDays(-1), QuantityS=3, QuantityM=12, QuantityL=7, QuantityXL=4, Notes="Excelente" },
                new Models.EggProduction { Id=3, Date=DateTime.Today.AddDays(-2), QuantityS=4, QuantityM=8, QuantityL=6, QuantityXL=3, Notes="Regular" },
                new Models.EggProduction { Id=4, Date=DateTime.Today.AddDays(-3), QuantityS=6, QuantityM=9, QuantityL=5, QuantityXL=1, Notes="Buen día" },
                new Models.EggProduction { Id=5, Date=DateTime.Today.AddDays(-4), QuantityS=2, QuantityM=7, QuantityL=3, QuantityXL=2, Notes="Promedio" },
                new Models.EggProduction { Id=6, Date=DateTime.Today.AddDays(-5), QuantityS=5, QuantityM=11, QuantityL=9, QuantityXL=3, Notes="Alta producción" }
            };
        }

        private void BindGrid()
        {
            // Obtener todos los registros desde MySQL
            allProductions = dalEggProduction.GetAll();
            ViewState["Productions"] = allProductions;

            GridViewEggProduction.DataSource = allProductions;
            GridViewEggProduction.DataBind();
        }

        protected void GridViewEggProduction_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            GridViewEggProduction.PageIndex = e.NewPageIndex;
            GridViewEggProduction.DataSource = ViewState["Productions"];
            GridViewEggProduction.DataBind();
        }

        // Colores por TotalQuantity
        protected void GridViewEggProduction_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Obtener el valor de TotalQuantity
                int totalQty = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "TotalQuantity"));

                // Asignar color de fondo según la cantidad
                if (totalQty > 50)
                {
                    e.Row.CssClass = "table-success"; // verde claro
                }
                else if (totalQty >= 20 && totalQty <= 50)
                {
                    e.Row.CssClass = "table-warning"; // amarillo claro
                }
                else
                {
                    e.Row.CssClass = "table-danger";  // rojo claro
                }
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int id = int.Parse(btn.CommandArgument);

            Response.Redirect($"EggProductionForm.aspx?id={id}");
        }

        // Ordenamiento
        protected void GridViewEggProduction_Sorting(object sender, GridViewSortEventArgs e)
        {
            allProductions = ViewState["Productions"] as List<Models.EggProduction>;
            string sortExpression = e.SortExpression;
            string direction = ViewState["SortDirection"] as string == "ASC" ? "DESC" : "ASC";
            ViewState["SortDirection"] = direction;
            ViewState["SortExpression"] = sortExpression;

            if (direction == "ASC")
                GridViewEggProduction.DataSource = allProductions.OrderBy(p => DataBinder.Eval(p, sortExpression)).ToList();
            else
                GridViewEggProduction.DataSource = allProductions.OrderByDescending(p => DataBinder.Eval(p, sortExpression)).ToList();

            GridViewEggProduction.DataBind();
        }

        // Búsqueda
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            allProductions = ViewState["Productions"] as List<Models.EggProduction>;
            var filtered = allProductions.AsEnumerable();

            if (!string.IsNullOrEmpty(txtSearchDate.Text))
            {
                if (DateTime.TryParse(txtSearchDate.Text, out DateTime dt))
                    filtered = filtered.Where(p => p.Date.Date == dt.Date);
            }

            if (!string.IsNullOrEmpty(txtSearchType.Text))
                filtered = filtered.Where(p => !string.IsNullOrEmpty(p.EggTypeName) && p.EggTypeName.ToLower().Contains(txtSearchType.Text.ToLower()));

            if (!string.IsNullOrEmpty(txtSearchNotes.Text))
                filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Notes) && p.Notes.ToLower().Contains(txtSearchNotes.Text.ToLower()));

            GridViewEggProduction.DataSource = filtered.ToList();
            GridViewEggProduction.DataBind();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearchDate.Text = "";
            txtSearchType.Text = "";
            txtSearchNotes.Text = "";

            GridViewEggProduction.DataSource = ViewState["Productions"];
            GridViewEggProduction.DataBind();
        }
    }
}