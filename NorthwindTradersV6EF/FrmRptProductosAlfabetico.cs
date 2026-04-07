using BLL.EF;
using DTOs.EF;
using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptProductosAlfabetico: Form
    {
        public FrmRptProductosAlfabetico()
        {
            InitializeComponent();
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
                var productos = ProductBLL.ObtenerProductos(criterios);
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
                    CategoryName = p.CategoryName,
                    CompanyName = p.CompanyName,
                    CategoryID = p.CategoryID ?? 0,
                    SupplierID = p.SupplierID ?? 0
                }).ToList();
                // Conteo de categorías y proveedores distintos
                int totalCategorias = dtoProductos.Select(c => c.CategoryID).Distinct().Count();
                int totalProveedores = dtoProductos.Select(p => p.SupplierID).Distinct().Count();
                string leyenda = string.Empty;
                if (dtoProductos.Count > 0)
                    leyenda = $"Se encontraron {dtoProductos.Count} producto(s), en {totalCategorias} categoría(s) y {totalProveedores} proveedor(es)";
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
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.noDatos, true);
                    U.NotificacionWarning(Utils.noDatos);
                }
            }
            catch (Exception ex) { U.MsgCatchOue(ex); }
        }
    }
}
