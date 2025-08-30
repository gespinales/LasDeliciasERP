<%@ Page Title="Programación de Vacunación" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VaccinationScheduleList.aspx.cs" Inherits="LasDeliciasERP.Pages.Vaccination.VaccinationScheduleList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-3">
        <h2>Programación de Vacunación</h2>
        <a href="VaccinationSchedule.aspx?action=save" class="btn btn-success">
            <i class="bi bi-plus-circle"></i>Nueva Programación
        </a>
    </div>

    <asp:GridView ID="GridViewSchedules" runat="server"
        AutoGenerateColumns="False"
        AllowPaging="True"
        PageSize="10"
        OnPageIndexChanging="GridViewSchedules_PageIndexChanging"
        OnRowDataBound="GridViewSchedules_RowDataBound"
        CssClass="table table-striped table-hover align-middle text-center">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="text-center" />
            <asp:BoundField DataField="BarnName" HeaderText="Galpón" />
            <asp:BoundField DataField="VaccineName" HeaderText="Vacuna" />
            <asp:BoundField DataField="ScheduledDate" HeaderText="Fecha Programada" DataFormatString="{0:dd/MM/yyyy}" />
            <asp:BoundField DataField="Status" HeaderText="Estado" ItemStyle-CssClass="fw-bold" />
            <asp:BoundField DataField="Notes" HeaderText="Notas" />
            <asp:BoundField DataField="AppliedDate" HeaderText="Fecha Aplicada" DataFormatString="{0:dd/MM/yyyy}" />
            <asp:BoundField DataField="AppliedBy" HeaderText="Aplicado Por" />
            <asp:BoundField DataField="QuantityApplied" HeaderText="Cantidad Aplicada" />

            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <div class="d-flex flex-wrap justify-content-center gap-1">
                        <asp:HyperLink ID="btnEdit" runat="server"
                            NavigateUrl='<%# "VaccinationSchedule.aspx?id=" + Eval("Id") + "&action=update" %>'
                            CssClass="btn btn-warning btn-sm flex-fill">
                        <i class="bi bi-pencil-square"></i> Editar
                        </asp:HyperLink>

                        <asp:HyperLink ID="btnDelete" runat="server"
                            NavigateUrl='<%# "VaccinationSchedule.aspx?id=" + Eval("Id") + "&action=delete" %>'
                            CssClass="btn btn-danger btn-sm flex-fill"
                            OnClientClick="return confirm('¿Está seguro de eliminar esta programación?');">
                        <i class="bi bi-trash"></i> Eliminar
                        </asp:HyperLink>

                        <asp:HyperLink ID="btnApply" runat="server"
                            NavigateUrl='<%# "VaccinationRecord.aspx?scheduleId=" + Eval("Id") + "&action=apply" %>'
                            CssClass="btn btn-success btn-sm flex-fill">
                        <i class="bi bi-check-circle"></i> Aplicar
                        </asp:HyperLink>

                        <asp:HyperLink ID="btnRecords" runat="server"
                            NavigateUrl='<%# "VaccinationRecord.aspx?scheduleId=" + Eval("Id") + "&action=view" %>'
                            CssClass="btn btn-info btn-sm flex-fill">
                        <i class="bi bi-list-check"></i> Ver Aplicaciones
                        </asp:HyperLink>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>


</asp:Content>
