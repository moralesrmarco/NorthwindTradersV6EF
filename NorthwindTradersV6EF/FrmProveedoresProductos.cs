using BLL;
using System;
using System.Configuration;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmProveedoresProductos : Form
    {
        string _connectionString = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        ProveedorBLL _proveedorBLL;
        BindingSource bsProveedores = new BindingSource();
        BindingSource bsProductos = new BindingSource();
        bool ejecutarAlCargar = true;

        public FrmProveedoresProductos()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            // Suscripción al evento
            bsProveedores.ListChanged += bsProveedores_ListChanged;
            _proveedorBLL = new ProveedorBLL(_connectionString);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmProveedoresProductos_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmProveedoresProductos_Load(object sender, EventArgs e)
        {
            DgvProveedores.DataSource = bsProveedores;
            DgvProductos.DataSource = bsProductos;
            if (ejecutarAlCargar)
            {
                Utils.ConfDgv(DgvProveedores);
                Utils.ConfDgv(DgvProductos);
            }
            GetData();
            if (ejecutarAlCargar)
            {
                ConfDgvProveedores();
                ConfDgvProductos();
                ejecutarAlCargar = false;
            }
        }

        private void GetData()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var ds = _proveedorBLL.ObtenerProveedoresProductosDgv();

                bsProveedores.DataSource = ds;
                bsProveedores.DataMember = "Proveedores";

                bsProductos.DataSource = bsProveedores;
                bsProductos.DataMember = "ProveedoresProductos";

                // necesario para que funcione el ordenamiento al hacer clic en el encabezado de columna del dataGridView maestro
                DgvProveedores.AutoGenerateColumns = true;   // 🔑
                DgvProveedores.DataSource = bsProveedores;

                DgvProductos.AutoGenerateColumns = true;
                DgvProductos.DataSource = bsProductos;

                // 🔑 habilitar ordenamiento automático
                foreach (DataGridViewColumn col in DgvProveedores.Columns)
                    col.SortMode = DataGridViewColumnSortMode.Automatic;

                // Actualiza después de que el mensaje de UI regrese al loop (binding ya estable)
                BeginInvoke((Action)(ActualizarEstadoProveedores));

            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void ConfDgvProveedores()
        {
            DgvProveedores.Columns["SupplierID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["ContactTitle"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["City"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["Region"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["PostalCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["Country"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["Phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProveedores.Columns["Fax"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            DgvProveedores.Columns["City"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProveedores.Columns["Region"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProveedores.Columns["PostalCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProveedores.Columns["Country"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DgvProveedores.Columns["SupplierID"].HeaderText = "ID";
            DgvProveedores.Columns["CompanyName"].HeaderText = "Nombre de compañía";
            DgvProveedores.Columns["ContactName"].HeaderText = "Nombre de contacto";
            DgvProveedores.Columns["ContactTitle"].HeaderText = "Título de contacto";
            DgvProveedores.Columns["Address"].HeaderText = "Dirección";
            DgvProveedores.Columns["City"].HeaderText = "Ciudad";
            DgvProveedores.Columns["Region"].HeaderText = "Región";
            DgvProveedores.Columns["PostalCode"].HeaderText = "Código postal";
            DgvProveedores.Columns["Country"].HeaderText = "País";
            DgvProveedores.Columns["Phone"].HeaderText = "Teléfono";
        }

        private void ConfDgvProductos()
        {
            DgvProductos.Columns["CategoryID"].Visible = false;
            DgvProductos.Columns["SupplierID"].Visible = false;

            DgvProductos.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProductos.Columns["UnitsInStock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProductos.Columns["UnitsOnOrder"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvProductos.Columns["ReorderLevel"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DgvProductos.Columns["ProductID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["UnitPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["UnitsInStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DgvProductos.Columns["UnitsOnOrder"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DgvProductos.Columns["ReorderLevel"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            DgvProductos.Columns["Discontinued"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DgvProductos.Columns["CategoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            DgvProductos.Columns["CompanyName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            DgvProductos.Columns["UnitPrice"].DefaultCellStyle.Format = "c";
            DgvProductos.Columns["UnitsInStock"].DefaultCellStyle.Format = "n0";
            DgvProductos.Columns["UnitsOnOrder"].DefaultCellStyle.Format = "n0";
            DgvProductos.Columns["ReorderLevel"].DefaultCellStyle.Format = "n0";

            DgvProductos.Columns["ProductID"].HeaderText = "ID";
            DgvProductos.Columns["ProductName"].HeaderText = "Producto";
            DgvProductos.Columns["QuantityPerUnit"].HeaderText = "Cantidad por unidad";
            DgvProductos.Columns["UnitPrice"].HeaderText = "Precio";
            DgvProductos.Columns["UnitsInStock"].HeaderText = "Unidades en inventario";
            DgvProductos.Columns["UnitsOnOrder"].HeaderText = "Unidades en pedido";
            DgvProductos.Columns["ReorderLevel"].HeaderText = "Nivel de reorden";
            DgvProductos.Columns["Discontinued"].HeaderText = "Descontinuado";
            DgvProductos.Columns["CategoryName"].HeaderText = "Categoría";
            DgvProductos.Columns["Description"].HeaderText = "Descripción de categoría";
            DgvProductos.Columns["CompanyName"].HeaderText = "Proveedor";

            DgvProductos.Columns["ProductID"].DisplayIndex = 0;
            DgvProductos.Columns["ProductName"].DisplayIndex = 1;
            DgvProductos.Columns["QuantityPerUnit"].DisplayIndex = 2;
            DgvProductos.Columns["UnitPrice"].DisplayIndex = 3;
            DgvProductos.Columns["UnitsInStock"].DisplayIndex = 4;
            DgvProductos.Columns["UnitsOnOrder"].DisplayIndex = 5;
            DgvProductos.Columns["ReorderLevel"].DisplayIndex = 6;
            DgvProductos.Columns["Discontinued"].DisplayIndex = 7;
            DgvProductos.Columns["CategoryName"].DisplayIndex = 8;
            DgvProductos.Columns["Description"].DisplayIndex = 9;
            DgvProductos.Columns["CompanyName"].DisplayIndex = 10;
        }

        private void DgvProveedores_SelectionChanged(object sender, EventArgs e) => ActualizarEstadoProveedores();

        private void DgvProveedores_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) => ActualizarEstadoProveedores();

        private void FrmProveedoresProductos_Shown(object sender, EventArgs e) => ActualizarEstadoProveedores();

        // Si cambian los datos (filtros, reload, etc.), refresca
        private void bsProveedores_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) => ActualizarEstadoProveedores();

        private void ActualizarEstadoProveedores()
        {
            // Conteo lógico desde el BindingSource (maestro)
            int totalProveedores = bsProveedores?.Count ?? 0;

            // Conteo visible desde el grid (por si hay filtros/ocultas)
            int filasVisibles = DgvProveedores.Rows.GetRowCount(DataGridViewElementStates.Visible);

            // Nombre de proveedor seleccionado (seguro)
            string proveedor = null;
            if (DgvProveedores.CurrentRow != null &&
                DgvProveedores.CurrentRow.Cells["CompanyName"].Value != null)
            {
                proveedor = DgvProveedores.CurrentRow.Cells["CompanyName"].Value.ToString();
            }

            string msg = proveedor == null
                ? $"Se encontraron {totalProveedores} proveedor(es) (visible(s): {filasVisibles}) y {bsProductos?.Count ?? 0} producto(s)."
                : $"Se encontraron {totalProveedores} proveedor(es) (visible(s): {filasVisibles}) y {bsProductos?.Count ?? 0} producto(s), del proveedor {proveedor}";

            MDIPrincipal.ActualizarBarraDeEstado(msg);
        }

    }
}