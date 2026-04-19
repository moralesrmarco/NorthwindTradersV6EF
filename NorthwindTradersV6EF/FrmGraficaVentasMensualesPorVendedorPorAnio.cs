using BLL.EF.Services;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmGraficaVentasMensualesPorVendedorPorAnio : Form
    {
        public FrmGraficaVentasMensualesPorVendedorPorAnio()
        {
            InitializeComponent();
            ChartVentas.MouseMove += ChartVentas_MouseMove;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaVentasMensualesPorVendedorPorAnio_Load(object sender, EventArgs e)
        {
            ChartVentas.Cursor = Cursors.Cross;
            LlenarComboBox();
            CargarVentasMensualesPorVendedorPorAnio(DateTime.Now.Year);
            ChartVentas.AntiAliasing = AntiAliasingStyles.All;
            ChartVentas.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
        }

        private void LlenarComboBox()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                ComboBoxAño.SelectedIndexChanged -= ComboBoxAño_SelectedIndexChanged;
                ComboBoxAño.DataSource = BLL.EF.Services.GraficasService.ObtenerTop10AñosDeVentas();
                ComboBoxAño.DisplayMember = "Texto";
                ComboBoxAño.ValueMember = "Valor";
                ComboBoxAño.SelectedValue = DateTime.Today.Year;
                ComboBoxAño.SelectedIndexChanged += ComboBoxAño_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            MDIPrincipal.ActualizarBarraDeEstado();
        }

        private void CargarVentasMensualesPorVendedorPorAnio(int anio)
        {
            ChartVentas.Series.Clear();
            ChartVentas.Titles.Clear();
            string tit = string.Empty;
            if (anio > 0)
                tit = $"Ventas mensuales por vendedores del año {anio}";
            else
                tit = "Ventas mensuales por vendedores de todos los años";
                Title titulo = new Title
                {
                    Text = tit,
                    Font = new Font("Arial", 16, FontStyle.Bold)
                };
            ChartVentas.Titles.Add(titulo);
            groupBox1.Text = $"» {titulo.Text} «";
            // ChartArea
            var area = ChartVentas.ChartAreas[0];
            area.AxisX.Interval = 1;
            area.AxisX.CustomLabels.Clear();
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            area.AxisX.Title = "Meses";

            area.AxisY.Title = "Ventas totales";
            area.AxisY.LabelStyle.Format = "C0";
            area.AxisY.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.LineColor = Color.Gray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            area.AxisY.MinorGrid.Enabled = true;
            area.AxisY.MinorGrid.LineColor = Color.LightGray;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            area.AxisY.LabelStyle.Angle = -45;
            // Leer datos
            var dt = new DataTable();
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dt = GraficasService.ObtenerVentasMensualesPorVendedoresPorAño(anio);
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
                return;
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }

            // Pivot dinámico por vendedor
            var grupos = dt.AsEnumerable()
                          .GroupBy(r => r.Field<string>("Vendedor"));

            int i = 0;

            foreach (var grupo in grupos)
            {
                // Serie por vendedor
                var serie = new Series(grupo.Key)
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 6,
                    ToolTip = "#SERIESNAME\nMes: #AXISLABEL\nVentas: #VALY{C2}",
                    LabelForeColor = Color.Black,
                    Font = new Font("Segoe UI", 8f, FontStyle.Regular),
                    IsValueShownAsLabel = false,
                    LabelFormat = "C2"
                };

                serie.Color = ChartColors.Paleta[i % ChartColors.Paleta.Length];

                foreach (var row in grupo)
                {
                    string nombreMes = row.Field<string>("NombreMes");

                    object raw = row["TotalVentas"];
                    double ventas = raw != DBNull.Value
                                    ? Convert.ToDouble(raw)
                                    : 0D;

                    serie.Points.AddXY(nombreMes, ventas);
                }

                // filtro para mostrar etiqueta solo si Y > 0
                foreach (DataPoint p in serie.Points)
                {
                    if (p.YValues[0] > 0)
                    {
                        p.IsValueShownAsLabel = true;                        
                    }
                }
                // Sumar todos los valores Y de la serie
                double totalVendedor = serie.Points.Sum(p => p.YValues[0]);

                serie.LegendText = $"{serie.Name} (Total: {totalVendedor:C2})";

                ChartVentas.Series.Add(serie);
                i++;
            }
            Title subTitulo = new Title();
            string subTit = string.Empty;
            if (anio > 0)
                subTit = $"Total de ventas ({anio}):";
            else
                subTit = "Total de ventas (todos los años):";
            subTitulo.Text = subTit + $" {dt.Compute("SUM(TotalVentas)", string.Empty):C2}";
            subTitulo.Font = new Font("Arial", 8, FontStyle.Bold);
            subTitulo.Alignment = ContentAlignment.TopLeft;
            subTitulo.IsDockedInsideChartArea = false;
            subTitulo.DockingOffset = -5;
            ChartVentas.Titles.Add(subTitulo);
            // ————— Aquí forzamos el recálculo de la escala del eje Y —————
            ChartVentas.ResetAutoValues();
        }

        private void ChartVentas_MouseMove(object sender, MouseEventArgs e)
        {
            var result = ChartVentas.HitTest(e.X, e.Y);

            // Restaurar todas las líneas
            foreach (Series s in ChartVentas.Series)
            {
                s.BorderWidth = 2;
                s.MarkerSize = 6;
            }

            // Resaltar la línea donde está el mouse
            if (result.Series != null)
            {
                result.Series.BorderWidth = 4;
                result.Series.MarkerSize = 10;
            }
        }

        private void ComboBoxAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxAño.SelectedIndex == 0)
            {
                U.MsgExclamation("Seleccione un año válido.");
                return;
            }
            CargarVentasMensualesPorVendedorPorAnio(Convert.ToInt32(ComboBoxAño.SelectedValue));
        }
    }
}
