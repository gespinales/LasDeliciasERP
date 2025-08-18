using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;

namespace LasDeliciasERP.Pages.Bird
{
    public partial class BirdBatchList : Page
    {
        BirdBatchDAL dalBirdBatch = new BirdBatchDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            var list = dalBirdBatch.GetAllWithNames(); 
            gvBirdBatch.DataSource = list;
            gvBirdBatch.DataBind();
        }

        protected void gvBirdBatch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBirdBatch.PageIndex = e.NewPageIndex;
            BindGrid();
        }
    }
}