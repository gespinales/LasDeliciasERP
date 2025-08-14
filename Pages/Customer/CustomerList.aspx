<%@ Page Title="Lista de Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomerList.aspx.cs" Inherits="LasDeliciasERP.Pages.Customer.CustomerList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-3">
        <h2>Clientes</h2>
        <a href="CustomerForm.aspx?action=save" class="btn btn-success">
            <i class="bi bi-plus-circle"></i> Nuevo Cliente
        </a>
    </div>

    <asp:GridView ID="GridViewCustomers" runat="server"
        AutoGenerateColumns="False"
        AllowPaging="True"
        PageSize="10"
        OnPageIndexChanging="GridViewCustomers_PageIndexChanging"
        CssClass="table table-striped table-hover align-middle text-center">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="text-center" />
            <asp:BoundField DataField="Name" HeaderText="Nombre" />
            <asp:BoundField DataField="Phone" HeaderText="Teléfono" />
            <asp:BoundField DataField="Email" HeaderText="Correo" />
            <asp:BoundField DataField="Address" HeaderText="Dirección" />
            <asp:BoundField DataField="Notes" HeaderText="Notas" />

            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <a href='<%# "CustomerForm.aspx?id=" + Eval("Id") + "&action=save" %>'
                        class="btn btn-warning btn-sm me-1">
                        <i class="bi bi-pencil-square"></i>Editar
                    </a>
                    <a href='<%# "CustomerForm.aspx?id=" + Eval("Id") + "&action=delete" %>'
                        class="btn btn-danger btn-sm"
                        onclick="return confirm('¿Está seguro de eliminar este cliente?');">
                        <i class="bi bi-trash"></i>Eliminar
                    </a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

</asp:Content>