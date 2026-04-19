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
    public partial class FrmGraficaEjemploTodas2 : Form
    {
        // Datos fijos: categorías y valores
        private readonly string[] categorias =
            { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago" };
        private readonly double[] valores =
            { 15,    30,    45,    20,    35,    50,    25,    40   };

        private DataTable dt = new DataTable();

        public FrmGraficaEjemploTodas2()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dt = GraficasService.ObtenerVentasMensualesPorVendedoresPorAño(1997);
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
            CargarTiposDeGrafica();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaEjemploTodas_Load(object sender, EventArgs e)
        {
            // Crea datos de ejemplo y dibuja la primera gráfica
            DibujarGrafica((SeriesChartType)cmbChartTypes.SelectedItem);
        }

        private void cmbChartTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            DibujarGrafica((SeriesChartType)cmbChartTypes.SelectedItem);
        }

        private void CargarTiposDeGrafica()
        {
            // Obtiene todos los valores del enum
            var tipos = Enum.GetValues(typeof(SeriesChartType))
                            .Cast<SeriesChartType>()
                            .OrderBy(t => t.ToString());

            // Llena el ComboBox
            cmbChartTypes.DataSource = tipos.ToList();
        }

        private void DibujarGrafica(SeriesChartType tipo)
        {
            if (tipo == SeriesChartType.Kagi || tipo == SeriesChartType.PointAndFigure || tipo == SeriesChartType.Renko || tipo == SeriesChartType.ThreeLineBreak)
            {
                MessageBox.Show("Este tipo de gráfica no se puede graficar");
                return;
            }
            int anio = 1997;
            chart1.Series.Clear();
            chart1.Titles.Clear();
            string tit = string.Empty;
            if (anio > 0)
                tit = $"Ventas mensuales por vendedores del año {anio}             Tipo de gráfica: {tipo}";
            else
                tit = "Ventas mensuales por vendedores de todos los años";
            Title titulo = new Title
            {
                Text = tit,
                Font = new Font("Arial", 16, FontStyle.Bold)
            };
            chart1.Titles.Add(titulo);
            groupBox1.Text = $"» {titulo.Text} «";
            // ChartArea
            var area = chart1.ChartAreas[0];
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

            // Pivot dinámico por vendedor
            var grupos = dt.AsEnumerable()
                          .GroupBy(r => r.Field<string>("Vendedor"));

            int i = 0;

            foreach (var grupo in grupos)
            {
                // Serie por vendedor
                var serie = new Series(grupo.Key)
                {
                    ChartType = tipo,
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

                chart1.Series.Add(serie);
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
            chart1.Titles.Add(subTitulo);
            // ————— Aquí forzamos el recálculo de la escala del eje Y —————
            chart1.ResetAutoValues();
        }
    }
}
