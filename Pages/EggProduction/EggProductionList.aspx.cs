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
                LoadEggTypes();
                LoadBarns();
            }
        }

        private void LoadEggTypes()
        {
            var eggTypes = new EggTypeDAL().GetAll();
            ddlEggTypeFilter.DataSource = eggTypes;
            ddlEggTypeFilter.DataTextField = "Name";
            ddlEggTypeFilter.DataValueField = "Id";
            ddlEggTypeFilter.DataBind();

            ddlEggTypeFilter.Items.Insert(0, new ListItem("-- Todos --", ""));
        }

        private void LoadBarns()
        {
            var barns = new BarnDAL().GetAll(); 
            ddlBarnFilter.DataSource = barns;
            ddlBarnFilter.DataTextField = "Name"; 
            ddlBarnFilter.DataValueField = "Id";  
            ddlBarnFilter.DataBind();

            ddlBarnFilter.Items.Insert(0, new ListItem("-- Todos --", ""));
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

            if (!string.IsNullOrEmpty(ddlEggTypeFilter.SelectedValue))
                filtered = filtered.Where(p => p.EggTypeId == int.Parse(ddlEggTypeFilter.SelectedValue));

            GridViewEggProduction.DataSource = filtered.ToList();
            GridViewEggProduction.DataBind();
        }

        protected void FilterChanged(object sender, EventArgs e)
        {
            // Obtener los datos originales
            var allProductions = ViewState["Productions"] as List<Models.EggProduction>;
            var filtered = allProductions.AsEnumerable();

            // Filtrar por fecha
            if (!string.IsNullOrEmpty(txtSearchDate.Text) && DateTime.TryParse(txtSearchDate.Text, out DateTime dt))
            {
                filtered = filtered.Where(p => p.Date.Date == dt.Date);
            }

            // Filtrar por tipo de huevo
            if (!string.IsNullOrEmpty(ddlEggTypeFilter.SelectedValue))
            {
                int eggTypeId = int.Parse(ddlEggTypeFilter.SelectedValue);
                filtered = filtered.Where(p => p.EggTypeId == eggTypeId);
            }

            // Filtrar por galpón
            if (!string.IsNullOrEmpty(ddlBarnFilter.SelectedValue))
            {
                int barnId = int.Parse(ddlBarnFilter.SelectedValue);
                filtered = filtered.Where(p => p.BarnId == barnId);
            }

            // Asignar al GridView
            GridViewEggProduction.DataSource = filtered.ToList();
            GridViewEggProduction.DataBind();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearchDate.Text = "";
            ddlEggTypeFilter.SelectedIndex = 0;
            ddlBarnFilter.SelectedIndex = 0;

            GridViewEggProduction.DataSource = ViewState["Productions"];
            GridViewEggProduction.DataBind();
        }
    }
}