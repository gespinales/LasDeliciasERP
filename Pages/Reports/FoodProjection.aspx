<%@ Page Title="Proyección de Alimento" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FoodProjection.aspx.cs" Inherits="LasDeliciasERP.Pages.Reports.FoodProjection" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow-sm p-4">
        <h2 class="mb-4">📊 Proyección de Alimento</h2>

        <div class="mb-3">
            <asp:Label ID="lblAves" runat="server" CssClass="h5 text-primary d-block mb-1"></asp:Label>
            <asp:Label ID="lblComida" runat="server" CssClass="h5 text-success d-block mb-1"></asp:Label>
            <asp:Label ID="lblDias" runat="server" CssClass="h5 text-danger d-block mb-3"></asp:Label>
        </div>

        <!-- Selector de tipo de gráfico -->
        <div class="mb-4">
            <label for="chartTypeSelect" class="form-label">Seleccione tipo de gráfico:</label>
            <select id="chartTypeSelect" class="form-select" onchange="toggleCharts()">
                <option value="bar">Barras</option>
                <option value="doughnut">Doughnut Semáforo</option>
            </select>
        </div>

        <div class="d-flex flex-wrap gap-3">
            <!-- Contenedor principal de proyección (barras o doughnut) -->
            <div id="projectionContainer" class="flex-grow-1" style="min-width: 400px; max-width: 600px; height: 450px;">
                <div id="barChartContainer" class="card shadow-sm p-3 mb-4" style="width: 100%; height: 100%;">
                    <canvas id="foodBarChart"></canvas>
                </div>

                <div id="doughnutChartContainer" class="card shadow-sm p-3 mb-4" style="width: 100%; height: 100%; display: none;">
                    <canvas id="foodDoughnutChart"></canvas>
                </div>
            </div>

            <!-- Histograma de historial (siempre visible a la derecha) -->
            <div class="flex-grow-1" style="min-width: 400px; max-width: 600px; height: 450px;">
                <!-- Histograma de historial de alimentación -->
                <div id="lineChartContainer" class="card shadow-sm p-3 mb-4" style="max-width: 900px; width: 100%; height: 450px; margin: auto; display: none;">
                    <canvas id="feedingLineChart"></canvas>
                </div>
            </div>
        </div>
        <script>
            function toggleCharts() {
                const type = document.getElementById('chartTypeSelect').value;
                document.getElementById('barChartContainer').style.display = (type === "bar") ? "block" : "none";
                document.getElementById('doughnutChartContainer').style.display = (type === "doughnut") ? "block" : "none";
            }

            document.addEventListener("DOMContentLoaded", function () {
                // -------------------------------
                // Gráfica de barras
                // -------------------------------
                if (typeof projectionData !== "undefined" && projectionData) {
                    const ctxBar = document.getElementById('foodBarChart').getContext('2d');
                    new Chart(ctxBar, {
                        type: 'bar',
                        data: {
                            labels: ["Consumo diario (lbs)", "Comida disponible (lbs)", "Días disponibles"],
                            datasets: [{
                                label: "Proyección de alimento",
                                data: [
                                    projectionData.DailyConsumptionKg,
                                    projectionData.TotalFoodKg,
                                    projectionData.AvailableDays
                                ],
                                backgroundColor: ["#ff6384", "#36a2eb", "#4bc0c0"]
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: { legend: { display: false }, tooltip: { enabled: true } },
                            scales: { y: { beginAtZero: true, ticks: { callback: value => value.toLocaleString() } } }
                        }
                    });

                    // -------------------------------
                    // Doughnut tipo semáforo
                    // -------------------------------
                    const maxDays = 30;
                    const availableDays = Math.min(projectionData.AvailableDays, maxDays);
                    const remainingDays = maxDays - availableDays;

                    let color = "#36a2eb";
                    if (projectionData.AvailableDays > 15) color = "#4caf50";
                    else if (projectionData.AvailableDays >= 5) color = "#ffeb3b";
                    else color = "#f44336";

                    const ctxDoughnut = document.getElementById('foodDoughnutChart').getContext('2d');
                    new Chart(ctxDoughnut, {
                        type: 'doughnut',
                        data: {
                            labels: ["Días disponibles", "Días restantes"],
                            datasets: [{ data: [availableDays, remainingDays], backgroundColor: [color, "#e0e0e0"], hoverOffset: 4 }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            cutout: "70%",
                            plugins: {
                                legend: { display: false },
                                tooltip: { enabled: true },
                                beforeDraw: function (chart) {
                                    const width = chart.width, height = chart.height, ctx = chart.ctx;
                                    ctx.restore();
                                    const fontSize = (height / 5).toFixed(2);
                                    ctx.font = fontSize + "px Arial";
                                    ctx.textBaseline = "middle";
                                    const text = Math.round(projectionData.AvailableDays) + " días",
                                        textX = Math.round((width - ctx.measureText(text).width) / 2),
                                        textY = height / 2;
                                    ctx.fillStyle = "#000";
                                    ctx.fillText(text, textX, textY);
                                    ctx.save();
                                }
                            }
                        }
                    });
                }

                // -------------------------------
                // Histograma de historial de alimentación
                // -------------------------------
                if (typeof historyData !== "undefined" && historyData && historyData.length > 0) {
                    const ctxHist = document.getElementById('feedingLineChart').getContext('2d');

                    // Mostrar el contenedor
                    document.getElementById('lineChartContainer').style.display = "block";

                    if (window.feedingLineChartInstance) window.feedingLineChartInstance.destroy();

                    window.feedingLineChartInstance = new Chart(ctxHist, {
                        type: 'bar', // histograma
                        data: {
                            labels: historyData.map(h => new Date(h.FeedingDate).toLocaleDateString()),
                            datasets: [{
                                label: "Alimento administrado (lbs)",
                                data: historyData.map(h => h.Quantity),
                                backgroundColor: "#36a2eb"
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: { legend: { display: true }, tooltip: { enabled: true } },
                            scales: {
                                y: { beginAtZero: true },
                                x: { ticks: { autoSkip: true, maxTicksLimit: 10 } }
                            }
                        }
                    });
                }
            });
        </script>
    </div>
</asp:Content>
