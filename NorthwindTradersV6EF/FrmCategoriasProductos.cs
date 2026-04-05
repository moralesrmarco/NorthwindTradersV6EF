using BLL.EF;
using DAL.EF;
using System;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmCategoriasProductos : Form
    {

        BindingSource bsCategorias = new BindingSource();
        BindingSource bsProductos = new BindingSource();
        bool ejecutarAlCargar = true;

        public FrmCategoriasProductos()
        {
            InitializeComponent();
            // Suscripción al evento
            bsCategorias.ListChanged += bsCategorias_ListChanged;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmCategoriasProductos_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmCategoriasProductos_Load(object sender, EventArgs e)
        {
            dgvCategorias.DataSource = bsCategorias;
            dgvProductos.DataSource = bsProductos;
            if (ejecutarAlCargar)
            {
                Utils.ConfDgv(dgvCategorias);
                Utils.ConfDgv(dgvProductos);
            }
            GetData();
            if (ejecutarAlCargar)
            {
                ConfDgvCategorias(dgvCategorias);
                ConfDgvProductos(dgvProductos);
                ejecutarAlCargar = false;
            }
        }

        private void GetData()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var categorias = CategoryBLL.ObtenerCategoriasProductosDgv();

                bsCategorias.DataSource = categorias;
                dgvCategorias.AutoGenerateColumns = true;
                dgvCategorias.DataSource = bsCategorias;

                bsProductos.DataSource = bsCategorias;
                bsProductos.DataMember = nameof(Category.Products);
                dgvProductos.AutoGenerateColumns = true;
                dgvProductos.DataSource = bsProductos;

                // Actualiza después de que el mensaje de UI regrese al loop (binding ya estable)
                BeginInvoke((Action)(ActualizarEstadoCategorias));

            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void ConfDgvCategorias(DataGridView dgv)
        {
            dgv.Columns["RowVersion"].Visible = false;
            dgv.Columns["Products"].Visible = false;
            dgv.Columns["RowVersionStr"].Visible = false;
            dgv.Columns["Picture"].Visible = false;

            dgv.Columns["CategoryId"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["CategoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgv.Columns["PictureImage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgv.Columns["PictureImage"].Width = 80;
            dgv.RowTemplate.Height = 80;
            dgv.Columns["PictureImage"].DefaultCellStyle.Padding = new Padding(4);
            ((DataGridViewImageColumn)dgv.Columns["PictureImage"]).ImageLayout = DataGridViewImageCellLayout.Zoom;

            dgv.Columns["CategoryId"].HeaderText = "Id";
            dgv.Columns["CategoryName"].HeaderText = "Categoría";
            dgv.Columns["Description"].HeaderText = "Descripción";
            dgv.Columns["PictureImage"].HeaderText = "Foto";
        }

        private void ConfDgvProductos(DataGridView dgv)
        {
            dgv.Columns["CategoryId"].Visible = false;
            dgv.Columns["SupplierId"].Visible = false;
            dgv.Columns["RowVersion"].Visible = false;
            dgv.Columns["Category"].Visible = false;
            dgv.Columns["Order_Details"].Visible = false;
            dgv.Columns["Supplier"].Visible = false;

            dgv.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["UnitsInStock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["UnitsOnOrder"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["ReorderLevel"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv.Columns["ProductId"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["QuantityPerUnit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["UnitPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["UnitsInStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["UnitsOnOrder"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["ReorderLevel"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Discontinued"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["CategoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["CategoryDescription"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns["SupplierCompanyName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


            dgv.Columns["UnitPrice"].DefaultCellStyle.Format = "c";
            dgv.Columns["UnitsInStock"].DefaultCellStyle.Format = "n0";
            dgv.Columns["UnitsOnOrder"].DefaultCellStyle.Format = "n0";
            dgv.Columns["ReorderLevel"].DefaultCellStyle.Format = "n0";
            dgv.Columns["ProductId"].HeaderText = "Id";
            dgv.Columns["CategoryName"].HeaderText = "Categoría";
            dgv.Columns["ProductName"].HeaderText = "Producto";
            dgv.Columns["QuantityPerUnit"].HeaderText = "Cantidad por unidad";
            dgv.Columns["UnitPrice"].HeaderText = "Precio";
            dgv.Columns["UnitsInStock"].HeaderText = "Unidades en inventario";
            dgv.Columns["UnitsOnOrder"].HeaderText = "Unidades en pedido";
            dgv.Columns["ReorderLevel"].HeaderText = "Punto de pedido";
            dgv.Columns["Discontinued"].HeaderText = "Descontinuado";
            dgv.Columns["CategoryDescription"].HeaderText = "Descripción de categoría";
            dgv.Columns["SupplierCompanyName"].HeaderText = "Proveedor";

            dgvProductos.Columns["ProductID"].DisplayIndex = 0;
            dgvProductos.Columns["ProductName"].DisplayIndex = 1;
            dgvProductos.Columns["QuantityPerUnit"].DisplayIndex = 2;
            dgvProductos.Columns["UnitPrice"].DisplayIndex = 3;
            dgvProductos.Columns["UnitsInStock"].DisplayIndex = 4;
            dgvProductos.Columns["UnitsOnOrder"].DisplayIndex = 5;
            dgvProductos.Columns["ReorderLevel"].DisplayIndex = 6;
            dgvProductos.Columns["Discontinued"].DisplayIndex = 7;
            dgvProductos.Columns["CategoryName"].DisplayIndex = 8;
            dgvProductos.Columns["CategoryDescription"].DisplayIndex = 9;
            dgvProductos.Columns["SupplierCompanyName"].DisplayIndex = 10;
        }

        private void dgvCategorias_SelectionChanged(object sender, EventArgs e) => ActualizarEstadoCategorias();

        private void FrmCategoriasProductos_Shown(object sender, EventArgs e) => ActualizarEstadoCategorias();

        private void dgvCategorias_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) => ActualizarEstadoCategorias();

        // Si cambian los datos (filtros, reload, etc.), refresca
        private void bsCategorias_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) => ActualizarEstadoCategorias();

        private void ActualizarEstadoCategorias()
        {
            // Conteo lógico desde el BindingSource (maestro)
            int totalCategorias = bsCategorias?.Count ?? 0;

            // Conteo visible desde el grid (por si hay filtros/ocultas)
            int filasVisibles = dgvCategorias.Rows.GetRowCount(DataGridViewElementStates.Visible);

            // Nombre de categoría seleccionado (seguro)
            string categoria = null;
            if (dgvCategorias.CurrentRow != null &&
                dgvCategorias.CurrentRow.Cells["CategoryName"].Value != null)
            {
                categoria = dgvCategorias.CurrentRow.Cells["CategoryName"].Value.ToString();
            }

            string msg = categoria == null
                ? $"Se encontraron {totalCategorias} categoría(s) (visibles: {filasVisibles}) y {bsProductos?.Count ?? 0} producto(s)."
                : $"Se encontraron {totalCategorias} categoría(s) (visibles: {filasVisibles}) y {bsProductos?.Count ?? 0} producto(s), en la categoría {categoria}";

            MDIPrincipal.ActualizarBarraDeEstado(msg);
        }
    }
}