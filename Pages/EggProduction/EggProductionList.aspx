<%@ Page Title="Listado de Producción de Huevos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EggProductionList.aspx.cs" Inherits="LasDeliciasERP.Pages.EggProduction.EggProductionList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow-sm p-4">
        <h2 class="mb-4">Listado de Producción de Huevos</h2>

        <!-- Bloque de Inventario Resumido -->
        <div class="card shadow-sm p-4 mt-4">
            <h3 class="mb-3">Inventario de Huevos</h3>

            <div class="table-responsive">
                <asp:GridView ID="GridViewEggInventory" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-striped table-hover align-middle text-center"
                    AllowSorting="False">

                    <Columns>
                        <asp:BoundField DataField="EggTypeName" HeaderText="Tipo de Huevo" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="QuantityS" HeaderText="Huevos S" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="QuantityM" HeaderText="Huevos M" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="QuantityL" HeaderText="Huevos L" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="QuantityXL" HeaderText="Huevos XL" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="TotalQuantity" HeaderText="Total Huevos" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- Botón Agregar nueva recolección -->
        <div class="mb-3 mt-4">
            <a href="EggProductionForm.aspx" class="btn btn-success">
                <i class="bi bi-plus-circle-fill"></i> Agregar Nueva Recolección
            </a>
        </div>

        <div class="card shadow-sm p-4">
            <!-- Buscador -->
            <div class="mb-3 d-flex gap-2 align-items-center flex-wrap">
                <label class="form-label mb-0">Fecha:</label>
                <asp:TextBox ID="txtSearchDate" runat="server" CssClass="form-control form-control-sm" placeholder="dd/mm/yyyy" TextMode="Date"></asp:TextBox>

                <label class="form-label mb-0">Tipo de Huevo:</label>
                <asp:DropDownList ID="ddlEggTypeFilter" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                    <asp:ListItem Text="-- Tipo de Huevo --" Value="" />
                </asp:DropDownList>

                <label class="form-label mb-0">Galpón:</label>
                <asp:DropDownList ID="ddlBarnFilter" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                    <asp:ListItem Text="-- Galpón --" Value="" />
                </asp:DropDownList>

                <!-- Botones elegantes -->
                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary btn-sm d-flex align-items-center shadow-sm" OnClick="btnSearch_Click">
        <i class="bi bi-search me-2"></i> Buscar
                </asp:LinkButton>

                <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-danger btn-sm d-flex align-items-center shadow-sm" OnClick="btnClear_Click">
        <i class="bi bi-x-circle me-2"></i> Limpiar
                </asp:LinkButton>
            </div>

            <!-- Grid responsive -->
            <div class="table-responsive">
                <asp:GridView ID="GridViewEggProduction" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-striped table-hover align-middle text-center"
                    AllowPaging="True" PageSize="10"
                    AllowSorting="True"
                    OnPageIndexChanging="GridViewEggProduction_PageIndexChanging"
                    OnRowDataBound="GridViewEggProduction_RowDataBound"
                    OnSorting="GridViewEggProduction_Sorting">

                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="ProductionDate" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" SortExpression="ProductionDate" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <%--<asp:BoundField DataField="EggTypeName" HeaderText="Tipo de Huevo" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />--%>
                        <asp:BoundField DataField="BarnName" HeaderText="Galpón" SortExpression="BarnName" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="QuantityS" HeaderText="Huevos S" SortExpression="QuantityS" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="QuantityM" HeaderText="Huevos M" SortExpression="QuantityM" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="QuantityL" HeaderText="Huevos L" SortExpression="QuantityL" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="QuantityXL" HeaderText="Huevos XL" SortExpression="QuantityXL" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="TotalQuantity" HeaderText="Total Huevos" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="TotalWeight" HeaderText="Peso Total (g)" DataFormatString="{0:F2}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />
                        <asp:BoundField DataField="AverageWeight" HeaderText="Peso Promedio (g)" DataFormatString="{0:F2}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="fw-bold bg-light text-center" />

                        <asp:TemplateField HeaderText="Notas" HeaderStyle-CssClass="fw-bold bg-light text-center">
                            <ItemTemplate>
                                <span title='<%# Eval("Notes") %>'>
                                    <%# Eval("Notes").ToString().Length > 20 ? Eval("Notes").ToString().Substring(0, 20) + "..." : Eval("Notes") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Acciones" HeaderStyle-CssClass="fw-bold bg-light text-center">
                            <ItemTemplate>
                                <div class="d-flex justify-content-center gap-2">
                                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn btn-sm btn-warning d-flex align-items-center"
                                        OnClick="btnEdit_Click" CommandArgument='<%# Eval("Id") %>'>
                                <i class="bi bi-pencil-fill me-1"></i> Editar
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
