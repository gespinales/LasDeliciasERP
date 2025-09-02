using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Inventory
{
    public partial class InventoryPurchaseList : System.Web.UI.Page
    {
        private readonly InventoryPurchaseDAL dalPurchase = new InventoryPurchaseDAL();
        private readonly InventoryDAL dalInventory = new InventoryDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindInventorySummary();
                BindPurchases();
            }
        }

        private void BindInventorySummary()
        {
            var summary = dalInventory.GetInventorySummary();
            GridViewInventorySummary.DataSource = summary;
            GridViewInventorySummary.DataBind();
        }

        private void BindPurchases()
        {
            var purchases = dalPurchase.GetAll(); 
            GridViewPurchases.DataSource = purchases;
            GridViewPurchases.DataBind();
        }

        protected void GridViewPurchases_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewPurchases.PageIndex = e.NewPageIndex;
            BindPurchases();
        }
    }
}
