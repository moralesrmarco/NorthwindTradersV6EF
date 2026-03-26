using BLL;
using Entities.DTOs;
using Microsoft.Reporting.WinForms;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptProductosAlfabetico: Form
    {
        string _connectionString = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private ProductoBLL _productoBLL;

        public FrmRptProductosAlfabetico()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            _productoBLL = new ProductoBLL(_connectionString);
            reportViewer1.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptProductosAlfabetico_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptProductosAlfabetico_Load(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DtoProductosBuscar criterios = null;
                criterios = new DtoProductosBuscar
                {
                    OrdenadoPor = "ProductName",
                    AscDesc = "Asc"
                };
                string titulo = "» Reporte de productos en orden alfabético «";
                string subtitulo = $" Ordenado por: [ Producto ] [ Ascendente ]";
                groupBox1.Text = titulo + " | » " + subtitulo + " «";
                //var dt = new ProductoRepository(ConfigurationManager.ConnectionStrings["NorthwindMySql"].ConnectionString).RptProductosListado(dtoProductosBuscar, strProcedure);
                var productos = _productoBLL.ObtenerProductos(criterios);
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
                    CompanyName = p.Proveedor?.CompanyName,
                    CategoryID = p.Categoria?.CategoryID ?? 0,
                    SupplierID = p.Proveedor?.SupplierID ?? 0
                }).ToList();
                // Conteo de categorías y proveedores distintos
                int totalCategorias = dtoProductos.Select(c => c.CategoryID).Distinct().Count();
                int totalProveedores = dtoProductos.Select(p => p.SupplierID).Distinct().Count();
                string leyenda = string.Empty;
                if (dtoProductos.Count > 0)
                    leyenda = $"Se encontraron {dtoProductos.Count} producto(s), en {totalCategorias} categoría(s) y {totalProveedores} proveedor(es)";
                else
                    leyenda = "No se encontraron registros";
                MDIPrincipal.ActualizarBarraDeEstado(leyenda);
                ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dtoProductos);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                ReportParameter rp = new ReportParameter("titulo", titulo);
                ReportParameter rp2 = new ReportParameter("subtitulo", subtitulo);
                reportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp2 });
                reportViewer1.BackColor = Color.White;
                reportViewer1.RefreshReport();
                if (dtoProductos.Count == 0)
                    U.NotificacionWarning(Utils.noDatos);
            }
            catch (Exception ex) { U.MsgCatchOue(ex); }
        }
    }
}
