<%@ Page Title="Listado de Producción de Huevos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EggProductionList.aspx.cs" Inherits="LasDeliciasERP.Pages.EggProduction.EggProductionList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow-sm p-4">
        <h2 class="mb-4">Listado de Producción de Huevos</h2>

        <!-- Botón Agregar nueva recolección -->
        <div class="mb-3">
            <a href="EggProductionForm.aspx" class="btn btn-success">
                <i class="bi bi-plus-circle-fill"></i> Agregar Nueva Recolección
            </a>
        </div>

        <!-- Grid de producción de huevos -->
        <asp:GridView ID="GridViewEggProduction" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered"
            AllowPaging="True" PageSize="10" OnPageIndexChanging="GridViewEggProduction_PageIndexChanging">
            
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="Date" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="QuantityS" HeaderText="Huevos S" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="QuantityM" HeaderText="Huevos M" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="QuantityL" HeaderText="Huevos L" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="QuantityXL" HeaderText="Huevos XL" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="TotalQuantity" HeaderText="Total Huevos" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="TotalWeight" HeaderText="Peso Total (g)" DataFormatString="{0:F2}" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="AverageWeight" HeaderText="Peso Promedio (g)" DataFormatString="{0:F2}" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="EggTypeName" HeaderText="Tipo de Huevo" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="Notes" HeaderText="Notas" ItemStyle-CssClass="text-center" />

                <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-center">
                    <ItemTemplate>
                        <asp:Button ID="btnEdit" runat="server" Text="✏️" CssClass="btn btn-warning btn-sm me-1" OnClick="btnEdit_Click" CommandArgument='<%# Eval("Id") %>' />
                        <%-- Descomenta si quieres eliminar registros
                        <asp:Button ID="btnDelete" runat="server" Text="🗑️" CssClass="btn btn-danger btn-sm" OnClick="btnDelete_Click" CommandArgument='<%# Eval("Id") %>' /> 
                        --%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>