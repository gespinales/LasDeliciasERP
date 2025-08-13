using System;
using System.Collections.Generic;
using LasDeliciasERP.AccesoADatos; 
using LasDeliciasERP.Models;

namespace LasDeliciasERP.Pages.EggProduction
{
    public partial class EggProductionList : System.Web.UI.Page
    {
        //objeto para el acceso a los datos
        EggProductionDAL dalEggProduction = new EggProductionDAL(); 

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
            GridViewEggProduction.DataSource = dalEggProduction.GetAll();
            GridViewEggProduction.DataBind();
        }

        protected void GridViewEggProduction_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            GridViewEggProduction.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void GridViewEggProduction_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as Models.EggProduction;
                if (item != null)
                {
                    // Color según producción total
                    if (item.TotalQuantity < 15)
                        e.Row.CssClass = "low-production";
                    else if (item.TotalQuantity > 25)
                        e.Row.CssClass = "high-production";
                }
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            var btn = (System.Web.UI.WebControls.Button)sender;
            int id = int.Parse(btn.CommandArgument);

            Response.Redirect($"EggProductionForm.aspx?id={id}");
        }

        //protected void btnDelete_Click(object sender, EventArgs e)
        //{
        //    var btn = (System.Web.UI.WebControls.Button)sender;
        //    int id = int.Parse(btn.CommandArgument);
        //    Response.Write($"Eliminar registro con ID: {id}");
        //}
    }
}