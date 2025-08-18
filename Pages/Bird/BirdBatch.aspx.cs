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
    public partial class BirdBatch : Page
    {
        BirdBatchDAL dalBatch = new BirdBatchDAL();
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
                    LoadBatch(id);

                    if (action == "delete")
                    {
                        formTitle.InnerText = "Eliminar Lote";
                        btnSave.Text = "Eliminar";

                        // Deshabilitar campos
                        txtBatchDate.Enabled = false;
                        ddlBarn.Enabled = false;
                        ddlBirdType.Enabled = false;
                        txtQuantity.Enabled = false;
                        txtEstimatedAgeWeeks.Enabled = false;
                        txtNotes.Enabled = false;
                    }
                    else
                    {
                        formTitle.InnerText = "Editar Lote";
                        btnSave.Text = "Actualizar";
                    }
                }
                else
                {
                    formTitle.InnerText = "Nuevo Lote";
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

        private void LoadBatch(int id)
        {
            var batch = dalBatch.GetById(id);
            if (batch != null)
            {
                txtBatchDate.Text = batch.BatchDate.ToString("yyyy-MM-dd");
                ddlBarn.SelectedValue = batch.BarnId.ToString();
                ddlBirdType.SelectedValue = batch.BirdTypeId.ToString();
                txtQuantity.Text = batch.Quantity.ToString();
                txtEstimatedAgeWeeks.Text = batch.EstimatedAgeWeeks.ToString();
                txtNotes.Text = batch.Notes;
            }
            else
            {
                Response.Redirect("BirdBatchList.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string action = hfAction.Value;

            if (action == "delete" && !string.IsNullOrEmpty(hfId.Value))
            {
                dalBatch.Delete(int.Parse(hfId.Value));
                Response.Redirect("BirdBatchList.aspx");
                return;
            }

            var batch = new Models.BirdBatch
            {
                BarnId = int.Parse(ddlBarn.SelectedValue),
                BirdTypeId = int.Parse(ddlBirdType.SelectedValue),
                BatchDate = DateTime.Parse(txtBatchDate.Text),
                Quantity = int.Parse(txtQuantity.Text),
                EstimatedAgeWeeks = int.Parse(txtEstimatedAgeWeeks.Text),
                Notes = txtNotes.Text
            };

            if (!string.IsNullOrEmpty(hfId.Value))
            {
                batch.Id = int.Parse(hfId.Value);
                dalBatch.Update(batch);
            }
            else
            {
                dalBatch.Insert(batch);
            }

            Response.Redirect("BirdBatchList.aspx");
        }
    }
}