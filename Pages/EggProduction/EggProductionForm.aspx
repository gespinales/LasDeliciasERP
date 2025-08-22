<%@ Page Title="Producción de Huevos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EggProductionForm.aspx.cs" Inherits="LasDeliciasERP.Pages.EggProduction.EggProductionForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow-sm p-4">
        <h2 class="mb-4" id="formTitle" runat="server">Agregar Producción de Huevos</h2>

        <asp:HiddenField ID="hfId" runat="server" />

        <!-- Fecha -->
        <div class="mb-3">
            <label class="form-label">Fecha:</label>
            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
        </div>

        <!-- Galpón -->
        <div class="mb-3">
            <label class="form-label">Galpón:</label>
            <asp:DropDownList ID="ddlBarn" runat="server" CssClass="form-select"></asp:DropDownList>
        </div>

        <!-- Agregar detalle de huevos -->
        <div class="mb-3">
            <label class="form-label">Agregar Huevos:</label>
            <div class="row g-2">
                <div class="col-6 col-md-4">
                    <asp:DropDownList ID="ddlProduct" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col-3 col-md-2">
                    <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" Placeholder="Cantidad"></asp:TextBox>
                </div>
                <div class="col-3 col-md-2">
                    <asp:Button ID="btnAddDetail" runat="server" CssClass="btn btn-primary" Text="Agregar" OnClick="btnAddDetail_Click" />
                </div>
            </div>
        </div>

        <!-- Grid de detalles -->
        <div class="mb-3 table-responsive">
            <asp:GridView ID="GridViewDetails" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-hover align-middle text-center">
                <Columns>
                    <asp:BoundField DataField="ProductName" HeaderText="Producto" />
                    <asp:BoundField DataField="Quantity" HeaderText="Cantidad" />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnRemove" runat="server" 
                                Text="Eliminar" CommandArgument='<%# Container.DataItemIndex %>'
                                CssClass="btn btn-danger btn-sm"
                                OnClick="btnRemove_Click"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <!-- Notas -->
        <div class="mb-3">
            <label class="form-label">Notas:</label>
            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
        </div>

        <!-- Botones -->
        <div class="d-flex flex-column flex-sm-row justify-content-center gap-2 mt-3">
            <a href="EggProductionList.aspx" class="btn btn-danger d-flex align-items-center justify-content-center">
                <i class="bi bi-arrow-left-circle-fill me-2"></i>Volver
            </a>
            <asp:Button ID="btnSave" runat="server"
                CssClass="btn btn-success d-flex align-items-center justify-content-center"
                Text="Guardar"
                OnClick="btnSave_Click"
                ValidationGroup="EggProductionForm" />
        </div>
    </div>
</asp:Content>
