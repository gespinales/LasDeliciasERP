<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="LasDeliciasERP.Pages.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="mb-4 text-center">Panel de Control - LasDeliciasERP</h1>

    <div class="row g-4">
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
            <a href="FeedExpenses.aspx" class="text-decoration-none">
                <div class="card text-white shadow-sm h-100 hover-shadow bg-success">
                    <div class="card-body">
                        <i class="bi bi-basket-fill display-4"></i>
                        <h5 class="card-title mt-3">Gastos de Concentrado</h5>
                        <p class="card-text">Registrar compras y gastos de alimentos para las gallinas.</p>
                    </div>
                </div>
            </a>
        </div>

        <!-- Vacunas -->
        <div class="col-md-4">
            <a href="Vaccines.aspx" class="text-decoration-none">
                <div class="card text-white shadow-sm h-100 hover-shadow bg-danger">
                    <div class="card-body">
                        <i class="bi bi-heart-pulse-fill display-4"></i>
                        <h5 class="card-title mt-3">Vacunas</h5>
                        <p class="card-text">Controlar aplicaciones de vacunas y salud de las gallinas.</p>
                    </div>
                </div>
            </a>
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