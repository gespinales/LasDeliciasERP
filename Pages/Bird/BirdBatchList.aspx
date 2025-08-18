<%@ Page Title="Lista de Lotes de Aves" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="BirdBatchList.aspx.cs" Inherits="LasDeliciasERP.Pages.Bird.BirdBatchList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-4 text-center">Lotes de Aves</h2>

    <div class="mb-3 text-end">
        <a href="BirdBatch.aspx" class="btn btn-success">
            <i class="bi bi-plus-circle-fill"></i> Agregar Lote
        </a>
    </div>

    <asp:GridView ID="gvBirdBatch" runat="server" 
        AutoGenerateColumns="False" 
        AllowPaging="True" 
        PageSize="10" 
        OnPageIndexChanging="gvBirdBatch_PageIndexChanging"
        CssClass="table table-striped table-hover align-middle text-center">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="text-center" />
            <asp:BoundField DataField="BarnName" HeaderText="Galpón" />
            <asp:BoundField DataField="BirdTypeName" HeaderText="Tipo de Ave" />
            <asp:BoundField DataField="BatchDate" HeaderText="Fecha de Lote" DataFormatString="{0:dd/MM/yyyy}"/>
            <asp:BoundField DataField="Quantity" HeaderText="Cantidad" />
            <asp:BoundField DataField="EstimatedAgeWeeks" HeaderText="Edad Estimada (semanas)" />
            <asp:BoundField DataField="Notes" HeaderText="Notas" />

            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <a href='BirdBatch.aspx?id=<%# Eval("Id") %>&action=edit' class="btn btn-primary btn-sm me-1">
                        <i class="bi bi-pencil-fill"></i> Editar
                    </a>
                    <a href='BirdBatch.aspx?id=<%# Eval("Id") %>&action=delete' class="btn btn-danger btn-sm"
                        onclick="return confirm('¿Seguro que desea eliminar este lote?');">
                        <i class="bi bi-trash-fill"></i> Eliminar
                    </a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
