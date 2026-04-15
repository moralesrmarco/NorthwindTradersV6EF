using BLL.EF;
using BLL.EF.Services;
using DAL.EF;
using NorthwindTradersV6EF.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmVentasDetalleCrud : Form
    {
        int numDetalle = 1;
        internal Dictionary<string, object> valoresOriginales;
        private short CantidadOld = 0;
        private short UInventarioOld = 0;

        public FrmVentasDetalleCrud()
        {
            InitializeComponent();
            DgvVentas.ColumnHeaderMouseClick += DgvVentas_ColumnHeaderMouseClick;

            // Hacer que se pinten en negro los groupboxes de los controles anidados
            foreach (var gb in controlBuscarVenta.Controls.OfType<GroupBox>())
                gb.Paint += GrbPaint;
            foreach (var gb in controlTotalesDeLaVenta.Controls.OfType<GroupBox>())
                gb.Paint += GrbPaint;
            foreach (var gb in controlDetalleDeLaVenta.Controls.OfType<GroupBox>())
                gb.Paint += GrbPaint;
            // los groupboxes de controlAgregarProducto se pintaran directamente desde el control... porque se pintan de dos distintas maneras

            // Suscripción al evento del UserControl
            controlBuscarVenta.LimpiarClick += ControlBuscarVenta_LimpiarClick;
            controlBuscarVenta.BuscarClick += ControlBuscarVenta_BuscarClick;

            controlAgregarProducto.NudEnter += NudEnterHandler;
            controlAgregarProducto.NudCantidadDescuento_LeaveValueChanged += NudCantidadDescuento_LeaveValueChangedHandler;
            controlAgregarProducto.CboCategoria_SelectedIndexChanged += CboCategoria_SelectedIndexChangedHandler;
            controlAgregarProducto.CboProducto_SelectedIndexChanged += CboProducto_SelectedIndexChangedHandler;
            controlAgregarProducto.BtnAgregar_Click += BtnAgregar_ClickHandler;
            BtnNota.Click += BtnNota_Click;
            controlDetalleDeLaVenta.DgvDetalle_CellClick += DgvDetalle_CellClickHandler;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmVentasDetalleCrud_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        internal void FrmVentasDetalleCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrar) == DialogResult.No)
                    e.Cancel = true;
        }

        private void FrmVentasDetalleCrud_Load(object sender, EventArgs e)
        {
            DeshabilitarControles();
            LlenarCboCategoria();
            LlenarDgvVentas(false);
            Utils.ConfDgv(DgvVentas);
            Utils.ConfDgv(controlDetalleDeLaVenta.DgvDetalle);
            ConfDgvVentas();
            ConfDgvDetalle();
            DeshabilitarNudsNoSeleccionables();
            InicializarCboProducto();
            CargarValoresOriginales();
            controlDetalleDeLaVenta.DgvDetalle.Columns["Modificar"].Visible = false;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Eliminar"].Visible = false;
        }

        private void CargarValoresOriginales()
        {
            valoresOriginales = Utils.CapturarValoresOriginales(this);
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

            Utilities.NudHelper.SetEnabled(controlTotalesDeLaVenta.NudNumProd, false);
            Utilities.NudHelper.SetEnabled(controlTotalesDeLaVenta.NudTotalDeUnidades, false);
            Utilities.NudHelper.SetEnabled(controlTotalesDeLaVenta.NudSubtotalDelImporte, false);
            Utilities.NudHelper.SetEnabled(controlTotalesDeLaVenta.NudSubtotalDelImporteDelDescuento, false);
            Utilities.NudHelper.SetEnabled(controlTotalesDeLaVenta.NudSubtotalDelImporteConDescuento, false);
            Utilities.NudHelper.SetEnabled(controlTotalesDeLaVenta.NudSubtotalDelImporteSinIVA, false);
            Utilities.NudHelper.SetEnabled(controlTotalesDeLaVenta.NudSubtotalDelImporteDelIVA, false);
            Utilities.NudHelper.SetEnabled(controlTotalesDeLaVenta.NudTotal, false);
        }

        private void DeshabilitarCantidadDescuento()
        {
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudCantidad, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudDescuento, false);
        }

        private void HabilitarCantidadDescuento()
        {
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudCantidad, true);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudDescuento, true);
        }

        private void InicializarCboProducto()
        {
            DataTable dtCboProductos = new DataTable();
            dtCboProductos.Columns.Add("ProductID", typeof(int));
            dtCboProductos.Columns.Add("ProductName", typeof(string));
            DataRow dr = dtCboProductos.NewRow();
            dr["ProductID"] = 0;
            dr["ProductName"] = "«--- Seleccione ---»";
            dtCboProductos.Rows.Add(dr);
            controlAgregarProducto.CboProducto.DataSource = dtCboProductos;
            controlAgregarProducto.CboProducto.DisplayMember = "ProductName";
            controlAgregarProducto.CboProducto.ValueMember = "ProductID";
            controlAgregarProducto.CboProducto.Enabled = false;
        }

        private void InicializarNuds()
        {
            controlTotalesDeLaVenta.NudNumProd.Value = controlTotalesDeLaVenta.NudTotalDeUnidades.Value = controlTotalesDeLaVenta.NudSubtotalDelImporte.Value = controlTotalesDeLaVenta.NudSubtotalDelImporteDelDescuento.Value = controlTotalesDeLaVenta.NudSubtotalDelImporteConDescuento.Value = controlTotalesDeLaVenta.NudSubtotalDelImporteSinIVA.Value = controlTotalesDeLaVenta.NudSubtotalDelImporteDelIVA.Value = controlTotalesDeLaVenta.NudTotal.Value = 0;
            InicializarNudsProducto();
        }

        private void InicializarNudsProducto()
        {
            controlAgregarProducto.NudPrecioPorUnidadSinIVAIncluidoAntesDescuento.Value = controlAgregarProducto.NudIVADelPrecioPorUnidadAntesDescuento.Value = controlAgregarProducto.NudPrecioPorUnidadConIVADespuesDescuento.Value = controlAgregarProducto.NudIVADelPrecioPorUnidadDespuesDescuento.Value = controlAgregarProducto.NudPrecioPorUnidadSinIVADepuesDescuento.Value = controlAgregarProducto.NudAhorroPorUnidadSinIVA.Value = controlAgregarProducto.NudAhorroEnIVAPorUnidadDespuesDescuento.Value = controlAgregarProducto.NudAhorroTotalPorUnidadConIVA.Value = 0;

            controlAgregarProducto.NudSubtotalDelImporteConIVAIncluido2.Value = controlAgregarProducto.NudSubtotalDelImporteSinIVASinDescuento2.Value = controlAgregarProducto.NudSubtotalDelImporteDelIVASinDescuento2.Value = controlAgregarProducto.NudSubtotalIVADespuesDelDescuento2.Value = controlAgregarProducto.NudSubtotalDelImporteSinIVAConDescuento2.Value = controlAgregarProducto.NudSubtotalDelAhorroSinIvaDespuesDescuento2.Value = controlAgregarProducto.NudSubtotalDelAhorroEnIVADespuesDescuento2.Value = controlAgregarProducto.NudSubtotalDelAhorroTotalDespuesDescuento2.Value = 0;

            controlAgregarProducto.NudTotal2.Value = 0;
        }

        private void DeshabilitarControles()
        {
            controlAgregarProducto.CboCategoria.Enabled = false;
            controlAgregarProducto.CboProducto.Enabled = false;
            controlAgregarProducto.BtnAgregar.Enabled = false;
        }

        private void HabilitarControles()
        {
            controlAgregarProducto.CboCategoria.Enabled = true;
        }

        private void DeshabilitarControlesProducto()
        {
            DeshabilitarCantidadDescuento();
            OcultarIconosValidacion();
            controlAgregarProducto.BtnAgregar.Enabled = false;
            controlAgregarProducto.CboProducto.Enabled = false;
        }

        private void HabilitarControlesProducto() => HabilitarCantidadDescuento();

        private void LlenarCboCategoria()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dtCboCategoria = CategoryService.ObtenerCategoriasCbo(true);
                ComboBoxHelper.LlenarCbo(controlAgregarProducto.CboCategoria, dtCboCategoria, "CategoryName", "CategoryID");
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void LlenarDgvVentas(bool selectorRealizaBusqueda)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DTOs.EF.DtoVentasBuscar criterios;
                if (selectorRealizaBusqueda == true)
                {
                    criterios = new DTOs.EF.DtoVentasBuscar
                    {
                        IdIni = Convert.ToInt32(controlBuscarVenta.NudBIdIni.Value),
                        IdFin = Convert.ToInt32(controlBuscarVenta.NudBIdFin.Value),
                        Cliente = controlBuscarVenta.TxtBCliente.Text.Trim(),

                        FVenta = controlBuscarVenta.DtpFVentaIni.Checked && controlBuscarVenta.DtpFVentaFin.Checked,
                        FVentaIni = controlBuscarVenta.DtpFVentaIni.Checked ? controlBuscarVenta.DtpFVentaIni.Value.Date : (DateTime?)null,
                        FVentaFin = controlBuscarVenta.DtpFVentaFin.Checked ? controlBuscarVenta.DtpFVentaFin.Value.Date.AddDays(1) : (DateTime?)null,
                        FVentaNull = controlBuscarVenta.ChkbFVentaNull.Checked,

                        FRequerido = controlBuscarVenta.DtpFRequeridoIni.Checked && controlBuscarVenta.DtpFRequeridoFin.Checked,
                        FRequeridoIni = controlBuscarVenta.DtpFRequeridoIni.Checked ? controlBuscarVenta.DtpFRequeridoIni.Value.Date : (DateTime?)null,
                        FRequeridoFin = controlBuscarVenta.DtpFRequeridoFin.Checked ? controlBuscarVenta.DtpFRequeridoFin.Value.Date.AddDays(1) : (DateTime?)null,
                        FRequeridoNull = controlBuscarVenta.ChkbFRequeridoNull.Checked,

                        FEnvio = controlBuscarVenta.DtpFEnvioIni.Checked && controlBuscarVenta.DtpFEnvioFin.Checked,
                        FEnvioIni = controlBuscarVenta.DtpFEnvioIni.Checked ? controlBuscarVenta.DtpFEnvioIni.Value.Date : (DateTime?)null,
                        FEnvioFin = controlBuscarVenta.DtpFEnvioFin.Checked ? controlBuscarVenta.DtpFEnvioFin.Value.Date.AddDays(1) : (DateTime?)null,
                        FEnvioNull = controlBuscarVenta.ChkbFEnvioNull.Checked,

                        Empleado = controlBuscarVenta.TxtBEmpleado.Text.Trim(),
                        CompañiaT = controlBuscarVenta.TxtBCompañiaT.Text.Trim(),
                        DirigidoA = controlBuscarVenta.TxtBDirigidoa.Text.Trim()
                    };
                }
                else
                    criterios = null;
                var ventas = OrderBLL.ObtenerVentas(selectorRealizaBusqueda, criterios, false);
                DgvVentas.DataSource = ventas;
                if (!selectorRealizaBusqueda)
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran las últimas {DgvVentas.RowCount} venta(s) registrada(s)");
                else
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {DgvVentas.RowCount} registro(s)");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void ConfDgvVentas()
        {
            DgvVentas.Columns["RowVersionStr"].Visible = false;
            DgvVentas.Columns["OrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            DgvVentas.Columns["OrderDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DgvVentas.Columns["RequiredDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DgvVentas.Columns["ShippedDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DgvVentas.Columns["ShipperCompanyName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvVentas.Columns["EmployeeName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DgvVentas.Columns["OrderDate"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";
            DgvVentas.Columns["RequiredDate"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";
            DgvVentas.Columns["ShippedDate"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";

            DgvVentas.Columns["OrderID"].HeaderText = "Id";
            DgvVentas.Columns["CustomerCompanyName"].HeaderText = "Cliente";
            DgvVentas.Columns["CustomerContactName"].HeaderText = "Nombre de contacto";
            DgvVentas.Columns["OrderDate"].HeaderText = "Fecha de venta";
            DgvVentas.Columns["RequiredDate"].HeaderText = "Fecha de entrega";
            DgvVentas.Columns["ShippedDate"].HeaderText = "Fecha de envío";
            DgvVentas.Columns["EmployeeName"].HeaderText = "Vendedor";
            DgvVentas.Columns["ShipperCompanyName"].HeaderText = "Compañía transportista";
            DgvVentas.Columns["ShipName"].HeaderText = "Enviar a";
        }

        private void ConfDgvDetalle()
        {
            controlDetalleDeLaVenta.DgvDetalle.Columns["Id"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Descuento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Importe"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteDelDescuento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteConDescuento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            controlDetalleDeLaVenta.DgvDetalle.Columns["TasaIVA"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteSinIVA"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteDelIVA"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Subtotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            controlDetalleDeLaVenta.DgvDetalle.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Precio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Cantidad"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Importe"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Descuento"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteDelDescuento"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteConDescuento"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["TasaIVA"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteSinIVA"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteDelIVA"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Subtotal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Modificar"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Eliminar"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            controlDetalleDeLaVenta.DgvDetalle.Columns["Precio"].HeaderText = "Precio\ncon IVA\nincluido";
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteDelDescuento"].HeaderText = "Importe\ndel\ndescuento";
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteConDescuento"].HeaderText = "Importe\ncon\ndescuento";

            controlDetalleDeLaVenta.DgvDetalle.Columns["Precio"].DefaultCellStyle.Format = "c2";
            controlDetalleDeLaVenta.DgvDetalle.Columns["Cantidad"].DefaultCellStyle.Format = "n0";
            controlDetalleDeLaVenta.DgvDetalle.Columns["Descuento"].DefaultCellStyle.Format = "p2";
            controlDetalleDeLaVenta.DgvDetalle.Columns["Importe"].DefaultCellStyle.Format = "c2";
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteDelDescuento"].DefaultCellStyle.Format = "c2";
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteConDescuento"].DefaultCellStyle.Format = "c2";
            controlDetalleDeLaVenta.DgvDetalle.Columns["TasaIVA"].DefaultCellStyle.Format = "p2";
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteSinIVA"].DefaultCellStyle.Format = "c2";
            controlDetalleDeLaVenta.DgvDetalle.Columns["ImporteDelIVA"].DefaultCellStyle.Format = "c2";
            controlDetalleDeLaVenta.DgvDetalle.Columns["Subtotal"].DefaultCellStyle.Format = "c2";
        }

        private void ControlBuscarVenta_LimpiarClick(object sender, EventArgs e)
        {
            BorrarDatosVenta();
            BorrarDatosDetalleVenta();
            BorrarMensajesError();
            BorrarDatosBusqueda();
            DeshabilitarControles();
            BtnNota.Enabled = false;
            LlenarDgvVentas(false);
            CargarValoresOriginales();
            DgvVentas.Focus();
        }

        private void ControlBuscarVenta_BuscarClick(object sender, EventArgs e)
        {
            BorrarDatosVenta();
            BorrarDatosDetalleVenta();
            BorrarMensajesError();
            DeshabilitarControles();
            BtnNota.Enabled = false;
            LlenarDgvVentas(true);
            CargarValoresOriginales();
            DgvVentas.Focus();
        }

        private void BorrarDatosVenta()
        {
            errorProvider1.Clear();
            txtId.Text = txtCliente.Text = "";
            txtId.Tag = null;
            InicializarValoresAgregarProducto();
            InicializarCboProducto();
            controlAgregarProducto.CboCategoria.SelectedIndex = 0;
            controlDetalleDeLaVenta.DgvDetalle.Rows.Clear();
        }

        private void BorrarDatosDetalleVenta()
        {
            controlAgregarProducto.CboCategoria.SelectedIndex = 0;
            InicializarValoresAgregarProducto();
            InicializarCboProducto();
            InicializarNuds();
            controlDetalleDeLaVenta.DgvDetalle.Rows.Clear();
        }

        private void BorrarMensajesError() => errorProvider1.Clear();

        private bool ValidarControles()
        {
            errorProvider1.Clear();
            bool valida = true;
            if (controlAgregarProducto.CboCategoria.SelectedIndex <= 0)
            {
                valida = false;
                errorProvider1.SetError(controlAgregarProducto.CboCategoria, "Seleccione la categoría");
            }
            if (controlAgregarProducto.CboProducto.SelectedIndex <= 0)
            {
                valida = false;
                errorProvider1.SetError(controlAgregarProducto.CboProducto, "Seleccione el producto");
            }
            if (controlAgregarProducto.CboProducto.SelectedIndex > 0)
            {
                int numProd = int.Parse(controlAgregarProducto.CboProducto.SelectedValue.ToString());
                bool productoDuplicado = false;
                foreach (DataGridViewRow dgvr in controlDetalleDeLaVenta.DgvDetalle.Rows)
                {
                    if (int.Parse(dgvr.Cells["ProductoId"].Value.ToString()) == numProd)
                    {
                        productoDuplicado = true;
                        break;
                    }
                }
                if (productoDuplicado)
                {
                    valida = false;
                    errorProvider1.SetError(controlAgregarProducto.CboProducto, "No se puede tener un producto duplicado en el detalle del pedido");
                }
            }
            // necesario crear un objeto temporal para calcular el subtotal con la formulas ya definidas en la clase VentaDetalle
            Order_Detail ventaDetalle = new Order_Detail();
            ventaDetalle.UnitPrice = controlAgregarProducto.NudPrecioConIVAIncluido.Value;
            ventaDetalle.Quantity = (short)controlAgregarProducto.NudCantidad.Value;
            ventaDetalle.Discount = (float)(controlAgregarProducto.NudDescuento.Value / 100m);
            CalcularTotalProducto(ventaDetalle);
            if (ventaDetalle.Subtotal == 0)
            {
                valida = false;
                if (controlAgregarProducto.NudCantidad.Value == 0)
                    errorProvider1.SetError(controlAgregarProducto.BtnAgregar, "Ingrese el detalle del pedido");
                else if (ventaDetalle.Subtotal == 0)
                {
                    errorProvider1.SetError(controlAgregarProducto.BtnAgregar, "El valor del subtotal del producto no puede ser cero");
                    errorProvider1.SetError(controlAgregarProducto.NudTotal2, "El valor del subtotal del producto no puede ser cero");
                }
            }
            InventarioHelper.ActualizarInventarioUi
            (
                controlAgregarProducto.NudCantidad.Value,
                CantidadOld,
                UInventarioOld,
                controlAgregarProducto.NudUInventario
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

            // Valida reglas de negocio con StatusIconHelper
            // Validación restrictiva (cantidad)
            if (!ValidarCantidadEInventarioHelper.ValidarCantidad
                (
                    controlAgregarProducto.NudCantidad.Value,
                    CantidadOld,
                    UInventarioOld,
                    controlAgregarProducto.NudUInventario.Value,
                    controlAgregarProducto.NudCantidad,
                    toolTip1,
                    controlAgregarProducto.PbError,
                    controlAgregarProducto.PbInfo,
                    controlAgregarProducto.PbWarning,
                    errorProvider1
                )
            )
            {
                valida = false;
                controlAgregarProducto.BtnAgregar.Enabled = false;
            }
            else
                controlAgregarProducto.BtnAgregar.Enabled = true;

            return valida;
        }

        private void DgvVentas_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // debe estar vinculado a la clase List<> a la cual esta vinculado el DataGridView.DataSource
            Utils.OrdenarPorColumna<DTOs.EF.DtoVentaDgv>(DgvVentas, e);
        }

        private void BorrarDatosBusqueda()
        {
            controlBuscarVenta.NudBIdIni.Value = 0;
            controlBuscarVenta.NudBIdFin.Value = 0;

            controlBuscarVenta.TxtBCliente.Text = "";
            controlBuscarVenta.TxtBEmpleado.Text = "";
            controlBuscarVenta.TxtBCompañiaT.Text = "";
            controlBuscarVenta.TxtBDirigidoa.Text = "";

            controlBuscarVenta.DtpFVentaIni.Value = DateTime.Today;
            controlBuscarVenta.DtpFVentaFin.Value = DateTime.Today;
            controlBuscarVenta.DtpFVentaIni.Checked = false;
            controlBuscarVenta.DtpFVentaFin.Checked = false;
            controlBuscarVenta.ChkbFVentaNull.Checked = false;

            controlBuscarVenta.DtpFRequeridoIni.Value = DateTime.Today;
            controlBuscarVenta.DtpFRequeridoFin.Value = DateTime.Today;
            controlBuscarVenta.DtpFRequeridoIni.Checked = false;
            controlBuscarVenta.DtpFRequeridoFin.Checked = false;
            controlBuscarVenta.ChkbFRequeridoNull.Checked = false;

            controlBuscarVenta.DtpFEnvioIni.Value = DateTime.Today;
            controlBuscarVenta.DtpFEnvioFin.Value = DateTime.Today;
            controlBuscarVenta.DtpFEnvioIni.Checked = false;
            controlBuscarVenta.DtpFEnvioFin.Checked = false;
            controlBuscarVenta.ChkbFEnvioNull.Checked = false;
        }

        #region eventosDeControles

        private void NudEnterHandler(object sender, EventArgs e)
        {
            if (sender is NumericUpDown nud && nud.Controls[1] is TextBox tb)
            {
                // Diferir la selección para que ocurra después de que el TextBox reciba el foco
                tb.BeginInvoke((Action)(() => tb.SelectAll()));
            }
        }

        private void NudCantidadDescuento_LeaveValueChangedHandler(object sender, EventArgs e) => ValidarControles();

        #endregion

        private void InicializarValoresAgregarProducto() => controlAgregarProducto.NudPrecioConIVAIncluido.Value = controlAgregarProducto.NudCantidad.Value = controlAgregarProducto.NudUInventario.Value = controlAgregarProducto.NudDescuento.Value = 0;

        private void CboCategoria_SelectedIndexChangedHandler(object sender, EventArgs e)
        {
            InicializarValoresAgregarProducto();
            InicializarNudsProducto();
            BorrarMensajesError();
            if (controlAgregarProducto.CboCategoria.SelectedIndex > 0)
            {
                try
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    var dtCboProductos = ProductService.ObtenerProductosPorCategoriaCbo(int.Parse(controlAgregarProducto.CboCategoria.SelectedValue.ToString()));
                    controlAgregarProducto.CboProducto.DataSource = dtCboProductos;
                    controlAgregarProducto.CboProducto.DisplayMember = "ProductName";
                    controlAgregarProducto.CboProducto.ValueMember = "ProductID";
                    controlAgregarProducto.CboProducto.Enabled = true;
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {DgvVentas.RowCount} registro(s) en ventas");
                }
                catch (Exception ex)
                {
                    U.MsgCatchOue(ex);
                }
            }
            else
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                InicializarCboProducto();
                MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {DgvVentas.RowCount} registro(s) en ventas");
            }
        }

        private void CboProducto_SelectedIndexChangedHandler(object sender, EventArgs e)
        {
            BorrarMensajesError();
            if (controlAgregarProducto.CboProducto.SelectedIndex > 0)
            {
                try
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    var productId = controlAgregarProducto.CboProducto.SelectedValue?.ToString();
                    InicializarValoresAgregarProducto();
                    InicializarNudsProducto();
                    var dtoProductoCostoInventario = ProductService.ObtenerProductoCostoEInventario(int.Parse(productId));
                    if (dtoProductoCostoInventario != null)
                    {
                        controlAgregarProducto.NudPrecioConIVAIncluido.Value = dtoProductoCostoInventario.UnitPrice;
                        controlAgregarProducto.NudUInventario.Value = dtoProductoCostoInventario.UnitsInStock;
                        UInventarioOld = short.Parse(dtoProductoCostoInventario.UnitsInStock.ToString());
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
                            controlAgregarProducto.NudUInventario.Value,
                            controlAgregarProducto.NudCantidad,
                            toolTip1,
                            controlAgregarProducto.PbError,
                            controlAgregarProducto.PbInfo,
                            controlAgregarProducto.PbWarning,
                            errorProvider1
                        );
                        if (dtoProductoCostoInventario.UnitsInStock == 0)
                        {
                            DeshabilitarControlesProducto();
                            U.NotificacionWarning("No hay este producto en existencia.");
                            controlAgregarProducto.CboProducto.SelectedIndex = 0;
                            InicializarValoresAgregarProducto();
                        }
                        else
                            HabilitarControlesProducto();
                    }
                    else
                    {
                        DeshabilitarControlesProducto();
                        InicializarValoresAgregarProducto();
                        InicializarCboProducto();
                    }
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {DgvVentas.RowCount} registro(s) en ventas");
                }
                catch (Exception ex)
                {
                    U.MsgCatchOue(ex);
                }
            }
            else
            {
                DeshabilitarControlesProducto();
                InicializarValoresAgregarProducto();
            }
        }

        private void DgvVentas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            BtnNota.Enabled = false;
            BorrarDatosVenta();
            BorrarDatosDetalleVenta();
            BorrarMensajesError();
            DataGridViewRow dgvr = DgvVentas.CurrentRow;
            txtId.Text = dgvr.Cells["OrderId"].Value.ToString();
            txtCliente.Text = dgvr.Cells["CustomerCompanyName"].Value.ToString();
            txtId.Tag = dgvr.Cells["RowVersionStr"].Value;
            int orderId = string.IsNullOrEmpty(txtId.Text) ? 0 : Convert.ToInt32(txtId.Text);
            LlenarDatosVenta(ref orderId);
            LlenarDatosDetalleVenta(orderId);
            if (orderId != 0)
                HabilitarControles();
            else
            {
                DeshabilitarControles();
                BorrarDatosVenta();
            }
            CargarValoresOriginales();
            controlDetalleDeLaVenta.DgvDetalle.Focus();
        }

        private void LlenarDatosVenta(ref int orderId)
        {
            if (orderId == 0) return;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var venta = OrderService.ObtenerVentaPorId(orderId);
                if (venta != null)
                {
                    txtId.Text = venta.OrderID.ToString();
                    txtCliente.Text = venta.Customer.CompanyName;
                    txtId.Tag = venta.RowVersionStr;
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {DgvVentas.RowCount} registro(s) en ventas");
                }
                else
                {
                    txtId.Text = string.Empty;
                    txtId.Tag = null;
                    orderId = 0;
                    U.NotificacionWarning("[orange]No se encontró la venta especificada." + Utils.erfep);
                }
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void LlenarDatosDetalleVenta(int orderId)
        {
            if (orderId == 0) return;
            try
            {
                numDetalle = 1;
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var detalles = Order_DetailService.ObtenerVentaDetallePorVentaId(orderId);
                if (detalles.Count == 0)
                {
                    controlDetalleDeLaVenta.DgvDetalle.Columns["Modificar"].Visible = false;
                    controlDetalleDeLaVenta.DgvDetalle.Columns["Eliminar"].Visible = false;
                    U.NotificacionWarning("No se encontraron detalles para la venta especificada");
                }
                else
                {
                    controlDetalleDeLaVenta.DgvDetalle.Columns["Modificar"].Visible = true;
                    controlDetalleDeLaVenta.DgvDetalle.Columns["Eliminar"].Visible = true;
                    foreach (var ventaDetalle in detalles)
                    {
                        controlDetalleDeLaVenta.DgvDetalle.Rows.Add(new object[]
                        {
                            numDetalle,
                            ventaDetalle.Product.ProductName,
                            ventaDetalle.UnitPrice,
                            ventaDetalle.Quantity,
                            ventaDetalle.SubtotalDelImporteConIVAIncluido,
                            ventaDetalle.Discount,
                            ventaDetalle.SubtotalDelAhorroTotalDespuesDescuento,
                            ventaDetalle.SubtotalDelImporteConIVAConDescuento,
                            ventaDetalle.TasaIVA,
                            ventaDetalle.SubtotalDelImporteSinIVAConDescuento,
                            ventaDetalle.SubtotalIVADespuesDelDescuento,
                            ventaDetalle.Subtotal,
                            "  Modificar  ",
                            "  Eliminar  ",
                            ventaDetalle.Product.ProductID,
                            ventaDetalle.RowVersion
                        });
                        ++numDetalle;
                    }
                }
                CalcularTotales();
                MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {DgvVentas.RowCount} registro(s) en ventas");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void CalcularTotalProducto(Order_Detail ventaDetalle)
        {
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

        private void CalcularTotales()
        {
            decimal importe, total, totalDeUnidades, subtotalDelImporte, subtotalDelImporteDelDescuento, subtotalDelImporteConDescuento, subtotalDelImporteSinIVA, subtotalDelImporteDelIVA;
            importe = total = totalDeUnidades = subtotalDelImporte = subtotalDelImporteDelDescuento = subtotalDelImporteConDescuento = subtotalDelImporteSinIVA = subtotalDelImporteDelIVA = 0;
            numDetalle = 0;
            foreach (DataGridViewRow dgvr in controlDetalleDeLaVenta.DgvDetalle.Rows)
            {
                totalDeUnidades += decimal.Parse(dgvr.Cells["Cantidad"].Value.ToString());
                subtotalDelImporte += decimal.Parse(dgvr.Cells["Importe"].Value.ToString());
                subtotalDelImporteDelDescuento += decimal.Parse(dgvr.Cells["ImporteDelDescuento"].Value.ToString());
                subtotalDelImporteConDescuento += decimal.Parse(dgvr.Cells["ImporteConDescuento"].Value.ToString());
                subtotalDelImporteSinIVA += decimal.Parse(dgvr.Cells["ImporteSinIVA"].Value.ToString());
                subtotalDelImporteDelIVA += decimal.Parse(dgvr.Cells["ImporteDelIVA"].Value.ToString());
                total += decimal.Parse(dgvr.Cells["Subtotal"].Value.ToString());
                dgvr.Cells["Id"].Value = ++numDetalle;
            }
            controlTotalesDeLaVenta.NudNumProd.Value = numDetalle;
            controlTotalesDeLaVenta.NudTotalDeUnidades.Value = totalDeUnidades;
            controlTotalesDeLaVenta.NudSubtotalDelImporte.Value = subtotalDelImporte;
            controlTotalesDeLaVenta.NudSubtotalDelImporteDelDescuento.Value = subtotalDelImporteDelDescuento;
            controlTotalesDeLaVenta.NudSubtotalDelImporteConDescuento.Value = subtotalDelImporteConDescuento;
            controlTotalesDeLaVenta.NudSubtotalDelImporteSinIVA.Value = subtotalDelImporteSinIVA;
            controlTotalesDeLaVenta.NudSubtotalDelImporteDelIVA.Value = subtotalDelImporteDelIVA;
            controlTotalesDeLaVenta.NudTotal.Value = total;
        }

        private void BtnAgregar_ClickHandler(object sender, EventArgs e)
        {
            int numRegs = 0;
            BorrarMensajesError();
            if (!ValidarControles())
            {
                grbVentas.Focus();
                return;
            }
            else
            {
                controlAgregarProducto.BtnAgregar.Enabled = false;
                try
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
                    DeshabilitarControles();
                    DeshabilitarControlesProducto();
                    Order_Detail ventaDetalle = new Order_Detail();
                    ventaDetalle.Order.OrderID = int.Parse(txtId.Text);
                    ventaDetalle.Order.RowVersion = (txtId.Tag != null && long.TryParse(txtId.Tag.ToString(), out long tagVal))
                                                    ? BitConverter.GetBytes(tagVal)
                                                    : null; // para evitar excepcion devuelve null si el valor no es convertible a long
                    ventaDetalle.Product.ProductID = int.Parse(controlAgregarProducto.CboProducto.SelectedValue.ToString());
                    ventaDetalle.UnitPrice = controlAgregarProducto.NudPrecioConIVAIncluido.Value;
                    ventaDetalle.Quantity = Convert.ToInt16(controlAgregarProducto.NudCantidad.Value);
                    ventaDetalle.Discount = (float)(controlAgregarProducto.NudDescuento.Value / 100m);
                    ventaDetalle.Product.ProductName = controlAgregarProducto.CboProducto.Text;
                    numRegs = Order_DetailBLL.Insertar(ventaDetalle);
                    // Recuperar el nuevo RowVersion y asignarlo al Tag
                    if (ventaDetalle.Order.RowVersion != null)
                    {
                        // Convierte el byte[] a long para almacenarlo en Tag
                        txtId.Tag = ventaDetalle.Order.RowVersionStr;
                    }
                    string strProductoVenta = $"El producto: {ventaDetalle.ProductName} - Venta: {ventaDetalle.Order.OrderID}:";
                    string strVenta = $"La venta con Id: {ventaDetalle.Order.OrderID}:";
                    if (numRegs > 0)
                    {
                        controlAgregarProducto.NudCantidadDescuento_LeaveValueChanged -= NudCantidadDescuento_LeaveValueChangedHandler;
                        int orderId = string.IsNullOrEmpty(txtId.Text) ? 0 : Convert.ToInt32(txtId.Text);
                        BorrarDatosVenta();
                        BorrarDatosDetalleVenta();
                        LlenarDatosVenta(ref orderId); // necesario para actualizar el RowVersion de la venta
                        LlenarDatosDetalleVenta(orderId);
                        BtnNota.Enabled = true;
                        CargarValoresOriginales();
                        HabilitarControles();
                        controlAgregarProducto.CboCategoria.Focus();
                        controlAgregarProducto.NudCantidadDescuento_LeaveValueChanged += NudCantidadDescuento_LeaveValueChangedHandler;
                    }
                    else
                    {
                        if (numRegs == -1)
                            U.NotificacionError(strProductoVenta + Utils.nfrfa);
                        else if (numRegs == -3)
                            U.NotificacionError(strVenta + Utils.fepou);
                        else if (numRegs == -4)
                            U.NotificacionError(strProductoVenta + "\n[red]No fue registrado en la base de datos.\n" + strVenta + Utils.fmpou);
                        else if (numRegs == -5)
                            U.NotificacionError(strProductoVenta + Utils.nfrs);
                        else if (numRegs == -6)
                            U.NotificacionError(strProductoVenta + Utils.nfrii); // Stock insuficiente
                        else if (numRegs == -7)
                            U.NotificacionError(strProductoVenta + Utils.nfrie); // Stock excedió el máximo permitido. Este caso nunca debería ocurrir porque un alta solo descuenta del inventario, nunca lo aumenta. 
                        else if (numRegs == -8)
                            U.NotificacionError(strProductoVenta + Utils.nfrin); // stock negativo. Este caso nunca debería ocurrir porque para que suceda se necesitaria tener un valor negativo en el inventario y eso nunca sucede porque el sistema ya tiene validaciones que no lo permiten.
                        else if (numRegs < -8) // Este caso aun no está definido, por lo tanto es un error desconocido
                            U.NotificacionError(strProductoVenta + Utils.nfrs); // motivo desconocido
                        if (numRegs <= 0)
                        {
                            if (numRegs == -6)
                            {
                                InicializarValoresAgregarProducto();
                                controlAgregarProducto.CboCategoria.SelectedIndex = 0;
                                controlAgregarProducto.CboCategoria.Enabled = true;
                            }
                            else 
                            {
                                DeshabilitarControles();
                                BorrarDatosDetalleVenta();
                                if (numRegs != -3 && numRegs != -4 && numRegs != -5 && numRegs < -8)
                                {
                                    LlenarDatosDetalleVenta(int.Parse(txtId.Text));
                                    controlAgregarProducto.CboCategoria.Enabled = true;
                                }
                                if (numRegs == -3 || numRegs == -4 || numRegs == -5 || numRegs < -8)
                                {
                                    BorrarDatosVenta();
                                    LlenarDgvVentas(false);
                                }
                            }
                            CargarValoresOriginales();
                        }
                    }
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {DgvVentas.RowCount} registro(s) en ventas");
                }
                catch (Exception ex)
                {
                    U.MsgCatchOue(ex);
                }
            }
        }

        private void BorrarDatosAgregarProducto()
        {
            controlAgregarProducto.CboCategoria.SelectedIndex = 0;
            InicializarCboProducto();
            InicializarValoresAgregarProducto();
        }

        private void DgvDetalle_CellClickHandler(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var dgv = controlDetalleDeLaVenta.DgvDetalle; // acceso al DataGridView interno
            try
            {

                if (e.ColumnIndex == dgv.Columns["Eliminar"].Index)
                {
                    DataGridViewRow dgvr = dgv.CurrentRow;
                    Order_Detail ventaDetalle = new Order_Detail();
                    ventaDetalle.Order.OrderID = int.Parse(txtId.Text);
                    ventaDetalle.Product.ProductID = (int)dgvr.Cells["ProductoId"].Value;
                    ventaDetalle.Product.ProductName = dgvr.Cells["Producto"].Value.ToString();
                    object cellValue = dgvr.Cells["RowVersion"].Value;
                    ventaDetalle.RowVersion = (cellValue == null || cellValue == DBNull.Value) ? null : (byte[])cellValue;
                    if (txtId.Tag != null && long.TryParse(txtId.Tag.ToString(), out long valor)) // para evitar excepcion devuelve null si el valor no es convertible a long
                    {
                        ventaDetalle.Order.RowVersion = BitConverter.GetBytes(valor);
                    }
                    else
                    {
                        ventaDetalle.Order.RowVersion = null; // o manejar el error según tu lógica
                    }
                    EliminarProducto(ventaDetalle);
                }
                if (e.ColumnIndex == dgv.Columns["Modificar"].Index)
                {
                    DataGridViewRow dgvr = dgv.CurrentRow;
                    using (FrmVentasDetalleModificar frmVentasDetalleModificar = new FrmVentasDetalleModificar())
                    {
                        Order_Detail ventaDetalle = new Order_Detail()
                        {
                            Order = new Order()
                            {
                                OrderID = int.Parse(txtId.Text),
                                RowVersion = (txtId.Tag != null && long.TryParse(txtId.Tag.ToString(), out long tagVal))
                                                ? BitConverter.GetBytes(tagVal)
                                                : null // para evitar excepcion devuelve null si el valor no es convertible a long
                            },
                            Product = new Product()
                            {
                                ProductID = (int)dgvr.Cells["ProductoId"].Value,
                                ProductName = dgvr.Cells["Producto"].Value.ToString()
                            },
                            UnitPrice = decimal.Parse(dgvr.Cells["Precio"].Value.ToString()),
                            Quantity = short.Parse(dgvr.Cells["Cantidad"].Value.ToString()),
                            Discount = float.Parse(dgvr.Cells["Descuento"].Value.ToString()),
                            RowVersion = dgvr.Cells["RowVersion"].Value as byte[] // devuelve null si es DBNull o no es byte[]
                        };
                        frmVentasDetalleModificar.ventaDetalle = ventaDetalle;
                        DialogResult dialogResult = frmVentasDetalleModificar.ShowDialog();
                        int orderId = string.IsNullOrEmpty(txtId.Text) ? 0 : Convert.ToInt32(txtId.Text);
                        BorrarDatosVenta();
                        BorrarDatosDetalleVenta();
                        if (dialogResult == DialogResult.OK)
                        {
                            BtnNota.Enabled = true;
                            LlenarDatosVenta(ref orderId); // necesario para actualizar el RowVersion de la venta
                            LlenarDatosDetalleVenta(orderId);
                            CargarValoresOriginales();
                        }
                        else
                        {
                            BtnNota.Enabled = false;
                            DeshabilitarControles();
                            LlenarDgvVentas(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {DgvVentas.RowCount} registro(s) en ventas");
            dgv.Focus();
        }

        private void EliminarProducto(Order_Detail ventaDetalle)
        {
            int numRegs = 0;
            BorrarMensajesError();
            BorrarDatosAgregarProducto();
            try
            {
                if (U.NotificacionQuestion($"[orange]¿Esta seguro de eliminar el producto: {ventaDetalle.ProductName} de la venta: {ventaDetalle.Order.OrderID}?") == DialogResult.Yes)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
                    DeshabilitarControles();
                    DeshabilitarControlesProducto();
                    numRegs = Order_DetailBLL.Eliminar(ventaDetalle);
                    string strProductoVenta = $"El producto: {ventaDetalle.ProductName} - Venta: {ventaDetalle.Order.OrderID}:";
                    string strVenta = $"La venta con Id: {ventaDetalle.Order.OrderID}:";
                    if (numRegs > 0 || numRegs == -1 || numRegs == -2 || numRegs == -4)
                    {
                        int orderId = string.IsNullOrEmpty(txtId.Text) ? 0 : Convert.ToInt32(txtId.Text);
                        BorrarDatosVenta();
                        BorrarDatosDetalleVenta();
                        LlenarDatosVenta(ref orderId);
                        LlenarDatosDetalleVenta(orderId);
                        CargarValoresOriginales();
                        BtnNota.Enabled = true;
                        MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {DgvVentas.RowCount} registro(s) en ventas");
                    }
                    if (numRegs == -1)
                        U.NotificacionError(strProductoVenta + Utils.nfefe);
                    if (numRegs == -2)
                        U.NotificacionError(strProductoVenta + Utils.nfefm);
                    if (numRegs == -3)
                        U.NotificacionError(strVenta + Utils.fepou);
                    else if (numRegs == -4)
                        U.NotificacionError(strProductoVenta + "\n[red]No fue eliminado en la base de datos.\n" + strVenta + Utils.fmpou);
                    if (numRegs == -5)
                        U.NotificacionError(strProductoVenta + Utils.nfecqn); // El campo Quantity del detalle de la venta es nulo, no se da este caso porque la base de datos no lo permite
                    // el caso -6 no existe en el stored procedure 
                    if (numRegs == -7)
                        U.NotificacionError(strProductoVenta + Utils.nfeie); // Stock excedió el máximo permitido
                    if (numRegs == -8)
                        U.NotificacionError(strProductoVenta + Utils.nfein); // stock negativo, este caso nunca ocurre porque la base de datos no lo permite con un check constraint
                    if (numRegs < -9)
                        U.NotificacionError(strProductoVenta + Utils.nfemd);
                    if (numRegs == -3)
                    {
                        BorrarDatosVenta();
                        BorrarDatosDetalleVenta();
                        DeshabilitarControles();
                        LlenarDgvVentas(false);
                        CargarValoresOriginales();
                    }
                    if (numRegs != -3)
                        HabilitarControles();
                }
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void BtnNota_Click(object sender, EventArgs e)
        {
            int result = ChkRowVersion();
            string strVenta = $"La venta con Id: {txtId.Text}:";
            if (result == -1)
                U.NotificacionError(strVenta + Utils.oevvd);
            else if (result == -2)
                U.NotificacionError(strVenta + Utils.fepou);
            else if (result == -3)
                U.NotificacionError(strVenta + Utils.fmpousmn);
            else if (result == -4)
                U.NotificacionError(strVenta + Utils.oed);
            if (result == 1 || result == -3)
            {
                FrmRptNotaRemision8 frmRptNotaRemision8 = new FrmRptNotaRemision8();
                frmRptNotaRemision8.Id = int.Parse(txtId.Text);
                frmRptNotaRemision8.ShowDialog();
            }
            if (result == -2)
            {
                controlAgregarProducto.NudCantidadDescuento_LeaveValueChanged -= NudCantidadDescuento_LeaveValueChangedHandler;
                DeshabilitarControles();
                BtnNota.Enabled = false;
                BorrarDatosVenta();
                BorrarDatosDetalleVenta();
                LlenarDgvVentas(false);
                CargarValoresOriginales();
                label1.Focus();
                controlAgregarProducto.NudCantidadDescuento_LeaveValueChanged += NudCantidadDescuento_LeaveValueChangedHandler;
            }
            return;
        }

        private void OcultarIconosValidacion()
        {
            StatusIconHelper.HideIcons(controlAgregarProducto.PbError, controlAgregarProducto.PbInfo, controlAgregarProducto.PbWarning);
            StatusIconHelper.HideIcons(controlAgregarProducto.PbError1, controlAgregarProducto.PbInfo1, controlAgregarProducto.PbWarning1);
        }

        private int ChkRowVersion()
        {
            if (txtId.Tag == null)
                return -1;
            byte[] rowVersion = RowVersionHelper.RowVersionObjToByteArray(txtId.Tag);
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                Order venta = OrderService.ObtenerVentaPorId(int.Parse(txtId.Text));
                if (venta == null)
                    return -2;
                // no se necesita checar los rowversions de los detalles de la venta porque si un detalle cambia o es eliminado o es insertado uno nuevo, el rowversion de la venta también cambia, es suficiente con checar el rowversion de la venta
                if (!venta.RowVersion.SequenceEqual(rowVersion))
                    return -3;
                MDIPrincipal.ActualizarBarraDeEstado();
                return 1;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
                return -4;
            }
        }
    }
}
