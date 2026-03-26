using BLL;
using BLL.Services;
using Entities;
using Entities.DTOs;
using NorthwindTradersV6EF.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Utilities;

// es importante que la programacion de los datetimepickers se haga justo como esta especificado para que funcionen las validaciones lo resolvi con la ayuda de IA chatgpt

namespace NorthwindTradersV6EF
{
    public partial class FrmVentasCrud : Form
    {
        string _connectionString = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private VentaBLL _ventaBLL;
        private VentaDetalleBLL _ventaDetalleBLL;
        private ClienteService _clienteService;
        private EmpleadoService _empleadoService;
        private TransportistaService _transportistaService;
        private CategoriaService _categoriaService;
        private ProductoService _productoService;
        private VentaService _ventaService;
        internal Dictionary<string, object> valoresOriginales;
        bool EventoCargado = true; // esta variable es necesaria para controlar el manejador de eventos de la celda del dgv ojo no quitar
        int numDetalle = 1;
        bool VentaGenerada = false;
        private short CantidadOld = 0;
        private short UInventarioOld = 0;
        private readonly DateTime FechaBaseMinDate = new DateTime(1753, 1, 1, 0, 0, 0);

        public FrmVentasCrud()
        {
            InitializeComponent();
            headerOperacion.TabControl = tabcOperacion;
            headerOperacion.IconOn = Properties.Resources.pestanaOn;
            headerOperacion.IconOff = Properties.Resources.pestanaOff;
            headerOperacion.Build();
            this.Load += FrmVentasCrud_Load;
            this.FormClosed += FrmVentasCrud_FormClosed;
            this.FormClosing += FrmVentasCrud_FormClosing;
            tabcOperacion.Selected += tabcOperacion_Selected;
            tabcOperacion.Selecting += tabcOperacion_Selecting;
            grbVentas.Paint += GrbPaint;
            grbVenta.Paint += GrbPaint;
            grbTransportista.Paint += GrbPaint2;
            GrbOperaciones.Paint += GrbPaint;

            // Hacer que se pinten en negro los groupboxes de los controles anidados
            foreach (var gb in controlBuscarVenta.Controls.OfType<GroupBox>())
                gb.Paint += GrbPaint;
            foreach (var gb in controlTotalesDeLaVenta.Controls.OfType<GroupBox>())
                gb.Paint += GrbPaint;
            foreach (var gb in controlDetalleDeLaVenta.Controls.OfType<GroupBox>())
                gb.Paint += GrbPaint;
            // los groupboxes de controlAgregarProducto se pintaran directamente desde el control... porque se pintan de dos distintas maneras

            dgvVentas.ColumnHeaderMouseClick += dgvVentas_ColumnHeaderMouseClick;
            dgvVentas.CellClick += dgvVentas_CellClick;

            HabilitarEventosFechas();

            // Suscripción al evento del UserControl
            controlBuscarVenta.LimpiarClick += ControlBuscarVenta_LimpiarClick;
            controlBuscarVenta.BuscarClick += ControlBuscarVenta_BuscarClick;

            nudFlete.Enter += NudEnterHandler;

            controlAgregarProducto.NudEnter += NudEnterHandler;
            controlAgregarProducto.NudCantidadDescuento_LeaveValueChanged += NudCantidadDescuento_LeaveValueChangedHandler;
            controlAgregarProducto.CboCategoria_SelectedIndexChanged += CboCategoria_SelectedIndexChangedHandler;
            controlAgregarProducto.CboProducto_SelectedIndexChanged += CboProducto_SelectedIndexChangedHandler;
            controlAgregarProducto.BtnAgregar_Click += BtnAgregar_ClickHandler;

            cboCliente.SelectedIndexChanged += cboCliente_SelectedIndexChanged;

            btnNuevo.Click += btnNuevo_Click;
            btnNota.Click += btnNota_Click;
            btnGenerar.Click += btnGenerar_Click;

            controlDetalleDeLaVenta.DgvDetalle_CellClick += DgvDetalle_CellClickHandler;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void GrbPaint2(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmVentasCrud_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        internal void FrmVentasCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrar) == DialogResult.No)
                    e.Cancel = true;
        }

        private void FrmVentasCrud_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            _ventaBLL = new VentaBLL(_connectionString);
            _ventaDetalleBLL = new VentaDetalleBLL(_connectionString);
            _clienteService = new ClienteService(_connectionString);
            _empleadoService = new EmpleadoService(_connectionString);
            _transportistaService = new TransportistaService(_connectionString);
            _categoriaService = new CategoriaService(_connectionString);
            _productoService = new ProductoService(_connectionString);
            _ventaService = new VentaService(_connectionString);

            tabcOperacion.Appearance = TabAppearance.Normal;
            tabcOperacion.ItemSize = new Size(0, 1);
            tabcOperacion.SizeMode = TabSizeMode.Fixed;

            InicializarDtps();

