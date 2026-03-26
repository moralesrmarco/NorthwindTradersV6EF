using BLL;
using Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using Utilities;
using static Utilities.InventarioHelper;

namespace NorthwindTradersV6EF
{
    public partial class FrmVentasDetalleModificar2 : Form
    {

        string _connectionString = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        internal Dictionary<string, object> valoresOriginales;
        private VentaDetalleBLL _ventaDetalleBLL;
        private ProductoBLL _productoBLL;

        public VentaDetalle ventaDetalle { get; set; }

        private short CantidadOld;
        private decimal DescuentoOld;
        private short UInventarioOld;
        private decimal InventarioRealDb;
        private decimal InventarioCalculado;

        public FrmVentasDetalleModificar2()
        {
            InitializeComponent();
            _ventaDetalleBLL = new VentaDetalleBLL(_connectionString);
            _productoBLL = new ProductoBLL(_connectionString);
        }

        private void FrmVentasDetalleModificar2_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        internal void FrmVentasDetalleModificar2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrar) == DialogResult.No)
                    e.Cancel = true;
        }

        private void FrmVentasDetalleModificar2_Load(object sender, EventArgs e)
        {
            controlAgregarProducto.GrbProducto.Text = "»   Modificar producto de la venta:   «";
            controlAgregarProducto.BtnAgregar.Visible = false;
            txtId.Text = string.Empty;
            controlAgregarProducto.CboCategoria.Enabled = false;
            controlAgregarProducto.CboProducto.Enabled = false;

            var categoria = _productoBLL.ObtenerProductoPorId(ventaDetalle.Producto.ProductID).Categoria.CategoryName;
            // Si el ComboBox está vacío o no contiene ese texto, lo agregas
            if (string.IsNullOrEmpty(categoria))
            {
                // Evita la excepción: no intentes agregar null
                categoria = "(Sin categoría)";
            }
            if (!controlAgregarProducto.CboCategoria.Items.Contains(categoria))
            {
                controlAgregarProducto.CboCategoria.Items.Add(categoria);
            }
            controlAgregarProducto.CboCategoria.SelectedItem = categoria;

            if (!controlAgregarProducto.CboProducto.Items.Contains(ventaDetalle.Producto.ProductName))
            {
                controlAgregarProducto.CboProducto.Items.Add(ventaDetalle.Producto.ProductName);
            }
            controlAgregarProducto.CboProducto.SelectedItem = ventaDetalle.Producto.ProductName;
            controlAgregarProducto.NudPrecioConIVAIncluido.Value = ventaDetalle.UnitPrice;
            InventarioRealDb = ObtenerUInventario();
            InventarioCalculado = InventarioHelper.ActualizarInventarioUi(
                ventaDetalle.Quantity,   // cantidad nueva
                0,  // cantidad vieja
                InventarioRealDb,   // inventario real
                controlAgregarProducto.NudUInventario,
                ModoInventario.Disponibilidad
            );
            UInventarioOld = (short)InventarioCalculado;

            //// Guardar el valor para validaciones posteriores

            controlAgregarProducto.NudCantidad.Value = ventaDetalle.Quantity;
            controlAgregarProducto.NudDescuento.Value = ventaDetalle.TasaDescuentoPorcentaje;
            DeshabilitarNudsNoSeleccionables();
            CalcularTotalProducto();
            CantidadOld = ventaDetalle.Quantity;
            DescuentoOld = ventaDetalle.TasaDescuentoPorcentaje;
            ValidarCantidadEInventarioHelper.ValidarInventario
            (
                controlAgregarProducto.NudCantidad.Value,
                CantidadOld,
                UInventarioOld,
                controlAgregarProducto.NudUInventario.Value,
                controlAgregarProducto.NudUInventario,
                toolTip1,
                controlAgregarProducto.PbError1,
                controlAgregarProducto.PbInfo1,
                controlAgregarProducto.PbWarning1,
                errorProvider1
            );
            ValidarCantidadEInventarioHelper.ValidarCantidad
            (
                controlAgregarProducto.NudCantidad.Value,
                CantidadOld,
                UInventarioOld,
                InventarioRealDb, // <-- usar inventario real
                controlAgregarProducto.NudCantidad,
                toolTip1,
                controlAgregarProducto.PbError,
                controlAgregarProducto.PbInfo,
                controlAgregarProducto.PbWarning,
                errorProvider1
            );
            CargarValoresOriginales();
            controlAgregarProducto.NudEnter += NudEnterHandler;
            controlAgregarProducto.NudCantidadDescuento_LeaveValueChanged += NudCantidadDescuento_LeaveValueChangedHandler;
        }

        private Decimal ObtenerUInventario()
        {
            Decimal uInventario = 0;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                uInventario = Convert.ToDecimal(_ventaDetalleBLL.ObtenerUInventario(ventaDetalle.Producto.ProductID));
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            return uInventario;
        }

        private void DeshabilitarNudsNoSeleccionables()
        {
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudPrecioConIVAIncluido, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudUInventario, false);

            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudPrecioPorUnidadSinIVAIncluidoAntesDescuento, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudIVADelPrecioPorUnidadAntesDescuento, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudPrecioPorUnidadSinIVADepuesDescuento, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudAhorroPorUnidadSinIVA, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudIVADelPrecioPorUnidadDespuesDescuento, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudAhorroEnIVAPorUnidadDespuesDescuento, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudPrecioPorUnidadConIVADespuesDescuento, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudAhorroTotalPorUnidadConIVA, false);

            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudSubtotalDelImporteConIVAIncluido2, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudSubtotalDelImporteSinIVASinDescuento2, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudSubtotalDelImporteDelIVASinDescuento2, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudSubtotalDelImporteSinIVAConDescuento2, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudSubtotalIVADespuesDelDescuento2, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudSubtotalDelAhorroSinIvaDespuesDescuento2, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudSubtotalDelAhorroEnIVADespuesDescuento2, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudSubtotalDelAhorroTotalDespuesDescuento2, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudTotal2, false);
        }

        private void CargarValoresOriginales()
        {
            valoresOriginales = Utils.CapturarValoresOriginales(this);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidarControles()
        {
            try
            {
                btnModificar.Enabled = false;
                errorProvider1.Clear();
                // Recalcula importes
                CalcularTotalProducto();
                // Actualiza inventario en UI y guarda el valor calculado
                // Recalcular inventario en UI con modo Disponibilidad
                InventarioHelper.ActualizarInventarioUi(
                    controlAgregarProducto.NudCantidad.Value, // cantidad nueva
                    0,  // cantidad vieja
                    InventarioRealDb, // inventario viejo
                    controlAgregarProducto.NudUInventario,
                    ModoInventario.Disponibilidad
                );

                // Validación informativa (inventario)
                // no afecta el retorno, solo muestra íconos
                ValidarCantidadEInventarioHelper.ValidarInventario
                (
                    controlAgregarProducto.NudCantidad.Value,
                    CantidadOld,
                    UInventarioOld,
                    controlAgregarProducto.NudUInventario.Value,
                    controlAgregarProducto.NudUInventario,
                    toolTip1,
                    controlAgregarProducto.PbError1,
                    controlAgregarProducto.PbInfo1,
                    controlAgregarProducto.PbWarning1,
                    errorProvider1
                );
                //// Valida reglas de negocio con StatusIconHelper
                //// Validación restrictiva (cantidad)
                if (!ValidarCantidadEInventarioHelper.ValidarCantidad
                    (
                        controlAgregarProducto.NudCantidad.Value,
                        CantidadOld,
                        UInventarioOld,
                        InventarioRealDb, // ✅ inventario real
                        controlAgregarProducto.NudCantidad,
                        toolTip1,
                        controlAgregarProducto.PbError,
                        controlAgregarProducto.PbInfo,
                        controlAgregarProducto.PbWarning,
                        errorProvider1
                    )
                )
                {
                    return false;
                }
                if (controlAgregarProducto.NudTotal2.Value <= 0)
                {
                    errorProvider1.SetError(controlAgregarProducto.NudTotal2, "El valor del total del producto no puede ser cero.");
                    return false;
                }
                // Habilitar el botón Modificar si hubo cambios y las validaciones pasaron
                bool hayCambios = (controlAgregarProducto.NudCantidad.Value != CantidadOld) || (controlAgregarProducto.NudDescuento.Value != DescuentoOld);
                if (hayCambios)
                {
                    btnModificar.Enabled = true;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
                return false;
            }
        }

        private void CalcularTotalProducto()
        {
            try
            {
                VentaDetalle ventaDetalle = new VentaDetalle()
                {
                    UnitPrice = controlAgregarProducto.NudPrecioConIVAIncluido.Value,
                    Quantity = Convert.ToInt16(controlAgregarProducto.NudCantidad.Value),
                    Discount = controlAgregarProducto.NudDescuento.Value / 100m
                };
                controlAgregarProducto.NudPrecioPorUnidadSinIVAIncluidoAntesDescuento.Value = ventaDetalle.PrecioPorUnidadSinIVASinDescuento;
                controlAgregarProducto.NudIVADelPrecioPorUnidadAntesDescuento.Value = ventaDetalle.IVADelPrecioPorUnidadSinDescuento;
                controlAgregarProducto.NudPrecioPorUnidadConIVADespuesDescuento.Value = ventaDetalle.PrecioPorUnidadConIVADespuesDescuento;
                controlAgregarProducto.NudIVADelPrecioPorUnidadDespuesDescuento.Value = ventaDetalle.IVADelPrecioporUnidadDespuesDescuento;
                controlAgregarProducto.NudPrecioPorUnidadSinIVADepuesDescuento.Value = ventaDetalle.PrecioPorUnidadSinIVADepuesDescuento;
                controlAgregarProducto.NudAhorroPorUnidadSinIVA.Value = ventaDetalle.AhorroPorUnidadSinIVA;
                controlAgregarProducto.NudAhorroEnIVAPorUnidadDespuesDescuento.Value = ventaDetalle.AhorroEnIVAPorUnidadDespuesDescuento;
                controlAgregarProducto.NudAhorroTotalPorUnidadConIVA.Value = ventaDetalle.AhorroTotalPorUnidadConIVA;

                controlAgregarProducto.NudSubtotalDelImporteConIVAIncluido2.Value = ventaDetalle.SubtotalDelImporteConIVAIncluido;
                controlAgregarProducto.NudSubtotalDelImporteSinIVASinDescuento2.Value = ventaDetalle.SubtotalDelImporteSinIVASinDescuento;
                controlAgregarProducto.NudSubtotalDelImporteDelIVASinDescuento2.Value = ventaDetalle.SubtotalDelImporteDelIVASinDescuento;
                controlAgregarProducto.NudSubtotalIVADespuesDelDescuento2.Value = ventaDetalle.SubtotalIVADespuesDelDescuento;
                controlAgregarProducto.NudSubtotalDelImporteSinIVAConDescuento2.Value = ventaDetalle.SubtotalDelImporteSinIVAConDescuento;
                controlAgregarProducto.NudSubtotalDelAhorroSinIvaDespuesDescuento2.Value = ventaDetalle.SubtotalDelAhorroSinIvaDespuesDescuento;
                controlAgregarProducto.NudSubtotalDelAhorroEnIVADespuesDescuento2.Value = ventaDetalle.SubtotalDelAhorroEnIVADespuesDescuento;
                controlAgregarProducto.NudSubtotalDelAhorroTotalDespuesDescuento2.Value = ventaDetalle.SubtotalDelAhorroTotalDespuesDescuento;

                controlAgregarProducto.NudTotal2.Value = ventaDetalle.Subtotal;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            // No se realiza la validación porque ya se han realizado previamente en el evento leave y valuechanged de 
            // txtdescuento y txtcantidad
            try
            {
                btnModificar.Enabled = false;
                MDIPrincipal.ActualizarBarraDeEstado(Utils.modificandoRegistro);
                ventaDetalle.Quantity = (short)controlAgregarProducto.NudCantidad.Value;
                ventaDetalle.Discount = controlAgregarProducto.NudDescuento.Value / 100m;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
                DialogResult = DialogResult.Cancel;
                CargarValoresOriginales();
                this.Close();
                return;
            }
            // La siguientes linea es necesaria para que se permita cerrar la ventana. 
            // ya que se validan las variables en FrmPedidosDetalleModificar_FormClosing
            CargarValoresOriginales();
            //if (numRegs == -3)
            //    DialogResult = DialogResult.Cancel;
            //else
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void NudCantidadDescuento_LeaveValueChangedHandler(object sender, EventArgs e) => ValidarControles();

        private void NudEnterHandler(object sender, EventArgs e)
        {
            if (sender is NumericUpDown nud && nud.Controls[1] is TextBox tb)
            {
                // Diferir la selección para que ocurra después de que el TextBox reciba el foco
                tb.BeginInvoke((Action)(() => tb.SelectAll()));
            }
        }
    }
}
