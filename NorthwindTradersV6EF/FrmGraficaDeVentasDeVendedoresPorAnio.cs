using BLL.Services;
using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmGraficaDeVentasDeVendedoresPorAnio : Form
    {
        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly GraficasService _graficasService;

        public FrmGraficaDeVentasDeVendedoresPorAnio()
        {
            InitializeComponent();
            _graficasService = new GraficasService(cnStr);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaDeVentasDeVendedoresPorAnio_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
            CargarVentasPorVendedores(DateTime.Now.Year); // Cargar ventas del año actual por defecto
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            if (ComboBoxAño.SelectedIndex == 0)
            {
                U.MsgExclamation("Seleccione un año válido.");
                return;
            }
            CargarVentasPorVendedores(Convert.ToInt32(ComboBoxAño.SelectedValue));
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

        private void CargarVentasPorVendedores(int anio)
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();
            chart1.Legends.Clear();

            var leyenda = new Legend("Vendedores")
            {
                Title = "Vendedores",
                TitleFont = new Font("Arial", 10, FontStyle.Bold),
                Docking = Docking.Right,
                LegendStyle = LegendStyle.Table
            };
            chart1.Legends.Add(leyenda);

            // Título del gráfico
            Title titulo = new Title
            {
                Text = $"» Gráfica de ventas por vendedores del año {anio} «",
                Font = new Font("Arial", 16, FontStyle.Bold),
            };
            chart1.Titles.Add(titulo);
            groupBox1.Text = titulo.Text; // Actualizar el texto del GroupBox
            // Configuración de la serie
            Series serie = new Series
            {
                Name = "Ventas",
                Color = Color.FromArgb(0, 51, 102),
                IsValueShownAsLabel = true,
                ChartType = SeriesChartType.Doughnut,
                Label = "#AXISLABEL: #VALY{C2}",
                ToolTip = "Vendedor: #AXISLABEL\nTotal ventas: #VALY{C2}",
                Legend = leyenda.Name,
                LegendText = "#AXISLABEL: #VALY{C2}"
            };
            // 1. Configurar ChartArea 3D
            var area = chart1.ChartAreas[0];
            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 40;
            area.Area3DStyle.Rotation = 60;
            area.Area3DStyle.LightStyle = LightStyle.Realistic;
            area.Area3DStyle.WallWidth = 0;

            serie["PieLabelStyle"] = "Outside";
            serie["PieDrawingStyle"] = "Cylinder";
            serie["DoughnutRadius"] = "60";
            chart1.Series.Add(serie);
            // Consulta SQL para obtener las ventas por vendedor del año seleccionado
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var ventas = _graficasService.ObtenerVentasPorVendedores(anio);
                int i = 0;
                foreach (var (vendedor, totalVentas) in ventas)
                {
                    int puntoIndex = serie.Points.AddXY(vendedor, totalVentas);
                    serie.Points[puntoIndex].LegendText = string.Format(
                    CultureInfo.GetCultureInfo("es-MX"),
                    "{0}: {1:C2}",
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
            Title subTitulo = new Title
            {
                Text = $"Total de ventas del año {anio}: {serie.Points.Sum(p => p.YValues[0]):C2}",
                Docking = Docking.Top,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                IsDockedInsideChartArea = false,
                Alignment = ContentAlignment.TopLeft,
                DockingOffset = -3
            };
            // Agregar el subtítulo al chart
            chart1.Titles.Add(subTitulo);
            MDIPrincipal.ActualizarBarraDeEstado();
        }
    }
}
