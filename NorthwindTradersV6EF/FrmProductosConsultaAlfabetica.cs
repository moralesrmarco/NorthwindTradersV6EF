using BLL.EF;
using DTOs.EF;
using System;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmProductosConsultaAlfabetica : Form
    {
        public FrmProductosConsultaAlfabetica()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmProductosConsultaAlfabetica_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmProductosConsultaAlfabetica_Load(object sender, EventArgs e)
        {
            Dgv.ColumnHeaderMouseClick += Dgv_ColumnHeaderMouseClick;
            Utils.ConfDgv(Dgv);
            LlenarDgv();
        }

        private void LlenarDgv()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var productos = ProductBLL.ObtenerProductos(false, null, true); // se esta reutilizando lo que ya esta programado, por eso esos parametros
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
                    CategoryName = p.Category?.CategoryName,
                    Description = p.Category?.Description,
                    CompanyName = p.Supplier?.CompanyName,
                    CategoryID = p.Category?.CategoryID ?? 0,
                    SupplierID = p.Supplier?.SupplierID ?? 0
                })
                .OrderBy(p => p.ProductName) // se reordena para cumplir con el orden alfabético
                .ToList();
                Dgv.DataSource = dtoProductos;
                ConfDgv();
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
    }
}