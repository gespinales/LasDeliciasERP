<%@ Page Title="Gestión de Clientes" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="CustomerForm.aspx.cs" Inherits="LasDeliciasERP.Pages.Customer.CustomerForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow-sm p-4">
        <h2 class="mb-4" id="formTitle" runat="server">Agregar Cliente</h2>

        <asp:HiddenField ID="hfId" runat="server" />
        <asp:HiddenField ID="hfAction" runat="server" Value="save" />

        <!-- Nombre -->
        <div class="mb-3">
            <label for="txtName" class="form-label">Nombre: <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvName" runat="server"
                ControlToValidate="txtName"
                ErrorMessage="El nombre es obligatorio"
                CssClass="text-danger"
                Display="Dynamic"
                ValidationGroup="CustomerForm"
                CausesValidation="true" />
        </div>

        <!-- Teléfono -->
        <div class="mb-3">
            <label for="txtPhone" class="form-label">Teléfono:</label>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control"></asp:TextBox>            
        </div>

        <!-- Correo -->
        <div class="mb-3">
            <label for="txtEmail" class="form-label">Correo:</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
            <asp:RegularExpressionValidator ID="revEmail" runat="server"
                ControlToValidate="txtEmail"
                ErrorMessage="Formato de correo inválido"
                CssClass="text-danger"
                Display="Dynamic"
                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" 
                ValidationGroup="CustomerForm" />
        </div>

        <!-- Dirección -->
        <div class="mb-3">
            <label for="txtAddress" class="form-label">Dirección:</label>
            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
        </div>

        <!-- Notas -->
        <div class="mb-3">
            <label for="txtNotes" class="form-label">Notas:</label>
            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
        </div>

        <!-- Botones -->
        <div class="d-flex flex-column flex-sm-row justify-content-center gap-2 mt-3">
            <a href="CustomerList.aspx" class="btn btn-danger d-flex align-items-center justify-content-center">
                <i class="bi bi-arrow-left-circle-fill me-2"></i>Volver
            </a>

            <asp:Button ID="btnSave" runat="server"
                CssClass="btn btn-success d-flex align-items-center justify-content-center"
                Text=" Guardar"
                OnClick="btnSave_Click"
                ValidationGroup="CustomerForm" />
        </div>
    </div>
</asp:Content>
