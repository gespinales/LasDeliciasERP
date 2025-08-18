<%@ Page Title="Registrar Deceso de Aves" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="BirdDeath.aspx.cs" Inherits="LasDeliciasERP.Pages.Bird.BirdDeath" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow-sm p-4">
        <h2 class="mb-4" id="formTitle" runat="server">Registrar Deceso de Aves</h2>

        <asp:HiddenField ID="hfId" runat="server" />
        <asp:HiddenField ID="hfAction" runat="server" Value="save" />

        <!-- Galpón -->
        <div class="mb-3">
            <label for="ddlBarn" class="form-label">Galpón: <span class="text-danger">*</span></label>
            <asp:DropDownList ID="ddlBarn" runat="server" CssClass="form-select" />
            <asp:RequiredFieldValidator ID="rfvBarn" runat="server"
                ControlToValidate="ddlBarn"
                InitialValue=""
                ErrorMessage="Debe seleccionar un galpón"
                CssClass="text-danger" Display="Dynamic" />
        </div>

        <!-- Tipo de Ave -->
        <div class="mb-3">
            <label for="ddlBirdType" class="form-label">Tipo de Ave (Raza): <span class="text-danger">*</span></label>
            <asp:DropDownList ID="ddlBirdType" runat="server" CssClass="form-select" />
            <asp:RequiredFieldValidator ID="rfvBirdType" runat="server"
                ControlToValidate="ddlBirdType"
                InitialValue=""
                ErrorMessage="Debe seleccionar un tipo de ave"
                CssClass="text-danger" Display="Dynamic" />
        </div>

        <!-- Fecha de Deceso -->
        <div class="mb-3">
            <label for="txtDeathDate" class="form-label">Fecha del Deceso: <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtDeathDate" runat="server" CssClass="form-control" TextMode="Date" />
            <asp:RequiredFieldValidator ID="rfvDeathDate" runat="server"
                ControlToValidate="txtDeathDate"
                ErrorMessage="La fecha del deceso es obligatoria"
                CssClass="text-danger" Display="Dynamic" />
        </div>

        <!-- Cantidad de Aves Fallecidas -->
        <div class="mb-3">
            <label for="txtQuantity" class="form-label">Cantidad: <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvQuantity" runat="server"
                ControlToValidate="txtQuantity"
                ErrorMessage="La cantidad es obligatoria"
                CssClass="text-danger" Display="Dynamic" />
        </div>

        <!-- Motivo -->
        <div class="mb-3">
            <label for="txtReason" class="form-label">Motivo:</label>
            <asp:TextBox ID="txtReason" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
        </div>

        <!-- Botones -->
        <div class="d-flex flex-column flex-sm-row justify-content-center gap-2 mt-3">
            <a href="BirdDeathList.aspx" class="btn btn-danger d-flex align-items-center justify-content-center">
                <i class="bi bi-arrow-left-circle-fill me-2"></i>Volver
            </a>
            <asp:Button ID="btnSave" runat="server"
                CssClass="btn btn-success d-flex align-items-center justify-content-center"
                Text=" Guardar"
                OnClick="btnSave_Click"
                ValidationGroup="BirdDeath" />
        </div>
    </div>
</asp:Content>