            DeshabilitarNudsNoSeleccionables();
            DeshabilitarTodosControles();
            LlenarCboCliente();
            LlenarCboEmpleado();
            LlenarCboTransportista();
            LlenarCboCategoria();
            Utils.ConfDgv(dgvVentas);
            Utils.ConfDgv(controlDetalleDeLaVenta.DgvDetalle);
            LlenarDgvVentas(false);
            ConfDgvVentas();
            ConfDgvDetalle();
            OcultarCols();
            InicializarCboProducto();
            CargarValoresOriginales();
            OcultarControlAgregarProducto();
            grbVenta.Text = "»   Consulta de ventas:   «";
            controlDetalleDeLaVenta.DgvDetalle.AutoGenerateColumns = false;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Modificar"].Visible = false; // No se va a ocupar en este formulario, se quedará para que no cause errores
        }

        private void InicializarDtps()
        {
            DeshabilitarEventosFechas();
            // 1. PRIMERO: Liberamos las restricciones (MinDate) para que acepten cualquier fecha
            dtpVenta.MinDate = dtpRequerido.MinDate = dtpEnvio.MinDate = FechaBaseMinDate;

            // 2. SEGUNDO: Quitamos los checks (IMPORTANTE: hacerlo antes de asignar el valor)
            dtpVenta.Checked = dtpRequerido.Checked = dtpEnvio.Checked = false;

            // 3. TERCERO: Asignamos los valores mínimos
            // Ahora no dará error porque el MinDate ya es 1753
            dtpVenta.Value = dtpRequerido.Value = dtpEnvio.Value = FechaBaseMinDate;

            // 4. Resetear las horas a la medianoche de hoy
            dtpHoraVenta.Value = dtpHoraRequerido.Value = dtpHoraEnvio.Value = DateTime.Today;
            HabilitarEventosFechas();
        }

        private void OcultarCols() => controlDetalleDeLaVenta.DgvDetalle.Columns["Eliminar"].Visible = false;

        private void MostrarCols() => controlDetalleDeLaVenta.DgvDetalle.Columns["Eliminar"].Visible = true;

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
            DeshabilitarFlete();
        }

        private void DeshabilitarCantidadDescuento()
        {
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudCantidad, false);
            Utilities.NudHelper.SetEnabled(controlAgregarProducto.NudDescuento, false);
        }

        private void DeshabilitarFlete()
        {
            Utilities.NudHelper.SetEnabled(nudFlete, false);
        }

        private void HabilitarFlete()
        {
            Utilities.NudHelper.SetEnabled(nudFlete, true);
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

        private void DeshabilitarControles()
        {
            cboCliente.Enabled = cboEmpleado.Enabled = cboTransportista.Enabled = false;
            txtDirigidoa.ReadOnly = txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCP.ReadOnly = txtPais.ReadOnly = true;

            dtpVenta.Enabled = dtpHoraVenta.Enabled = dtpRequerido.Enabled = dtpHoraRequerido.Enabled = dtpEnvio.Enabled = dtpHoraEnvio.Enabled = false;

            btnGenerar.Enabled = btnNota.Enabled = false;

            controlAgregarProducto.CboCategoria.Enabled = controlAgregarProducto.CboProducto.Enabled = controlAgregarProducto.BtnAgregar.Enabled = false;

            DeshabilitarCantidadDescuento();
            DeshabilitarFlete();
        }

        private void HabilitarControles()
        {
            cboCliente.Enabled = cboEmpleado.Enabled = cboTransportista.Enabled = true;
            txtDirigidoa.ReadOnly = txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCP.ReadOnly = txtPais.ReadOnly = false;

            dtpVenta.Enabled = true;
            dtpRequerido.Enabled = dtpEnvio.Enabled = true;

            controlAgregarProducto.CboProducto.Enabled = controlAgregarProducto.BtnAgregar.Enabled = false;
            controlAgregarProducto.CboCategoria.Enabled = true;

            HabilitarFlete();
            btnGenerar.Enabled = true;
        }

        private void DeshabilitarControlesProducto()
        {
            DeshabilitarCantidadDescuento();
            OcultarIconosValidacion();
            controlAgregarProducto.BtnAgregar.Enabled = false;
            controlAgregarProducto.CboProducto.Enabled = false;
        }

        private void DeshabilitarTodosControles()
        {
            DeshabilitarControles();
            DeshabilitarControlesProducto();
        }

        private void OcultarIconosValidacion()
        {
            StatusIconHelper.HideIcons(controlAgregarProducto.PbError, controlAgregarProducto.PbInfo, controlAgregarProducto.PbWarning);
            StatusIconHelper.HideIcons(controlAgregarProducto.PbError1, controlAgregarProducto.PbInfo1, controlAgregarProducto.PbWarning1);
        }

        private void HabilitarControlesProducto() => HabilitarCantidadDescuento();

        private void LlenarCboCliente()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dtCboCliente = _clienteService.ObtenerClientesCbo();
                ComboBoxHelper.LlenarCbo(cboCliente, dtCboCliente, "CompanyName", "CustomerID");
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void LlenarCboEmpleado()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dtCboEmpleado = _empleadoService.ObtenerEmpleadosCbo();
                ComboBoxHelper.LlenarCbo(cboEmpleado, dtCboEmpleado, "EmployeeName", "EmployeeID");
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void LlenarCboTransportista()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dtCboTransportista = _transportistaService.ObtenerTransportistasCbo();
                ComboBoxHelper.LlenarCbo(cboTransportista, dtCboTransportista, "CompanyName", "ShipperID");
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void LlenarCboCategoria()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dtCboCategoria = _categoriaService.ObtenerCategoriasCbo();
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
                DtoVentasBuscar criterios;
                if (selectorRealizaBusqueda == true)
                {
                    criterios = new DtoVentasBuscar
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
                var ventas = _ventaBLL.ObtenerVentas(selectorRealizaBusqueda, criterios, false);
                dgvVentas.DataSource = ventas;
                if (!selectorRealizaBusqueda)
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran las últimas {dgvVentas.RowCount} venta(s) registrada(s)");
                else
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {dgvVentas.RowCount} registro(s)");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void ConfDgvVentas()
        {
            dgvVentas.Columns["RowVersionStr"].Visible = false;
            dgvVentas.Columns["OrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvVentas.Columns["OrderDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvVentas.Columns["RequiredDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvVentas.Columns["ShippedDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvVentas.Columns["ShipperCompanyName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvVentas.Columns["EmployeeName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvVentas.Columns["OrderDate"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";
            dgvVentas.Columns["RequiredDate"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";
            dgvVentas.Columns["ShippedDate"].DefaultCellStyle.Format = "ddd dd\" de \"MMM\" de \"yyyy\n hh:mm:ss tt";

            dgvVentas.Columns["OrderID"].HeaderText = "Id";
            dgvVentas.Columns["CustomerCompanyName"].HeaderText = "Cliente";
            dgvVentas.Columns["CustomerContactName"].HeaderText = "Nombre de contacto";
            dgvVentas.Columns["OrderDate"].HeaderText = "Fecha de venta";
            dgvVentas.Columns["RequiredDate"].HeaderText = "Fecha de entrega";
            dgvVentas.Columns["ShippedDate"].HeaderText = "Fecha de envío";
            dgvVentas.Columns["EmployeeName"].HeaderText = "Vendedor";
            dgvVentas.Columns["ShipperCompanyName"].HeaderText = "Compañía transportista";
            dgvVentas.Columns["ShipName"].HeaderText = "Enviar a";
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
            controlDetalleDeLaVenta.DgvDetalle.Columns["Modificar"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            controlDetalleDeLaVenta.DgvDetalle.Columns["Eliminar"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

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

        private void dgvVentas_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // debe estar vinculado a la clase List<> a la cual esta vinculado el DataGridView.DataSource
            Utils.OrdenarPorColumna<DtoVentaDgv>(dgvVentas, e);
        }

        private void ControlBuscarVenta_LimpiarClick(object sender, EventArgs e)
        {
            BorrarDatosVenta();
            BorrarDatosDetalleVenta();
            BorrarMensajesError();
            BorrarDatosBusqueda();
            if (tabcOperacion.SelectedTab != tabpRegistrar)
                DeshabilitarControles();
            LlenarDgvVentas(false);
            tableLayoutPanel1.Focus();
        }

        private void ControlBuscarVenta_BuscarClick(object sender, EventArgs e)
        {
            BorrarDatosVenta();
            BorrarDatosDetalleVenta();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tabpRegistrar)
                DeshabilitarControles();
            LlenarDgvVentas(true);
            tableLayoutPanel1.Focus();
        }

        private void BorrarDatosVenta()
        {
            errorProvider1.Clear();
            txtId.Text = "";
            txtId.Tag = null;
            cboCliente.SelectedIndex = cboEmpleado.SelectedIndex = cboTransportista.SelectedIndex = 0;

            InicializarDtps();

            txtDirigidoa.Text = txtDomicilio.Text = txtCiudad.Text = txtRegion.Text = txtCP.Text = txtPais.Text = "";
            nudFlete.Value = 0;
            btnNota.Enabled = false;
        }

        private void BorrarDatosDetalleVenta()
        {
            controlAgregarProducto.CboCategoria.SelectedIndex = 0;
            InicializarValoresAgregarProducto();
            InicializarCboProducto();
            InicializarNuds();
            controlDetalleDeLaVenta.DgvDetalle.Rows.Clear();
        }

        private void InicializarValoresAgregarProducto() => controlAgregarProducto.NudPrecioConIVAIncluido.Value = controlAgregarProducto.NudCantidad.Value = controlAgregarProducto.NudUInventario.Value = controlAgregarProducto.NudDescuento.Value = 0;

        private void InicializarValoresEnvio() => txtDirigidoa.Text = txtDomicilio.Text = txtCiudad.Text = txtRegion.Text = txtCP.Text = txtPais.Text = "";

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

        private void BorrarMensajesError() => errorProvider1.Clear();

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

        private bool ValidarControlesVenta()
        {
            errorProvider1.Clear();
            bool valida = true;
            if (cboCliente.SelectedIndex == 0)
            {
                valida = false;
                errorProvider1.SetError(cboCliente, "Ingrese el cliente");
            }
            if (cboEmpleado.SelectedIndex == 0)
            {
                valida = false;
                errorProvider1.SetError(cboEmpleado, "Ingrese el empleado");
            }
            if (dtpVenta.Checked == false)
            {
                valida = false;
                errorProvider1.SetError(dtpVenta, "Ingrese la fecha de la venta");
            }
            if (cboTransportista.SelectedIndex == 0)
            {
                valida = false;
                errorProvider1.SetError(cboTransportista, "Ingrese la compañía transportista");
            }
            // Validación de Fecha Requerida Vs Venta
            if (dtpVenta.Checked &&
                dtpRequerido.Checked &&
                dtpRequerido.Value.Date < dtpVenta.Value.Date)
            {
                // Comparamos solo la parte Date para evitar problemas con los segundos
                valida = false;
                errorProvider1.SetError(dtpRequerido, "La fecha de entrega no puede ser anterior a la fecha de venta");
            }

            DateTime? fVenta = Utils.ObtenerFechaHora(dtpVenta, dtpHoraVenta);
            DateTime? fRequerido = Utils.ObtenerFechaHora(dtpRequerido, dtpHoraRequerido);
            DateTime? fEnvio = Utils.ObtenerFechaHora(dtpEnvio, dtpHoraEnvio);

            // 1. Validar Requerido vs Venta
            if (fVenta.HasValue && fRequerido.HasValue && fRequerido.Value < fVenta.Value)
            {
                valida = false;
                errorProvider1.SetError(dtpHoraRequerido, "La fecha/hora requerida no puede ser anterior a la venta.");
            }
            // 2. Validar Envío vs Venta
            if (fVenta.HasValue && fEnvio.HasValue && fEnvio.Value < fVenta.Value)
            {
                valida = false;
                errorProvider1.SetError(dtpHoraEnvio, "La fecha/hora de envío no puede ser anterior a la venta.");
            }
            if (controlTotalesDeLaVenta.NudTotal.Value == 0)
            {
                valida = false;
                errorProvider1.SetError(controlAgregarProducto.BtnAgregar, "Ingrese el detalle de la venta");
                errorProvider1.SetError(controlTotalesDeLaVenta.NudTotal, "El total de la venta no puede ser cero");
            }
            if (controlAgregarProducto.CboProducto.SelectedIndex > 0)
            {
                valida = false;
                errorProvider1.SetError(controlAgregarProducto.CboProducto, "Se ha seleccionado un producto y no lo ha agregado a la venta");
            }
            return valida;
        }

        private bool ValidarControlesProducto()
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
                    errorProvider1.SetError(controlAgregarProducto.CboProducto, "No se puede tener un producto duplicado en el detalle de la venta");
                }
            }
            // necesario crear un objeto temporal para calcular el subtotal con la formulas ya definidas en la clase VentaDetalle
            VentaDetalle ventaDetalle = new VentaDetalle();
            ventaDetalle.UnitPrice = controlAgregarProducto.NudPrecioConIVAIncluido.Value;
            ventaDetalle.Quantity = (short)controlAgregarProducto.NudCantidad.Value;
            ventaDetalle.Discount = controlAgregarProducto.NudDescuento.Value / 100m;
            CalcularTotalProducto(ventaDetalle);
            if (ventaDetalle.Subtotal == 0)
            {
                valida = false;
                if (controlAgregarProducto.NudCantidad.Value == 0)
                    errorProvider1.SetError(controlAgregarProducto.BtnAgregar, "Ingrese el detalle de la venta");
                else if (ventaDetalle.Subtotal == 0)
                {
                    errorProvider1.SetError(controlAgregarProducto.BtnAgregar, "El valor del total del producto no puede ser cero");
                    errorProvider1.SetError(controlAgregarProducto.NudTotal2, "El valor del total del producto no puede ser cero");
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

        private void CalcularTotalProducto(VentaDetalle ventaDetalle)
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
            foreach (DataGridViewRow dgvr in controlDetalleDeLaVenta.DgvDetalle.Rows)
            {
                totalDeUnidades += decimal.Parse(dgvr.Cells["Cantidad"].Value.ToString());
                subtotalDelImporte += decimal.Parse(dgvr.Cells["Importe"].Value.ToString());
                subtotalDelImporteDelDescuento += decimal.Parse(dgvr.Cells["ImporteDelDescuento"].Value.ToString());
                subtotalDelImporteConDescuento += decimal.Parse(dgvr.Cells["ImporteConDescuento"].Value.ToString());
                subtotalDelImporteSinIVA += decimal.Parse(dgvr.Cells["ImporteSinIVA"].Value.ToString());
                subtotalDelImporteDelIVA += decimal.Parse(dgvr.Cells["ImporteDelIVA"].Value.ToString());
                total += decimal.Parse(dgvr.Cells["Subtotal"].Value.ToString());
            }
            ReenumerarFilas();
            controlTotalesDeLaVenta.NudNumProd.Value = controlDetalleDeLaVenta.DgvDetalle.Rows.Count;
            controlTotalesDeLaVenta.NudTotalDeUnidades.Value = totalDeUnidades;
            controlTotalesDeLaVenta.NudSubtotalDelImporte.Value = subtotalDelImporte;
            controlTotalesDeLaVenta.NudSubtotalDelImporteDelDescuento.Value = subtotalDelImporteDelDescuento;
            controlTotalesDeLaVenta.NudSubtotalDelImporteConDescuento.Value = subtotalDelImporteConDescuento;
            controlTotalesDeLaVenta.NudSubtotalDelImporteSinIVA.Value = subtotalDelImporteSinIVA;
            controlTotalesDeLaVenta.NudSubtotalDelImporteDelIVA.Value = subtotalDelImporteDelIVA;
            controlTotalesDeLaVenta.NudTotal.Value = total;
        }

        private void NudEnterHandler(object sender, EventArgs e)
        {
            if (sender is NumericUpDown nud && nud.Controls[1] is TextBox tb)
            {
                // Diferir la selección para que ocurra después de que el TextBox reciba el foco
                tb.BeginInvoke((Action)(() => tb.SelectAll()));
            }
        }

        private void NudCantidadDescuento_LeaveValueChangedHandler(object sender, EventArgs e) => ValidarControlesProducto();

        private void dtpVenta_ValueChanged(object sender, EventArgs e)
        {
            if (dtpVenta.Checked)
            {
                if (dtpHoraVenta.Value.TimeOfDay == TimeSpan.Zero)
                    dtpHoraVenta.Value = DateTime.Now;
            }
            else
            {
                dtpRequerido.MinDate = dtpEnvio.MinDate = FechaBaseMinDate;
                dtpHoraVenta.Enabled = false;
            }

            SincronizarJerarquiaFechas();
        }

        private void dtpRequerido_ValueChanged(object sender, EventArgs e)
        {
            SincronizarJerarquiaFechas();
        }

        private void dtpEnvio_ValueChanged(object sender, EventArgs e)
        {
            SincronizarJerarquiaFechas();
        }

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
                    var dtCboProductos = _productoService.ObtenerProductosPorCategoriaCbo(int.Parse(controlAgregarProducto.CboCategoria.SelectedValue.ToString()));
                    controlAgregarProducto.CboProducto.DataSource = dtCboProductos;
                    controlAgregarProducto.CboProducto.DisplayMember = "ProductName";
                    controlAgregarProducto.CboProducto.ValueMember = "ProductID";
                    controlAgregarProducto.CboProducto.Enabled = true;
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {dgvVentas.RowCount} registro(s) en ventas");
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
                MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {dgvVentas.RowCount} registro(s) en ventas");
            }
        }

        private void cboCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCliente.SelectedIndex > 0)
            {
                try
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    var dtoEnvioInformacion = _ventaService.ObtenerUltimaInformacionDeEnvio(cboCliente.SelectedValue?.ToString());
                    if (dtoEnvioInformacion != null)
                    {
                        txtDirigidoa.Text = dtoEnvioInformacion.ShipName ?? "";
                        txtDomicilio.Text = dtoEnvioInformacion.ShipAddress ?? "";
                        txtCiudad.Text = dtoEnvioInformacion.ShipCity ?? "";
                        txtRegion.Text = dtoEnvioInformacion.ShipRegion ?? "";
                        txtCP.Text = dtoEnvioInformacion.ShipPostalCode ?? "";
                        txtPais.Text = dtoEnvioInformacion.ShipCountry ?? "";
                    }
                    else
                        InicializarValoresEnvio();
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {dgvVentas.RowCount} registro(s) en ventas");
                }
                catch (Exception ex)
                {
                    U.MsgCatchOue(ex);
                }
            }
            else
                InicializarValoresEnvio();
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
                    var dtoProductoCostoInventario = _productoService.ObtenerProductoCostoEInventario(int.Parse(productId));
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
                            InicializarNudsProducto();
                        }
                        else
                            HabilitarControlesProducto();
                    }
                    else
                    {
                        DeshabilitarControlesProducto();
                        InicializarValoresAgregarProducto();
                        InicializarNudsProducto();
                        InicializarCboProducto();
                    }
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {dgvVentas.RowCount} registro(s) en ventas");
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
                InicializarNudsProducto();
            }
        }

        private void dgvVentas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            btnNota.Enabled = false;
            if (tabcOperacion.SelectedTab != tabpRegistrar)
            {
                BorrarDatosVenta();
                BorrarDatosDetalleVenta();
                BorrarMensajesError();
                DataGridViewRow dgvr = dgvVentas.CurrentRow;
                txtId.Text = dgvr.Cells["OrderId"].Value.ToString();
                // se tiene que definir aqui para verificar la concurrencia porque como lo venia haciendo habia un lapso de tiempo que podia cambiar el registro, se tiene que comparar contra lo que esta definido en el dgvVentas
                txtId.Tag = dgvr.Cells["RowVersionStr"].Value;
                int orderId = string.IsNullOrEmpty(txtId.Text) ? 0 : Convert.ToInt32(txtId.Text);
                LlenarDatosVenta(ref orderId);
                if (orderId != 0)
                {
                    LlenarDatosDetalleVenta(orderId);
                    DeshabilitarTodosControles();
                    if (tabcOperacion.SelectedTab == tabpConsultar)
                    {
                        btnNota.Enabled = true;
                    }
                    else if (tabcOperacion.SelectedTab == tabpModificar)
                    {
                        HabilitarControles();

                        SincronizarJerarquiaFechas();

                        btnGenerar.Enabled = true;
                        btnNota.Enabled = false;
                    }
                    else if (tabcOperacion.SelectedTab == tabpEliminar)
                    {
                        btnGenerar.Enabled = true;
                        btnNota.Enabled = false;
                    }
                }
                else
                {
                    LlenarDgvVentas(false);
                    DeshabilitarTodosControles();
                }
            }
            CargarValoresOriginales();
            grbVenta.Focus();
        }

        private void LlenarDatosVenta(ref int orderId)
        {
            if (orderId == 0) return;
            // 1. Apagar eventos para que no se disparen los ValueChanged al cargar
            DeshabilitarEventosFechas();
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var venta = _ventaBLL.ObtenerVentaPorId(orderId);
                if (venta != null)
                {
                    txtId.Text = venta.OrderID.ToString();
                    txtId.Tag = venta.RowVersionString;
                    cboCliente.SelectedIndexChanged -= new EventHandler(cboCliente_SelectedIndexChanged);
                    cboCliente.SelectedValue = venta.Cliente.CustomerID;
                    cboCliente.SelectedIndexChanged += new EventHandler(cboCliente_SelectedIndexChanged);
                    cboEmpleado.SelectedValue = venta.Empleado.EmployeeID;
                    cboTransportista.SelectedValue = venta.Transportista.ShipperID;
                    txtDirigidoa.Text = venta.ShipName ?? "";
                    txtDomicilio.Text = venta.ShipAddress ?? "";
                    txtCiudad.Text = venta.ShipCity ?? "";
                    txtRegion.Text = venta.ShipRegion ?? "";
                    txtCP.Text = venta.ShipPostalCode ?? "";
                    txtPais.Text = venta.ShipCountry ?? "";
                    nudFlete.Value = venta.Freight ?? 0;

                    // --- Paso 1: abrir límites ---
                    dtpVenta.MinDate = FechaBaseMinDate;
                    dtpRequerido.MinDate = FechaBaseMinDate;
                    dtpEnvio.MinDate = FechaBaseMinDate;
                    dtpHoraVenta.MinDate = FechaBaseMinDate;
                    dtpHoraRequerido.MinDate = FechaBaseMinDate;
                    dtpHoraEnvio.MinDate = FechaBaseMinDate;

                    // --- Paso 2: inicializar valores seguros ---
                    dtpVenta.Value = DateTime.Today;
                    dtpRequerido.Value = DateTime.Today;
                    dtpEnvio.Value = DateTime.Today;
                    dtpHoraVenta.Value = DateTime.Today;
                    dtpHoraRequerido.Value = DateTime.Today;
                    dtpHoraEnvio.Value = DateTime.Today;

                    if (venta.OrderDate.HasValue)
                    {
                        dtpVenta.Checked = true;
                        dtpHoraVenta.Enabled = true;
                        dtpVenta.Value = venta.OrderDate.Value;
                        dtpHoraVenta.Value = venta.OrderDate.Value;
                    }
                    else
                    {
                        dtpVenta.Value = dtpHoraVenta.Value = DateTime.Today;
                        dtpHoraVenta.Enabled = false;
                        dtpVenta.Checked = false;
                    }
                    if (venta.RequiredDate.HasValue)
                    {
                        dtpRequerido.Value = venta.RequiredDate.Value;
                        dtpHoraRequerido.Value = venta.RequiredDate.Value;
                        dtpRequerido.Checked = true; // Forzamos el check si hay dato
                    }
                    else
                    {
                        dtpRequerido.Value = dtpHoraRequerido.Value = DateTime.Today;
                        dtpRequerido.Checked = false; // FORZAMOS UNCHECK SI ES NULO
                    }
                    if (venta.ShippedDate.HasValue)
                    {
                        dtpEnvio.Value = venta.ShippedDate.Value;
                        dtpHoraEnvio.Value = venta.ShippedDate.Value;
                        dtpEnvio.Checked = true;
                    }
                    else
                    {
                        dtpEnvio.Value = dtpHoraEnvio.Value = DateTime.Today;
                        dtpEnvio.Checked = false; // FORZAMOS UNCHECK SI ES NULO
                    }
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {dgvVentas.RowCount} registro(s) en ventas");
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
            finally
            {
                SincronizarJerarquiaFechas();
                // 2. Volver a prenderlos para que el usuario sí pueda interactuar
                HabilitarEventosFechas();
            }
        }

        private void LlenarDatosDetalleVenta(int orderId)
        {
            if (orderId == 0) return;
            try
            {
                numDetalle = 1;
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var detalles = _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(orderId);
                if (detalles.Count == 0)
                {
                    U.NotificacionWarning("No se encontraron detalles para la venta especificada");
                }
                else
                {
                    foreach (var ventaDetalle in detalles)
                    {
                        controlDetalleDeLaVenta.DgvDetalle.Rows.Add(new object[]
                        {
                            numDetalle,
                            ventaDetalle.Producto.ProductName,
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
                            " Modificar ",
                            " Eliminar ",
                            ventaDetalle.Producto.ProductID,
                            ventaDetalle.RowVersion
                        });
                        ++numDetalle;
                    }
                }
                CalcularTotales();
                MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {dgvVentas.RowCount} registro(s) en ventas");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void BtnAgregar_ClickHandler(object sender, EventArgs e)
        {
            BorrarMensajesError();
            if (!ValidarControlesProducto())
            {
                grbVenta.Focus();
                return;
            }
            else
            {
                DeshabilitarControlesProducto();
                btnNuevo.Enabled = false;
                InicializarNudsProducto();
                var ventaDetalle = new VentaDetalle()
                {
                    Producto = new Producto()
                    {
                        ProductID = (int)controlAgregarProducto.CboProducto.SelectedValue,
                        ProductName = controlAgregarProducto.CboProducto.Text
                    },
                    UnitPrice = controlAgregarProducto.NudPrecioConIVAIncluido.Value,
                    Quantity = (short)controlAgregarProducto.NudCantidad.Value,
                    Discount = controlAgregarProducto.NudDescuento.Value / 100m,
                };
                AgregarFila(ventaDetalle);
                CalcularTotales();
                controlAgregarProducto.CboCategoria.SelectedIndex = 0;
                InicializarCboProducto();
                InicializarValoresAgregarProducto();
                InicializarNudsProducto();
                controlDetalleDeLaVenta.Focus();
                controlAgregarProducto.CboCategoria.Focus();
                btnNuevo.Enabled = true;
            }
        }

        private void AgregarFila(VentaDetalle ventaDetalle)
        {
            controlDetalleDeLaVenta.DgvDetalle.Rows.Insert(0, new object[]
            {
                0, // valor temporal, se corregirá en ReenumerarFilas
                ventaDetalle.Producto.ProductName,
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
                " Modificar ",
                " Eliminar ",
                ventaDetalle.Producto.ProductID
            });
        }

        private void ReenumerarFilas()
        {
            int totalFilas = controlDetalleDeLaVenta.DgvDetalle.Rows.Count;
            for (int i = 0; i < totalFilas; i++)
            {
                // La columna 0 es la de numDetalle
                controlDetalleDeLaVenta.DgvDetalle.Rows[i].Cells["Id"].Value = totalFilas - i;
            }
        }

        private void DgvDetalle_CellClickHandler(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || (e.ColumnIndex != controlDetalleDeLaVenta.DgvDetalle.Columns["Eliminar"].Index))
                return;
            var dgv = controlDetalleDeLaVenta.DgvDetalle; // acceso al DataGridView interno
            try
            { 
                dgv.Rows.RemoveAt(e.RowIndex);
                CalcularTotales();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            MDIPrincipal.ActualizarBarraDeEstado($"Se muestran {dgvVentas.RowCount} registro(s) en ventas");
            dgv.Focus();
        }

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            numDetalle = 1;
            BorrarDatosVenta();
            BorrarDatosDetalleVenta();
            BorrarMensajesError();
            OcultarControlAgregarProducto();
            if (tabcOperacion.SelectedTab == tabpRegistrar)
            {
                if (EventoCargado)
                {
                    dgvVentas.CellClick -= dgvVentas_CellClick;
                    EventoCargado = false;
                }
                grbVenta.Text = "»   Registro de ventas:   «";
                MostrarControlAgregarProducto();
                VentaGenerada = false;
                BorrarDatosBusqueda();
                HabilitarControles();

                dtpVenta.Value = dtpRequerido.Value = dtpEnvio.Value = DateTime.Today;
                dtpRequerido.Checked = dtpEnvio.Checked = false;
                dtpRequerido.Enabled = dtpEnvio.Enabled = true;
                // Sincronizar las horas para que se deshabiliten al no tener check
                //SincronizarHabilitarHoras();
                SincronizarJerarquiaFechas();

                btnGenerar.Text = "Generar venta";
                MostrarCols();
                dtpHoraRequerido.Enabled = dtpHoraEnvio.Enabled = false;
                btnNota.Enabled = false;
                // IMPORTANTE: Tomar la foto AQUÍ
                CargarValoresOriginales();
            }
            else
            {
                if (!EventoCargado)
                {
                    //dgvVentas.CellClick -= dgvVentas_CellClick; // para evitar que se duplique el evento al cambiar varias veces de pestaña
                    dgvVentas.CellClick += dgvVentas_CellClick;
                    EventoCargado = true;
                }
                DeshabilitarTodosControles();
                OcultarCols();
                InicializarDtps();
                if (tabcOperacion.SelectedTab == tabpConsultar)
                {
                    DeshabilitarEventosFechas();
                    dtpRequerido.Checked = dtpEnvio.Checked = false;
                    dtpRequerido.Value = dtpEnvio.Value = FechaBaseMinDate;
                    grbVenta.Text = "»   Consulta de ventas:   «";
                    btnGenerar.Text = "Generar venta";
                    btnNota.Enabled = false;
                    HabilitarEventosFechas();
                    // IMPORTANTE: Tomar la foto AQUÍ
                    CargarValoresOriginales();
                }
                else if (tabcOperacion.SelectedTab == tabpModificar)
                {
                    grbVenta.Text = "»   Modificación de ventas:   «";
                    VentaGenerada = false;
                    btnGenerar.Text = "Modificar venta";

                    // Usamos esta técnica para asegurar que se ejecute DESPUÉS de que la pestaña cargue
                    this.BeginInvoke(new Action(() => {
                        DeshabilitarEventosFechas();

                        dtpRequerido.Checked = dtpEnvio.Checked = false;
                        dtpRequerido.Value = dtpEnvio.Value = dtpHoraRequerido.Value = dtpHoraEnvio.Value = FechaBaseMinDate;

                        SincronizarJerarquiaFechas();
                        HabilitarEventosFechas();

                        // IMPORTANTE: Tomar la foto AQUÍ, después de limpiar
                        CargarValoresOriginales();
                    }));
                }
                else if (tabcOperacion.SelectedTab == tabpEliminar)
                {
                    grbVenta.Text = "»   Eliminarción de ventas:   «";
                    btnGenerar.Text = "Eliminar venta";
                    // IMPORTANTE: Tomar la foto AQUÍ
                    CargarValoresOriginales();
                }
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            int numRegs = 0;
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tabpRegistrar)
            {
                try
                {
                    if (!ValidarControlesVenta())
                    {
                        grbVenta.Focus();
                        return;
                    }
                    else
                    {
                        MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
                        DeshabilitarTodosControles();
                        btnGenerar.Enabled = false;
                        Venta venta = new Venta();
                        venta.Cliente.CustomerID = cboCliente.SelectedValue.ToString().Trim();
                        venta.Empleado.EmployeeID = Convert.ToInt32(cboEmpleado.SelectedValue);

                        if (dtpVenta != null && dtpHoraVenta != null)
                            venta.OrderDate = Utils.ObtenerFechaHora(dtpVenta, dtpHoraVenta);
                        if (dtpRequerido != null && dtpHoraRequerido != null)
                            venta.RequiredDate = Utils.ObtenerFechaHora(dtpRequerido, dtpHoraRequerido);
                        if (dtpEnvio != null && dtpHoraEnvio != null)
                            venta.ShippedDate = Utils.ObtenerFechaHora(dtpEnvio, dtpHoraEnvio);

                        venta.Transportista.ShipperID = int.Parse(cboTransportista.SelectedValue.ToString());
                        venta.ShipName = txtDirigidoa.Text.Trim();
                        venta.ShipAddress = txtDomicilio.Text.Trim();
                        venta.ShipCity = txtCiudad.Text.Trim();
                        venta.ShipRegion = txtRegion.Text.Trim();
                        venta.ShipPostalCode = txtCP.Text.Trim();
                        venta.ShipCountry = txtPais.Text.Trim();
                        venta.Freight = nudFlete.Value;
                        // llenado de elementos hijos ordenados en orden inverso para que el ultimo en capturarse en el dgv sea el primero en insertarse en la base de datos
                        for (int i = controlDetalleDeLaVenta.DgvDetalle.Rows.Count - 1; i >= 0; i--)
                        {
                            DataGridViewRow row = controlDetalleDeLaVenta.DgvDetalle.Rows[i];
                            // defensiva: ignorar filas nuevas o vacías
                            if (row.IsNewRow) continue;
                            VentaDetalle ventaDetalles = new VentaDetalle
                            {
                                Producto = new Producto
                                {
                                    ProductID = Convert.ToInt32(row.Cells["ProductoId"].Value),
                                    ProductName = row.Cells["Producto"].Value.ToString()
                                },
                                UnitPrice = Convert.ToDecimal(row.Cells["Precio"].Value),
                                Quantity = Convert.ToInt16(row.Cells["Cantidad"].Value),
                                Discount = Convert.ToDecimal(row.Cells["Descuento"].Value),
                            };
                            venta.VentaDetalles.Add(ventaDetalles);
                        }
                        numRegs = _ventaBLL.InsertarVentaCompleta(venta, out int orderId, out byte[] rowVersion);
                        txtId.Text = orderId.ToString();
                        venta.RowVersion = rowVersion;
                        txtId.Tag = venta.RowVersionStr;
                        MDIPrincipal.ActualizarBarraDeEstado($"Se insertaron 1 registro en ventas y {venta.VentaDetalles.Count} registro(s) en el detalle de ventas");
                        string paraNotificacion = $"La venta con Id: {txtId.Text} del Cliente: {cboCliente.Text}:";
                        if (numRegs > 0)
                        {
                            MDIPrincipal.ActualizarBarraDeEstado($"Se insertaron 1 registro en ventas y {venta.VentaDetalles.Count} registro(s) en el detalle de ventas");
                            U.NotificacionInformation(paraNotificacion + Utils.srs);
                            VentaGenerada = true;
                            numDetalle = 1;
                            btnNota.Enabled = true;
                            BorrarDatosBusqueda();
                            LlenarDgvVentas(false);
                            controlDetalleDeLaVenta.DgvDetalle.Rows.Clear();
                            LlenarDatosDetalleVenta(Convert.ToInt32(txtId.Text));
                            controlAgregarProducto.CboCategoria.Enabled = true;
                            OcultarCols();
                            grbVenta.Focus();
                        }
                        else
                            U.NotificacionError(paraNotificacion + Utils.nfrs);
                    }
                }
                catch (Exception ex)
                {
                    U.MsgCatchOue(ex);
                    HabilitarControles();
                    dtpRequerido.Enabled = dtpEnvio.Enabled = true;
                    HabilitarControlesProducto();
                    btnGenerar.Enabled = true;
                }
            }
            else if (tabcOperacion.SelectedTab == tabpModificar)
            {
                try
                {
                    if (!ValidarControlesVenta())
                    {
                        grbVenta.Focus();
                        return;
                    }
                    else
                    {
                        MDIPrincipal.ActualizarBarraDeEstado(Utils.modificandoRegistro);
                        DeshabilitarTodosControles();
                        btnGenerar.Enabled = false;
                        Venta venta = new Venta();
                        venta.OrderID = int.Parse(txtId.Text);
                        venta.Cliente.CustomerID = cboCliente.SelectedValue.ToString().Trim();
                        venta.Empleado.EmployeeID = Convert.ToInt32(cboEmpleado.SelectedValue);

                        if (dtpVenta != null && dtpHoraVenta != null)
                            venta.OrderDate = Utils.ObtenerFechaHora(dtpVenta, dtpHoraVenta);
                        if (dtpRequerido != null && dtpHoraRequerido != null)
                            venta.RequiredDate = Utils.ObtenerFechaHora(dtpRequerido, dtpHoraRequerido);
                        if (dtpEnvio != null && dtpHoraEnvio != null)
                            venta.ShippedDate = Utils.ObtenerFechaHora(dtpEnvio, dtpHoraEnvio);

                        venta.Transportista.ShipperID = Convert.ToInt32(cboTransportista.SelectedValue);
                        venta.ShipName = txtDirigidoa.Text.Trim();
                        venta.ShipAddress = txtDomicilio.Text.Trim();
                        venta.ShipCity = txtCiudad.Text.Trim();
                        venta.ShipRegion = txtRegion.Text.Trim();
                        venta.ShipPostalCode = txtCP.Text.Trim();
                        venta.ShipCountry = txtPais.Text.Trim();
                        venta.Freight = nudFlete.Value;
                        venta.RowVersion = RowVersionHelper.RowVersionObjToByteArray(txtId.Tag);
                        numRegs = _ventaBLL.Actualizar(venta);
                        txtId.Tag = venta.RowVersionStr; // se tiene que actualizar por la nota de remision no detecte un cambio
                        MDIPrincipal.ActualizarBarraDeEstado($"Se actualizaron {(numRegs < 0 ? 0 : numRegs)} registro(s)");
                        string idVentaCliente = $"La venta con Id: {venta.OrderID} - Cliente: {cboCliente.Text}:";

                        if (numRegs > 0)
                        {
                            LlenarDgvVentas(false);
                            U.NotificacionInformation(idVentaCliente + Utils.sms);
                            VentaGenerada = true;
                            btnNota.Enabled = true;
                        }
                        else if (numRegs == -1)
                        {
                            LlenarDgvVentas(false);
                            U.NotificacionError(idVentaCliente + Utils.nfmfe);
                            BorrarDatosVenta();
                            BorrarDatosDetalleVenta();
                        }
                        else if (numRegs == -2)
                        {
                            int orderId = string.IsNullOrEmpty(txtId.Text) ? 0 : Convert.ToInt32(txtId.Text);
                            LlenarDgvVentas(false);
                            U.NotificacionError(idVentaCliente + Utils.nfmfm);
                            BorrarDatosVenta();
                            BorrarDatosDetalleVenta();
                            LlenarDatosVenta(ref orderId);
                            LlenarDatosDetalleVenta(orderId);
                            DeshabilitarTodosControles();
                        }
                        else
                        {
                            U.NotificacionError(idVentaCliente + Utils.nfmmd);
                            BorrarDatosVenta();
                            BorrarDatosDetalleVenta();
                        }
                    }
                }
                catch (Exception ex)
                {
                    U.MsgCatchOue(ex);
                }
            }
            else if (tabcOperacion.SelectedTab == tabpEliminar)
            {
                if (U.NotificacionQuestion($"[orange]¿Esta seguro de eliminar la venta con Id: {txtId.Text} del Cliente: {cboCliente.Text}?") == DialogResult.Yes)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
                    btnGenerar.Enabled = false;
                    try
                    {
                        Venta venta = new Venta();
                        venta.OrderID = int.Parse(txtId.Text);
                        venta.RowVersion = RowVersionHelper.RowVersionObjToByteArray(txtId.Tag);
                        numRegs = _ventaBLL.Eliminar(venta, out string productoExcede);
                        string idVentaCliente = $"La venta con Id: {txtId.Text} - Cliente: {cboCliente.Text}:";
                        if (numRegs > 0)
                            U.NotificacionInformation(idVentaCliente + Utils.ses);
                        else if (numRegs == -1)
                            U.NotificacionError(idVentaCliente + Utils.nfefe);
                        else if (numRegs == -2)
                            U.NotificacionError(idVentaCliente + Utils.nfefm);
                        else if (numRegs == -7)
                            U.NotificacionError(idVentaCliente + $"\n[red]No fue eliminada de la base de datos, el nuevo inventario del producto {productoExcede}, excedió el límite máximo que se puede almacenar en la base de datos (32,767 unidades)"); // Stock excedió el máximo permitido
                        else if (numRegs == -8)
                            U.NotificacionError(idVentaCliente + $"\n[red]No fue eliminada de la base de datos, el nuevo inventario del producto {productoExcede}, sería invalido (negativo)"); // stock negativo, este caso nunca ocurre porque la base de datos no lo permite con un check constraint
                        else
                            U.NotificacionError(idVentaCliente + Utils.nfemd);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    if (numRegs >= -8)
                    {
                        LlenarDgvVentas(false);
                        BorrarDatosVenta();
                        BorrarDatosDetalleVenta();
                    }
                }
                else
                {
                    BorrarDatosVenta();
                    BorrarDatosDetalleVenta();
                    btnGenerar.Enabled = false;
                }
            }
            CargarValoresOriginales();
        }

        private void tabcOperacion_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!VentaGenerada && Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion("[gold]Se detectaron cambios en los datos de la venta que no han sido guardados.\n[blue]Si cambia de pestaña se perderan los datos no guardados.\n[red]¿Desea cambiar de pestaña?") == DialogResult.No)
                {
                    tableLayoutPanel1.Focus();
                    e.Cancel = true;
                }
        }

        private void btnNota_Click(object sender, EventArgs e)
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
                btnNota.Enabled = false;
                BorrarDatosVenta();
                BorrarDatosDetalleVenta();
                LlenarDgvVentas(false);
                CargarValoresOriginales();
                tableLayoutPanel1.Focus();
                controlAgregarProducto.NudCantidadDescuento_LeaveValueChanged += NudCantidadDescuento_LeaveValueChangedHandler;
            }
            return;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // Caso 1: si no estamos en la pestaña de registrar
            if (tabcOperacion.SelectedTab != tabpRegistrar)
            {
                tabcOperacion.SelectedTab = tabpRegistrar;
                tableLayoutPanel1.Focus();
                return;
            }

            // Caso 2: estamos en la pestaña de registrar
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
            {
                if (U.NotificacionQuestion("[gold]Se detectaron cambios en los datos de la venta que no han sido guardados.\n[blue]Si no guarda los cambios se perderan los datos no guardados.\n[red]¿Desea borrar los datos no guardados?") == DialogResult.No)
                {
                    tableLayoutPanel1.Focus();
                    return;
                }
            }

            // --- INICIO DE LIMPIEZA TÉCNICA DE FECHAS ---
            DeshabilitarEventosFechas(); // Crucial para que no se marquen solos
            try
            {
                BorrarDatosVenta();
                BorrarDatosDetalleVenta();
                HabilitarControles();

                // 1. Resetear límites (MinDate) para permitir asignar 'Hoy' sin errores
                DateTime hoy = DateTime.Today;

                dtpVenta.MinDate = dtpRequerido.MinDate = dtpEnvio.MinDate = FechaBaseMinDate;

                // 2. Configurar valores para una VENTA NUEVA
                dtpVenta.Value = hoy;
                dtpVenta.Checked = true; // La venta siempre debe tener fecha

                dtpRequerido.Value = hoy;
                dtpRequerido.Checked = false; // Inicia desmarcado (Gris)

                dtpEnvio.Value = hoy;
                dtpEnvio.Checked = false; // Inicia desmarcado (Gris)

                // 3. Configurar horas iniciales
                dtpHoraVenta.Value = DateTime.Today.Add(DateTime.Now.TimeOfDay);
                dtpHoraRequerido.Value = dtpHoraEnvio.Value = hoy.AddHours(12);

                dtpRequerido.Enabled = dtpEnvio.Enabled = true;
                btnNota.Enabled = false;
                VentaGenerada = false;
                numDetalle = 1;
            }
            finally
            {
                // 1. Empuja la lógica de las FECHAS (esto pone los MinDates y valida cascada)
                SincronizarJerarquiaFechas();

                HabilitarEventosFechas();
                CargarValoresOriginales();   // Captura la foto "limpia" para evitar alertas de cambios falsas
            }
            MostrarCols();
            tableLayoutPanel1.Focus();
        }

        private int ChkRowVersion()
        {
            if (txtId.Tag == null)
                return -1;
            byte[] rowVersion = RowVersionHelper.RowVersionObjToByteArray(txtId.Tag);
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                Venta venta = _ventaBLL.ObtenerVentaPorId(int.Parse(txtId.Text));
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

        private void OcultarControlAgregarProducto()
        {
            // algoritmo para colapsar tablelayout2
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            grbVenta.AutoSize = true;
            grbVenta.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            tableLayoutPanel2.RowStyles[1].SizeType = SizeType.Absolute;
            tableLayoutPanel2.RowStyles[1].Height = 10;
            tableLayoutPanel2.RowStyles[3].SizeType = SizeType.Absolute;
            tableLayoutPanel2.RowStyles[3].Height = 10;

            tableLayoutPanel2.RowStyles[0].SizeType = SizeType.AutoSize;
            tableLayoutPanel2.RowStyles[2].SizeType = SizeType.AutoSize;
            tableLayoutPanel2.RowStyles[4].SizeType = SizeType.AutoSize;

            tableLayoutPanel1.RowStyles[5].SizeType = SizeType.AutoSize; // fila 6 → índice 5

            controlBuscarVenta.AutoSize = true;
            controlBuscarVenta.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            controlAgregarProducto.Visible = false;
            tableLayoutPanel2.RowStyles[4].Height = 0;
            tableLayoutPanel2.RowStyles[4].SizeType = SizeType.Absolute;
            tableLayoutPanel2.PerformLayout();   // fuerza recalculo
            grbVenta.PerformLayout();            // fuerza recalculo
            tableLayoutPanel1.PerformLayout();   // fuerza recalculo
            this.PerformLayout(); // fuerza al formulario entero
        }

        private void MostrarControlAgregarProducto()
        {
            // Mostrar el control y restaurar fila
            controlAgregarProducto.Visible = true;
            tableLayoutPanel2.RowStyles[4].SizeType = SizeType.AutoSize; // fila 5 → índice 4
            tableLayoutPanel2.RowStyles[4].Height = 0; // AutoSize ignora este valor

            // Forzar recalculo
            tableLayoutPanel2.PerformLayout();
            grbVenta.PerformLayout();
            tableLayoutPanel1.PerformLayout();
            this.PerformLayout();
        }

        private void DeshabilitarEventosFechas()
        {
            // Fechas
            dtpVenta.ValueChanged -= dtpVenta_ValueChanged;
            dtpRequerido.ValueChanged -= dtpRequerido_ValueChanged;
            dtpEnvio.ValueChanged -= dtpEnvio_ValueChanged;
            // Horas
            dtpHoraVenta.ValueChanged -= dtpHoraVenta_ValueChanged;
            dtpHoraRequerido.ValueChanged -= dtpHoraRequerido_ValueChanged;
            dtpHoraEnvio.ValueChanged -= dtpHoraEnvio_ValueChanged;
        }

        private void HabilitarEventosFechas()
        {
            // para que sea idempotente debe ser definido como esta aqui. si no su comportamiento sera como si se llamara recursivamente
            //-Luego, cuando llamas a HabilitarEventosFechas(), puede que agregue los handlers duplicados, provocando que se disparen varias veces o que parezca “recursivo”.

            dtpVenta.ValueChanged -= dtpVenta_ValueChanged;
            dtpVenta.ValueChanged += dtpVenta_ValueChanged;

            dtpHoraVenta.ValueChanged -= dtpHoraVenta_ValueChanged;
            dtpHoraVenta.ValueChanged += dtpHoraVenta_ValueChanged;

            dtpRequerido.ValueChanged -= dtpRequerido_ValueChanged;
            dtpRequerido.ValueChanged += dtpRequerido_ValueChanged;

            dtpHoraRequerido.ValueChanged -= dtpHoraRequerido_ValueChanged;
            dtpHoraRequerido.ValueChanged += dtpHoraRequerido_ValueChanged;

            dtpEnvio.ValueChanged -= dtpEnvio_ValueChanged;
            dtpEnvio.ValueChanged += dtpEnvio_ValueChanged;

            dtpHoraEnvio.ValueChanged -= dtpHoraEnvio_ValueChanged;
            dtpHoraEnvio.ValueChanged += dtpHoraEnvio_ValueChanged;
        }

        private void dtpHoraVenta_ValueChanged(object sender, EventArgs e)
        {
            SincronizarJerarquiaFechas();
        }

        private void dtpHoraRequerido_ValueChanged(object sender, EventArgs e)
        {
            SincronizarJerarquiaFechas();
        }

        private void dtpHoraEnvio_ValueChanged(object sender, EventArgs e)
        {
            SincronizarJerarquiaFechas();
        }

        private void SincronizarJerarquiaFechas()
        {
            DeshabilitarEventosFechas();

            try
            {
                DateTimePickerHelper.SincronizarJerarquiaFechas(
                    dtpVenta, dtpHoraVenta,
                    dtpRequerido, dtpHoraRequerido,
                    dtpEnvio, dtpHoraEnvio,
                    tabcOperacion.SelectedTab == tabpModificar || tabcOperacion.SelectedTab == tabpRegistrar
                );
            }
            finally
            {
                HabilitarEventosFechas();
            }
        }
    }
}
