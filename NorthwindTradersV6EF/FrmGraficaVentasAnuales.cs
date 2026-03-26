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
    public partial class FrmGraficaVentasAnuales : Form
    {
        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly GraficasService _graficasService;

        public FrmGraficaVentasAnuales()
        {
            InitializeComponent();
            _graficasService = new GraficasService(cnStr);
            ChartVentasAnuales.MouseMove += ChartVentasAnuales_MouseMove;
            ChartVentasAnuales.GetToolTipText += ChartVentasAnuales_GetToolTipText;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaVentasAnuales_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
            GroupBox.Text = $"» Comparativo de ventas mensuales de los últimos 2 años «";
            CargarComparativoVentasMensuales(2);
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            if (ComboBox.SelectedIndex == 0)
            {
                Utils.MsgExclamation("Seleccione un número de años válido.");
                return;
            }
            int years = Convert.ToInt32(ComboBox.SelectedValue);

            int totalAñosDisponibles = _graficasService.ObtenerTotalAñosConVentas();

            if (years > totalAñosDisponibles)
            {
                Utils.MsgExclamation($"Solo existen datos para {totalAñosDisponibles} años en la base de datos.");
                return;
            }
            CargarComparativoVentasMensuales(Convert.ToInt32(ComboBox.SelectedValue));
        }

        private void LlenarComboBox()
        {
            int totalAñosDisponibles = _graficasService.ObtenerTotalAñosConVentas();

            int limite = Math.Min(totalAñosDisponibles, 10);

            var items = new List<KeyValuePair<string, int>>();

            items.Add(new KeyValuePair<string, int>("»--- Seleccione ---«", 0));

            for (int i = 2; i <= limite; i++)
            {
                items.Add(new KeyValuePair<string, int>($"{i} Años", i));
            }

            ComboBox.DataSource = items;
            ComboBox.DisplayMember = "Key";
            ComboBox.ValueMember = "Value";
        }

        private void CargarComparativoVentasMensuales(int years)
        {
            ChartVentasAnuales.Series.Clear();
            ChartVentasAnuales.Titles.Clear();
            ChartVentasAnuales.Legends.Clear();

            var legend = new Legend("Default")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Center,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            ChartVentasAnuales.Legends.Add(legend);

            var datos = _graficasService.ObtenerVentasMensualesPorAños(years);

            var datosAgrupados = datos
                .GroupBy(x => x.Year)
                .OrderByDescending(x => x.Key);

            int i = 0;

            foreach (var grupo in datosAgrupados)
            {
                string nombreSerie = $"Ventas {grupo.Key}";

                var serie = ChartVentasAnuales.Series.Add(nombreSerie);

                //serie.ChartType = SeriesChartType.Line;
                serie.ChartType = SeriesChartType.Spline;
                serie.BorderWidth = 3;

                serie.MarkerStyle = MarkerStyle.Circle;
                serie.MarkerSize = 8;

                serie["LineTension"] = "0.5";

                serie.ShadowOffset = 2;
                serie.ShadowColor = Color.FromArgb(100, Color.Gray);

                serie.Color = ChartColors.Paleta[i % ChartColors.Paleta.Length];

                decimal totalAnual = grupo.Sum(x => x.Total);

                serie.Legend = legend.Name;
                serie.LegendText = $"{nombreSerie} (Total: {totalAnual:C2})";

                serie.ToolTip = $"{nombreSerie} #VALX: #VALY{{C2}}";

                serie.MarkerBorderWidth = 2;
                serie.MarkerBorderColor = Color.White;
                serie.BorderWidth = 3;
                serie.MarkerStyle = MarkerStyle.Circle;
                serie.MarkerSize = 7;
                foreach (var dato in grupo.OrderBy(x => x.Mes))
                {
                    int index = serie.Points.AddXY(dato.NombreMes, dato.Total);

                    DataPoint dp = serie.Points[index];

                    dp.ToolTip = $"{dato.NombreMes} {dato.Year}\nVentas: {dato.Total:C2}";

                    if (dato.Total != 0)
                    {
                        dp.Label = $"{dato.Total:C2}";
                        dp.MarkerSize = 10;
                    }
                    if (dato.Mes == 12)
                    {
                        dp.MarkerSize = 12;
                        dp.MarkerBorderWidth = 2;
                    }
                }

                i++;
            }

            ConfigurarAreaGrafica();

            Title titulo = new Title
            {
                Text = $"» Comparativo de ventas mensuales de los últimos {years} años «",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Alignment = ContentAlignment.TopCenter
            };

            ChartVentasAnuales.Titles.Add(titulo);
        }

        private void ConfigurarAreaGrafica()
        {
            var area = ChartVentasAnuales.ChartAreas[0];

            area.BackColor = Color.White;
            area.BackSecondaryColor = Color.FromArgb(245, 245, 245);
            area.BackGradientStyle = GradientStyle.TopBottom;

            // ===== EJE X =====
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.Title = "Meses";

            // SOLO líneas en cada mes
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            // DESACTIVAR líneas secundarias verticales
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisX.MinorTickMark.Enabled = false;

            // ===== EJE Y =====
            area.AxisY.Title = "Ventas Totales";
            area.AxisY.LabelStyle.Format = "C0";
            area.AxisY.IsStartedFromZero = true;

            area.AxisY.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.LineColor = Color.Gray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;

            area.AxisY.MinorGrid.Enabled = true;
            area.AxisY.MinorGrid.LineColor = Color.LightGray;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;

            ChartVentasAnuales.AntiAliasing = AntiAliasingStyles.All;
            ChartVentasAnuales.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
        }

        private void ChartVentasAnuales_MouseMove(object sender, MouseEventArgs e)
        {
            var result = ChartVentasAnuales.HitTest(e.X, e.Y);

            foreach (Series s in ChartVentasAnuales.Series)
            {
                s.BorderWidth = 2;
                s.MarkerSize = 6;
            }

            if (result.Series != null)
            {
                result.Series.BorderWidth = 4;
                result.Series.MarkerSize = 10;
            }
        }

        private void ChartVentasAnuales_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            if (e.HitTestResult.Series != null && e.HitTestResult.PointIndex >= 0)
            {
                var punto = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex];

                e.Text = punto.ToolTip;

                ChartVentasAnuales.Cursor = Cursors.Cross;
            }
        }
    }
}
