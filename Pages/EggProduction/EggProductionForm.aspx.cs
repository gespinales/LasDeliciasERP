using System;
using System.Collections.Generic;
using System.Web.UI;
using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;

namespace LasDeliciasERP.Pages.EggProduction
{
    public partial class EggProductionForm : Page
    {
        private EggProductionDAL dalEggProduction = new EggProductionDAL();
        private BarnDAL dalBarn = new BarnDAL();
        private ProductDAL dalProduct = new ProductDAL();

        // Guardamos los detalles de producción en ViewState
        private List<EggProductionDetail> ProductionDetails
        {
            get => ViewState["ProductionDetails"] as List<EggProductionDetail> ?? new List<EggProductionDetail>();
            set => ViewState["ProductionDetails"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                LoadBarns();
                LoadProducts();

                string idStr = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(idStr))
                {
                    int id = int.Parse(idStr);
                    LoadData(id);
                    formTitle.InnerText = "Editar Producción de Huevos";
                    btnSave.Text = "Actualizar";
                }

                BindGrid();
            }
        }

        private void LoadBarns()
        {
            var barns = dalBarn.GetAll();
            ddlBarn.DataSource = barns;
            ddlBarn.DataTextField = "Name";
            ddlBarn.DataValueField = "Id";
            ddlBarn.DataBind();
            ddlBarn.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", ""));
        }

        private void LoadProducts()
        {
            var products = dalProduct.GetAllEggProductsUnit();
            ddlProduct.DataSource = products;
            ddlProduct.DataTextField = "Name"; // Debe traer Tipo + Tamaño
            ddlProduct.DataValueField = "Id";
            ddlProduct.DataBind();
            ddlProduct.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", ""));
        }

        private void LoadData(int id)
        {
            var production = dalEggProduction.GetById(id);
            if (production != null)
            {
                hfId.Value = production.Id.ToString();
                txtDate.Text = production.ProductionDate.ToString("yyyy-MM-dd");
                ddlBarn.SelectedValue = production.BarnId.ToString();
                txtNotes.Text = production.Notes;

                // Cargar detalles
                ProductionDetails = dalEggProduction.GetDetailsByProductionId(id);
                BindGrid();
            }
            else
            {
                Response.Redirect("EggProductionList.aspx");
            }
        }

        protected void btnAddDetail_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlProduct.SelectedValue) && !string.IsNullOrEmpty(txtQuantity.Text))
            {
                if (!int.TryParse(txtQuantity.Text, out int quantity))
                {
                    // Validación simple
                    Response.Write("<script>alert('Cantidad inválida');</script>");
                    return;
                }

                int productId = int.Parse(ddlProduct.SelectedValue);
                var product = dalProduct.GetEggproductById(productId);

                if (product != null)
                {
                    // Obtener la lista actual desde ViewState
                    var details = ProductionDetails;

                    // Verificar si ya existe el producto en la lista, si es así sumar cantidad
                    var existing = details.Find(d => d.ProductId == product.Id);
                    if (existing != null)
                    {
                        existing.Quantity += quantity;
                    }
                    else
                    {
                        details.Add(new EggProductionDetail
                        {
                            ProductId = product.Id,
                            ProductName = product.Name, // Asegúrate que DAL devuelva DisplayName
                            Quantity = quantity
                        });
                    }

                    // Guardar nuevamente en ViewState
                    ProductionDetails = details;

                    // Refrescar GridView
                    BindGrid();

                    // Limpiar inputs
                    ddlProduct.SelectedIndex = 0;
                    txtQuantity.Text = "";
                }
            }
        }

        private void BindGrid()
        {
            // Obtener la lista actual desde ViewState
            var details = ProductionDetails;

            GridViewDetails.DataSource = details;
            GridViewDetails.DataBind();
        }


        protected void btnRemove_Click(object sender, EventArgs e)
        {
            var btn = (System.Web.UI.WebControls.LinkButton)sender;
            int index = int.Parse(btn.CommandArgument);

            if (index >= 0 && index < ProductionDetails.Count)
            {
                ProductionDetails.RemoveAt(index);
                BindGrid();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlBarn.SelectedValue == "" || ProductionDetails.Count == 0)
            {
                Response.Write("<script>alert('Debe seleccionar galpón y agregar al menos un producto.');</script>");
                return;
            }

            var production = new Models.EggProduction
            {
                ProductionDate = DateTime.Parse(txtDate.Text),
                BarnId = int.Parse(ddlBarn.SelectedValue),
                Notes = txtNotes.Text,
                Details = ProductionDetails // asignamos los detalles
            };

            if (!string.IsNullOrEmpty(hfId.Value))
            {
                production.Id = int.Parse(hfId.Value);
                dalEggProduction.Update(production);
            }
            else
            {
                dalEggProduction.Insert(production);
            }

            Response.Redirect("EggProductionList.aspx");
        }
    }
}
