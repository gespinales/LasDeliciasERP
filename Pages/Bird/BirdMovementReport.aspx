<%@ Page Title="Reporte de Movimientos" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="BirdMovementReport.aspx.cs" Inherits="LasDeliciasERP.Pages.Bird.BirdMovementReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2 class="mb-4">Reporte de Movimientos de Aves</h2>

        <!-- FILTROS -->
        <div class="row g-3 mb-3">
            <div class="col-md-3">
                <label for="ddlBatch" class="form-label">Lote</label>
                <asp:DropDownList ID="ddlBatch" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>

            <div class="col-md-3">
                <label for="ddlBarn" class="form-label">Galpón</label>
                <asp:DropDownList ID="ddlBarn" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>

            <div class="col-md-3">
                <label for="ddlBirdType" class="form-label">Tipo de Ave (Raza)</label>
                <asp:DropDownList ID="ddlBirdType" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>

            <div class="col-md-3">
                <label for="ddlChartType" class="form-label">Tipo de Gráfica</label>
                <asp:DropDownList ID="ddlChartType" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Barras" Value="bar" />
                    <asp:ListItem Text="Líneas" Value="line" />
                    <asp:ListItem Text="Pastel" Value="pie" />
                    <asp:ListItem Text="Dona" Value="doughnut" />
                </asp:DropDownList>
            </div>
        </div>

        <div class="row g-3 mb-4">
            <div class="col-md-4">
                <label for="txtStartDate" class="form-label">Fecha Inicio</label>
                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>

            <div class="col-md-4">
                <label for="txtEndDate" class="form-label">Fecha Fin</label>
                <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>

            <div class="col-md-2 d-flex align-items-end">
                <asp:Button ID="btnFilter" runat="server" Text="Filtrar" CssClass="btn btn-primary w-100" OnClick="btnFilter_Click" />
            </div>
        </div>

        <!-- GRÁFICO -->
        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <div class="chart-container" style="position: relative; height:400px; width:100%;">
                    <canvas id="movementChart"></canvas>
                </div>
                <asp:Literal ID="ltChartData" runat="server"></asp:Literal>
            </div>
        </div>

        <!-- TABLA DE MOVIMIENTOS -->
        <asp:GridView ID="gvMovements" runat="server" AutoGenerateColumns="False"
            CssClass="table table-striped table-hover"
            AllowPaging="True" PageSize="10"
            OnPageIndexChanging="gvMovements_PageIndexChanging">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="text-center" />
                <asp:BoundField DataField="MovementType" HeaderText="Movimiento" />
                <asp:BoundField DataField="Quantity" HeaderText="Cantidad" />
                <asp:BoundField DataField="MovementDate" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Reason" HeaderText="Motivo" />
                <asp:BoundField DataField="BirdTypeName" HeaderText="Tipo de Ave" />
                <asp:BoundField DataField="BarnName" HeaderText="Galpón" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
