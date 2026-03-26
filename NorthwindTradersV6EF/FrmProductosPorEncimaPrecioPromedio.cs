using BLL;
using System;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmProductosPorEncimaPrecioPromedio : Form
    {

        string _connectionString = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private ProductoBLL _productoBLL;

        public FrmProductosPorEncimaPrecioPromedio()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            _productoBLL = new ProductoBLL(_connectionString);
            // las dos siguientes lineas es para que se pueda habilitar el ordenamiento por cada columna
            Dgv.ColumnHeaderMouseClick += (s, e) => Utils.OrdenarPorColumna(Dgv, e); // vinculacion del evento al metodo
            Dgv.DataBindingComplete += Dgv_DataBindingComplete;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmProductosPorEncimaPrecioPromedio_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmProductosPorEncimaPrecioPromedio_Load(object sender, EventArgs e)
        {
            CalcularPrecioPromedio();
            Utils.ConfDgv(Dgv);
            LlenarDgv();
            ConfDgv();
        }

        private void CalcularPrecioPromedio()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var precioPromedio = _productoBLL.ObtenerPrecioPromedio();
                string strPrecioPromedio = precioPromedio.ToString("c4");
                Grb.Text = $"» Listado de productos con el precio por encima del precio promedio: {strPrecioPromedio} «";
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void LlenarDgv()
        {
            try
            {
                var dt = _productoBLL.ObtenerProductosPorEncimaDelPrecioPromedio();
                Dgv.DataSource = dt.DefaultView; // para activar el ordenamiento se debe enlazar al DefaultView del DataTable
                MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {Dgv.RowCount} registro(s)");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void Dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var dgv = (DataGridView)sender;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                // Si la columna está ligada a un campo de datos
                if (!string.IsNullOrEmpty(col.DataPropertyName))
                {
                    col.SortMode = DataGridViewColumnSortMode.Programmatic;
                }
                else
                {
                    // Columnas sin DataPropertyName (imágenes, botones, calculadas)
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
        }

        private void ConfDgv()
        {
            Dgv.Columns["Fila"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Precio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["PrecioPromedio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Diferencia"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["PorcentajeSobrePromedio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["PrecioPromedio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Diferencia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["PorcentajeSobrePromedio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // se usa c4 y p4 para que haya mayor aproximacion en los calculos
            // en la base de datos se usa CAST(AVG(UnitPrice) AS DECIMAL(18,10)), CAST(p.UnitPrice AS DECIMAL(18,10)), etc. tambien para obtener una mayor aproximación en los calculos
            Dgv.Columns["Precio"].DefaultCellStyle.Format = "c4";
            Dgv.Columns["PrecioPromedio"].DefaultCellStyle.Format = "c4";
            Dgv.Columns["Diferencia"].DefaultCellStyle.Format = "c4";
            Dgv.Columns["PorcentajeSobrePromedio"].DefaultCellStyle.Format = "p4";

            Dgv.Columns["PrecioPromedio"].HeaderText = "Precio promedio";
            Dgv.Columns["Diferencia"].HeaderText = "Diferencia con respecto al precio promedio";
            Dgv.Columns["PorcentajeSobrePromedio"].HeaderText = "Porcentaje sobre el precio promedio";
            Dgv.Columns["Categoria"].HeaderText = "Categoría";
        }
    }
}
