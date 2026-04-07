using BLL.EF;
using BLL.EF.Services;
using DTOs.EF;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptProductos : Form
    {
        string titulo = "» Reporte de productos «";
        string subtitulo = "";

        // esto es un campo - Un campo es una variable declarada dentro de la clase, que guarda datos asociados a las instancias de esa clase.
        private readonly List<KeyValuePair<string, string>> _itemsOrdenadoPor =
            new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ProductID", "ID Producto"),
                new KeyValuePair<string, string>("ProductName", "Producto"),
                new KeyValuePair<string, string>("QuantityPerUnit", "Cantidad por unidad"),
                new KeyValuePair<string, string>("UnitPrice", "Precio"),
                new KeyValuePair<string, string>("UnitsInStock", "Unidades en inventario"),
                new KeyValuePair<string, string>("UnitsOnOrder", "Unidades en pedido"),
                new KeyValuePair<string, string>("ReorderLevel", "Nivel de reorden"),
                new KeyValuePair<string, string>("Discontinued", "Descontinuado"),
                new KeyValuePair<string, string>("CategoryName", "Categoría"),
                new KeyValuePair<string, string>("CompanyName", "Proveedor")
            };

        // esto tambien es un campo
        private readonly List<KeyValuePair<string, string>> _itemsAscDesc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ASC", "Ascendente"),
                new KeyValuePair<string, string>("DESC", "Descendente")
            };

        public FrmRptProductos()
        {
            InitializeComponent();
            nudBIdIni.Leave += nudBIdIni_Leave;
            nudBIdFin.Leave += nudBIdFin_Leave;
            nudBIdIni.Enter += Nud_Enter;
            nudBIdFin.Enter += Nud_Enter;
            tabcOperacion.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabcOperacion.DrawItem += tabcOperacion_DrawItem;
            reportViewer1.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmRptProductos_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void tabcOperacion_DrawItem(object sender, DrawItemEventArgs e) => Utils.DibujarPestañas(sender as TabControl, e);

        private void FrmRptProductos_Load(object sender, EventArgs e)
        {
            LlenarCboCategoria();
            LlenarCboProveedor();

            Cbo1OrdenadoPor.DataSource = _itemsOrdenadoPor.ToList();
            Cbo1OrdenadoPor.DisplayMember = "Value";
            Cbo1OrdenadoPor.ValueMember = "Key";
            Cbo2OrdenadoPor.DataSource = _itemsOrdenadoPor.ToList();
            Cbo2OrdenadoPor.DisplayMember = "Value";
            Cbo2OrdenadoPor.ValueMember = "Key";
            Cbo1AscDesc.DataSource = _itemsAscDesc.ToList();
            Cbo1AscDesc.DisplayMember = "Value";
            Cbo1AscDesc.ValueMember = "Key";
            Cbo2AscDesc.DataSource = _itemsAscDesc.ToList();
            Cbo2AscDesc.DisplayMember = "Value";
            Cbo2AscDesc.ValueMember = "Key";
            Cbo1OrdenadoPor.SelectedIndex = Cbo1AscDesc.SelectedIndex = 0;
        }

        private void LlenarCboCategoria()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                cboCategoria.DataSource = CategoryService.ObtenerCategoriasCbo();
                cboCategoria.ValueMember = "CategoryID";
                cboCategoria.DisplayMember = "CategoryName";
                cboCategoria.SelectedIndex = 0;
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
                cboProveedor.DataSource = SupplierService.ObtenerProveedoresCbo();
                cboProveedor.ValueMember = "SupplierID";
                cboProveedor.DisplayMember = "CompanyName";
                cboProveedor.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            nudBIdIni.Value = nudBIdFin.Value = 0;
            txtProducto.Text = "";
            cboCategoria.SelectedIndex = cboProveedor.SelectedIndex = 0;
            Cbo1OrdenadoPor.SelectedIndex = Cbo1AscDesc.SelectedIndex = 0;
            Cbo2OrdenadoPor.SelectedIndex = Cbo2AscDesc.SelectedIndex = 0;
            MDIPrincipal.ActualizarBarraDeEstado();
        }

        private void btnImprimirTodos_Click(object sender, EventArgs e)
        {
            LlenarReporte(false);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            LlenarReporte(true);
        }

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e) => btnLimpiar.PerformClick();

        private void tabcOperacion_SelectedIndexChanged(object sender, EventArgs e) => btnLimpiar.PerformClick();

        private void LlenarReporte(bool selectorRealizaBusqueda)
        {
            try
            {
                titulo = "» Reporte de todos los productos «";
                subtitulo = "";
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DtoProductosBuscar criterios = null;
                if (selectorRealizaBusqueda)
                {
                    criterios = new DtoProductosBuscar
                    {
                        IdIni = Convert.ToInt32(nudBIdIni.Value),
                        IdFin = Convert.ToInt32(nudBIdFin.Value),
                        Producto = txtProducto.Text.Trim(),
                        Categoria = cboCategoria.SelectedIndex <= 0 ? 0 : Convert.ToInt32(cboCategoria.SelectedValue),
                        Proveedor = cboProveedor.SelectedIndex <= 0 ? 0 : Convert.ToInt32(cboProveedor.SelectedValue),
                        OrdenadoPor = Cbo2OrdenadoPor.SelectedValue.ToString(),
                        AscDesc = Cbo2AscDesc.SelectedValue.ToString()
                    };
                    titulo = "» Reporte filtrado de productos «";
                    subtitulo = $"Filtrado por: ";
                    if (nudBIdIni.Value != 0 & nudBIdFin.Value != 0)
                        subtitulo += $" [ Id: {nudBIdIni.Value} al {nudBIdFin.Value} ] ";
                    if (txtProducto.Text != "")
                        subtitulo += $" [ Producto: {txtProducto.Text} ] ";
                    if (cboCategoria.SelectedIndex > 0)
                        subtitulo += $" [ Categoría: {cboCategoria.Text}] ";
                    if (cboProveedor.SelectedIndex > 0)
                        subtitulo += $" [ Proveedor: {cboProveedor.Text}] ";
                    if (subtitulo == "Filtrado por: ")
                    {
                        titulo = "» Reporte de todos los productos «";
                        subtitulo = "";
                    }
                    subtitulo += $" Ordenado por: [ {Cbo2OrdenadoPor.Text} ] [ {Cbo2AscDesc.Text} ]";
                }
                else
                {
                    criterios = new DtoProductosBuscar
                    {
                        IdIni = 0,
                        IdFin = 0,
                        Producto = "",
                        Categoria = cboCategoria.SelectedIndex = 0,
                        Proveedor = cboProveedor.SelectedIndex = 0,
                        OrdenadoPor = Cbo1OrdenadoPor.SelectedValue.ToString(),
                        AscDesc = Cbo1AscDesc.SelectedValue.ToString()
                    };
                    subtitulo = $"Ordenado por: [ {Cbo1OrdenadoPor.Text} ] [ {Cbo1AscDesc.Text} ]";
                }
                groupBox1.Text = titulo + " | » " + subtitulo + " «";
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