using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Inventory
{
    public partial class InventoryPurchase : System.Web.UI.Page
    {
        private readonly InventoryPurchaseDAL dalPurchase = new InventoryPurchaseDAL();
        private readonly SupplierDAL dalSupplier = new SupplierDAL();
        private readonly ProductDAL dalProduct = new ProductDAL();

        private List<InventoryPurchaseDetail> PurchaseDetail
        {
            get
            {
                if (Session["PurchaseDetail"] == null)
                    Session["PurchaseDetail"] = new List<InventoryPurchaseDetail>();
                return (List<InventoryPurchaseDetail>)Session["PurchaseDetail"];
            }
            set { Session["PurchaseDetail"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["PurchaseDetail"] = null;

                LoadSuppliers();
                LoadProducts();

                string action = Request.QueryString["action"];
                string id = Request.QueryString["id"];

                if (!string.IsNullOrEmpty(action))
                {
                    hfAction.Value = action.ToLower();
                    hfId.Value = id ?? "0";
                }

                if ((hfAction.Value == "update" || hfAction.Value == "delete") && hfId.Value != "0")
                {
                    LoadPurchase(int.Parse(hfId.Value));

                    if (hfAction.Value == "delete")
                    {
                        ddlSupplier.Enabled = false;
                        ddlProducts.Enabled = false;
                        txtQuantity.ReadOnly = true;
                        txtPrice.ReadOnly = true;
                        btnAddProduct.Enabled = false;
                        formTitle.InnerText = "Eliminar Compra";
                        btnSave.Text = "Eliminar";
                    }
                    else
                    {
                        formTitle.InnerText = "Modificar Compra";
                        btnSave.Text = "Modificar";
                    }
                }
            }
        }

        private void LoadSuppliers()
        {
            ddlSupplier.DataSource = dalSupplier.GetAll();
            ddlSupplier.DataTextField = "Name";
            ddlSupplier.DataValueField = "Id";
            ddlSupplier.DataBind();
            ddlSupplier.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        }

        private void LoadProducts()
        {
            var products = dalProduct.GetAllProductsWithUnit();

            ddlProducts.Items.Clear();
            ddlProducts.Items.Add(new ListItem("-- Seleccione Producto --", ""));

            ddlUnit.Items.Clear();
            ddlUnit.Items.Add(new ListItem("-- Seleccione Unidad --", ""));

            foreach (var p in products)
            {
                // Producto único
                if (!ddlProducts.Items.Cast<ListItem>().Any(i => i.Text == p.Name))
                    ddlProducts.Items.Add(new ListItem(p.Name, p.Id.ToString()));

                if (!ddlUnit.Items.Cast<ListItem>().Any(i => i.Text == p.UnitName))
                    ddlUnit.Items.Add(new ListItem(p.UnitName, p.UnitTypeId.ToString()));
            }        
        }

        private void LoadPurchase(int purchaseId)
        {
            var purchase = dalPurchase.GetById(purchaseId);

            if (purchase != null)
            {
                ddlSupplier.SelectedValue = purchase.SupplierId.ToString();
                txtNotes.Text = purchase.Notes;
                txtInvoiceNumber.Text = purchase.InvoiceNumber;
                PurchaseDetail = purchase.Details;
                BindPurchaseProducts();
            }
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlProducts.SelectedValue) ||
                string.IsNullOrEmpty(txtQuantity.Text) ||
                string.IsNullOrEmpty(txtPrice.Text))
                return;

            int productId = int.Parse(ddlProducts.SelectedValue);
            string productName = ddlProducts.SelectedItem.Text;

            int unitTypeId = int.Parse(ddlUnit.SelectedValue);
            string unitName = ddlUnit.SelectedItem.Text;

            if (!decimal.TryParse(txtQuantity.Text, out decimal quantity)) return;
            if (!decimal.TryParse(txtPrice.Text, out decimal unitPrice)) return;

            PurchaseDetail.Add(new InventoryPurchaseDetail
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                ProductName = productName,
                UnitTypeId = unitTypeId,   
                UnitName = unitName        
            });

            BindPurchaseProducts();

            txtQuantity.Text = "";
            txtPrice.Text = "";
            ddlProducts.SelectedIndex = 0;
        }

        protected void gvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            PurchaseDetail.RemoveAt(index);
            BindPurchaseProducts();
        }

        private void BindPurchaseProducts()
        {
            gvProducts.DataSource = PurchaseDetail;
            gvProducts.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string action = hfAction.Value;
            int purchaseId = string.IsNullOrEmpty(hfId.Value) ? 0 : int.Parse(hfId.Value);

            if (action == "save")
            {
                // Calcular total
                decimal totalAmount = PurchaseDetail.Sum(d => d.Quantity * d.UnitPrice);
                totalAmount = Math.Round(totalAmount, 2);

                Models.InventoryPurchase purchase = new Models.InventoryPurchase
                {
                    SupplierId = int.Parse(ddlSupplier.SelectedValue),
                    Notes = txtNotes.Text,
                    PurchaseDate = DateTime.Now,
                    InvoiceNumber = txtInvoiceNumber.Text?.Trim(),
                    TotalAmount = totalAmount,
                    Details = PurchaseDetail
                };
                dalPurchase.Insert(purchase);
            }
            else if (action == "update")
            {
                // Calcular total
                decimal totalAmount = PurchaseDetail.Sum(d => d.Quantity * d.UnitPrice);
                totalAmount = Math.Round(totalAmount, 2);

                Models.InventoryPurchase purchase = new Models.InventoryPurchase
                {
                    Id = purchaseId,
                    SupplierId = int.Parse(ddlSupplier.SelectedValue),
                    Notes = txtNotes.Text,
                    PurchaseDate = DateTime.Now,
                    InvoiceNumber = txtInvoiceNumber.Text?.Trim(),
                    TotalAmount = totalAmount,
                    Details = PurchaseDetail
                };
                dalPurchase.Update(purchase);
            }
            else if (action == "delete")
            {
                dalPurchase.Delete(purchaseId);
            }

            Session["PurchaseDetail"] = null;
            Response.Redirect("InventoryPurchaseList.aspx");
        }
    }
}
