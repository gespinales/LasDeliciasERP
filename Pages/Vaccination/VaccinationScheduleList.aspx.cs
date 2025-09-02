using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Vaccination
{
    public partial class VaccinationScheduleList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSchedules();
            }
        }

        private void LoadSchedules()
        {
            var dal = new VaccinationScheduleDAL();
            // Obtener programación junto con última aplicación
            var schedules = dal.GetAllWithLastRecord();
            GridViewSchedules.DataSource = schedules;
            GridViewSchedules.DataBind();
        }

        protected void GridViewSchedules_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewSchedules.PageIndex = e.NewPageIndex;
            LoadSchedules();
        }

        protected void GridViewSchedules_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Obtenemos el status de la fila actual
                string status = DataBinder.Eval(e.Row.DataItem, "Status").ToString();

                // Encontramos el botón Aplicar
                var btnApply = e.Row.FindControl("btnApply") as HyperLink; // si es <asp:HyperLink>
                                                                           // o si es <a runat="server"> y ID="btnApply", entonces:
                                                                           // var btnApply = e.Row.FindControl("btnApply") as HtmlAnchor;

                if (btnApply != null && status == "Aplicada")
                {
                    btnApply.Attributes["onclick"] = "return false;";
                    btnApply.CssClass = "btn btn-success btn-sm disabled flex-fill"; // Bootstrap deshabilitado
                    btnApply.ToolTip = "Vacuna ya aplicada";
                }

                // Encontramos el HyperLink btnRecords
                var btnRecords = e.Row.FindControl("btnRecords") as HyperLink;

                if (btnRecords != null && status == "Programada")
                {
                    // Deshabilitar el botón y cambiar estilo
                    btnRecords.Enabled = false;
                    btnRecords.CssClass = "btn btn-secondary btn-sm flex-fill";
                    btnRecords.Attributes["onclick"] = "return false;"; // evita que sea clickeable
                }
            }
        }
    }
}
