using BLL.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmGraficaVentasPorVendedores : Form
    {
        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly GraficasService _graficasService;

        public FrmGraficaVentasPorVendedores()
        {
            InitializeComponent();
            _graficasService = new GraficasService(cnStr);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaVentasPorVendedores_Load(object sender, EventArgs e)
        {
            CargarVentasPorVendedores();
        }

        private void CargarVentasPorVendedores()
        {
            ChartVentasPorVendedores.Series.Clear();
            ChartVentasPorVendedores.Titles.Clear();
            // Título del gráfico
            Title titulo = new Title
            {
                Text = "» Gráfica de ventas por vendedores de todos los años «",
                Font = new Font("Arial", 16, FontStyle.Bold)
            };
            ChartVentasPorVendedores.Titles.Add(titulo);
            // Configuración de la serie
            Series serie = new Series
            {
                Name = "Ventas",
                Color = Color.FromArgb(0, 51, 102),
                IsValueShownAsLabel = true,
                ChartType = SeriesChartType.Doughnut,
                Label = "#VALX: #VALY{C2}"
            };
            serie["PieLabelStyle"] = "Outside";
            serie["DoughnutHoleSize"] = "65";
            serie.SmartLabelStyle.Enabled = true;
            serie.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;
            serie.SmartLabelStyle.CalloutLineColor = Color.Black;
            serie.LabelForeColor = Color.DarkSlateGray;
            serie.LabelBackColor = Color.WhiteSmoke;
            ChartVentasPorVendedores.Series.Add(serie);
            List<(string Vendedor, decimal TotalVentas)> ventas = new List<(string Vendedor, decimal TotalVentas)>();
            try
            {
                ventas = _graficasService.ObtenerVentasPorVendedores();
                if (!ventas.Any())
                {
                    U.MsgExclamation("No se encontraron datos de ventas por vendedores para mostrar en la gráfica.");
                    return;
                }
                int i = 0;
                foreach (var (vendedor, totalVentas) in ventas)
                {
                    int puntoIndex = serie.Points.AddXY(vendedor, totalVentas);
                    serie.Points[puntoIndex].Color =
                        ChartColors.Paleta[i % ChartColors.Paleta.Length];
                    i++;
                }
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            decimal total = ventas.Sum(v => v.TotalVentas);

            Title subTitulo = new Title
            {
                Text = $"Total de ventas: {total:C2}",
                Docking = Docking.Top,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Black,
                IsDockedInsideChartArea = false,
                Alignment = ContentAlignment.TopLeft,
                DockingOffset = -3
            };
            ChartVentasPorVendedores.Titles.Add(subTitulo);
        }
    }
}
