using BLL.EF;
using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmProveedoresCrud : Form
    {
        private bool EjecutarConfDgv = true;
        internal Dictionary<string, object> valoresOriginales;

        public FrmProveedoresCrud()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmProveedoresCrud_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        internal void FrmProveedoresCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrar) == DialogResult.No)
                    e.Cancel = true;
        }

        private void tabcOperacion_DrawItem(object sender, DrawItemEventArgs e) => Utils.DibujarPestañas(sender as TabControl, e);

        private void FrmProveedoresCrud_Load(object sender, EventArgs e)
        {
            tabcOperacion.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabcOperacion.DrawItem += tabcOperacion_DrawItem;
            DeshabilitarControles();
            LlenarCboPais();
            Utils.ConfDgv(Dgv);
            LlenarDgv(false);
            CargarValoresOriginales();
        }

        private void DeshabilitarControles()
        {
            txtCompañia.ReadOnly = txtContacto.ReadOnly = txtTitulo.ReadOnly = true;
            txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCodigoP.ReadOnly = true;
            txtTelefono.ReadOnly = txtFax.ReadOnly = true;
            cboPais.Enabled = false;
        }

        private void HabilitarControles()
        {
            txtCompañia.ReadOnly = txtContacto.ReadOnly = txtTitulo.ReadOnly = false;
            txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCodigoP.ReadOnly = false;
            txtTelefono.ReadOnly = txtFax.ReadOnly = false;
            cboPais.Enabled = true;
        }

        void LlenarCboPais()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var paises = SupplierBLL.ObtenerProveedorPaisesCbo();
                MDIPrincipal.ActualizarBarraDeEstado();
                cboBPais.DataSource = paises;
                cboBPais.ValueMember = "Id";
                cboBPais.DisplayMember = "Pais";
                cboBPais.SelectedIndex = 0;

                cboPais.DataSource = paises.Copy();
                cboPais.ValueMember = "Id";
                cboPais.DisplayMember = "Pais";
                cboPais.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        void LlenarDgv(bool selectorRealizaBusqueda)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DtoProveedoresBuscar criterios = new DtoProveedoresBuscar
                {
                    IdIni = string.IsNullOrWhiteSpace(txtBIdIni.Text) ? 0 : int.Parse(txtBIdIni.Text),
                    IdFin = string.IsNullOrWhiteSpace(txtBIdFin.Text) ? 0 : int.Parse(txtBIdFin.Text),
                    CompanyName = txtBCompañia.Text.Trim(),
                    ContactName = txtBContacto.Text.Trim(),
                    Address = txtBDomicilio.Text.Trim(),
                    City = txtBCiudad.Text.Trim(),
                    Region = txtBRegion.Text.Trim(),
                    PostalCode = txtBCodigoP.Text.Trim(),
                    Country = cboBPais.SelectedValue.ToString(),
                    Phone = txtBTelefono.Text.Trim(),
                    Fax = txtBFax.Text.Trim()
                };
                var proveedores = SupplierBLL.ObtenerProveedores(selectorRealizaBusqueda, criterios, false);
                Dgv.DataSource = proveedores;
                if (EjecutarConfDgv)
                {
                    ConfDgv();
                    EjecutarConfDgv = false;
                }
                if (selectorRealizaBusqueda)
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {Dgv.RowCount} registro(s)");
                else
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran los últimos {Dgv.RowCount} proveedor(es) registrado(s)");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void ConfDgv()
        {
            Dgv.Columns["RowVersion"].Visible = false;
            Dgv.Columns["HomePage"].Visible = false;
            Dgv.Columns["Products"].Visible = false;

            Dgv.Columns["SupplierID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["ContactTitle"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["City"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Region"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["PostalCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Country"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Fax"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["City"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Region"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["PostalCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Country"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Dgv.Columns["SupplierID"].HeaderText = "Id";
            Dgv.Columns["CompanyName"].HeaderText = "Nombre de compañía";
            Dgv.Columns["ContactName"].HeaderText = "Nombre de contacto";
            Dgv.Columns["ContactTitle"].HeaderText = "Título de contacto";
            Dgv.Columns["Address"].HeaderText = "Domicilio";
            Dgv.Columns["City"].HeaderText = "Ciudad";
            Dgv.Columns["Region"].HeaderText = "Región";
            Dgv.Columns["PostalCode"].HeaderText = "Código postal";
            Dgv.Columns["Country"].HeaderText = "País";
            Dgv.Columns["Phone"].HeaderText = "Teléfono";
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosProveedor();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(true);
            CargarValoresOriginales();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            BorrarDatosBusqueda();
            BorrarDatosProveedor();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(false);
            CargarValoresOriginales();
        }

        void BorrarMensajesError() => errorProvider1.Clear();

        private void BorrarDatosBusqueda()
        {
            txtBIdIni.Text = txtBIdFin.Text = "";
            txtBCompañia.Text = txtContacto.Text = txtDomicilio.Text = txtCiudad.Text = txtRegion.Text = txtCodigoP.Text = "";
            cboBPais.SelectedIndex = 0;
            txtBTelefono.Text = txtBFax.Text = "";
        }

        private void BorrarDatosProveedor()
        {
            txtId.Text = txtCompañia.Text = txtContacto.Text = txtTitulo.Text = "";
            txtDomicilio.Text = txtCiudad.Text = txtRegion.Text = txtCodigoP.Text = "";
            txtTelefono.Text = txtFax.Text = "";
            cboPais.SelectedIndex = 0;
        }

        void txtBId_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtBId_Enter(object sender, EventArgs e) => ((TextBox)sender).SelectAll();

        void txtBId_Leave(object sender, EventArgs e)
        {
            // Castear el objeto que disparó el evento
            TextBox tb = sender as TextBox;
            if (tb == null) return; // seguridad
            if (tb == txtBIdIni)
                Utils.ValidaTxtBIdIni(txtBIdIni, txtBIdFin);
            else if (tb == txtBIdFin)
                Utils.ValidaTxtBIdFin(txtBIdIni, txtBIdFin);
        }

        private bool ValidarControles()
        {
            bool valida = true;
            if (txtCompañia.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtCompañia, "Ingrese el nombre de la compañía");
            }
            if (txtContacto.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtContacto, "Ingrese el nombre de contacto");
            }
            if (txtTitulo.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtTitulo, "Ingrese el título del contacto");
            }
            if (txtDomicilio.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtDomicilio, "Ingrese el domicilio");
            }
            if (txtCiudad.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtCiudad, "Ingrese la ciudad");
            }
            if (cboPais.Text.Trim() == "" || cboPais.SelectedIndex == 0)
            {
                valida = false;
                errorProvider1.SetError(cboPais, "Ingrese o seleccione el país");
            }
            if (txtTelefono.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtTelefono, "Ingrese el teléfono");
            }
            return valida;
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
            {
                DeshabilitarControles();
                DataGridViewRow dgvr = Dgv.CurrentRow;
                txtId.Text = dgvr.Cells["SupplierID"].Value.ToString();
                Supplier proveedor = new Supplier();
                try
                {
                    proveedor = SupplierBLL.ObtenerProveedorPorId(Convert.ToInt32(txtId.Text));
                    if (proveedor != null)
                    {
                        txtId.Tag = proveedor.RowVersion;
                        txtCompañia.Text = proveedor.CompanyName;
                        txtContacto.Text = proveedor.ContactName;
                        txtTitulo.Text = proveedor.ContactTitle;
                        txtDomicilio.Text = proveedor.Address;
                        txtCiudad.Text = proveedor.City;
                        txtRegion.Text = proveedor.Region;
                        txtCodigoP.Text = proveedor.PostalCode;
                        cboPais.Text = proveedor.Country;
                        txtTelefono.Text = proveedor.Phone;
                        txtFax.Text = proveedor.Fax;
                    }
                    else
                    {
                        U.NotificacionWarning($"No se encontró el proveedor con Id: {txtId.Text}." + Utils.erfep);
                        ActualizaDgv();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    U.MsgCatchOue(ex);
                }
                if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    HabilitarControles();
                    btnOperacion.Enabled = true;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                    btnOperacion.Enabled = true;
            }
            CargarValoresOriginales();
        }

        private void Dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // debe estar vinculado a la clase List<> a la cual esta vinculado el DataGridView.DataSource
            Utils.OrdenarPorColumna<Supplier>(Dgv, e);
        }

        void ActualizaDgv() => btnLimpiar.PerformClick();

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            BorrarMensajesError();
            BorrarDatosProveedor();
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                Dgv.CellClick -= new DataGridViewCellEventHandler(Dgv_CellClick);
                Dgv.CellClick -= new DataGridViewCellEventHandler(Dgv_CellClick);
                BorrarDatosBusqueda();
                HabilitarControles();
                btnOperacion.Text = "Registrar proveedor";
                btnOperacion.Visible = true;
                btnOperacion.Enabled = true;
            }
            else
            {
                Dgv.CellClick -= new DataGridViewCellEventHandler(Dgv_CellClick);
                Dgv.CellClick += new DataGridViewCellEventHandler(Dgv_CellClick);
                DeshabilitarControles();
                btnOperacion.Enabled = false;
                if (tabcOperacion.SelectedTab == tbpConsultar)
                    btnOperacion.Visible = false;
                else if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    btnOperacion.Text = "Modificar proveedor";
                    btnOperacion.Visible = true;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnOperacion.Text = "Eliminar proveedor";
                    btnOperacion.Visible = true;
                }
            }
            CargarValoresOriginales();
        }

        private void btnOperacion_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                if (ValidarControles())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
                    DeshabilitarControles();
                    btnOperacion.Enabled = false;
                    try
                    {
                        var proveedor = new Supplier
                        {
                            CompanyName = txtCompañia.Text.Trim(),
                            ContactName = txtContacto.Text.Trim(),
                            ContactTitle = txtTitulo.Text.Trim(),
                            Address = txtDomicilio.Text.Trim(),
                            City = txtCiudad.Text.Trim(),
                            Region = string.IsNullOrWhiteSpace(txtRegion.Text.Trim()) ? null : txtRegion.Text.Trim(),
                            PostalCode = string.IsNullOrWhiteSpace(txtCodigoP.Text.Trim()) ? null : txtCodigoP.Text.Trim(),
                            Country = cboPais.Text.Trim(),
                            Phone = txtTelefono.Text.Trim(),
                            Fax = string.IsNullOrWhiteSpace(txtFax.Text.Trim()) ? null : txtFax.Text.Trim()
                        };
                        int numRegs = SupplierBLL.Insertar(proveedor);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se insertaron {numRegs} registro(s)");
                        string idyNombreCompania = $"El proveedor con Id: {txtId.Text} - Nombre de compañía: {txtCompañia.Text}:";
                        if (numRegs > 0)
                        {
                            txtId.Text = proveedor.SupplierID.ToString();
                            idyNombreCompania = $"El proveedor con Id: {txtId.Text} - Nombre de compañía: {txtCompañia.Text}:";
                            U.NotificacionInformation(idyNombreCompania + Utils.srs);
                        }
                        else
                            U.NotificacionError(idyNombreCompania + Utils.nfrs);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    LlenarCboPais();
                    HabilitarControles();
                    btnOperacion.Enabled = true;
                    ActualizaDgv();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpModificar)
            {
                // Verificar si hubo cambios en el formulario
                if (!Utils.HayCambios(this, valoresOriginales))
                {
                    U.NotificacionWarning(Utils.ndc);
                    return; // Salir sin hacer UPDATE
                }
                if (ValidarControles())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.modificandoRegistro);
                    DeshabilitarControles();
                    btnOperacion.Enabled = false;
                    try
                    {
                        var proveedor = new Supplier
                        {
                            SupplierID = int.Parse(txtId.Text),
                            CompanyName = txtCompañia.Text.Trim(),
                            ContactName = txtContacto.Text.Trim(),
                            ContactTitle = txtTitulo.Text.Trim(),
                            Address = txtDomicilio.Text.Trim(),
                            City = txtCiudad.Text.Trim(),
                            Region = txtRegion.Text.Trim(),
                            PostalCode = txtCodigoP.Text.Trim(),
                            Country = cboPais.Text.Trim(),
                            Phone = txtTelefono.Text.Trim(),
                            Fax = txtFax.Text.Trim(),
                            RowVersion = txtId.Tag as byte[]
                        };
                        int numRegs = SupplierBLL.Actualizar(proveedor);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se actualizaron {(numRegs < 0 ? 0 : numRegs)} registro(s)");
                        string idyNombreCompania = $"El proveedor con Id: {txtId.Text} - Nombre de compañía: {txtCompañia.Text}:";
                        if (numRegs > 0)
                            U.NotificacionInformation(idyNombreCompania + Utils.sms);
                        else if (numRegs == -1)
                            U.NotificacionError(idyNombreCompania + Utils.nfmfe);
                        else if (numRegs == -2)
                            U.NotificacionError(idyNombreCompania + Utils.nfmfm);
                        else
                            U.NotificacionError(idyNombreCompania + Utils.nfmmd);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    LlenarCboPais();
                    ActualizaDgv();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpEliminar)
            {
                if (U.NotificacionQuestion($"[orange]¿Está seguro de eliminar el proveedor con Id: {txtId.Text} - Nombre de compañía: {txtCompañia.Text}?") == DialogResult.Yes)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
                    btnOperacion.Enabled = false;
                    try
                    {
                        Supplier proveedor = new Supplier
                        {
                            SupplierID = int.Parse(txtId.Text),
                            RowVersion = txtId.Tag as byte[]
                        };
                        int numRegs = SupplierBLL.Eliminar(proveedor);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se eliminaron {(numRegs < 0 ? 0 : numRegs)} registro(s)");
                        string idyNombre = $"El proveedor con Id: {txtId.Text} - Nombre de compañía: {txtCompañia.Text}:";
                        if (numRegs > 0)
                            U.NotificacionInformation(idyNombre + Utils.ses);
                        else if (numRegs == -1)
                            U.NotificacionError(idyNombre + Utils.nfefe);
                        else if (numRegs == -2)
                            U.NotificacionError(idyNombre + Utils.nfefm);
                        else
                            U.NotificacionError(idyNombre + Utils.nfemd);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    LlenarCboPais();
                    ActualizaDgv();
                }
            }
            CargarValoresOriginales();
        }

        private void CargarValoresOriginales()
        {
            valoresOriginales = Utils.CapturarValoresOriginales(this);
        }

        private void tabcOperacion_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrarPestaña) == DialogResult.No)
                    e.Cancel = true;
        }
    }
}
