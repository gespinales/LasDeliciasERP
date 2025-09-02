<%@ Page Title="Aplicar Vacuna" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="VaccinationRecord.aspx.cs"
    Inherits="LasDeliciasERP.Pages.Vaccination.VaccinationRecord" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow-sm p-4">
        <h2 class="mb-4 text-center fw-bold" id="formTitle" runat="server">Aplicar Vacuna</h2>

        <asp:HiddenField ID="hfId" runat="server" />
        <asp:HiddenField ID="hfAction" runat="server" Value="apply" />
        <asp:HiddenField ID="hfScheduleId" runat="server" />

        <!-- Galpón -->
        <div class="mb-3">
            <label class="form-label">Galpón:</label>
            <asp:Label ID="lblBarn" runat="server" CssClass="form-control-plaintext fw-bold"></asp:Label>
        </div>

        <!-- Vacuna -->
        <div class="mb-3">
            <label class="form-label">Vacuna:</label>
            <asp:Label ID="lblVaccine" runat="server" CssClass="form-control-plaintext fw-bold"></asp:Label>
        </div>

        <!-- Fecha programada -->
        <div class="mb-3">
            <label class="form-label">Fecha programada:</label>
            <asp:Label ID="lblScheduledDate" runat="server" CssClass="form-control-plaintext"></asp:Label>
        </div>

        <!-- Estado -->
        <div class="mb-3">
            <label class="form-label">Estado:</label>
            <asp:Label ID="lblStatus" runat="server" CssClass="form-control-plaintext fw-bold"></asp:Label>
        </div>

        <!-- Fecha de aplicación real -->
        <div class="mb-3">
            <label for="txtAppliedDate" class="form-label">Fecha de aplicación: <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtAppliedDate" runat="server" CssClass="form-control" TextMode="Date" />
            <asp:RequiredFieldValidator ID="rfvAppliedDate" runat="server"
                ControlToValidate="txtAppliedDate"
                ErrorMessage="Debe indicar la fecha de aplicación"
                CssClass="text-danger"
                Display="Dynamic"
                ValidationGroup="VaccinationRecord" />
        </div>

        <!-- Cantidad aplicada -->
        <div class="mb-3">
            <label for="txtQuantityApplied" class="form-label">Cantidad aplicada: <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtQuantityApplied" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvQuantityApplied" runat="server"
                ControlToValidate="txtQuantityApplied"
                ErrorMessage="Debe indicar la cantidad aplicada"
                CssClass="text-danger"
                Display="Dynamic"
                ValidationGroup="VaccinationRecord" />
            <asp:RangeValidator ID="rvQuantityApplied" runat="server"
                ControlToValidate="txtQuantityApplied"
                Type="Integer" MinimumValue="1" MaximumValue="1000000"
                ErrorMessage="La cantidad debe ser un entero mayor a 0"
                CssClass="text-danger"
                ValidationGroup="VaccinationRecord"
                Display="Dynamic" />
        </div>

        <!-- Aplicado por (opcional) -->
        <div class="mb-3">
            <label for="txtAppliedBy" class="form-label">Aplicado por:</label>
            <asp:TextBox ID="txtAppliedBy" runat="server" CssClass="form-control" MaxLength="100" />
        </div>

        <!-- Observaciones -->
        <div class="mb-3">
            <label for="txtNotes" class="form-label">Observaciones:</label>
            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
        </div>

        <!-- Mensajes -->
        <asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="text-danger fw-bold d-block mb-2" Visible="false"></asp:Label>

        <!-- Botones -->
        <div class="d-flex flex-column flex-sm-row justify-content-center gap-2 mt-3">
            <a href="VaccinationScheduleList.aspx" 
               class="btn btn-danger d-flex align-items-center justify-content-center">
                <i class="bi bi-arrow-left-circle-fill me-2"></i>Volver
            </a>
            <asp:Button ID="btnApply" runat="server"
                CssClass="btn btn-success d-flex align-items-center justify-content-center"
                Text="Aplicar"
                OnClick="btnApply_Click"
                ValidationGroup="VaccinationRecord" />
        </div>
    </div>

    <!-- Script de validación extra con SweetAlert (usa el nuevo id txtQuantityApplied) -->
    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%= btnApply.ClientID %>").click(function (e) {
                var qty = $("#<%= txtQuantityApplied.ClientID %>").val();
                if (!qty || parseInt(qty) <= 0) {
                    e.preventDefault();
                    Swal.fire({
                        icon: 'warning',
                        title: 'Cantidad requerida',
                        text: 'Por favor ingrese una cantidad válida antes de guardar.',
                        confirmButtonText: 'Entendido',
                        confirmButtonColor: '#3085d6'
                    });
                    return false;
                }
            });
        });
    </script>

</asp:Content>
