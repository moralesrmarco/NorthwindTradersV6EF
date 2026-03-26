using BLL;
using BLL.Services;
using Entities.DTOs;
using NorthwindTradersV6EF.Helpers;
using System;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmProductosListado : Form
    {

        string _connectionString = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private ProductoBLL _productoBLL;
        private readonly CategoriaService _categoriaService;
        private readonly ProveedorService _proveedorService;
        private bool EjecutarConfDgv = true;

        public FrmProductosListado()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            _productoBLL = new ProductoBLL(_connectionString);
            _categoriaService = new CategoriaService(_connectionString);
            _proveedorService = new ProveedorService(_connectionString);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmProductosListado_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmProductosListado_Load(object sender, EventArgs e)
        {
            Dgv.ColumnHeaderMouseClick += Dgv_ColumnHeaderMouseClick;
            LlenarCboCategoria();
            LlenarCboProveedor();
            Utils.ConfDgv(Dgv);
            LlenarDgv(true);
        }

        private void LlenarCboCategoria()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dtCategorias = _categoriaService.ObtenerCategoriasCbo();
                ComboBoxHelper.LlenarCbo(cboBCategoria, dtCategorias, "CategoryName", "CategoryId");
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void LlenarCboProveedor()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dtProveedores = _proveedorService.ObtenerProveedoresCbo();
                ComboBoxHelper.LlenarCbo(cboBProveedor, dtProveedores, "CompanyName", "SupplierId");
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            nudBIdIni.Value = nudBIdFin.Value = 0;
            txtBProducto.Text = "";
            cboBCategoria.SelectedIndex = cboBProveedor.SelectedIndex = 0;
            LlenarDgv(true);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenarDgv(true);
        }

        private void LlenarDgv(bool selectorRealizaBusqueda)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DtoProductosBuscar criterios = null;
                if (selectorRealizaBusqueda)
                {
                    criterios = new DtoProductosBuscar
                    {
                        IdIni = Convert.ToInt32(nudBIdIni.Value),
                        IdFin = Convert.ToInt32(nudBIdFin.Value),
                        Producto = txtBProducto.Text.Trim(),
                        Categoria = cboBCategoria.SelectedValue == null ? 0 : Convert.ToInt32(cboBCategoria.SelectedValue),
                        Proveedor = cboBProveedor.SelectedValue == null ? 0 : Convert.ToInt32(cboBProveedor.SelectedValue)
                    };
                }
                else
                    criterios = null;
                var productos = _productoBLL.ObtenerProductos(selectorRealizaBusqueda, criterios, false);
                var dtoProductos = productos.Select(p => new DtoProducto
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    QuantityPerUnit = p.QuantityPerUnit,
                    UnitPrice = p.UnitPrice,
                    UnitsInStock = p.UnitsInStock,
                    UnitsOnOrder = p.UnitsOnOrder,
                    ReorderLevel = p.ReorderLevel,
                    Discontinued = p.Discontinued,
                    CategoryName = p.Categoria?.CategoryName,
                    Description = p.Categoria?.Description,
                    CompanyName = p.Proveedor?.CompanyName,
                    CategoryID = p.Categoria?.CategoryID ?? 0,
                    SupplierID = p.Proveedor?.SupplierID ?? 0
                }).ToList();
                Dgv.DataSource = dtoProductos;
                if (EjecutarConfDgv)
                {
                    ConfDgv();
                    EjecutarConfDgv = false;
                }
                // Conteo de categorías y proveedores distintos
                int totalCategorias = dtoProductos.Select(c => c.CategoryID).Distinct().Count();
                int totalProveedores = dtoProductos.Select(p => p.SupplierID).Distinct().Count();
                string leyenda = string.Empty;
                if (Dgv.RowCount > 0)
                    leyenda = $"Se encontraron {Dgv.RowCount} producto(s), en {totalCategorias} categoría(s) y {totalProveedores} proveedor(es)";
                else
                    leyenda = "No se encontraron registros";
                MDIPrincipal.ActualizarBarraDeEstado(leyenda);
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void ConfDgv()
        {
            Dgv.Columns["CategoryID"].Visible = false;
            Dgv.Columns["SupplierID"].Visible = false;

            Dgv.Columns["ProductID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["QuantityPerUnit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["UnitPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["UnitsInStock"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["UnitsOnOrder"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["ReorderLevel"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Discontinued"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["CategoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["UnitsInStock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["UnitsOnOrder"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["ReorderLevel"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            Dgv.Columns["UnitPrice"].DefaultCellStyle.Format = "c";
            Dgv.Columns["UnitsInStock"].DefaultCellStyle.Format = "n0";
            Dgv.Columns["UnitsOnOrder"].DefaultCellStyle.Format = "n0";
            Dgv.Columns["ReorderLevel"].DefaultCellStyle.Format = "n0";

            Dgv.Columns["ProductID"].HeaderText = "Id";
            Dgv.Columns["ProductName"].HeaderText = "Producto";
            Dgv.Columns["QuantityPerUnit"].HeaderText = "Cantidad por unidad";
            Dgv.Columns["UnitPrice"].HeaderText = "Precio";
            Dgv.Columns["UnitsInStock"].HeaderText = "Unidades en inventario";
            Dgv.Columns["UnitsOnOrder"].HeaderText = "Unidades en pedido";
            Dgv.Columns["ReorderLevel"].HeaderText = "Nivel de reorden";
            Dgv.Columns["Discontinued"].HeaderText = "Descontinuado";
            Dgv.Columns["CategoryName"].HeaderText = "Categoría";
            Dgv.Columns["Description"].HeaderText = "Descripción de categoría";
            Dgv.Columns["CompanyName"].HeaderText = "Proveedor";
        }

        private void Dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // debe estar vinculado a la clase List<> a la cual esta vinculado el DataGridView.DataSource
            Utils.OrdenarPorColumna<DtoProducto>(Dgv, e);
        }

        private void nudBIdIni_Leave(object sender, EventArgs e) => Utils.ValidarRango(sender, nudBIdIni, nudBIdFin);

        private void nudBIdFin_Leave(object sender, EventArgs e) => Utils.ValidarRango(sender, nudBIdIni, nudBIdFin);

        private void Nud_Enter(object sender, EventArgs e)
        {
            if (sender is NumericUpDown nud && nud.Controls[1] is TextBox tb)
            {
                // Diferir la selección para que ocurra después de que el TextBox reciba el foco
                tb.BeginInvoke((Action)(() => tb.SelectAll()));
            }
        }
    }
}