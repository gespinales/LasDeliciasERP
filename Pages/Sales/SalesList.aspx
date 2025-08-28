<%@ Page Title="Lista de Ventas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SalesList.aspx.cs" Inherits="LasDeliciasERP.Pages.Sales.SalesList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-3">
        <h2>Ventas</h2>
        <a href="Sales.aspx?action=save" class="btn btn-success">
            <i class="bi bi-plus-circle"></i> Nueva Venta
        </a>
    </div>

    <asp:GridView ID="GridViewSales" runat="server"
        AutoGenerateColumns="False"
        AllowPaging="True"
        PageSize="10"
        OnPageIndexChanging="GridViewSales_PageIndexChanging"
        CssClass="table table-striped table-hover align-middle text-center">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="text-center" />
            <asp:BoundField DataField="CustomerName" HeaderText="Cliente" />
            <asp:BoundField DataField="SaleDate" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
            <asp:BoundField DataField="Notes" HeaderText="Notas" />
            <asp:BoundField DataField="TotalQuantity" HeaderText="Total Huevos" ItemStyle-CssClass="fw-bold" />
            <asp:BoundField DataField="TotalAmount" HeaderText="Total Venta Estimada"
                DataFormatString="{0:C}" ItemStyle-CssClass="fw-bold" />
            <asp:BoundField DataField="TotalSaleAmount" HeaderText="Total Venta Real"
                DataFormatString="{0:C}" ItemStyle-CssClass="fw-bold" />

            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <a href='<%# "Sales.aspx?id=" + Eval("Id") + "&action=update" %>'
                        class="btn btn-warning btn-sm me-1">
                        <i class="bi bi-pencil-square"></i> Editar
                    </a>
                    <a href='<%# "Sales.aspx?id=" + Eval("Id") + "&action=delete" %>'
                        class="btn btn-danger btn-sm"
                        onclick="return confirm('¿Está seguro de eliminar esta venta?');">
                        <i class="bi bi-trash"></i> Eliminar
                    </a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

</asp:Content>
