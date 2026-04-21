using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmTableroControlVendedores : Form
    {
        public FrmTableroControlVendedores()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmTableroControlVendedores_Load(object sender, EventArgs e)
        {
            LlenarCmbVentasMensualesPorVendedorPorAño();
            LlenarCmbTipoGrafica1();

            LlenarCmbUltimosAnios();
            LLenarCmbTipoGrafica2();

            LlenarCmbNumeroProductos();
            LlenarCmbAñoTopProd();
            LlenarCmbTipoGrafica3();

            LlenarCmbVentasDelAño4();

            LlenarCmbVentasVendedorAño();
            LlenarCmbTipoGrafica5();

            LlenarCmbVentasDelAño6();
        }

        /******************************************************************************************************/
        #region Grafica1

        private void LlenarCmbVentasMensualesPorVendedorPorAño()
        {
            cmbVentasMensualesPorVendedorPorAño.SelectedIndexChanged -= cmbVentasMensualesPorVendedorPorAño_SelectedIndexChanged;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                cmbVentasMensualesPorVendedorPorAño.DataSource = BLL.EF.Services.GraficasService.ObtenerTop10AñosDeVentas();
                cmbVentasMensualesPorVendedorPorAño.DisplayMember = "Texto";
                cmbVentasMensualesPorVendedorPorAño.ValueMember = "Valor";
                cmbVentasMensualesPorVendedorPorAño.SelectedValue = 1997;
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            cmbVentasMensualesPorVendedorPorAño.SelectedIndexChanged += cmbVentasMensualesPorVendedorPorAño_SelectedIndexChanged;
        }

        private void LlenarCmbTipoGrafica1()
        {
            CmbTipoGrafica1.SelectedIndexChanged -= CmbTipoGrafica1_SelectedIndexChanged;
            CmbTipoGrafica1.DataSource = Enum.GetValues(typeof(SeriesChartType))
                .Cast<SeriesChartType>()
                .Where(t => t != SeriesChartType.Doughnut && t != SeriesChartType.ErrorBar && t != SeriesChartType.Funnel && t != SeriesChartType.Kagi && t != SeriesChartType.Pie && t != SeriesChartType.PointAndFigure && t != SeriesChartType.Polar && t != SeriesChartType.Pyramid && t != SeriesChartType.Renko && t != SeriesChartType.ThreeLineBreak) // Excluye tipos no deseados
                .OrderBy(t => t.ToString())
                .ToList();
            CmbTipoGrafica1.SelectedIndexChanged += CmbTipoGrafica1_SelectedIndexChanged;
            CmbTipoGrafica1.SelectedItem = SeriesChartType.Line;
        }

        private void cmbVentasMensualesPorVendedorPorAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarVentasMensualesPorVendedorPorAño(Convert.ToInt32(cmbVentasMensualesPorVendedorPorAño.SelectedValue), (SeriesChartType)CmbTipoGrafica1.SelectedItem);
        }

        private void CmbTipoGrafica1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarVentasMensualesPorVendedorPorAño(Convert.ToInt32(cmbVentasMensualesPorVendedorPorAño.SelectedValue), (SeriesChartType)CmbTipoGrafica1.SelectedItem);
        }

        private void CargarVentasMensualesPorVendedorPorAño(int year, SeriesChartType tipoGrafica)
        {
            if (cmbVentasMensualesPorVendedorPorAño.SelectedIndex == 0)
            {
                Utils.MsgExclamation("Seleccione un año válido.");
                return;
            }
            ChartVentas.Series.Clear();
            ChartVentas.Titles.Clear();
            string tit = string.Empty;
            if (year == -1)
                tit = "Ventas mensuales por vendedores (todos los años).";
            else
                tit = $"Ventas mensuales por vendedores ({year}).";
            Title titulo = new Title
            {
                Text = tit,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            ChartVentas.Titles.Add(titulo);
            groupBox1.Text = $"» {titulo.Text} Tipo de gráfica: {tipoGrafica} «";
            // Configurar la fuente de la leyenda
            ChartVentas.Legends[0].Font = new Font("Segoe UI", 6, FontStyle.Regular);
            // ChartArea
            var area = ChartVentas.ChartAreas[0];
            area.AxisX.Interval = 1;
            area.AxisX.CustomLabels.Clear();
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.TitleFont = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisX.Title = "Meses";

            area.AxisY.Title = "Ventas totales";
            area.AxisY.LabelStyle.Format = "C0";
            area.AxisY.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.LineColor = Color.Gray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            area.AxisY.MinorGrid.Enabled = false;
            area.AxisY.MinorGrid.LineColor = Color.LightGray;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisY.TitleFont = new Font("Segoe UI", 7, FontStyle.Bold);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisY.LabelStyle.Angle = -45;

            area.AxisY.LabelStyle.Enabled = false;
            area.AxisY.Title = string.Empty;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisY.MinorGrid.Enabled = false;
            // Leer datos
            var dt = new DataTable();
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dt = BLL.EF.Services.GraficasService.ObtenerVentasMensualesPorVendedoresPorAño(year);
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

            // Agrupar y calcular totales y ordena de mayor a menor
            var grupos = dt.AsEnumerable()
                           .GroupBy(r => r.Field<string>("Vendedor"))
                           .Select(g => new
                           {
                               Vendedor = g.Key,
                               Datos = g,
                               Total = g.Sum(r => r.Field<object>("TotalVentas") != DBNull.Value
                                                  ? Convert.ToDouble(r["TotalVentas"])
                                                  : 0D)
                           })
                           .OrderByDescending(x => x.Total); // ordenar de mayor a menor
            int i = 0;
            foreach (var grupo in grupos)
            {
                // Serie por vendedor
                var serie = new Series(grupo.Vendedor)
                {
                    ChartType = tipoGrafica,
                    BorderWidth = 2,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 4,
                    ToolTip = "#SERIESNAME\nMes: #AXISLABEL",
                    LabelForeColor = Color.Black,
                    Font = new Font("Segoe UI", 6, FontStyle.Regular),
                    IsValueShownAsLabel = false,
                    LabelFormat = "C2",
                    Color = ChartColors.Paleta[i % ChartColors.Paleta.Length]
                };
                foreach (var row in grupo.Datos)
                {
                    string nombreMes = row.Field<string>("NombreMes");

                    object raw = row["TotalVentas"];
                    double ventas = raw != DBNull.Value
                                    ? Convert.ToDouble(raw)
                                    : 0D;

                    serie.Points.AddXY(nombreMes, ventas);
                }
                serie.LegendText = $"{serie.Name}";
                ChartVentas.Series.Add(serie);
                i++;
            }
            // ————— Aquí forzamos el recálculo de la escala del eje Y —————
            ChartVentas.ResetAutoValues();
        }
        #endregion
        /******************************************************************************************************/
        #region Grafica2
        private void LlenarCmbUltimosAnios()
        {
            cmbUltimosAnios.SelectedIndexChanged -= cmbUltimosAnios_SelectedIndexChanged;
            int totalAñosDisponibles = BLL.EF.Services.GraficasService.ObtenerTotalAñosConVentas();

            int limite = Math.Min(totalAñosDisponibles, 10);

            var items = new List<KeyValuePair<string, int>>();

            for (int i = 2; i <= limite; i++)
            {
                items.Add(new KeyValuePair<string, int>($"{i} Años", i));
            }

            cmbUltimosAnios.DataSource = items;
            cmbUltimosAnios.DisplayMember = "Key";
            cmbUltimosAnios.ValueMember = "Value";
            cmbUltimosAnios.SelectedIndex = cmbUltimosAnios.Items.Count - 1;
            cmbUltimosAnios.SelectedIndexChanged += cmbUltimosAnios_SelectedIndexChanged;
        }

        private void LLenarCmbTipoGrafica2()
        {
            CmbTipoGrafica2.SelectedIndexChanged -= CmbTipoGrafica2_SelectedIndexChanged;

            var tipos = Enum.GetValues(typeof(SeriesChartType))
                .Cast<SeriesChartType>()
                .Where(t => t != SeriesChartType.Kagi
                         && t != SeriesChartType.ErrorBar
                         && t != SeriesChartType.PointAndFigure
                         && t != SeriesChartType.Renko
                         && t != SeriesChartType.StackedArea
                         && t != SeriesChartType.StackedArea100
                         && t != SeriesChartType.StackedBar100
                         && t != SeriesChartType.StackedColumn100
                         && t != SeriesChartType.ThreeLineBreak)
                .OrderBy(t => t.ToString())
                .ToList();

            CmbTipoGrafica2.DataSource = tipos;
            CmbTipoGrafica2.SelectedIndexChanged += CmbTipoGrafica2_SelectedIndexChanged;
            CmbTipoGrafica2.SelectedItem = SeriesChartType.Spline;
        }

        private void cmbUltimosAnios_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarComparativoVentasMensuales(Convert.ToInt32(cmbUltimosAnios.SelectedValue), (SeriesChartType)CmbTipoGrafica2.SelectedItem);
        }

        private void CmbTipoGrafica2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarComparativoVentasMensuales(Convert.ToInt32(cmbUltimosAnios.SelectedValue), (SeriesChartType)CmbTipoGrafica2.SelectedItem);
        }

        private void CargarComparativoVentasMensuales(int years, SeriesChartType tipoGrafica)
        {
            chart2.Series.Clear();
            chart2.Titles.Clear();
            chart2.Legends.Clear();

            var legend = new Legend("Default")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Center,
                Font = new Font("Segoe UI", 7, FontStyle.Regular)
            };
            chart2.Legends.Add(legend);

            List<DTOs.EF.DtoVentasMensualesPorAños> datos = null;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                datos = BLL.EF.Services.GraficasService.ObtenerVentasMensualesPorAños(years);
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
                return;
            }

            var datosAgrupados = datos.GroupBy(x => x.Year).OrderByDescending(x => x.Key);
            int i = 0;

            foreach (var grupo in datosAgrupados)
            {
                string nombreSerie = $"Ventas {grupo.Key}";
                var serie = chart2.Series.Add(nombreSerie);

                serie.ChartType = tipoGrafica;
                serie.BorderWidth = 2;
                serie.MarkerStyle = MarkerStyle.Circle;
                serie.MarkerSize = 8;
                serie["LineTension"] = "0.5";
                serie.ShadowOffset = 1;
                serie.ShadowColor = Color.FromArgb(100, Color.Gray);
                serie.Color = ChartColors.Paleta[i % ChartColors.Paleta.Length];

                decimal totalAnual = grupo.Sum(x => x.Total);
                serie.Legend = legend.Name;
                serie.LegendText = $"{nombreSerie}";
                serie.ToolTip = $"{nombreSerie} #VALX: #VALY{{C2}}";

                foreach (var dato in grupo.OrderBy(x => x.Mes))
                {
                    int index = serie.Points.AddXY(dato.NombreMes, dato.Total);
                    DataPoint dp = serie.Points[index];
                    dp.ToolTip = $"{dato.NombreMes} {dato.Year}";
                    dp.Font = new Font("Segoe UI", 7, FontStyle.Regular);
                    dp.MarkerStyle = MarkerStyle.Circle;
                    dp.MarkerSize = 5;
                }
                i++;
            }

            // Configuración del área
            var area = chart2.ChartAreas[0];
            area.BackColor = Color.White;
            area.BackSecondaryColor = Color.FromArgb(245, 245, 245);
            area.BackGradientStyle = GradientStyle.TopBottom;

            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.Title = "Meses";
            area.AxisX.TitleFont = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            // Eje Y oculto (sin etiquetas ni título)
            area.AxisY.LabelStyle.Enabled = false;
            area.AxisY.Title = string.Empty;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisY.MinorGrid.Enabled = false;

            chart2.AntiAliasing = AntiAliasingStyles.All;
            chart2.TextAntiAliasingQuality = TextAntiAliasingQuality.High;

            // Título principal
            Title titulo = new Title
            {
                Text = $"Comparativo de ventas mensuales de los últimos {years} años.",
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Alignment = ContentAlignment.TopCenter
            };
            titulo.Position.Auto = false;
            titulo.Position.X = 0;
            titulo.Position.Y = 0;
            titulo.Position.Width = 100;
            titulo.Position.Height = 5;

            chart2.Titles.Add(titulo);

            groupBox2.Text = $"» Comparativo de ventas mensuales de los últimos {years} años. Tipo de gráfica: {tipoGrafica}. «";
        }
        #endregion
        /******************************************************************************************************/
        #region Grafica3
        private void LlenarCmbNumeroProductos()
        {
            cmbNumeroProductos.SelectedIndexChanged -= cmbNumeroProductos_SelectedIndexChanged;
            var items = new List<KeyValuePair<string, int>>();
            for (int i = 10; i <= 30; i += 5)
            {
                items.Add(new KeyValuePair<string, int>($"{i} productos", i));
            }
            cmbNumeroProductos.DataSource = items;
            cmbNumeroProductos.DisplayMember = "Key";
            cmbNumeroProductos.ValueMember = "Value";
            cmbNumeroProductos.SelectedIndex = 2;
            cmbNumeroProductos.SelectedIndexChanged += cmbNumeroProductos_SelectedIndexChanged;
        }

        private void LlenarCmbAñoTopProd()
        {
            CmbAñoTopProd.SelectedIndexChanged -= CmbAñoTopProd_SelectedIndexChanged;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                CmbAñoTopProd.DataSource = BLL.EF.Services.GraficasService.ObtenerTop10AñosDeVentas();
                CmbAñoTopProd.DisplayMember = "Texto";
                CmbAñoTopProd.ValueMember = "Valor";
                CmbAñoTopProd.SelectedValue = 1997;
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            CmbAñoTopProd.SelectedIndexChanged += CmbAñoTopProd_SelectedIndexChanged;
        }

        private void LlenarCmbTipoGrafica3()
        {
            CmbTipoGrafica3.SelectedIndexChanged -= CmbTipoGrafica3_SelectedIndexChanged;
            CmbTipoGrafica3.DataSource = Enum.GetValues(typeof(SeriesChartType))
                .Cast<SeriesChartType>()
                .Where(t => t != SeriesChartType.Kagi && t != SeriesChartType.ErrorBar && t != SeriesChartType.PointAndFigure && t != SeriesChartType.Polar && t != SeriesChartType.Renko && t != SeriesChartType.ThreeLineBreak) // Excluye tipos no deseados
                .OrderBy(t => t.ToString())
                .ToList();
            CmbTipoGrafica3.SelectedIndexChanged += CmbTipoGrafica3_SelectedIndexChanged;
            CmbTipoGrafica3.SelectedItem = SeriesChartType.Column;
        }

        private void cmbNumeroProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTopProductos(Convert.ToInt32(cmbNumeroProductos.SelectedValue), (SeriesChartType)CmbTipoGrafica3.SelectedItem, Convert.ToInt32(CmbAñoTopProd.SelectedValue));
        }

        private void CmbAñoTopProd_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTopProductos(Convert.ToInt32(cmbNumeroProductos.SelectedValue), (SeriesChartType)CmbTipoGrafica3.SelectedItem, Convert.ToInt32(CmbAñoTopProd.SelectedValue));
        }

        private void CmbTipoGrafica3_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTopProductos(Convert.ToInt32(cmbNumeroProductos.SelectedValue), (SeriesChartType)CmbTipoGrafica3.SelectedItem, Convert.ToInt32(CmbAñoTopProd.SelectedValue));
        }

        private void CargarTopProductos(int cantidad, SeriesChartType tipoGrafica, int año)
        {
            if (CmbAñoTopProd.SelectedIndex == 0)
            {
                Utils.MsgExclamation("Seleccione un año válido.");
                return;
            }
            ChartTopProductos.Series.Clear();
            ChartTopProductos.Titles.Clear();
            Title titulo = new Title();
            titulo.Text = año == -1
                        ? $"Top {cantidad} productos más vendidos (todos los años)"
                        : $"Top {cantidad} productos más vendidos ({año})";
            groupBox3.Text = $"» {titulo.Text} Tipo de gráfica: {tipoGrafica} «";
            titulo.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            titulo.Alignment = ContentAlignment.TopCenter;
            ChartTopProductos.Titles.Add(titulo);
            DataTable datos;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                // Datos
                datos = BLL.EF.Services.GraficasService.ObtenerTopProductos(cantidad, año);
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

            // 1 serie única
            var series = ChartTopProductos.Series.Add("Productos más vendidos");
            series.ChartType = tipoGrafica;
            series["DrawingStyle"] = "Cylinder";

            series.ShadowOffset = 3;
            series.ShadowColor = Color.FromArgb(120, Color.Black);

            series.IsValueShownAsLabel = false;
            series.Label = string.Empty;
            series.BorderWidth = 2;
            series.ToolTip = "Producto: #VALX";
            series.Font = new Font("Segoe UI", 7, FontStyle.Bold);
            series.Points.Clear();

            int idx = 0;
            foreach (DataRow row in datos.Rows)
            {
                string nombre = (idx + 1).ToString() + ".- " + row["NombreProducto"].ToString();
                int qty = Convert.ToInt32(row["CantidadVendida"]);

                int pointIndex = series.Points.AddXY(nombre, qty);
                series.Points[pointIndex].Color = ChartColors.Paleta[idx % ChartColors.Paleta.Length];
                idx++;
            }

            ChartTopProductos.Legends.Clear();

            // Configurar ChartArea en 3D y ejes
            var area = ChartTopProductos.ChartAreas[0];

            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 30;
            area.Area3DStyle.Rotation = 20;
            area.Area3DStyle.LightStyle = LightStyle.Realistic;

            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 6, FontStyle.Regular);

            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.LabelStyle.Angle = -45;
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisY.Title = "Cantidad vendida (unidades)";
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisY.LabelStyle.Enabled = false;   // oculta los números
            area.AxisY.Title = string.Empty;         // elimina el título
            area.AxisY.MajorGrid.Enabled = false;    // quita la cuadrícula
            area.AxisY.MinorGrid.Enabled = false;    // quita la cuadrícula menor
        }
        #endregion
        /******************************************************************************************************/
        #region Grafica4
        private void LlenarCmbVentasDelAño4()
        {
            CmbVentasDelAño4.SelectedIndexChanged -= CmbVentasDelAño4_SelectedIndexChanged;
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                DataTable dt = BLL.EF.Services.GraficasService.ObtenerTop10AñosDeVentas();
                CmbVentasDelAño4.DisplayMember = "Texto";
                CmbVentasDelAño4.ValueMember = "Valor";
                CmbVentasDelAño4.DataSource = dt;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            CmbVentasDelAño4.SelectedIndexChanged += CmbVentasDelAño4_SelectedIndexChanged;
            CmbVentasDelAño4.SelectedValue = 1997;
        }

        private void CmbVentasDelAño4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbVentasDelAño4.SelectedIndex == 0)
            {
                Utils.MsgExclamation("Seleccione un año válido.");
                return;
            }
            CargarGrafica4(Convert.ToInt32(CmbVentasDelAño4.SelectedValue));
        }

        private void CargarGrafica4(int anio)
        {
            ChartVentas4.Series.Clear();
            ChartVentas4.Titles.Clear();
            string tit = anio == -1
                        ? $"Ventas mensuales por vendedores (todos los años)"
                        : $"Ventas mensuales por vendedores ({anio})";
            Title titulo = new Title()
            {
                Text = tit,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            ChartVentas4.Titles.Add(titulo);
            groupBox4.Text = $"» {titulo.Text} (barras) «";

            var area = ChartVentas4.ChartAreas[0];
            area.AxisX.Interval = 1;
            area.AxisX.CustomLabels.Clear();
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.TitleFont = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisX.Title = "Meses";

            // Ocultar eje Y y cantidades
            area.AxisY.LabelStyle.Enabled = false;
            area.AxisY.Title = string.Empty;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisY.MinorGrid.Enabled = false;

            ChartVentas4.Legends[0].Font = new Font("Segoe UI", 6, FontStyle.Regular);

            // Habilitar 3D
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
                dt = BLL.EF.Services.GraficasService.ObtenerVentasMensualesPorVendedoresPorAño(anio);
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

            var nombresMes = dt.AsEnumerable()
                .OrderBy(r =>
                {
                    var v = r["Mes"];
                    return (v == DBNull.Value) ? int.MaxValue : Convert.ToInt32(v);
                })
                .Select(r => r.Field<string>("NombreMes") ?? string.Empty)
                .Distinct()
                .ToList();

            var grupos = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Vendedor"))
                .Select(g => new
                {
                    Vendedor = g.Key,
                    Datos = g,
                    Total = g.Sum(r => r.Field<object>("TotalVentas") != DBNull.Value
                                       ? Convert.ToDouble(r["TotalVentas"])
                                       : 0D)
                })
                .OrderByDescending(x => x.Total);

            int colorIndex = 0;
            foreach (var grupo in grupos)
            {
                var serie = new Series(grupo.Vendedor)
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = false,
                    Font = new Font("Segoe UI", 6, FontStyle.Regular),
                    ToolTip = "#SERIESNAME\nMes: #AXISLABEL", // sin cantidades
                    LabelFormat = "C2",
                    Color = ChartColors.Paleta[colorIndex % ChartColors.Paleta.Length]
                };
                serie["DrawingStyle"] = "Cylinder";

                foreach (var nombreMes in nombresMes)
                    serie.Points.AddXY(nombreMes, 0D);

                foreach (var row in grupo.Datos)
                {
                    object rawMes = row["Mes"];
                    int mes = rawMes == DBNull.Value ? 0 : Convert.ToInt32(rawMes);
                    object raw = row["TotalVentas"];
                    double ventas = (raw == DBNull.Value) ? 0.0 : Convert.ToDouble(raw);
                    if (mes >= 1 && mes <= serie.Points.Count)
                        serie.Points[mes - 1].YValues[0] = ventas;
                }

                // Leyenda solo con nombre del vendedor
                serie.LegendText = $"{serie.Name}";

                ChartVentas4.Series.Add(serie);
                colorIndex++;
            }

            ChartVentas4.ResetAutoValues();
        }
        #endregion
        /******************************************************************************************************/
        #region Grafica5
        private void LlenarCmbVentasVendedorAño()
        {
            cmbVentasVendedorAño.SelectedIndexChanged -= cmbVentasVendedorAño_SelectedIndexChanged;
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                cmbVentasVendedorAño.DataSource = BLL.EF.Services.GraficasService.ObtenerTop10AñosDeVentas();
                cmbVentasVendedorAño.DisplayMember = "Texto";
                cmbVentasVendedorAño.ValueMember = "Valor";
                cmbVentasVendedorAño.SelectedValue = 1997;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            MDIPrincipal.ActualizarBarraDeEstado();
            cmbVentasVendedorAño.SelectedIndexChanged += cmbVentasVendedorAño_SelectedIndexChanged;
        }

        private void LlenarCmbTipoGrafica5()
        {
            CmbTipoGrafica5.SelectedIndexChanged -= CmbTipoGrafica5_SelectedIndexChanged;
            CmbTipoGrafica5.DataSource = Enum.GetValues(typeof(SeriesChartType))
                .Cast<SeriesChartType>()
                .Where(t => t != SeriesChartType.BoxPlot && t != SeriesChartType.ErrorBar && t != SeriesChartType.Kagi && t != SeriesChartType.PointAndFigure && t != SeriesChartType.Polar && t != SeriesChartType.Renko && t != SeriesChartType.StackedArea && t != SeriesChartType.StackedArea100 && t != SeriesChartType.StackedColumn100 && t != SeriesChartType.ThreeLineBreak) // Excluye tipos no deseados
                .OrderBy(t => t.ToString())
                .ToList();
            CmbTipoGrafica5.SelectedIndexChanged += CmbTipoGrafica5_SelectedIndexChanged;
            CmbTipoGrafica5.SelectedItem = SeriesChartType.Doughnut;
        }

        private void cmbVentasVendedorAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVentasVendedorAño.SelectedIndex < 0)
                return;
            CargarVentasPorVendedoresAño(Convert.ToInt32(cmbVentasVendedorAño.SelectedValue), (SeriesChartType)CmbTipoGrafica5.SelectedItem);
        }

        private void CmbTipoGrafica5_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarVentasPorVendedoresAño(Convert.ToInt32(cmbVentasVendedorAño.SelectedValue), (SeriesChartType)CmbTipoGrafica5.SelectedItem);
        }

        private void CargarVentasPorVendedoresAño(int anio, SeriesChartType tipoGrafica)
        {
            if (cmbVentasVendedorAño.SelectedIndex == 0)
            {
                Utils.MsgExclamation("Seleccione un año válido.");
                return;
            }
            chart5.Series.Clear();
            chart5.Titles.Clear();
            chart5.Legends.Clear();
            var leyenda = new Legend("Vendedores")
            {
                Title = "Vendedores",
                TitleFont = new Font("Segoe UI", 7, FontStyle.Bold),
                Docking = Docking.Right,
                LegendStyle = LegendStyle.Table,
                Font = new Font("Segoe UI", 6, FontStyle.Regular),
                IsTextAutoFit = false
            };

            chart5.Legends.Add(leyenda);
            string tit = string.Empty;
            if (anio == -1)
                tit = "Ventas por vendedores (todos los años).";
            else
                tit = $"Ventas por vendedores ({anio}).";
            // Título del gráfico
            Title titulo = new Title
            {
                Text = tit,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
            };
            chart5.Titles.Add(titulo);
            groupBox5.Text = $"» {titulo.Text}. Tipo de grafica: {tipoGrafica} «";
            Series serie = new Series
            {
                Name = "Ventas",
                IsValueShownAsLabel = false,
                ChartType = tipoGrafica,
                Label = "#AXISLABEL: #VALY{C2}",
                ToolTip = "Vendedor: #AXISLABEL",
                Legend = leyenda.Name,
                LegendText = "#AXISLABEL: #VALY{C2}"
            };
            if (tipoGrafica == SeriesChartType.Area || tipoGrafica == SeriesChartType.Bar || tipoGrafica == SeriesChartType.Bubble || tipoGrafica == SeriesChartType.Candlestick || tipoGrafica == SeriesChartType.Column || tipoGrafica == SeriesChartType.FastLine || tipoGrafica == SeriesChartType.FastPoint || tipoGrafica == SeriesChartType.Funnel || tipoGrafica == SeriesChartType.Line || tipoGrafica == SeriesChartType.Point || tipoGrafica == SeriesChartType.Pyramid || tipoGrafica == SeriesChartType.Radar || tipoGrafica == SeriesChartType.Range || tipoGrafica == SeriesChartType.RangeBar || tipoGrafica == SeriesChartType.RangeColumn || tipoGrafica == SeriesChartType.Spline || tipoGrafica == SeriesChartType.SplineArea || tipoGrafica == SeriesChartType.SplineRange || tipoGrafica == SeriesChartType.StackedBar || tipoGrafica == SeriesChartType.StackedBar100 || tipoGrafica == SeriesChartType.StackedColumn || tipoGrafica == SeriesChartType.StepLine || tipoGrafica == SeriesChartType.Stock)
            {
                chart5.Legends.Clear();
            }
            // 1. Configurar ChartArea 3D
            var area = chart5.ChartAreas[0];
            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 30;
            area.Area3DStyle.Rotation = 20;
            area.Area3DStyle.LightStyle = LightStyle.Realistic;
            // Opciones 3D y estilo dona
            serie["PieLabelStyle"] = "Disabled";
            serie["PieDrawingStyle"] = "Cylinder";
            serie["DoughnutRadius"] = "60";

            // 3.Agregar la serie al chart
            chart5.Series.Clear();
            chart5.Series.Add(serie);
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var ventas = BLL.EF.Services.GraficasService.ObtenerVentasPorVendedores(anio);
                int i = 0;
                foreach (var (vendedor, totalVentas) in ventas)
                {
                    int puntoIndex = serie.Points.AddXY(vendedor, totalVentas);
                    serie.Points[puntoIndex].LegendText = string.Format(
                    CultureInfo.GetCultureInfo("es-MX"),
                    "{0}",
                    vendedor,
                    totalVentas
                    );
                    serie.Points[puntoIndex].Color =
                        ChartColors.Paleta[i % ChartColors.Paleta.Length];
                    i++;
                }
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }
        #endregion
        /******************************************************************************************************/
        #region Grafica6
        private void LlenarCmbVentasDelAño6()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                CmbVentasDelAño6.SelectedIndexChanged -= CmbVentasDelAño6_SelectedIndexChanged;
                DataTable dt = BLL.EF.Services.GraficasService.ObtenerTop10AñosDeVentas();
                CmbVentasDelAño6.DisplayMember = "Texto";
                CmbVentasDelAño6.ValueMember = "Valor";
                CmbVentasDelAño6.DataSource = dt;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            CmbVentasDelAño6.SelectedIndexChanged += CmbVentasDelAño6_SelectedIndexChanged;
            CmbVentasDelAño6.SelectedValue = 1997;
        }

        private void CmbVentasDelAño6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbVentasDelAño6.SelectedIndex == 0)
            {
                Utils.MsgExclamation("Seleccione un año válido.");
                return;
            }
            CargarGrafica(Convert.ToInt32(CmbVentasDelAño6.SelectedValue));
        }

        private void CargarGrafica(int anio)
        {
            ChartVentas6.Series.Clear();
            ChartVentas6.Titles.Clear();
            string tit = string.Empty;
            if (anio == -1)
                tit = "Ventas mensuales por vendedores (todos los años)";
            else
                tit = $"Ventas mensuales por vendedores ({anio})";
            Title titulo = new Title()
            {
                Text = tit,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            ChartVentas6.Titles.Add(titulo);
            groupBox6.Text = $"» {titulo.Text} (barras 2) «";

            ChartVentas6.Palette = ChartColorPalette.None;

            var area = ChartVentas6.ChartAreas[0];
            area.AxisX.Interval = 1;
            area.AxisX.CustomLabels.Clear();
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.TitleFont = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisX.Title = "Meses";

            // Ocultar eje Y y cantidades
            area.AxisY.LabelStyle.Enabled = false;
            area.AxisY.Title = string.Empty;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisY.MinorGrid.Enabled = false;

            ChartVentas6.Legends[0].Font = new Font("Segoe UI", 6, FontStyle.Regular);

            DataTable dt = null;

            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dt = BLL.EF.Services.GraficasService.ObtenerVentasMensualesPorVendedoresPorAño(anio);
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

            var nombresMes = dt.AsEnumerable()
                .OrderBy(r => {
                    var v = r["Mes"];
                    return (v == DBNull.Value) ? int.MaxValue : Convert.ToInt32(v);
                })
                .Select(r => r.Field<string>("NombreMes") ?? string.Empty)
                .Distinct()
                .ToList();

            // Agrupar y calcular totales
            var grupos = dt.AsEnumerable()
                .GroupBy(row => row.Field<string>("Vendedor"))
                .Select(g => new
                {
                    Vendedor = g.Key,
                    Datos = g,
                    Total = g.Sum(r => r.Field<object>("TotalVentas") != DBNull.Value
                                       ? Convert.ToDouble(r["TotalVentas"])
                                       : 0D)
                })
                .OrderByDescending(x => x.Total); // ordenar de mayor a menor

            int colorIndex = 0;
            foreach (var grupo in grupos)
            {
                var serie = new Series(grupo.Vendedor)
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = false,
                    Font = new Font("Segoe UI", 8, FontStyle.Regular),
                    ToolTip = "#SERIESNAME\nMes: #AXISLABEL",
                    LabelFormat = "C2",
                    Color = ChartColors.Paleta[colorIndex % ChartColors.Paleta.Length]
                };

                foreach (var nombreMes in nombresMes)
                    serie.Points.AddXY(nombreMes, 0D);

                foreach (var row in grupo.Datos)
                {
                    object rawMes = row["Mes"];
                    int mes = rawMes == DBNull.Value ? 0 : Convert.ToInt32(rawMes); // validar 1..n
                    object raw = row["TotalVentas"];
                    double ventas = (raw == DBNull.Value) ? 0.0 : Convert.ToDouble(raw);
                    if (mes >= 1 && mes <= serie.Points.Count)
                        serie.Points[mes - 1].YValues[0] = ventas;
                }
                serie.LegendText = $"{serie.Name})";
                ChartVentas6.Series.Add(serie);
                colorIndex++;
            }
            ChartVentas6.ResetAutoValues();
        }
        #endregion
    }
}
