using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Sales
{
    public partial class Sales : System.Web.UI.Page
    {
        private readonly SalesDAL dalSales = new SalesDAL();
        private readonly CustomerDAL dalCustomer = new CustomerDAL();
        private readonly ProductDAL dalProduct = new ProductDAL();

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

                BindInventory();
                LoadCustomers();
                LoadProducts();

                string action = Request.QueryString["action"];
                string id = Request.QueryString["id"];

                if (!string.IsNullOrEmpty(action))
                {
                    hfAction.Value = action.ToLower();
                    hfId.Value = id ?? "0";
                }

                if ((hfAction.Value == "update" || 
                    hfAction.Value == "delete") && 
                    hfId.Value != "0")
                    LoadSale(int.Parse(hfId.Value));
            }
        }

        private void BindInventory()
        {
            var inventory = new EggInventoryDAL().GetAll();
            var eggTypes = new EggTypeDAL().GetAll();
            var inventoryWithNames = from inv in inventory
                                     join et in eggTypes on inv.EggTypeId equals et.Id
                                     select new
                                     {
                                         EggTypeName = et.Name,
                                         inv.QuantityS,
                                         inv.QuantityM,
                                         inv.QuantityL,
                                         inv.QuantityXL,
                                         TotalQuantity = inv.QuantityS + inv.QuantityM + inv.QuantityL + inv.QuantityXL
                                     };

            GridViewEggInventory.DataSource = inventoryWithNames.ToList();
            GridViewEggInventory.DataBind();
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
            var products = dalProduct.GetAllEggsWithUnitAndPrice(); // traer ProductId, Name, UnitName, EggSizeName, Price

            ddlProducts.Items.Clear();
            ddlProducts.Items.Add(new ListItem("-- Seleccione Producto --", ""));

            ddlUnit.Items.Clear();
            ddlUnit.Items.Add(new ListItem("-- Seleccione Unidad --", ""));

            ddlSize.Items.Clear();
            ddlSize.Items.Add(new ListItem("-- Seleccione Tamaño --", ""));

            var productPrices = new Dictionary<string, decimal>();
            var productIdMap = new Dictionary<string, int>();

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

                // Diccionario ProductId
                if (!productIdMap.ContainsKey(key))
                    productIdMap[key] = p.Id;
            }

            Session["ProductPrices"] = productPrices;
            Session["ProductIdMap"] = productIdMap;
        }

        private void LoadSale(int saleId)
        {
            var sale = dalSales.GetSaleById(saleId);

            if (sale != null)
            {
                ddlCustomer.SelectedValue = sale.CustomerId.ToString();
                txtNotes.Text = sale.Notes;

                // Guardar detalles en tu variable de sesión o propiedad
                SaleDetail = sale.Details;

                // Enlazar al grid de productos
                BindSaleProducts();
            }
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

            //int productId = int.Parse(ddlProducts.SelectedValue);
            string productName = ddlProducts.SelectedItem.Text;
            int unitTypeId = int.Parse(ddlUnit.SelectedValue);
            string unitName = ddlUnit.SelectedItem.Text;
            int sizeId = int.Parse(ddlSize.SelectedValue);
            string sizeName = ddlSize.SelectedItem.Text;

            if (!decimal.TryParse(txtQuantity.Text, out decimal quantity))
                return;
         
            var productPrices = (Dictionary<string, decimal>)Session["ProductPrices"];
            var productIdMap = (Dictionary<string, int>)Session["ProductIdMap"];

            string key = $"{productName}|{unitTypeId}|{sizeId}";
            decimal price = productPrices.ContainsKey(key) ? productPrices[key] : 0;

            // Precio de venta ingresado por el usuario (si no pone nada, usamos el precio base)
            decimal salePrice = price;
            if (decimal.TryParse(txtSalePrice.Text, out decimal customPrice) && customPrice > 0)
                salePrice = customPrice;

            int productId = productIdMap.ContainsKey(key) ? productIdMap[key] : 0;

            SaleDetail.Add(new SaleDetail
            {
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity,
                Price = price,
                SalePrice = salePrice,
                DisplayName = $"{productName} - {unitName} - {sizeName}",
                UnitTypeId = unitTypeId,
                UnitName = unitName,
                EggSizeId = sizeId,
                EggSizeName = sizeName
            });

            BindSaleProducts();

            // Limpiar campos
            txtQuantity.Text = "";
            txtPrice.Text = "";
            txtSalePrice.Text = "";
            ddlProducts.SelectedIndex = 0;
            ddlUnit.SelectedIndex = 0;
            ddlSize.SelectedIndex = 0;
        }

        protected void gvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            SaleDetail.RemoveAt(index);
            BindSaleProducts();
        }

        private void BindSaleProducts()
        {
            gvProducts.DataSource = SaleDetail;
            gvProducts.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Validar que se haya seleccionado un cliente
            if (string.IsNullOrEmpty(ddlCustomer.SelectedValue))
            {
                // Mostrar alerta
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "Swal.fire({icon:'warning',title:'Cliente requerido',text:'Por favor seleccione un cliente antes de guardar.',confirmButtonText:'Entendido',confirmButtonColor:'#3085d6'});", true);
                return; // Salir sin guardar
            }

            // Validar que haya productos agregados
            if (SaleDetail == null || SaleDetail.Count == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "Swal.fire({icon:'warning',title:'Productos requeridos',text:'Debe agregar al menos un producto antes de guardar.',confirmButtonText:'Entendido',confirmButtonColor:'#3085d6'});", true);
                return;
            }

            Sale sale = new Sale
            {
                CustomerId = int.Parse(ddlCustomer.SelectedValue),
                Notes = txtNotes.Text,
                SaleDate = DateTime.Now,
                Details = SaleDetail
            };

            // Guardar en DB con DAL
            if (hfAction.Value == "save")
                dalSales.Insert(sale);
            else if (hfAction.Value == "update")
            {
                int saleId = int.Parse(hfId.Value);
                sale.Id = saleId;
                dalSales.Update(sale);
            }
            else if (hfAction.Value == "delete")
            {
                int saleId = int.Parse(hfId.Value);
                dalSales.Delete(saleId);
            }

            Session["SaleProducts"] = null;
            Session["SaleDetail"] = null;
            Response.Redirect("SalesList.aspx");
        }
    }
}
