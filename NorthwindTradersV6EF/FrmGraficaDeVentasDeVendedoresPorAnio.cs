using BLL.EF.Services;
using System;
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

        public FrmGraficaDeVentasDeVendedoresPorAnio()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaDeVentasDeVendedoresPorAnio_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
            CargarVentasPorVendedores(DateTime.Today.Year); // Cargar ventas del año actual por defecto
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
            string tit = string.Empty;
            if (anio < 0)
                tit = "» Gráfica de ventas por vendedores de todos los años «";
            else
                tit = $"» Gráfica de ventas por vendedores del año {anio} «";
            // Título del gráfico
            Title titulo = new Title
            {
                Text = tit,
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
                var ventas = GraficasService.ObtenerVentasPorVendedores(anio);
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
            string subTit = string.Empty;
            if (anio < 0)
                subTit = "Total de ventas (todos los años):";
            else
                subTit = $"Total de ventas ({anio}):";

            Title subTitulo = new Title
            {
                Text = $"{subTit} {serie.Points.Sum(p => p.YValues[0]):C2}",
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

        private void ComboBoxAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxAño.SelectedIndex == 0)
            {
                U.MsgExclamation("Seleccione un año válido.");
                return;
            }
            CargarVentasPorVendedores(Convert.ToInt32(ComboBoxAño.SelectedValue));
        }
    }
}
