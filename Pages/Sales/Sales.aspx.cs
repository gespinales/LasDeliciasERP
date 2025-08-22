using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Sales
{
    [Serializable]
    public class SaleProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }

    public partial class Sales : System.Web.UI.Page
    {
        private readonly SalesDAL dalSales = new SalesDAL();
        private readonly CustomerDAL dalCustomer = new CustomerDAL();
        private readonly ProductDAL dalProduct = new ProductDAL();

        private List<SaleProduct> SaleProducts
        {
            get
            {
                if (Session["SaleProducts"] == null)
                    Session["SaleProducts"] = new List<SaleProduct>();
                return (List<SaleProduct>)Session["SaleProducts"];
            }
            set { Session["SaleProducts"] = value; }
        }

        private List<SaleDetail> SaleDetail
        {
            get
            {
                if (Session["SaleDetail"] == null)
                    Session["SaleDetail"] = new List<SaleDetail>();
                return (List<SaleDetail>)Session["SaleDetail"];
            }
            set { Session["SaleDetail"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Limpiamos la sesión para iniciar fresco
                Session["SaleProducts"] = null;
                Session["SaleDetail"] = null;

                LoadCustomers();
                LoadProducts();

                string action = Request.QueryString["action"];
                string id = Request.QueryString["id"];

                if (!string.IsNullOrEmpty(action))
                {
                    hfAction.Value = action.ToLower();
                    hfId.Value = id ?? "0";
                }

                if (hfAction.Value == "update" && hfId.Value != "0")
                    LoadSale(int.Parse(hfId.Value));
                else if (hfAction.Value == "delete" && hfId.Value != "0")
                    DeleteSale(int.Parse(hfId.Value));
            }
        }

        private void LoadCustomers()
        {
            ddlCustomer.DataSource = dalCustomer.GetAll();
            ddlCustomer.DataTextField = "Name";
            ddlCustomer.DataValueField = "Id";
            ddlCustomer.DataBind();
            ddlCustomer.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        }

        private void LoadProducts()
        {
            var products = dalProduct.GetAllWithUnitAndPrice();

            ddlProducts.Items.Clear();
            ddlProducts.Items.Add(new ListItem("-- Seleccione Producto --", ""));

            var productPrices = new Dictionary<int, decimal>();

            foreach (var p in products)
            {
                string textProductUnit = $"{p.Name} ({p.UnitName})";
                ListItem item = new ListItem(textProductUnit, p.Id.ToString());
                ddlProducts.Items.Add(item);

                productPrices[p.Id] = p.Price;
            }

            Session["ProductPrices"] = productPrices;
        }

        private void LoadSale(int saleId)
        {
            ddlCustomer.SelectedValue = "1";
            txtNotes.Text = "Notas de prueba";

            SaleProducts = new List<SaleProduct>
            {
                new SaleProduct { ProductId = 1, ProductName = "Producto 1 (kg)", Quantity = 2, UnitName = "kg", Price = 25.00M }
            };

            BindSaleProducts();
        }

        private void DeleteSale(int saleId)
        {
            Response.Redirect("SalesList.aspx");
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlProducts.SelectedValue) || ddlProducts.SelectedValue == "" || string.IsNullOrEmpty(txtQuantity.Text))
                return;

            int productId = int.Parse(ddlProducts.SelectedValue);
            string productName = ddlProducts.SelectedItem.Text;

            var productPrices = (Dictionary<int, decimal>)Session["ProductPrices"];
            decimal price = productPrices.ContainsKey(productId) ? productPrices[productId] : 0;

            if (!decimal.TryParse(txtQuantity.Text, out decimal quantity))
                return;

            string unitName = "";
            int start = productName.IndexOf("(");
            int end = productName.IndexOf(")");
            if (start >= 0 && end > start)
                unitName = productName.Substring(start + 1, end - start - 1);

            SaleProducts.Add(new SaleProduct
            {
                ProductId = productId,
                ProductName = productName,
                UnitName = unitName,
                Price = price,
                Quantity = quantity
            });

            SaleDetail.Add(new SaleDetail
            {
                ProductId = productId,
                Quantity = quantity
            });

            BindSaleProducts();

            txtQuantity.Text = "";
            txtPrice.Text = "";
            ddlProducts.SelectedIndex = 0;
        }

        protected void gvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            SaleProducts.RemoveAt(index);
            BindSaleProducts();
        }

        private void BindSaleProducts()
        {
            gvProducts.DataSource = SaleProducts;
            gvProducts.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Sale sale = new Sale
            {
                CustomerId = int.Parse(ddlCustomer.SelectedValue),
                Notes = txtNotes.Text,
                SaleDate = DateTime.Now
            };

            //if (hfAction.Value == "save")
            //    dalSales.Insert(sale, SaleDetail);
            //else if (hfAction.Value == "update")
            //{
            //    int saleId = int.Parse(hfId.Value);
            //    dalSales.Update(sale, SaleDetail);
            //}

            Session["SaleProducts"] = null;
            Session["SaleDetail"] = null;
            Response.Redirect("SalesList.aspx");
        }
    }
}
