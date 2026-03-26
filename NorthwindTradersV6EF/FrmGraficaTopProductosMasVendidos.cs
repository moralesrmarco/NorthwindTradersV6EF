using BLL.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmGraficaTopProductosMasVendidos : Form
    {

        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly GraficasService _graficasService;

        public FrmGraficaTopProductosMasVendidos()
        {
            InitializeComponent();
            _graficasService = new GraficasService(cnStr);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmGraficaTopProductosMasVendidos_Load(object sender, EventArgs e)
        {
            ChartTopProductos.Cursor = Cursors.Cross; 
            LlenarComboBoxTopProductos();
            LlenarComboBoxTop10AñosDeVentas();
            GroupBox.Text = $"» Top 10 productos más vendidos «";
            CargarTopProductos(10, -1); // Cargar los 10 productos por defecto
        }

        private void LlenarComboBoxTopProductos()
        {
            var items = new List<KeyValuePair<string, int>>();
            items.Add(new KeyValuePair<string, int>("»--- Seleccione ---«", 0));
            for (int i = 10; i <= 50; i = i + 5)
            {
                items.Add(new KeyValuePair<string, int>($"{i} productos", i));
            }
            ComboBoxTopProducto.DataSource = items;
            ComboBoxTopProducto.DisplayMember = "Key";
            ComboBoxTopProducto.ValueMember = "Value";
        }

        private void LlenarComboBoxTop10AñosDeVentas()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                ComboBoxAños.DataSource = _graficasService.ObtenerTop10AñosDeVentas();
                ComboBoxAños.DisplayMember = "Texto";
                ComboBoxAños.ValueMember = "Valor";
                ComboBoxAños.SelectedIndex = 0;
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
        }

        private void BtnMostrar_Click(object sender, EventArgs e)
        {
            if (ComboBoxTopProducto.SelectedIndex == 0)
            {
                Utils.MsgExclamation("Seleccione un número de productos válido.");
                return;
            }
            if (ComboBoxAños.SelectedIndex == 0)
            {
                Utils.MsgExclamation("Seleccione un año válido.");
                return;
            }

            int cantidad = Convert.ToInt32(ComboBoxTopProducto.SelectedValue);
            int anio = Convert.ToInt32(ComboBoxAños.SelectedValue);

            CargarTopProductos(cantidad, anio);
        }

        private void CargarTopProductos(int cantidad, int anio)
        {
            ChartTopProductos.Series.Clear();
            ChartTopProductos.Titles.Clear();
            Title titulo = new Title();
            titulo.Text = anio == -1
                        ? $"Top {cantidad} productos más vendidos (todos los años)"
                        : $"Top {cantidad} productos más vendidos ({anio})";
            GroupBox.Text = $"» {titulo.Text} «";
            titulo.Font = new Font("Arial", 14, FontStyle.Bold);
            titulo.Alignment = ContentAlignment.TopCenter;
            ChartTopProductos.Titles.Add(titulo);
            DataTable datos;
            int totalUnidades = 0;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                // Datos
                datos = _graficasService.ObtenerTopProductos(cantidad, anio);
                foreach (DataRow row in datos.Rows)
                {
                    totalUnidades += Convert.ToInt32(row["CantidadVendida"]);
                }
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
            series.ChartType = SeriesChartType.Column;
            series["DrawingStyle"] = "Cylinder";

            series.ShadowOffset = 3;
            series.ShadowColor = Color.FromArgb(120, Color.Black);

            series.IsValueShownAsLabel = true;
            series.Label = "#VALY{n0}";
            series.BorderWidth = 2;
            series.ToolTip = "Producto: #VALX, Cantidad vendida: #VALY{n0}";
            series.Font = new Font("Arial", 10, FontStyle.Bold);
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

            Title subtitulo = new Title();

            //subtitulo.Text = $"Total de unidades vendidas: {totalUnidades:N0}";
            subtitulo.Text = anio == -1
                            ? $"Total vendido en el Top (todos los años): {totalUnidades:N0} unidades"
                            : $"Total vendido en el Top ({anio}): {totalUnidades:N0} unidades";
            subtitulo.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            subtitulo.Alignment = ContentAlignment.TopRight;

            subtitulo.Position.Auto = false;
            subtitulo.Position.X = area.Position.X;
            subtitulo.Position.Width = area.Position.Width;
            subtitulo.Position.Y = 3;
            subtitulo.Position.Height = 5;
            ChartTopProductos.Titles.Add(subtitulo);

            area.Area3DStyle.Enable3D = true;
            area.Area3DStyle.Inclination = 30;
            area.Area3DStyle.Rotation = 20;
            area.Area3DStyle.LightStyle = LightStyle.Realistic;

            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.Title = "Productos más vendidos";
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.Black;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.LabelStyle.Angle = -45;
            area.AxisY.LabelStyle.Font = new Font("Arial", 8, FontStyle.Regular);
            area.AxisY.Title = "Cantidad vendida (unidades)";
            area.AxisY.MajorGrid.Enabled = true;
            area.AxisY.MajorGrid.LineColor = Color.Black;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisY.MinorGrid.Enabled = true;
            area.AxisY.MinorGrid.LineColor = Color.Black;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;
        }
    }
}
