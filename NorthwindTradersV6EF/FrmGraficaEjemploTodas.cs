using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmGraficaEjemploTodas : Form
    {
        // Datos fijos: categorías y valores
        private readonly string[] categorias =
            { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago" };
        private readonly double[] valores =
            { 15,    30,    45,    20,    35,    50,    25,    40   };

        public FrmGraficaEjemploTodas()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
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
            // Limpia configuraciones previas
            chart1.Series.Clear();
            chart1.Titles.Clear();

            // Título dinámico
            chart1.Titles.Add(new Title
            {
                Text = $"Tipo de gráfica: {tipo}",
                Docking = Docking.Top,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            });

            // Serie con datos fijos
            var serie = new Series("Ventas")
            {
                ChartType = tipo,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Circle,
                ToolTip = "#SERIESNAME\nMes: #AXISLABEL\nVentas: #VALY{C2}"
            };

            // Agrega puntos usando categorías y valores
            for (int i = 0; i < categorias.Length; i++)
            {
                serie.Points.AddXY(categorias[i], valores[i]);
            }

            chart1.Series.Add(serie);

            // Ajusta automáticamente las escalas de ejes
            chart1.ResetAutoValues();
        }
    }
}
