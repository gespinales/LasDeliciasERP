<%@ Page Title="Registrar Alimentación" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterFeeding.aspx.cs" Inherits="LasDeliciasERP.Pages.Feeding.RegisterFeeding" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow-sm p-4">
        <h2 class="mb-4">🍽️ Registrar Alimentación</h2>

        <asp:Panel ID="pnlForm" runat="server" CssClass="card shadow-sm p-4 mb-4" Style="max-width: 600px; margin: auto;">
            <asp:Label ID="lblInfo" runat="server" CssClass="d-block mb-3"></asp:Label>

            <asp:Button ID="btnRegistrar" runat="server" Text="Registrar Alimentación"
                CssClass="btn btn-primary btn-lg mb-3"
                OnClick="btnRegistrar_Click" />

            <asp:Label ID="lblMensaje" runat="server" CssClass="d-block mt-3 fw-bold"></asp:Label>
        </asp:Panel>

        <div class="card shadow-sm p-4" style="max-width: 800px; margin: auto;">
            <h4 class="mb-3">📑 Historial de Alimentación</h4>
            <asp:GridView ID="gvHistorial" runat="server" AutoGenerateColumns="False"
                CssClass="table table-striped table-hover text-center"
                AllowPaging="True" PageSize="10" OnPageIndexChanging="gvHistorial_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="FeedingDate" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="Quantity" HeaderText="Cantidad (lbs)" DataFormatString="{0:N2}" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>
