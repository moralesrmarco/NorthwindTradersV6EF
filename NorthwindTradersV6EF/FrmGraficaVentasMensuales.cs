using BLL.Services;
using Entities.DTOs;
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
    public partial class FrmGraficaVentasMensuales : Form
    {
        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly GraficasService _graficasService;

        public FrmGraficaVentasMensuales()
        {
            InitializeComponent();
            _graficasService = new GraficasService(cnStr);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaVentasMensuales_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
            CargarVentasMensuales(DateTime.Now.Year);
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            if (ComboBoxAño.SelectedIndex == 0)
            {
                Utils.MsgExclamation("Seleccione un año válido.");
                return;
            }
            CargarVentasMensuales(Convert.ToInt32(ComboBoxAño.SelectedValue));
        }

        private void LlenarComboBox()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                ComboBoxAño.DataSource = _graficasService.ObtenerAñosDeVentas();
                ComboBoxAño.DisplayMember = "YearOrderDate";
                ComboBoxAño.ValueMember = "YearOrderDate";
                ComboBoxAño.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            MDIPrincipal.ActualizarBarraDeEstado();
        }

        private void CargarVentasMensuales(int year)
        {
            List<DtoVentasMensuales> datos = new List<DtoVentasMensuales>();
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                datos = _graficasService.ObtenerVentasMensuales(year);
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
            // 2. Prepara la serie del Chart
            var serie = chartVentas.Series["Ventas mensuales"];
            serie.Points.Clear();
            serie.ChartType = SeriesChartType.Line;
            serie.BorderWidth = 3;
            serie.ToolTip = "Ventas de #VALX: #VALY{C2}";
            serie.IsValueShownAsLabel = true;
            serie.LabelFormat = "C2"; // Formato de moneda con 2 decimales
            serie.MarkerStyle = MarkerStyle.Circle;
            serie.MarkerSize = 10;

            serie["LineTension"] = "0.4";
            serie.ShadowOffset = 2;
            serie.ShadowColor = Color.FromArgb(100, Color.Gray);
            serie.Color = Color.FromArgb(52, 152, 219);

            // 3. Agrega puntos al gráfico
            foreach (var punto in datos)
            {
                serie.Points.AddXY(punto.NombreMes, punto.Total);
            }


            var area = chartVentas.ChartAreas[0];

            area.BackColor = Color.White;
            area.BackSecondaryColor = Color.FromArgb(245, 245, 245);
            area.BackGradientStyle = GradientStyle.TopBottom;

            // PRIMERO: forzar cada mes
            area.AxisX.Interval = 1;
            // LUEGO: asignar formato al label
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.Title = "Meses";
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisY.LabelStyle.Format = "C0";
            area.AxisY.Title = "Ventas Totales";
            area.AxisY.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.LineColor = Color.Gray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            area.AxisY.MinorGrid.Enabled = true;
            area.AxisY.MinorGrid.LineColor = Color.LightGray;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisY.IsStartedFromZero = true;

            chartVentas.Legends[0].Enabled = false;

            chartVentas.AntiAliasing = AntiAliasingStyles.All;
            chartVentas.TextAntiAliasingQuality = TextAntiAliasingQuality.High;

            // Crear el título
            Title titulo = new Title();
            titulo.Text = $"Ventas mensuales del año: {year}";
            titulo.Font = new Font("Arial", 14, FontStyle.Bold);
            titulo.Alignment = ContentAlignment.TopCenter;

            decimal totalVentas = datos.Sum(x => x.Total);
            Title subTitulo = new Title();
            subTitulo.Text = $"Total de ventas del año {year}: {totalVentas:C2}";
            //subTitulo.Docking = Docking.Top;
            subTitulo.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            subTitulo.Alignment = ContentAlignment.TopRight;

            area.InnerPlotPosition.Auto = false;
            area.InnerPlotPosition.X = 10;
            area.InnerPlotPosition.Y = 5;
            area.InnerPlotPosition.Width = 85;
            area.InnerPlotPosition.Height = 85;
            
            subTitulo.Position.Auto = false;

            subTitulo.Position.X = area.InnerPlotPosition.X - 1;
            subTitulo.Position.Width = area.InnerPlotPosition.Width + 1;
            subTitulo.Position.Y = 3;
            subTitulo.Position.Height = 5;

            chartVentas.Series[0]["AnimationDuration"] = "1500";

            // Agregar el título al chart
            chartVentas.Titles.Clear(); // Limpiar títulos previos
            chartVentas.Titles.Add(titulo);
            chartVentas.Titles.Add(subTitulo);

            GroupBox.Text = $"» Ventas mensuales del año: {year} «";
        }
    }
}
