using BLL.EF;
using Microsoft.Reporting.WinForms;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptProdPorProvConDetProv : Form
    {

        public FrmRptProdPorProvConDetProv()
        {
            InitializeComponent();
            reportViewer1.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmRptProdPorProvConDetProv_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptProdPorProvConDetProv_Load(object sender, System.EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var productosPorProveedorConDetProv = ProductBLL.ObtenerProductosPorProveedorConDetProv();
                // Conteos
                int totalProveedores = productosPorProveedorConDetProv
                            .Select(p => p.CompanyName) // ajusta al nombre real de la propiedad
                            .Distinct()
                            .Count();
                int totalProductos = productosPorProveedorConDetProv
                    .Where(p => !string.Equals(p.ProductName, "Sin producto", StringComparison.OrdinalIgnoreCase)) // excluye los productos ficticios "Sin producto" que aparecen en el reporte cuando un proveedor no tiene productos
                    .Select(p => new { p.ProductID, p.ProductName }) // cuenta por combinación Id + nombre para considerar los productos con el mismo nombre pero diferente Id
                    .Distinct()
                    .Count();
                // Conteo de ciudades distintas
                int totalCiudades = productosPorProveedorConDetProv
                    .Select(cp => cp.City?.Trim()) // quita espacios
                    .Where(c => !string.IsNullOrEmpty(c)) // descarta vacíos
                    .Distinct(StringComparer.OrdinalIgnoreCase) // ignora mayúsculas/minúsculas
                    .Count();
                // Conteo de países distintos
                int totalPaises = productosPorProveedorConDetProv
                    .Select(cp => cp.Country?.Trim()) // quita espacios
                    .Where(p => !string.IsNullOrEmpty(p)) // descarta vacíos
                    .Distinct(StringComparer.OrdinalIgnoreCase) // ignora mayúsculas/minúsculas
                    .Count();
                string leyenda = string.Empty;
                if (totalProveedores > 0)
                    leyenda = $"Se encontraron {totalProveedores} proveedor(es), en {totalCiudades} ciudad(es), en {totalPaises} país(es) y {totalProductos} producto(s)";
                MDIPrincipal.ActualizarBarraDeEstado(leyenda);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", productosPorProveedorConDetProv));
                reportViewer1.BackColor = Color.White;
                reportViewer1.RefreshReport();
                if (productosPorProveedorConDetProv.Count == 0)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.noDatos, true);
                    U.NotificacionWarning(Utils.noDatos);
                }
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }
    }
}
