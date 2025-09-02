<%@ Page Title="Programar Vacuna" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="VaccinationSchedule.aspx.cs"
    Inherits="LasDeliciasERP.Pages.Vaccination.VaccinationSchedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow-sm p-4">
        <h2 class="mb-4 text-center fw-bold" id="formTitle" runat="server">Programar Vacuna</h2>

        <asp:HiddenField ID="hfId" runat="server" />
        <asp:HiddenField ID="hfAction" runat="server" Value="save" />

        <!-- Selección de galpón -->
        <div class="mb-3">
            <label for="ddlBarn" class="form-label">Galpón: <span class="text-danger">*</span></label>
            <asp:DropDownList ID="ddlBarn" runat="server" CssClass="form-select" />
            <asp:RequiredFieldValidator ID="rfvBarn" runat="server"
                ControlToValidate="ddlBarn"
                InitialValue=""
                ErrorMessage="Debe seleccionar un galpón"
                CssClass="text-danger"
                Display="Dynamic"
                ValidationGroup="VaccinationForm" />
        </div>

        <!-- Selección de vacuna -->
        <div class="mb-3">
            <label for="ddlVaccine" class="form-label">Vacuna: <span class="text-danger">*</span></label>
            <asp:DropDownList ID="ddlVaccine" runat="server" CssClass="form-select" />
            <asp:RequiredFieldValidator ID="rfvVaccine" runat="server"
                ControlToValidate="ddlVaccine"
                InitialValue=""
                ErrorMessage="Debe seleccionar una vacuna"
                CssClass="text-danger"
                Display="Dynamic"
                ValidationGroup="VaccinationForm" />
        </div>

        <!-- Fecha programada -->
        <div class="mb-3">
            <label for="txtScheduledDate" class="form-label">Fecha programada: <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtScheduledDate" runat="server" CssClass="form-control" TextMode="Date" />
            <asp:RequiredFieldValidator ID="rfvScheduledDate" runat="server"
                ControlToValidate="txtScheduledDate"
                ErrorMessage="Debe indicar la fecha programada"
                CssClass="text-danger"
                Display="Dynamic"
                ValidationGroup="VaccinationForm" />
        </div>

        <!-- Notas -->
        <div class="mb-3">
            <label for="txtNotes" class="form-label">Notas:</label>
            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
        </div>

        <!-- Botones -->
        <div class="d-flex flex-column flex-sm-row justify-content-center gap-2 mt-3">
            <a href="VaccinationScheduleList.aspx" class="btn btn-danger d-flex align-items-center justify-content-center">
                <i class="bi bi-arrow-left-circle-fill me-2"></i>Volver
            </a>
            <asp:Button ID="btnSave" runat="server"
                CssClass="btn btn-success d-flex align-items-center justify-content-center"
                Text="Guardar"
                OnClick="btnSave_Click"
                ValidationGroup="VaccinationForm" />
        </div>
    </div>

</asp:Content>
