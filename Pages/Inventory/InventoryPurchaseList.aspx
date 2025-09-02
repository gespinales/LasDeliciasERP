<%@ Page Title="Lista de Compras" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InventoryPurchaseList.aspx.cs" Inherits="LasDeliciasERP.Pages.Inventory.InventoryPurchaseList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-3">
        <h2>Compras de Inventario</h2>
        <a href="InventoryPurchase.aspx?action=save" class="btn btn-success">
            <i class="bi bi-plus-circle"></i> Nueva Compra
        </a>
    </div>

    <!-- Bloque de Inventario Resumido -->
    <div class="card shadow-sm p-4 mt-4">
        <h3 class="mb-3">Inventario Resumido</h3>

        <div class="table-responsive">
            <asp:GridView ID="GridViewInventorySummary" runat="server" AutoGenerateColumns="False"
                CssClass="table table-striped table-hover align-middle text-center"
                AllowSorting="False">

                <Columns>
                    <asp:BoundField DataField="ProductName" HeaderText="Producto"
                        ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                    <asp:BoundField DataField="UnitName" HeaderText="Unidad"
                        ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                    <asp:BoundField DataField="Quantity" HeaderText="Cantidad (Libras)"
                        ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                    <asp:BoundField DataField="LastUpdated" HeaderText="Última Actualización"
                        DataFormatString="{0:dd/MM/yyyy HH:mm}"
                        ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Grid de Compras -->
    <div class="card shadow-sm p-4 mt-4">
        <h3 class="mb-3">Compras realizadas</h3>

        <asp:GridView ID="GridViewPurchases" runat="server"
            AutoGenerateColumns="False"
            AllowPaging="True"
            PageSize="10"
            OnPageIndexChanging="GridViewPurchases_PageIndexChanging"
            CssClass="table table-striped table-hover align-middle text-center">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="SupplierName" HeaderText="Proveedor" />
                <asp:BoundField DataField="PurchaseDate" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="InvoiceNumber" HeaderText="Factura" />
                <asp:BoundField DataField="TotalAmount" HeaderText="Total Compra"
                    DataFormatString="{0:C}" ItemStyle-CssClass="fw-bold" />
                <asp:BoundField DataField="Notes" HeaderText="Notas" />

                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <div class="d-flex justify-content-center gap-2">
                            <a href='<%# "InventoryPurchase.aspx?id=" + Eval("Id") + "&action=update" %>'
                                class="btn btn-sm btn-outline-warning">
                                <i class="bi bi-pencil-square"></i>Editar
                            </a>
                            <a href='<%# "InventoryPurchase.aspx?id=" + Eval("Id") + "&action=delete" %>'
                                class="btn btn-sm btn-outline-danger"
                                onclick="return confirm('¿Está seguro de eliminar esta compra?');">
                                <i class="bi bi-trash"></i>Eliminar
                            </a>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
   

</asp:Content>
