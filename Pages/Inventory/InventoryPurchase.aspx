<%@ Page Title="Registrar Compra" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="InventoryPurchase.aspx.cs" Inherits="LasDeliciasERP.Pages.Inventory.InventoryPurchase" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow-sm p-4">
        <h2 class="mb-4 text-center fw-bold" id="formTitle" runat="server">Registrar Compra</h2>

        <asp:HiddenField ID="hfId" runat="server" />
        <asp:HiddenField ID="hfAction" runat="server" Value="save" />

        <!-- Proveedor -->
        <div class="col-md-6 mb-3">
            <label for="ddlSupplier" class="form-label">Proveedor: <span class="text-danger">*</span></label>
            <asp:DropDownList ID="ddlSupplier" runat="server" CssClass="form-select" />
            <asp:RequiredFieldValidator ID="rfvSupplier" runat="server"
                ControlToValidate="ddlSupplier"
                InitialValue=""
                ErrorMessage="Debe seleccionar un proveedor"
                CssClass="text-danger"
                Display="Dynamic" />
        </div>

        <div class="col-md-4 mb-3">
            <label for="txtInvoiceNumber" class="form-label">No. de Factura</label>
            <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control" />
        </div>

        <!-- Selección de productos -->
        <div class="row mb-3">
            <div class="col-md-4">
                <label class="form-label">Producto</label>
                <asp:DropDownList ID="ddlProducts" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Unidad</label>
                <asp:DropDownList ID="ddlUnit" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Cantidad</label>
                <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" placeholder="Cantidad" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Precio Unitario</label>
                <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" placeholder="Precio" />
            </div>
            <div class="col-md-2 d-flex align-items-end">
                <asp:Button ID="btnAddProduct" runat="server"
                    CssClass="btn btn-primary w-100"
                    Text="Agregar Producto"
                    OnClick="btnAddProduct_Click" />
            </div>
        </div>

        <!-- Productos agregados -->
        <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="false"
            CssClass="table table-striped table-hover align-middle text-center"
            OnRowDeleting="gvProducts_RowDeleting">
            <Columns>
                <asp:BoundField DataField="ProductId" HeaderText="ID" Visible="false" />
                <asp:BoundField DataField="ProductName" HeaderText="Producto" />
                <asp:BoundField DataField="UnitName" HeaderText="Unidad" />
                <asp:BoundField DataField="Quantity" HeaderText="Cantidad" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Precio" DataFormatString="{0:C}" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete"
                            CssClass="btn btn-danger btn-sm"
                            OnClientClick="return confirm('¿Seguro que desea eliminar este producto?');">
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
            <a href="InventoryPurchaseList.aspx" class="btn btn-danger d-flex align-items-center justify-content-center">
                <i class="bi bi-arrow-left-circle-fill me-2"></i>Volver
            </a>
            <asp:Button ID="btnSave" runat="server"
                CssClass="btn btn-success d-flex align-items-center justify-content-center"
                Text="Guardar"
                OnClick="btnSave_Click" />
        </div>
    </div>

</asp:Content>
