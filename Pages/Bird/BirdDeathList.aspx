<%@ Page Title="Lista de Decesos de Aves" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="BirdDeathList.aspx.cs" Inherits="LasDeliciasERP.Pages.Bird.BirdDeathList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-4 text-center">Decesos de Aves</h2>

    <div class="mb-3 text-end">
        <a href="BirdDeath.aspx" class="btn btn-danger">
            <i class="bi bi-plus-circle-fill"></i> Registrar Deceso
        </a>
    </div>

    <asp:GridView ID="gvDeaths" runat="server" 
        AutoGenerateColumns="False" 
        AllowPaging="True" 
        PageSize="10" 
        OnPageIndexChanging="gvDeaths_PageIndexChanging"
        CssClass="table table-striped table-hover align-middle text-center">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="text-center" />
            <asp:BoundField DataField="DeathDate" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}"/>
            <asp:BoundField DataField="BarnName" HeaderText="Galpón" />
            <asp:BoundField DataField="BirdTypeName" HeaderText="Tipo de Ave" />
            <asp:BoundField DataField="Quantity" HeaderText="Cantidad" />
            <asp:BoundField DataField="Reason" HeaderText="Motivo" />

            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <a href='BirdDeath.aspx?id=<%# Eval("Id") %>&action=edit' class="btn btn-primary btn-sm me-1">
                        <i class="bi bi-pencil-fill"></i> Editar
                    </a>
                    <a href='BirdDeath.aspx?id=<%# Eval("Id") %>&action=delete' class="btn btn-danger btn-sm"
                        onclick="return confirm('¿Seguro que desea eliminar este registro?');">
                        <i class="bi bi-trash-fill"></i> Eliminar
                    </a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
