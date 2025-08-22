using LasDeliciasERP.AccesoADatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Sales
{
    public partial class SalesList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSales();
            }
        }

        private void LoadSales()
        {
            var dal = new SalesDAL();
            var sales = dal.GetAllSales();
            GridViewSales.DataSource = sales;
            GridViewSales.DataBind();
        }

        protected void GridViewSales_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewSales.PageIndex = e.NewPageIndex;
            LoadSales();
        }
    }
}