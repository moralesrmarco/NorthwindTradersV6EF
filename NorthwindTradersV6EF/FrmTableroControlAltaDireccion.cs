using BLL.EF.Services;
using DTOs.EF;
using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmTableroControlAltaDireccion : Form
    {
        public FrmTableroControlAltaDireccion()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmTableroControlAltaDireccion_Load(object sender, EventArgs e)
        {
            LlenarCmbVentasMensualesAño();
            LlenarCmbTipoGrafica1();

            LlenarCmbUltimosAnios();
            LLenarCmbTipoGrafica2();

            LlenarCmbNumeroProductos();
            LlenarCmbAñoTopProd();
            LlenarCmbTipoGrafica3();

            //CargarVentasPorVendedores();

            LlenarCmbVentasVendedorAño();
            LlenarCmbTipoGrafica5();

            LlenarCmbVentasMensualesPorVendedorPorAño();
            LlenarCmbTipoGrafica6();
        }

        /******************************************************************************************************/
        #region Grafica1
        private void LlenarCmbVentasMensualesAño()
        {
            cmbVentasMensualesAño.SelectedIndexChanged -= cmbVentasMensualesAño_SelectedIndexChanged;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                cmbVentasMensualesAño.DataSource = GraficasService.ObtenerTop10AñosDeVentas();
                cmbVentasMensualesAño.DisplayMember = "Texto";
                cmbVentasMensualesAño.ValueMember = "Valor";
                cmbVentasMensualesAño.SelectedValue = DateTime.Today.Year;
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            cmbVentasMensualesAño.SelectedIndexChanged += cmbVentasMensualesAño_SelectedIndexChanged;
        }

        private void LlenarCmbTipoGrafica1()
        {
            CmbTipoGrafica1.SelectedIndexChanged -= CmbTipoGrafica1_SelectedIndexChanged;
            var tipos = Enum.GetValues(typeof(SeriesChartType))
                            .Cast<SeriesChartType>()
                            .OrderBy(t => t.ToString());
            // Llena el ComboBox
            CmbTipoGrafica1.DataSource = tipos.ToList();
            CmbTipoGrafica1.SelectedIndexChanged += CmbTipoGrafica1_SelectedIndexChanged;
            CmbTipoGrafica1.SelectedItem = SeriesChartType.Range;
        }

        private void cmbVentasMensualesAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            DibujarGraficaChart1();
        }

        private void CmbTipoGrafica1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DibujarGraficaChart1();
        }

        private void DibujarGraficaChart1()
        {
            if (cmbVentasMensualesAño.SelectedIndex == 0 )
            {
                Utils.MsgExclamation("Seleccione un año válido.");
                return;
            }
            CargarVentasMensuales(Convert.ToInt32(cmbVentasMensualesAño.SelectedValue), (SeriesChartType)CmbTipoGrafica1.SelectedItem);
        }

        private void CargarVentasMensuales(int year, SeriesChartType tipoGrafica)
        {
            // 1. Obtiene los datos
            List<DtoVentasMensuales> datos = null;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                datos = GraficasService.ObtenerVentasMensuales(year);
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
                return;
            }
            // 2. Prepara la serie del Chart
            var serie = chart1.Series["Ventas"];
            serie.Points.Clear();
            serie.ChartType = tipoGrafica;
            serie.BorderWidth = 2;
            serie.ToolTip = "Ventas de #VALX: #VALY{C2}";
            serie.IsValueShownAsLabel = true;
            serie.LabelFormat = "C2"; // Formato de moneda con 2 decimales
            serie.MarkerStyle = MarkerStyle.Circle;
            serie.MarkerSize = 5;
            serie["LineTension"] = "0.4";             // suavizado de línea
            serie.ShadowOffset = 1;
            serie.ShadowColor = Color.FromArgb(100, Color.Gray);
            serie.Color = Color.FromArgb(52, 152, 219);

            // 3. Agrega puntos al gráfico
            foreach (var punto in datos)
            {
                //string nombreMes = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(punto.Mes);
                int index = serie.Points.AddXY(punto.NombreMes, punto.Total);
                DataPoint dataPoint = serie.Points[index];

                dataPoint.Label = $"${punto.Total:#,##0.00}";
                dataPoint.Font = new Font("Segoe UI", 7, FontStyle.Bold);
            }
            var area = chart1.ChartAreas[0];

            // PRIMERO: forzar cada mes
            area.AxisX.Interval = 1;
            // LUEGO: asignar formato al label
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.MajorGrid.Enabled = false;
            // Títulos de ejes
            area.AxisX.Title = "Meses";
            area.AxisX.TitleFont = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);

            chart1.Legends[0].Enabled = false;
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisY.LabelStyle.Format = "C0";
            area.AxisY.Title = "Ventas Totales";
            area.AxisY.TitleFont = new Font("Segoe UI", 7, FontStyle.Bold);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisY.LabelStyle.Angle = -45;
            string tit = string.Empty;
            if (year == -1)
                tit = "Ventas mensuales (todos los años).";
            else
                tit = $"Ventas mensuales ({year}).";
            // Crear el título
            Title titulo = new Title();
            titulo.Text = tit;
            titulo.ForeColor = Color.Black;
            titulo.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            titulo.Alignment = ContentAlignment.TopCenter;

            // Agregar el título al chart
            chart1.Titles.Clear(); // Limpiar títulos previos
            chart1.Titles.Add(titulo);
            string titGrb = string.Empty;
            if (year == -1)
                titGrb = $"» Ventas mensuales (todos los años). Tipo de gráfica: {tipoGrafica}. «";
            else
                titGrb = $"» Ventas mensuales ({year}). Tipo de gráfica: {tipoGrafica}. «";
            groupBox1.Text = titGrb;
        }
        #endregion
        /******************************************************************************************************/

        #region Grafica2
        private void LlenarCmbUltimosAnios()
        {
            cmbUltimosAnios.SelectedIndexChanged -= cmbUltimosAnios_SelectedIndexChanged;
            int totalAñosDisponibles = GraficasService.ObtenerTotalAñosConVentas();

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
            CmbTipoGrafica2.SelectedItem = SeriesChartType.SplineArea;
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
                Font = new Font("Segoe UI", 7, FontStyle.Regular) // respetando tu fuente
            };
            chart2.Legends.Add(legend);

            List<DtoVentasMensualesPorAños> datos = null;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                datos = GraficasService.ObtenerVentasMensualesPorAños(years);
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
                serie.LegendText = $"{nombreSerie} (Total: {totalAnual:C2})";
                serie.ToolTip = $"{nombreSerie} #VALX: #VALY{{C2}}";

                foreach (var dato in grupo.OrderBy(x => x.Mes))
                {
                    int index = serie.Points.AddXY(dato.NombreMes, dato.Total); // usar NombreMes de la BD
                    DataPoint dp = serie.Points[index];
                    dp.ToolTip = $"{dato.NombreMes} {dato.Year}\nVentas: {dato.Total:C2}";
                    // Configuración común para todos los puntos
                    dp.Font = new Font("Segoe UI", 7, FontStyle.Regular);
                    dp.MarkerStyle = MarkerStyle.Circle;
                    dp.MarkerSize = 5; // tamaño fijo por defecto

                    if (dato.Total != 0)
                    {
                        dp.Label = $"{dato.Total:C2}";
                    }
                    if (dato.Mes == 12)
                    {
                        dp.MarkerSize = 8;
                        dp.MarkerBorderWidth = 2;
                    }
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

            area.AxisY.Title = "Ventas Totales";
            area.AxisY.LabelStyle.Format = "C0";
            area.AxisY.TitleFont = new Font("Segoe UI", 7, FontStyle.Bold);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            area.AxisY.LabelStyle.Angle = -45;
            area.AxisY.IsStartedFromZero = true;
            area.AxisY.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.LineColor = Color.Gray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;

            chart2.AntiAliasing = AntiAliasingStyles.All;
            chart2.TextAntiAliasingQuality = TextAntiAliasingQuality.High;

            // Título principal
            Title titulo = new Title
            {
                Text = $"Comparativo de ventas mensuales de los últimos {years} años.",
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 8, FontStyle.Bold), // respetando tu fuente
                Alignment = ContentAlignment.TopCenter
            };
            titulo.Position.Auto = false;
            titulo.Position.X = 0;
            titulo.Position.Y = 0;   // pegado arriba
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
                CmbAñoTopProd.DataSource = GraficasService.ObtenerTop10AñosDeVentas();
                CmbAñoTopProd.DisplayMember = "Texto";
                CmbAñoTopProd.ValueMember = "Valor";
                CmbAñoTopProd.SelectedValue = DateTime.Today.Year;
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
            groupBox3.Text = $"» {titulo.Text} (Tipo de gráfica: {tipoGrafica}) «";
            titulo.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            titulo.Alignment = ContentAlignment.TopCenter;
            ChartTopProductos.Titles.Add(titulo);
            DataTable datos;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                // Datos
                datos = GraficasService.ObtenerTopProductos(cantidad, año);
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

            series.IsValueShownAsLabel = true;
            series.Label = "#VALY{n0}";
            series.BorderWidth = 2;
            series.ToolTip = "Producto: #VALX, Cantidad vendida: #VALY{n0}";
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
        }
        #endregion
        /******************************************************************************************************/
        //#region Grafica4
        //private void CargarVentasPorVendedores()
        //{
        //    chart4.Series.Clear();
        //    chart4.Titles.Clear();
        //    chart4.Titles.Add(new Title
        //    {
        //        Text = "» Ventas por vendedores de todos los años «",
        //        Docking = Docking.Top,
        //        Font = new Font("Segoe UI", 8, FontStyle.Bold)
        //    });
        //    // 1. Configurar ChartArea 3D
        //    var area = chart4.ChartAreas[0];
        //    area.Area3DStyle.Enable3D = true;
        //    area.Area3DStyle.Inclination = 40;
        //    area.Area3DStyle.Rotation = 60;
        //    area.Area3DStyle.LightStyle = LightStyle.Realistic;
        //    area.Area3DStyle.WallWidth = 0;
        //    // Configuración de la serie
        //    Series serie = new Series
        //    {
        //        Name = "Ventas",
        //        Color = Color.FromArgb(0, 51, 102),
        //        IsValueShownAsLabel = false,
        //        ChartType = SeriesChartType.Doughnut,
        //        Label = "#VALX, #VALY{c2}",
        //        ToolTip = "Vendedor: #VALX\nTotal Ventas: #VALY{C2}"
        //    };
        //    serie.Points.Clear();
        //    serie.SmartLabelStyle.Enabled = true;
        //    serie.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
        //    serie.SmartLabelStyle.CalloutLineColor = Color.Black;
        //    serie.LabelForeColor = Color.DarkSlateGray;
        //    serie.LabelBackColor = Color.WhiteSmoke;
        //    serie["PieLabelStyle"] = "Disabled";
        //    serie["PieDrawingStyle"] = "Cylinder";
        //    serie["DoughnutRadius"] = "60";
        //    chart4.Series.Add(serie);
        //    List<(string Vendedor, decimal TotalVentas)> ventas = new List<(string Vendedor, decimal TotalVentas)>();
        //    try
        //    {
        //        ventas = _graficasService.ObtenerVentasPorVendedores();
        //        if (!ventas.Any())
        //        {
        //            U.MsgExclamation("No se encontraron datos de ventas por vendedores para mostrar en la gráfica.");
        //            return;
        //        }
        //        int i = 0;
        //        foreach (var (vendedor, totalVentas) in ventas)
        //        {
        //            int puntoIndex = serie.Points.AddXY(vendedor, totalVentas);
        //            var punto = serie.Points[puntoIndex];
        //            punto.Color = ChartColors.Paleta[i % ChartColors.Paleta.Length];

        //            // Aquí personalizas la leyenda con salto de línea
        //            punto.LegendText = $"{vendedor}\n{totalVentas:C2}";

        //            i++;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        U.MsgCatchOue(ex);
        //    }
        //    var legend = chart4.Legends[0];
        //    legend.Font = new Font("Segoe UI", 7, FontStyle.Regular);
        //}
        //#endregion
        ///******************************************************************************************************/
        #region Grafica5
        private void LlenarCmbVentasVendedorAño()
        {
            cmbVentasVendedorAño.SelectedIndexChanged -= cmbVentasVendedorAño_SelectedIndexChanged;
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                cmbVentasVendedorAño.DataSource = GraficasService.ObtenerTop10AñosDeVentas();
                cmbVentasVendedorAño.DisplayMember = "Texto";
                cmbVentasVendedorAño.ValueMember = "Valor";
                cmbVentasVendedorAño.SelectedValue = DateTime.Today.Year;
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
            if (cmbVentasVendedorAño.SelectedIndex == 0)
            {
                Utils.MsgExclamation("Seleccione un año válido.");
                return;
            }
            CargarVentasPorVendedoresAño(Convert.ToInt32(cmbVentasVendedorAño.SelectedValue), (SeriesChartType)CmbTipoGrafica5.SelectedItem);
        }

        private void CmbTipoGrafica5_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarVentasPorVendedoresAño(Convert.ToInt32(cmbVentasVendedorAño.SelectedValue), (SeriesChartType)CmbTipoGrafica5.SelectedItem);
        }

        private void CargarVentasPorVendedoresAño(int anio, SeriesChartType tipoGrafica)
        {
            chart5.Series.Clear();
            chart5.Titles.Clear();
            chart5.Legends.Clear();
            var leyenda = new Legend("Vendedores")
            {
                Title = "Vendedores",
                TitleFont = new Font("Segoe UI", 7, FontStyle.Bold),
                Docking = Docking.Right,
                LegendStyle = LegendStyle.Table,
                Font = new Font("Segoe UI", 7, FontStyle.Regular),
                IsTextAutoFit = false
            };

            chart5.Legends.Add(leyenda);
            string tit = string.Empty;
            if (anio == -1)
                tit = $"Ventas por vendedores (todos los años)";
            else
                tit = $"Ventas por vendedores ({anio})";
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
                ToolTip = "Vendedor: #AXISLABEL\nTotal ventas: #VALY{C2}",
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
                var ventas = GraficasService.ObtenerVentasPorVendedores(anio);
                int i = 0;
                foreach (var (vendedor, totalVentas) in ventas)
                {
                    int puntoIndex = serie.Points.AddXY(vendedor, totalVentas);
                    serie.Points[puntoIndex].LegendText = string.Format(
                    CultureInfo.GetCultureInfo("es-MX"),
                    "{0}:\n{1:C2}",
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
        ///******************************************************************************************************/
        #region Grafica6
        private void LlenarCmbVentasMensualesPorVendedorPorAño()
        {
            cmbVentasMensualesPorVendedorPorAño.SelectedIndexChanged -= cmbVentasMensualesPorVendedorPorAño_SelectedIndexChanged;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dt = GraficasService.ObtenerTop10AñosDeVentas();
                cmbVentasMensualesPorVendedorPorAño.DisplayMember = "Texto";
                cmbVentasMensualesPorVendedorPorAño.ValueMember = "Valor";
                cmbVentasMensualesPorVendedorPorAño.DataSource = dt;
                cmbVentasMensualesPorVendedorPorAño.SelectedValue = DateTime.Today.Year;
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            cmbVentasMensualesPorVendedorPorAño.SelectedIndexChanged += cmbVentasMensualesPorVendedorPorAño_SelectedIndexChanged;
        }

        private void LlenarCmbTipoGrafica6()
        {
            CmbTipoGrafica6.SelectedIndexChanged -= CmbTipoGrafica6_SelectedIndexChanged;
            CmbTipoGrafica6.DataSource = Enum.GetValues(typeof(SeriesChartType))
                .Cast<SeriesChartType>()
                .Where(t => t != SeriesChartType.Doughnut && t != SeriesChartType.ErrorBar && t != SeriesChartType.Funnel && t != SeriesChartType.Kagi && t != SeriesChartType.Pie && t != SeriesChartType.PointAndFigure && t != SeriesChartType.Polar && t != SeriesChartType.Pyramid && t != SeriesChartType.Renko && t != SeriesChartType.ThreeLineBreak) // Excluye tipos no deseados
                .OrderBy(t => t.ToString())
                .ToList();
            CmbTipoGrafica6.SelectedIndexChanged += CmbTipoGrafica6_SelectedIndexChanged;
            CmbTipoGrafica6.SelectedItem = SeriesChartType.Line;
        }

        private void cmbVentasMensualesPorVendedorPorAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarVentasMensualesPorVendedorPorAño(Convert.ToInt32(cmbVentasMensualesPorVendedorPorAño.SelectedValue), (SeriesChartType)CmbTipoGrafica6.SelectedItem);
        }

        private void CmbTipoGrafica6_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarVentasMensualesPorVendedorPorAño(Convert.ToInt32(cmbVentasMensualesPorVendedorPorAño.SelectedValue), (SeriesChartType)CmbTipoGrafica6.SelectedItem);
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
            string tit = year == -1
                        ? $"Ventas mensuales por vendedores (todos los años)"
                        : $"Ventas mensuales por vendedores ({year})";
            Title titulo = new Title
            {
                Text = tit,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            ChartVentas.Titles.Add(titulo);
            groupBox6.Text = $"» {titulo.Text} Tipo de gráfica: {tipoGrafica} «";
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
            // Leer datos
            var dt = new DataTable();
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dt = GraficasService.ObtenerVentasMensualesPorVendedoresPorAño(year);
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
                    ToolTip = "#SERIESNAME\nMes: #AXISLABEL\nVentas: #VALY{C2}",
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
                serie.LegendText = $"{serie.Name}\n(Total: {grupo.Total:C2})";
                ChartVentas.Series.Add(serie);
                i++;
            }
            // ————— Aquí forzamos el recálculo de la escala del eje Y —————
            ChartVentas.ResetAutoValues();
        }
        #endregion
    }
}
