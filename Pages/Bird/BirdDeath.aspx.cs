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
    public partial class BirdDeath : Page
    {
        BirdDeathDAL dalDeath = new BirdDeathDAL();
        BarnDAL dalBarn = new BarnDAL();
        BirdTypesDAL dalBirdType = new BirdTypesDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBarns();
                LoadBirdTypes();

                string idStr = Request.QueryString["id"];
                string action = Request.QueryString["action"] ?? "save";

                if (!string.IsNullOrEmpty(idStr))
                {
                    int id = int.Parse(idStr);
                    hfId.Value = idStr;
                    hfAction.Value = action;
                    LoadDeath(id);

                    if (action == "delete")
                    {
                        formTitle.InnerText = "Eliminar Deceso";
                        btnSave.Text = "Eliminar";

                        // Deshabilitar campos
                        txtDeathDate.Enabled = false;
                        ddlBarn.Enabled = false;
                        ddlBirdType.Enabled = false;
                        txtQuantity.Enabled = false;
                        txtReason.Enabled = false;
                    }
                    else
                    {
                        formTitle.InnerText = "Editar Deceso";
                        btnSave.Text = "Actualizar";
                    }
                }
                else
                {
                    formTitle.InnerText = "Nuevo Deceso";
                    btnSave.Text = "Guardar";
                }
            }
        }

        private void LoadBarns()
        {
            ddlBarn.DataSource = dalBarn.GetAll();
            ddlBarn.DataTextField = "Name";
            ddlBarn.DataValueField = "Id";
            ddlBarn.DataBind();
            ddlBarn.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        }

        private void LoadBirdTypes()
        {
            var birdTypes = dalBirdType.GetAll()
                .Select(bt => new
                {
                    Id = bt.Id,
                    DisplayText = $"{bt.Species} - {bt.Breed}"
                })
                .ToList();

            ddlBirdType.DataSource = birdTypes;
            ddlBirdType.DataTextField = "DisplayText";
            ddlBirdType.DataValueField = "Id";
            ddlBirdType.DataBind();
            ddlBirdType.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        }

        private void LoadDeath(int id)
        {
            var death = dalDeath.GetById(id);
            if (death != null)
            {
                txtDeathDate.Text = death.DeathDate.ToString("yyyy-MM-dd");
                ddlBarn.SelectedValue = death.BarnId.ToString();
                ddlBirdType.SelectedValue = death.BirdTypeId.ToString();
                txtQuantity.Text = death.Quantity.ToString();
                txtReason.Text = death.Reason;
            }
            else
            {
                Response.Redirect("BirdDeathList.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string action = hfAction.Value;

            if (action == "delete" && !string.IsNullOrEmpty(hfId.Value))
            {
                dalDeath.Delete(int.Parse(hfId.Value));
                Response.Redirect("BirdDeathList.aspx");
                return;
            }

            var death = new Models.BirdDeath
            {
                BarnId = int.Parse(ddlBarn.SelectedValue),
                BirdTypeId = int.Parse(ddlBirdType.SelectedValue),
                DeathDate = DateTime.Parse(txtDeathDate.Text),
                Quantity = int.Parse(txtQuantity.Text),
                Reason = txtReason.Text
            };

            if (!string.IsNullOrEmpty(hfId.Value))
            {
                death.Id = int.Parse(hfId.Value);
                dalDeath.Update(death);
            }
            else
            {
                dalDeath.Insert(death);
            }

            Response.Redirect("BirdDeathList.aspx");
        }
    }
}
