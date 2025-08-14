using LasDeliciasERP.AccesoADatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Customer
{
    public partial class CustomerList : System.Web.UI.Page
    {
        private readonly CustomerDAL dalCustomer = new CustomerDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            var customers = dalCustomer.GetAll();
            GridViewCustomers.DataSource = customers;
            GridViewCustomers.DataBind();
        }

        protected void GridViewCustomers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewCustomers.PageIndex = e.NewPageIndex;

            // Reasigna el DataSource
            GridViewCustomers.DataSource = dalCustomer.GetAll();
            GridViewCustomers.DataBind();
        }
    }
}