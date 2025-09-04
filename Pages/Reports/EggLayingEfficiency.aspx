<%@ Page Title="Reporte de Eficiencia de Postura" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EggLayingEfficiency.aspx.cs" Inherits="LasDeliciasERP.Pages.Reports.EggLayingEfficiency" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow-sm p-4">
        <h2 class="mb-4">🥚 Eficiencia de Postura</h2>

        <!-- Filtros de fechas -->
        <div class="row mb-4">
            <div class="col-md-3 mb-2">
                <label class="form-label">Fecha inicio:</label>
                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-3 mb-2">
                <label class="form-label">Fecha fin:</label>
                <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-3 align-self-end mb-2">
                <asp:Button ID="btnFilter" runat="server" CssClass="btn btn-primary" Text="Filtrar" OnClick="btnFilter_Click" />
                <asp:Button ID="btnClear" runat="server" CssClass="btn btn-secondary ms-2" Text="Limpiar" OnClick="btnClear_Click" />
            </div>
        </div>

        <!-- Contenedor de gráficos en fila responsive -->
        <div class="row">
            <!-- Gráfico por galpón (barras) -->
            <div class="col-lg-6 col-12 mb-4">
                <div class="card shadow-sm p-3" style="height: 400px;">
                    <canvas id="barnEfficiencyChart"></canvas>
                </div>
            </div>

            <!-- Gráfico de evolución (línea) -->
            <div class="col-lg-6 col-12 mb-4">
                <div class="card shadow-sm p-3" style="height: 400px;">
                    <canvas id="timeEfficiencyChart"></canvas>
                </div>
            </div>
        </div>
    </div>

    <script>
        function renderCharts(data) {
            // -------------------------------
            // Gráfico de barras por galpón
            // -------------------------------
            const ctxBarn = document.getElementById('barnEfficiencyChart').getContext('2d');
            if (window.barnChartInstance) window.barnChartInstance.destroy();
            window.barnChartInstance = new Chart(ctxBarn, {
                type: 'bar',
                data: {
                    labels: data.map(r => r.BarnName),
                    datasets: [{
                        label: 'Eficiencia de postura (%)',
                        data: data.map(r => r.EfficiencyPercentage),
                        backgroundColor: data.map(r => {
                            if (r.EfficiencyPercentage >= 90) return '#4caf50';
                            else if (r.EfficiencyPercentage >= 80) return '#ffeb3b';
                            else return '#f44336';
                        })
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: { y: { beginAtZero: true, max: 100, ticks: { callback: v => v + '%' } } }
                }
            });

            // -------------------------------
            // Gráfico de línea por galpón en el tiempo
            // -------------------------------
            const ctxTime = document.getElementById('timeEfficiencyChart').getContext('2d');
            if (window.timeChartInstance) window.timeChartInstance.destroy();

            const barns = [...new Set(data.map(r => r.BarnName))];
            const datasets = barns.map(b => {
                const barnData = data.filter(r => r.BarnName === b)
                    .sort((a, b) => new Date(a.ProductionDate) - new Date(b.ProductionDate));
                return {
                    label: b,
                    data: barnData.map(r => ({ x: new Date(r.ProductionDate), y: r.EfficiencyPercentage })),
                    fill: false,
                    borderColor: '#36a2eb',
                    tension: 0.1
                };
            });

            // Líneas de referencia para estándar mínimo y máximo
            datasets.push(
                {
                    label: 'Mínimo esperado (90%)',
                    data: data.map(r => ({ x: new Date(r.ProductionDate), y: 90 })),
                    borderColor: 'red',
                    borderDash: [5, 5],
                    fill: false,
                    pointRadius: 0
                },
                {
                    label: 'Máximo esperado (95%)',
                    data: data.map(r => ({ x: new Date(r.ProductionDate), y: 95 })),
                    borderColor: 'green',
                    borderDash: [5, 5],
                    fill: false,
                    pointRadius: 0
                }
            );

            window.timeChartInstance = new Chart(ctxTime, {
                type: 'line',
                data: { datasets: datasets },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    parsing: { xAxisKey: 'x', yAxisKey: 'y' },
                    scales: {
                        x: {
                            type: 'time',
                            time: { unit: 'day' },
                            title: { display: true, text: 'Fecha' }
                        },
                        y: { min: 0, max: 200, title: { display: true, text: 'Eficiencia (%)' } }
                    },
                    plugins: { legend: { position: 'bottom' } }
                }
            });
        }

        document.addEventListener("DOMContentLoaded", function () {
            if (typeof efficiencyData !== "undefined") renderCharts(efficiencyData);
        });
    </script>
</asp:Content>
