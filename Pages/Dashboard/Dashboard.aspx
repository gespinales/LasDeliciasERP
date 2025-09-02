<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="LasDeliciasERP.Pages.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="mb-4 text-center">Panel de Control - LasDeliciasERP</h1>

    <div class="row g-4">
        <!-- Módulo general -->
        <div class="col-md-12">
            <h3 class="mb-3">Gestiones generales</h3>
            <div class="row g-3">
                <!-- Producción de Huevos -->
                <div class="col-md-4">
                    <a href="../EggProduction/EggProductionList.aspx" class="text-decoration-none">
                        <div class="card text-white shadow-sm h-100 hover-shadow bg-warning">
                            <div class="card-body">
                                <i class="bi bi-egg-fill display-4"></i>
                                <h5 class="card-title mt-3">Producción de Huevos</h5>
                                <p class="card-text">Registrar y controlar la recolección de huevos por tamaño y peso.</p>
                            </div>
                        </div>
                    </a>
                </div>

                <!-- Gastos de Concentrado -->
                <div class="col-md-4">
                    <a href="../Inventory/InventoryPurchaseList.aspx" class="text-decoration-none">
                        <div class="card text-white shadow-sm h-100 hover-shadow bg-info">
                            <div class="card-body">
                                <i class="bi bi-basket-fill display-4"></i>
                                <h5 class="card-title mt-3">Compras de insumos</h5>
                                <p class="card-text">Registrar compras yde insumos para las gallinas y más.</p>
                            </div>
                        </div>
                    </a>
                </div>

                <!-- Clientes -->
                <div class="col-md-4">
                    <a href="../Customer/CustomerList.aspx" class="text-decoration-none">
                        <div class="card text-white shadow-sm h-100 hover-shadow bg-primary">
                            <div class="card-body text-center">
                                <i class="bi bi-people-fill display-4"></i>
                                <h5 class="card-title mt-3">Clientes</h5>
                                <p class="card-text">Administrar información de clientes y contactos para ventas.</p>
                            </div>
                        </div>
                    </a>
                </div>
            </div>
        </div>

        <!-- Módulo Aves -->
        <div class="col-md-12">
            <h3 class="mb-3">Módulo Aves</h3>
            <div class="row g-3">
                <!-- Lotes -->
                <div class="col-md-4">
                    <a href="../Bird/BirdBatchList.aspx" class="text-decoration-none">
                        <div class="card text-white shadow-sm h-100 hover-shadow bg-primary">
                            <div class="card-body">
                                <i class="bi bi-box-seam display-4"></i>
                                <h5 class="card-title mt-3">Lotes</h5>
                                <p class="card-text">Gestiona los lotes de aves por galpón.</p>
                            </div>
                        </div>
                    </a>
                </div>

                <!-- Vacunas -->
                <div class="col-md-4">
                    <a href="../Vaccination/VaccinationScheduleList.aspx" class="text-decoration-none">
                        <div class="card text-white shadow-sm h-100 hover-shadow bg-success">
                            <div class="card-body">
                                <i class="bi bi-heart-pulse-fill display-4"></i>
                                <h5 class="card-title mt-3">Vacunas</h5>
                                <p class="card-text">Controlar aplicaciones de vacunas y salud de las gallinas.</p>
                            </div>
                        </div>
                    </a>
                </div>

                <!-- Decesos -->
                <div class="col-md-4">
                    <a href="../Bird/BirdDeathList.aspx" class="text-decoration-none">
                        <div class="card text-white shadow-sm h-100 hover-shadow bg-dark">
                            <div class="card-body">
                                <i class="bi bi-x-circle-fill display-4"></i>
                                <h5 class="card-title mt-3">Decesos</h5>
                                <p class="card-text">Registrar y controlar los decesos de aves por galpón.</p>
                            </div>
                        </div>
                    </a>
                </div>

                <!-- Movimientos -->
                <%--<div class="col-md-4">
                    <a href="../Bird/BirdMovementList.aspx" class="text-decoration-none">
                        <div class="card text-white shadow-sm h-100 hover-shadow bg-success">
                            <div class="card-body">
                                <i class="bi bi-arrow-left-right display-4"></i>
                                <h5 class="card-title mt-3">Movimientos</h5>
                                <p class="card-text">Reporte entradas y salidas de aves.</p>
                            </div>
                        </div>
                    </a>
                </div>--%>

                <!-- Población -->
                <%--<div class="col-md-4">
                    <a href="../Bird/PopulationList.aspx" class="text-decoration-none">
                        <div class="card text-white shadow-sm h-100 hover-shadow bg-warning">
                            <div class="card-body">
                                <i class="bi bi-people-fill display-4"></i>
                                <h5 class="card-title mt-3">Población</h5>
                                <p class="card-text">Visualiza la población actual por tipo de ave y galpón.</p>
                            </div>
                        </div>
                    </a>
                </div>--%>
            </div>
        </div>

        <!-- Módulo Financiero -->
        <div class="col-md-12">
            <h3 class="mb-3">Módulo Financiero</h3>
            <div class="row g-3">
                <!-- Ventas -->
                <div class="col-md-3">
                    <a href="../Sales/SalesList.aspx" class="text-decoration-none">
                        <div class="card text-white shadow-sm h-100 hover-shadow bg-success">
                            <div class="card-body text-center">
                                <i class="bi bi-currency-dollar display-4"></i>
                                <h5 class="card-title mt-3">Ventas</h5>
                                <p class="card-text">Registrar y consultar todas las ventas realizadas.</p>
                            </div>
                        </div>
                    </a>
                </div>

                <%--  <!-- Compras -->
        <div class="col-md-3">
            <a href="../Purchases/PurchaseList.aspx" class="text-decoration-none">
                <div class="card text-white shadow-sm h-100 hover-shadow bg-success">
                    <div class="card-body text-center">
                        <i class="bi bi-bag-fill display-4"></i>
                        <h5 class="card-title mt-3">Compras</h5>
                        <p class="card-text">Registrar y controlar todas las compras de insumos.</p>
                    </div>
                </div>
            </a>
        </div>

        <!-- Gastos -->
        <div class="col-md-3">
            <a href="../Expenses/ExpenseList.aspx" class="text-decoration-none">
                <div class="card text-white shadow-sm h-100 hover-shadow bg-danger">
                    <div class="card-body text-center">
                        <i class="bi bi-receipt-cutoff display-4"></i>
                        <h5 class="card-title mt-3">Gastos</h5>
                        <p class="card-text">Registrar todos los gastos operativos y financieros.</p>
                    </div>
                </div>
            </a>
        </div>--%>

                <!-- Reportes financieros -->
                <div class="col-md-3">
                    <a href="../Finance/FinancialReports.aspx" class="text-decoration-none">
                        <div class="card text-white shadow-sm h-100 hover-shadow bg-warning">
                            <div class="card-body text-center">
                                <i class="bi bi-graph-up-arrow display-4"></i>
                                <h5 class="card-title mt-3">Reportes</h5>
                                <p class="card-text">Visualiza reportes financieros y estados de resultados.</p>
                            </div>
                        </div>
                    </a>
                </div>
            </div>
        </div>


        <!-- Agrega más módulos aquí -->
    </div>

    <style>
        /* Efecto hover para las tarjetas */
        .hover-shadow:hover {
            transform: translateY(-5px);
            transition: all 0.3s ease;
            box-shadow: 0 8px 20px rgba(0,0,0,0.3);
        }
        /* Tarjetas con altura uniforme */
        .card {
            min-height: 200px;
        }
    </style>
</asp:Content>
