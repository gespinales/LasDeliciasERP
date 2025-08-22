using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Sales
{
    [Serializable]
    public class SaleProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int UnitTypeId { get; set; }
        public string UnitName { get; set; }
        public int EggSizeId { get; set; }
        public string EggSizeName { get; set; }
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
            var products = dalProduct.GetAllWithUnitAndPrice(); // traer ProductId, Name, UnitName, EggSizeName, Price

            ddlProducts.Items.Clear();
            ddlProducts.Items.Add(new ListItem("-- Seleccione Producto --", ""));

            ddlUnit.Items.Clear();
            ddlUnit.Items.Add(new ListItem("-- Seleccione Unidad --", ""));

            ddlSize.Items.Clear();
            ddlSize.Items.Add(new ListItem("-- Seleccione Tamaño --", ""));

            var productPrices = new Dictionary<string, decimal>();

            foreach (var p in products)
            {
                // Producto único
                if (!ddlProducts.Items.Cast<ListItem>().Any(i => i.Text == p.Name))
                    ddlProducts.Items.Add(new ListItem(p.Name, p.Id.ToString()));

                // Unidad única
                if (!ddlUnit.Items.Cast<ListItem>().Any(i => i.Text == p.UnitName))
                    ddlUnit.Items.Add(new ListItem(p.UnitName, p.UnitTypeId.ToString()));

                // Tamaño único
                if (!ddlSize.Items.Cast<ListItem>().Any(i => i.Text == p.EggSizeName))
                    ddlSize.Items.Add(new ListItem(p.EggSizeName, p.EggSizeId.ToString()));

                // Diccionario JS: ProductName|UnitId|SizeId -> Price
                string key = $"{p.Name}|{p.UnitTypeId}|{p.EggSizeId}";
                if (!productPrices.ContainsKey(key))
                    productPrices[key] = p.Price;
            }

            Session["ProductPrices"] = productPrices;
        }

        private void LoadSale(int saleId)
        {
            // Esto lo puedes reemplazar con tu lógica real de carga desde DAL
            ddlCustomer.SelectedValue = "1";
            txtNotes.Text = "Notas de prueba";

            SaleProducts = new List<SaleProduct>
            {
                new SaleProduct
                {
                    ProductId = 1,
                    ProductName = "Producto 1",
                    UnitTypeId = 1,
                    UnitName = "kg",
                    EggSizeId = 1,
                    EggSizeName = "M",
                    Price = 25.00M,
                    Quantity = 2
                }
            };

            BindSaleProducts();
        }

        private void DeleteSale(int saleId)
        {
            Response.Redirect("SalesList.aspx");
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlProducts.SelectedValue) ||
                string.IsNullOrEmpty(ddlUnit.SelectedValue) ||
                string.IsNullOrEmpty(ddlSize.SelectedValue) ||
                string.IsNullOrEmpty(txtQuantity.Text))
                return;

            int productId = int.Parse(ddlProducts.SelectedValue);
            string productName = ddlProducts.SelectedItem.Text;
            int unitTypeId = int.Parse(ddlUnit.SelectedValue);
            string unitName = ddlUnit.SelectedItem.Text;
            int sizeId = int.Parse(ddlSize.SelectedValue);
            string sizeName = ddlSize.SelectedItem.Text;

            if (!decimal.TryParse(txtQuantity.Text, out decimal quantity))
                return;

            var productPrices = (Dictionary<string, decimal>)Session["ProductPrices"];
            string key = $"{productName}|{unitTypeId}|{sizeId}";
            decimal price = productPrices.ContainsKey(key) ? productPrices[key] : 0;

            SaleProducts.Add(new SaleProduct
            {
                ProductId = productId,
                ProductName = productName,
                UnitTypeId = unitTypeId,
                UnitName = unitName,
                EggSizeId = sizeId,
                EggSizeName = sizeName,
                Price = price,
                Quantity = quantity
            });

            SaleDetail.Add(new SaleDetail
            {
                ProductId = productId,
                Quantity = quantity
            });

            BindSaleProducts();

            // Limpiar campos
            txtQuantity.Text = "";
            txtPrice.Text = "";
            ddlProducts.SelectedIndex = 0;
            ddlUnit.SelectedIndex = 0;
            ddlSize.SelectedIndex = 0;
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

            // Guardar en DB con DAL
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
