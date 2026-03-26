using BLL.Services;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmGraficaVentasMensualesPorVendedorPorAnioBarras : Form
    {

        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly GraficasService _graficasService;

        public FrmGraficaVentasMensualesPorVendedorPorAnioBarras()
        {
            InitializeComponent();
            _graficasService = new GraficasService(cnStr);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaVentasMensualesPorVendedorPorAnioBarras_Load(object sender, EventArgs e)
        {
            ChartVentas.Cursor = Cursors.Cross;
            LlenarCmbVentasDelAño();
        }

        private void LlenarCmbVentasDelAño()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                DataTable dt = _graficasService.ObtenerAñosDeVentas(false);
                CmbVentasDelAño.DisplayMember = "YearOrderDate";
                CmbVentasDelAño.ValueMember = "YearOrderDate";
                CmbVentasDelAño.DataSource = dt;
                CmbVentasDelAño.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
        }

        private void CmbVentasDelAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrafica(Convert.ToInt32(CmbVentasDelAño.SelectedValue));
        }

        private void CargarGrafica(int anio)
        {
            ChartVentas.Series.Clear();
            ChartVentas.Titles.Clear();

            Title titulo = new Title()
            {
                Text = $"Ventas mensuales por vendedores del año {anio}",
                Font = new Font("Arial", 16, FontStyle.Bold)
            };
            ChartVentas.Titles.Add(titulo);
            groupBox1.Text = $"» {titulo.Text} «";

            var area = ChartVentas.ChartAreas[0];
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.CustomLabels.Clear();
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
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

            // Aquí habilitas 3D
            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 30;
            area.Area3DStyle.Rotation = 30;
            area.Area3DStyle.PointGapDepth = 25;
            area.Area3DStyle.WallWidth = 0;
            area.Area3DStyle.LightStyle = LightStyle.Realistic;

            DataTable dt = null;

            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dt = _graficasService.ObtenerVentasMensualesPorVendedoresPorAño(anio);
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


            var grupos = dt.AsEnumerable().GroupBy(row => row.Field<string>("Vendedor"));

            var nombresMes = dt.AsEnumerable()
                .OrderBy(r => {
                    var v = r["Mes"];
                    return (v == DBNull.Value) ? int.MaxValue : Convert.ToInt32(v);
                })
                .Select(r => r.Field<string>("NombreMes") ?? string.Empty)
                .Distinct()
                .ToList(); 
            
            int colorIndex = 0;
            
            foreach (var grupo in grupos)
            {
                var serie = new Series(grupo.Key)
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = false,
                    Font = new Font("Segoe UI", 8, FontStyle.Regular),
                    ToolTip = "#SERIESNAME\nMes: #AXISLABEL\nVentas: #VALY{C2}",
                    LabelFormat = "C2",
                    Color = ChartColors.Paleta[colorIndex % ChartColors.Paleta.Length]
                };

                serie["DrawingStyle"] = "Cylinder";


                foreach (var nombreMes in nombresMes)
                    serie.Points.AddXY(nombreMes, 0D);

                foreach (var row in grupo)
                {
                    object rawMes = row["Mes"];
                    int mes = rawMes == DBNull.Value ? 0 : Convert.ToInt32(rawMes); // validar 1..n
                    object raw = row["TotalVentas"];
                    double ventas = (raw == DBNull.Value) ? 0.0 : Convert.ToDouble(raw);
                    if (mes >= 1 && mes <= serie.Points.Count)
                        serie.Points[mes - 1].YValues[0] = ventas;
                }
                // filtro para mostrar etiqueta solo si Y > 0
                foreach (DataPoint p in serie.Points)
                {
                    if (p.YValues[0] > 0)
                    {
                        p.IsValueShownAsLabel = true;
                        p.LabelForeColor = Color.Black; // Color de la etiqueta
                        p.Font = new Font("Segoe UI", 8, FontStyle.Regular); // Fuente de la etiqueta
                    }
                }
                // Sumar todos los valores Y de la serie
                double totalVendedor = serie.Points.Sum(p => p.YValues[0]);
                serie.LegendText = $"{serie.Name} (Total: {totalVendedor:C2})";
                ChartVentas.Series.Add(serie);
                colorIndex++;
            }
            Title subTitulo = new Title
            {
                Text = $"Total de ventas del año: {dt.Compute("SUM(TotalVentas)", string.Empty):C2}",
                Font = new Font("Arial", 8, FontStyle.Bold),
                Alignment = ContentAlignment.TopLeft,
                IsDockedInsideChartArea = false,
                DockingOffset = -5
            };
            ChartVentas.Titles.Add(subTitulo);
            ChartVentas.ResetAutoValues();
        }
    }
}
