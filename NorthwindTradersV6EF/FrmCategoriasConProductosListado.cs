using BLL.EF;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmCategoriasConProductosListado : Form
    {
        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmCategoriaConProductos_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        public FrmCategoriasConProductosListado()
        {
            InitializeComponent();
            // las dos siguientes lineas es para que se pueda habilitar el ordenamiento por cada columna
            DgvListado.ColumnHeaderMouseClick += (s, e) => Utils.OrdenarPorColumna(DgvListado, e); // vinculacion del evento al metodo
            DgvListado.DataBindingComplete += DgvListado_DataBindingComplete;
        }

        private void FrmCategoriasConProductosListado_Load(object sender, EventArgs e)
        {
            Utils.ConfDgv(DgvListado);
            LlenarDgv();
            ConfDgv();
        }

        private void LlenarDgv()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);

                // Obtener lista tipada desde EF
                var lista = CategoryBLL.ObtenerProductosPorCategoriaListado();

                // Enlazar directamente la lista al DataGridView
                DgvListado.DataSource = lista;

                // Totales usando LINQ sobre la lista
                int totalCategorias = lista
                    .Select(r => r.CategoryName)
                    .Distinct()
                    .Count();

                int totalProductos = lista
                    .Count(r => r.ProductID != null);

                int totalProveedores = lista
                    .Where(r => !string.IsNullOrEmpty(r.CompanyName))
                    .Select(r => r.CompanyName)
                    .Distinct()
                    .Count();

                // Actualizar barra de estado
                MDIPrincipal.ActualizarBarraDeEstado(
                    $"Se encontraron {totalCategorias} categoría(s), {totalProductos} producto(s) y {totalProveedores} proveedor(es) distinto(s)"
                );
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void DgvListado_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
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
            DgvListado.Columns["ProductID"].Visible = false;

            DgvListado.Columns["CategoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvListado.Columns["UnitPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvListado.Columns["UnitsInStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvListado.Columns["UnitsOnOrder"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvListado.Columns["ReorderLevel"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvListado.Columns["Discontinued"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            DgvListado.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvListado.Columns["UnitsInStock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvListado.Columns["UnitsOnOrder"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvListado.Columns["ReorderLevel"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DgvListado.Columns["UnitPrice"].DefaultCellStyle.Format = "c";
            DgvListado.Columns["UnitsInStock"].DefaultCellStyle.Format = "N0";
            DgvListado.Columns["UnitsOnOrder"].DefaultCellStyle.Format = "N0";
            DgvListado.Columns["ReorderLevel"].DefaultCellStyle.Format = "N0";

            DgvListado.Columns["CategoryName"].HeaderText = "Categoría";
            DgvListado.Columns["ProductName"].HeaderText = "Producto";
            DgvListado.Columns["QuantityPerUnit"].HeaderText = "Cantidad por unidad";
            DgvListado.Columns["UnitPrice"].HeaderText = "Precio";
            DgvListado.Columns["UnitsInStock"].HeaderText = "Unidades en inventario";
            DgvListado.Columns["UnitsOnOrder"].HeaderText = "Unidades en pedido";
            DgvListado.Columns["ReorderLevel"].HeaderText = "Punto de pedido";
            DgvListado.Columns["Discontinued"].HeaderText = "Descontinuado";
            DgvListado.Columns["CompanyName"].HeaderText = "Proveedor";
        }

    }
}