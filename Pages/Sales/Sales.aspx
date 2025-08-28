<%@ Page Title="Registrar Venta" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Sales.aspx.cs" Inherits="LasDeliciasERP.Pages.Sales.Sales" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- JS Validaciones -->
    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%= btnAddProduct.ClientID %>").click(function (e) {
                let cantidad = $("#<%= txtQuantity.ClientID %>").val();
                if (!cantidad || parseFloat(cantidad) <= 0) {
                    e.preventDefault();
                    Swal.fire({
                        icon: 'warning',
                        title: 'Cantidad requerida',
                        text: 'Por favor ingrese una cantidad válida antes de agregar el producto.',
                        confirmButtonText: 'Entendido',
                        confirmButtonColor: '#3085d6'
                    });
                    return false;
                }
            });
        });

        window.onload = function () {
            var ddlProducts = document.getElementById('<%= ddlProducts.ClientID %>');
            var ddlUnit = document.getElementById('<%= ddlUnit.ClientID %>');
            var ddlSize = document.getElementById('<%= ddlSize.ClientID %>');
            var txtPrice = document.getElementById('<%= txtPrice.ClientID %>');

            var productPrices = {
                <% 
                var prices = (Dictionary<string, decimal>)Session["ProductPrices"];
                if (prices != null)
                {
                    foreach (var kv in prices)
                    {
                %>
                '<%= kv.Key %>': <%= kv.Value.ToString("F2") %>,
                <%      
                    }
                }
                %>
            };

            function updatePrice() {
                var productName = ddlProducts.options[ddlProducts.selectedIndex].text;
                var unitId = ddlUnit.value;
                var sizeId = ddlSize.value;

                var key = productName + '|' + unitId + '|' + sizeId;
                txtPrice.value = productPrices[key] !== undefined ? productPrices[key] : '';
            }

            ddlProducts.addEventListener('change', updatePrice);
            ddlUnit.addEventListener('change', updatePrice);
            ddlSize.addEventListener('change', updatePrice);
        };
    </script>

    <div class="card shadow-sm p-4">
        <h2 class="mb-4 text-center fw-bold" id="formTitle" runat="server">Registrar Venta de Productos</h2>

        <asp:HiddenField ID="hfId" runat="server" />
        <asp:HiddenField ID="hfAction" runat="server" Value="save" />

        <!-- Inventario -->
        <div class="card shadow-sm p-4 mb-4">
            <h4 class="mb-3 fw-semibold">Inventario de Huevos</h4>
            <div class="table-responsive">
                <asp:GridView ID="GridViewEggInventory" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-striped table-hover align-middle text-center">
                    <Columns>
                        <asp:BoundField DataField="EggTypeName" HeaderText="Tipo de Huevo" />
                        <asp:BoundField DataField="QuantityS" HeaderText="Huevos S" />
                        <asp:BoundField DataField="QuantityM" HeaderText="Huevos M" />
                        <asp:BoundField DataField="QuantityL" HeaderText="Huevos L" />
                        <asp:BoundField DataField="QuantityXL" HeaderText="Huevos XL" />
                        <asp:BoundField DataField="TotalQuantity" HeaderText="Total Huevos" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- Cliente -->
        <div class="col-md-6 mb-3">
            <label for="ddlCustomer" class="form-label">Cliente: <span class="text-danger">*</span></label>
            <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="form-select" />
            <asp:RequiredFieldValidator ID="rfvCustomer" runat="server"
                ControlToValidate="ddlCustomer"
                InitialValue=""
                ErrorMessage="Debe seleccionar un cliente"
                CssClass="text-danger"
                Display="Dynamic"
                ValidationGroup="SalesForm" />
        </div>

        <!-- Selección de productos -->
        <div class="row mb-3">
            <div class="col-md-3">
                <label class="form-label">Producto</label>
                <asp:DropDownList ID="ddlProducts" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Unidad</label>
                <asp:DropDownList ID="ddlUnit" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Tamaño</label>
                <asp:DropDownList ID="ddlSize" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Cantidad</label>
                <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" placeholder="Cantidad" />
            </div>
            <div class="col-md-1">
                <label class="form-label">Precio Base</label>
                <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Precio" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Precio Venta</label>
                <asp:TextBox ID="txtSalePrice" runat="server" CssClass="form-control" placeholder="Precio de Venta" />
            </div>
        </div>

        <div class="row mb-4">
            <div class="col-md-3">
                <asp:Button ID="btnAddProduct" runat="server"
                    CssClass="btn btn-primary w-100"
                    Text="Agregar Producto"
                    OnClick="btnAddProduct_Click" />
            </div>
        </div>

        <!-- Productos agregados -->
        <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="false"
            CssClass="table table-striped table-hover align-middle text-center mt-3"
            OnRowDeleting="gvProducts_RowDeleting">
            <Columns>
                <asp:BoundField DataField="ProductId" HeaderText="ID" Visible="false" />
                <asp:BoundField DataField="ProductName" HeaderText="Producto" />
                <asp:BoundField DataField="UnitName" HeaderText="Unidad" />
                <asp:BoundField DataField="EggSizeName" HeaderText="Tamaño" />
                <asp:BoundField DataField="Quantity" HeaderText="Cantidad" />
                <asp:BoundField DataField="Price" HeaderText="Precio" DataFormatString="{0:C}" />
                <asp:BoundField DataField="SalePrice" HeaderText="Precio Venta" DataFormatString="{0:C}" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete"
                            CssClass="btn btn-danger btn-sm"
                            OnClientClick="return confirm('¿Seguro que desea eliminar esta venta?');">
                            <i class="bi bi-trash-fill"></i> Quitar
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- Notas -->
        <div class="mb-3">
            <label for="txtNotes" class="form-label">Notas:</label>
            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
        </div>

        <!-- Botones -->
        <div class="d-flex flex-column flex-sm-row justify-content-center gap-2 mt-3">
            <a href="SalesList.aspx" class="btn btn-danger d-flex align-items-center justify-content-center">
                <i class="bi bi-arrow-left-circle-fill me-2"></i>Volver
            </a>
            <asp:Button ID="btnSave" runat="server"
                CssClass="btn btn-success d-flex align-items-center justify-content-center"
                Text="Guardar"
                OnClick="btnSave_Click"
                ValidationGroup="SalesForm" />
        </div>
    </div>

</asp:Content>
