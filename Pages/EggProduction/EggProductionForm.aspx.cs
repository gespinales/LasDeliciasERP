using System;
using LasDeliciasERP.AccesoADatos;  
using LasDeliciasERP.Models;

namespace LasDeliciasERP.Pages.EggProduction
{
    public partial class EggProductionForm : System.Web.UI.Page
    {
        //objeto para el acceso a los datos
        EggProductionDAL dalEggProduction = new EggProductionDAL();
        EggTypeDAL dalEggType = new EggTypeDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Cargar primero la lista de valores de los tipos de huevos
                LoadEggTypes(); 

                //Se valida si viene de una creación o modificación
                string idStr = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(idStr))
                {
                    int id = int.Parse(idStr);
                    LoadData(id); // Método que carga los datos en los controles
                    formTitle.InnerText = "Editar Producción de Huevos";
                    btnSave.Text = "Actualizar";
                    btnSave.CssClass = "btn btn-success me-2";
                }
                else
                {
                    txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                }
            }
        }

        private void LoadEggTypes()
        {
            var eggTypes = dalEggType.GetAll(); 
            ddlEggType.DataSource = eggTypes;
            ddlEggType.DataTextField = "Name";
            ddlEggType.DataValueField = "Id";
            ddlEggType.DataBind();

            ddlEggType.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", ""));
        }

        private void LoadData(int id)
        {
            var production = dalEggProduction.GetById(id);
            if (production != null)
            {
                hfId.Value = production.Id.ToString();
                txtDate.Text = production.Date.ToString("yyyy-MM-dd");
                txtS.Text = production.QuantityS.ToString();
                txtM.Text = production.QuantityM.ToString();
                txtL.Text = production.QuantityL.ToString();
                txtXL.Text = production.QuantityXL.ToString();
                txtNotes.Text = production.Notes;
                ddlEggType.SelectedValue = production.EggTypeId.ToString();
            }
            else
            {
                // Si no se encuentra el registro, redirige al listado
                Response.Redirect("EggProductionList.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Crear instancia del modelo
            var production = new Models.EggProduction
            {
                Date = DateTime.Parse(txtDate.Text),
                QuantityS = string.IsNullOrEmpty(txtS.Text) ? 0 : int.Parse(txtS.Text),
                QuantityM = string.IsNullOrEmpty(txtM.Text) ? 0 : int.Parse(txtM.Text),
                QuantityL = string.IsNullOrEmpty(txtL.Text) ? 0 : int.Parse(txtL.Text),
                QuantityXL = string.IsNullOrEmpty(txtXL.Text) ? 0 : int.Parse(txtXL.Text),
                Notes = txtNotes.Text,
                EggTypeId = int.Parse(ddlEggType.SelectedValue)
            };

            if (!string.IsNullOrEmpty(hfId.Value))
            {
                // Actualización
                production.Id = int.Parse(hfId.Value);
                dalEggProduction.Update(production);
            }
            else
            {
                // Inserción
                dalEggProduction.Insert(production);
            }

            // Volver al listado
            Response.Redirect("EggProductionList.aspx");
        }
    }
}