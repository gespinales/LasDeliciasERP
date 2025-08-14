<%@ Page Title="Producción de Huevos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EggProductionForm.aspx.cs" Inherits="LasDeliciasERP.Pages.EggProduction.EggProductionForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow-sm p-4">
        <h2 class="mb-4" id="formTitle" runat="server">Agregar Producción de Huevos</h2>

        <asp:HiddenField ID="hfId" runat="server" />

        <!-- Fecha -->
        <div class="mb-3">
            <label for="txtDate" class="form-label">Fecha:</label>
            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" 
                         Text='<%# DateTime.Now.ToString("yyyy-MM-dd") %>' 
                         TextMode="Date"></asp:TextBox>
        </div>

        <!-- Tipo de huevo -->
        <div class="mb-3">
            <label for="ddlEggType" class="form-label">Tipo de Huevo:</label>
            <asp:DropDownList ID="ddlEggType" runat="server" CssClass="form-select"></asp:DropDownList>
        </div>

        <!-- Galpón / Barn -->
        <div class="mb-3">
            <label for="ddlBarn" class="form-label">Galpón:</label>
            <asp:DropDownList ID="ddlBarn" runat="server" CssClass="form-select"></asp:DropDownList>
        </div>

        <!-- Cantidad por tamaño -->
        <div class="mb-3">
            <label class="form-label">Cantidad de huevos por tamaño (gramos):</label>
            <div class="row">
                <div class="col">
                    <label for="txtS" class="form-label">S (&lt;53g)</label>
                    <asp:TextBox ID="txtS" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col">
                    <label for="txtM" class="form-label">M (53-63g)</label>
                    <asp:TextBox ID="txtM" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col">
                    <label for="txtL" class="form-label">L (63-73g)</label>
                    <asp:TextBox ID="txtL" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col">
                    <label for="txtXL" class="form-label">XL (&gt;73g)</label>
                    <asp:TextBox ID="txtXL" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Notas -->
        <div class="mb-3">
            <label for="txtNotes" class="form-label">Notas:</label>
            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" 
                         TextMode="MultiLine" Rows="3"></asp:TextBox>
        </div>

        <!-- Botones -->
        <div class="d-flex flex-column flex-sm-row justify-content-center gap-2 mt-3">
            <a href="EggProductionList.aspx" class="btn btn-danger d-flex align-items-center justify-content-center">
                <i class="bi bi-arrow-left-circle-fill me-2"></i> Volver
            </a>

            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-success d-flex align-items-center justify-content-center"
                Text=" Guardar" OnClick="btnSave_Click" />
        </div>
    </div>
</asp:Content>