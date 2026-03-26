using BLL;
using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmEmpleadosCrud : Form
    {

        string _connectionString = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private EmpleadoBLL _empleadoBLL;
        private bool EjecutarConfDgv = true;
        bool EventoCargado = true;
        OpenFileDialog openFileDialog;
        internal Dictionary<string, object> valoresOriginales;
        private byte[] fotoOriginalOle = null;

        public FrmEmpleadosCrud()
        {
            InitializeComponent();
            _empleadoBLL = new EmpleadoBLL(_connectionString);
        }

        private void FrmEmpleadosCrud_Load(object sender, EventArgs e)
        {
            tabcOperacion.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabcOperacion.DrawItem += tabcOperacion_DrawItem;
            panel1.AutoScrollMinSize = new Size(1000, 800);
            DeshabilitarControles();
            LlenarCboPais();
            LlenarCboReportaA();
            Utils.ConfDgv(dgv);
            LlenarDgv(false);
            CargarValoresOriginales();
        }

        private void tabcOperacion_DrawItem(object sender, DrawItemEventArgs e) => Utils.DibujarPestañas(sender as TabControl, e);

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmEmpleadosCrud_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        internal void FrmEmpleadosCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            // pone un error con errorprovider en cada control que ha cambiado
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrar) == DialogResult.No)
                    e.Cancel = true;
        }

        private void DeshabilitarControles()
        {
            txtNombres.ReadOnly = txtApellidos.ReadOnly = txtTitulo.ReadOnly = txtTitCortesia.ReadOnly = true;
            txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCodigoP.ReadOnly = true;
            txtPais.ReadOnly = txtTelefono.ReadOnly = txtExtension.ReadOnly = true;
            dtpFNacimiento.Enabled = dtpFContratacion.Enabled = false;
            txtNotas.ReadOnly = true;
            cboReportaA.Enabled = false;
            picFoto.Enabled = false;
            btnCargar.Enabled = false;
            txtNotas.BackColor = SystemColors.Control;
        }

        private void HabilitarControles()
        {
            txtNombres.ReadOnly = txtApellidos.ReadOnly = txtTitulo.ReadOnly = false;
            txtTitCortesia.ReadOnly = false;
            txtDomicilio.ReadOnly = txtCiudad.ReadOnly = txtRegion.ReadOnly = txtCodigoP.ReadOnly = false;
            txtPais.ReadOnly = txtTelefono.ReadOnly = txtExtension.ReadOnly = false;
            txtNotas.ReadOnly = false;
            dtpFNacimiento.Enabled = dtpFContratacion.Enabled = cboReportaA.Enabled = true;
            picFoto.Enabled = true;
            txtNotas.BackColor = SystemColors.Window;
            //btnCargar.Enabled = true;  // no se debe habilitar este control para los registros 1 al 8
        }

        private void LlenarCboPais()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var paises = _empleadoBLL.ObtenerEmpleadosPaisesCbo();
                cboBPais.DataSource = paises;
                cboBPais.ValueMember = "Id";
                cboBPais.DisplayMember = "Pais";
                cboBPais.SelectedIndex = 0;
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void LlenarCboReportaA()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var empleados = _empleadoBLL.ObtenerEmpleadoReportaaCbo();
                cboReportaA.DataSource = empleados;
                cboReportaA.ValueMember = "Id";
                cboReportaA.DisplayMember = "Nombre";
                cboReportaA.SelectedIndex = 0;
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void LlenarDgv(bool selectorRealizaBusqueda)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DtoEmpleadosBuscar criterios = new DtoEmpleadosBuscar
                {
                    IdIniTxt = txtBIdIni.Text,
                    IdFinTxt = txtBIdFin.Text,
                    Nombres = txtBNombres.Text.Trim(),
                    Apellidos = txtBApellidos.Text.Trim(),
                    Titulo = txtBTitulo.Text.Trim(),
                    Domicilio = txtBDomicilio.Text.Trim(),
                    Ciudad = txtBCiudad.Text.Trim(),
                    Region = txtBRegion.Text.Trim(),
                    CodigoP = txtBCodigoP.Text.Trim(),
                    Pais = cboBPais.SelectedValue.ToString(),
                    Telefono = txtBTelefono.Text.Trim()
                };
                var resultado = _empleadoBLL.ObtenerEmpleadosDgv(selectorRealizaBusqueda, criterios);
                dgv.DataSource = resultado.empleados;
                if (EjecutarConfDgv)
                {
                    ConfDgv();
                    EjecutarConfDgv = false;
                }
                MDIPrincipal.ActualizarBarraDeEstado(resultado.mensajeEstado);
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        void ConfDgv()
        {
            dgv.Columns["EmployeeID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["BirthDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["City"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Country"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["ReportsToName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgv.Columns["Photo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgv.Columns["Photo"].Width = 80;
            dgv.RowTemplate.Height = 80;
            dgv.Columns["Photo"].DefaultCellStyle.Padding = new Padding(4);
            ((DataGridViewImageColumn)dgv.Columns["Photo"]).ImageLayout = DataGridViewImageCellLayout.Zoom;

            dgv.Columns["Title"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["BirthDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["City"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Country"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["ReportsToName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv.Columns["BirthDate"].DefaultCellStyle.Format = "dd \" de \"MMM\" de \"yyyy";

            dgv.Columns["EmployeeID"].HeaderText = "Id";
            dgv.Columns["FirstName"].HeaderText = "Nombres";
            dgv.Columns["LastName"].HeaderText = "Apellidos";
            dgv.Columns["Title"].HeaderText = "Título";
            dgv.Columns["BirthDate"].HeaderText = "Fecha de nacimiento";
            dgv.Columns["City"].HeaderText = "Ciudad";
            dgv.Columns["Country"].HeaderText = "País";
            dgv.Columns["Photo"].HeaderText = "Foto";
            dgv.Columns["ReportsToName"].HeaderText = "Reporta a";
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosEmpleado();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(true);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            BorrarDatosBusqueda();
            BorrarDatosEmpleado();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(false);
        }

        void BorrarMensajesError() => errorProvider1.Clear();

        void BorrarDatosBusqueda()
        {
            txtBIdIni.Text = txtBIdFin.Text = txtBNombres.Text = txtBApellidos.Text = string.Empty;
            txtBTitulo.Text = txtBDomicilio.Text = txtBCiudad.Text = string.Empty;
            txtBRegion.Text = txtBCodigoP.Text = txtBTelefono.Text = string.Empty;
            cboBPais.SelectedIndex = 0;
        }

        void BorrarDatosEmpleado()
        {
            txtId.Text = txtNombres.Text = txtApellidos.Text = txtTitulo.Text = string.Empty;
            txtTitCortesia.Text = txtDomicilio.Text = txtCiudad.Text = string.Empty;
            txtRegion.Text = txtCodigoP.Text = txtTelefono.Text = txtPais.Text = string.Empty;
            txtExtension.Text = txtNotas.Text = string.Empty;
            cboReportaA.SelectedIndex = 0;
            picFoto.Image = Properties.Resources.FotoPerfil;
            dtpFNacimiento.Value = dtpFNacimiento.MinDate;
            dtpFContratacion.Value = dtpFContratacion.MinDate;
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
            if (txtNombres.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtNombres, "Ingrese el nombre");
            }
            if (txtApellidos.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtApellidos, "Ingrese el apellido");
            }
            if (txtTitulo.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtTitulo, "Ingrese el título");
            }
            if (txtTitCortesia.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtTitCortesia, "Ingrese el título de cortesia");
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
            if (txtPais.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtPais, "Ingrese el país");
            }
            if (txtTelefono.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtTelefono, "Ingrese el teléfono");
            }
            if (picFoto.Image == null)
            {
                valida = false;
                errorProvider1.SetError(btnCargar, "Ingrese la foto");
            }
            if (dtpFNacimiento.Value == new DateTime(1753, 1, 1))
            {
                valida = false;
                errorProvider1.SetError(dtpFNacimiento, "Ingrese la fecha de nacimiento");
            }
            if (dtpFContratacion.Value == new DateTime(1753, 1, 1))
            {
                valida = false;
                errorProvider1.SetError(dtpFContratacion, "Ingrese la fecha de contratación");
            }
            if (cboReportaA.SelectedValue == null || cboReportaA.SelectedIndex == 0)
            {
                valida = false;
                errorProvider1.SetError(cboReportaA, "Seleccione a quien reporta el empleado");
            }
            return valida;
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
            {
                DeshabilitarControles();
                DataGridViewRow dgvr = dgv.CurrentRow;
                txtId.Text = dgvr.Cells["EmployeeID"].Value.ToString();
                Empleado empleado = new Empleado();
                try
                {
                    empleado = _empleadoBLL.ObtenerEmpleadoPorId(Convert.ToInt32(txtId.Text));
                    if (empleado != null)
                    {
                        if (empleado.BirthDate != null)
                            dtpFNacimiento.Value = empleado.BirthDate.Value;
                        else
                            dtpFNacimiento.Value = dtpFNacimiento.MinDate;
                        if (empleado.HireDate != null)
                            dtpFContratacion.Value = empleado.HireDate.Value;
                        else
                            dtpFContratacion.Value = dtpFContratacion.MinDate;
                        if (empleado.Photo != null)
                        {
                            fotoOriginalOle = empleado.Photo;
                            if (empleado.EmployeeID <= 9)
                                btnCargar.Enabled = false;
                            else
                                btnCargar.Enabled = true;
                            using (var ms = new MemoryStream(empleado.Photo))
                                picFoto.Image = Image.FromStream(ms);
                        }
                        else
                        {
                            picFoto.Image = null;
                            btnCargar.Enabled = true;
                        }
                        if (empleado.ReportsTo != null)
                            cboReportaA.SelectedValue = empleado.ReportsTo.Value;
                        else
                            cboReportaA.SelectedValue = 0; // corresponde a N/A
                        txtId.Tag = empleado.RowVersion;
                        txtNombres.Text = empleado.FirstName;
                        txtApellidos.Text = empleado.LastName;
                        txtTitulo.Text = empleado.Title;
                        txtTitCortesia.Text = empleado.TitleOfCourtesy;
                        txtDomicilio.Text = empleado.Address;
                        txtCiudad.Text = empleado.City;
                        txtRegion.Text = empleado.Region;
                        txtCodigoP.Text = empleado.PostalCode;
                        txtPais.Text = empleado.Country;
                        txtTelefono.Text = empleado.HomePhone;
                        txtExtension.Text = empleado.Extension;
                        txtNotas.Text = empleado.Notes;
                    }
                    else
                    {
                        U.NotificacionWarning($"No se encontró el empleado con Id: {txtId.Text}." + Utils.erfep);
                        ActualizaDgv();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    U.MsgCatchOue(ex);
                }
                if (tabcOperacion.SelectedTab == tbpListar)
                {
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = true;
                    btnCargar.Visible = false;
                }
                if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    HabilitarControles();
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = true;
                    btnCargar.Visible = true;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnOperacion.Enabled = true;
                    btnOperacion.Visible = true;
                    btnCargar.Visible = false;
                }
            }
            CargarValoresOriginales();
        }

        private void dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // debe estar vinculado a la clase List<> a la cual esta vinculado el DataGridView.DataSource
            Utils.OrdenarPorColumna<DtoEmpleadosDgv>(dgv, e);
        }

        void ActualizaDgv() => btnLimpiar.PerformClick();

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            BorrarDatosEmpleado();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                if (EventoCargado)
                {
                    dgv.CellClick -= new DataGridViewCellEventHandler(dgv_CellClick);
                    EventoCargado = false;
                }
                BorrarDatosBusqueda();
                HabilitarControles();
                btnOperacion.Text = "Registrar empleado";
                btnOperacion.Visible = true;
                btnOperacion.Enabled = true;
                btnCargar.Enabled = true;
                btnCargar.Visible = true;
                cboReportaA.SelectedIndex = 0;
            }
            else
            {
                if (!EventoCargado)
                {
                    dgv.CellClick += new DataGridViewCellEventHandler(dgv_CellClick);
                    EventoCargado = true;
                }
                DeshabilitarControles();
                btnOperacion.Enabled = false;
                btnCargar.Enabled = false;
                if (tabcOperacion.SelectedTab == tbpListar)
                {
                    btnOperacion.Text = "Imprimir empleado";
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = false;
                    btnCargar.Visible = false;
                    btnCargar.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    btnOperacion.Text = "Modificar empleado";
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = false;
                    btnCargar.Visible = true;
                    btnCargar.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnOperacion.Text = "Eliminar empleado";
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = false;
                    btnCargar.Visible = false;
                    btnCargar.Enabled = false;
                }
            }
            CargarValoresOriginales();
        }

        private void tabcOperacion_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrarPestaña) == DialogResult.No)
                    e.Cancel = true;
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            // Mostrar el cuadro de diálogo OpenFileDialog
            //La instrucción siguiente es para que nos muestre todos los tipos juntos
            openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Archivos de imagen (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.InitialDirectory = "c:\\Imágenes\\";
            //La instrucción siguiente es para que nos muestre varias filas en el openfiledialog que nos permita abrir por un tipo especifico
            openFileDialog.Filter = "Archivos jpg (*.jpg)|*.jpg|Archivos jpeg (*.jpeg)|*.jpeg|Archivos png (*.png)|*.png|Archivos bmp (*.bmp)|*.bmp";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Cargar la imagen seleccionada en un objeto Image
                Image image = Image.FromFile(openFileDialog.FileName);

                // Mostrar la imagen en un control PictureBox
                picFoto.Image = image;
                errorProvider1.SetError(btnCargar, "");
            }
        }

        private void LlenarCombos()
        {
            LlenarCboPais();
            LlenarCboReportaA();
        }

        private void btnOperacion_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpListar)
            {
                FrmRptEmpleado frmRptEmpleado = new FrmRptEmpleado();
                frmRptEmpleado.Id = int.Parse(txtId.Text);
                frmRptEmpleado.ShowDialog();
                return;
            }
            else if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                if (ValidarControles())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
                    DeshabilitarControles();
                    btnOperacion.Enabled = false;
                    try
                    {
                        var empleado = new Empleado
                        {
                            FirstName = txtNombres.Text.Trim(),
                            LastName = txtApellidos.Text.Trim(),
                            Title = txtTitulo.Text.Trim(),
                            TitleOfCourtesy = txtTitCortesia.Text.Trim(),
                            BirthDate = dtpFNacimiento.Value == dtpFNacimiento.MinDate ? (DateTime?)null : dtpFNacimiento.Value,
                            HireDate = dtpFContratacion.Value == dtpFContratacion.MinDate ? (DateTime?)null : dtpFContratacion.Value,
                            Address = txtDomicilio.Text.Trim(),
                            City = txtCiudad.Text.Trim(),
                            Region = txtRegion.Text.Trim(),
                            PostalCode = txtCodigoP.Text.Trim(),
                            Country = txtPais.Text.Trim(),
                            HomePhone = txtTelefono.Text.Trim(),
                            Extension = txtExtension.Text.Trim(),
                            Notes = txtNotas.Text.Trim(),
                            ReportsTo = cboReportaA.SelectedValue.ToString() == "0" ? (int?)null : Convert.ToInt32(cboReportaA.SelectedValue),
                            Photo = picFoto.Image != null ? Utils.ImageToByteArray(picFoto.Image) : null
                        };
                        int numRegs = _empleadoBLL.Insertar(empleado);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se insertaron {numRegs} registros");
                        string idyNombre = $"El empleado con Id: {txtId.Text} - Nombre: {txtNombres.Text} {txtApellidos.Text}:";
                        if (numRegs > 0)
                        {
                            txtId.Text = empleado.EmployeeID.ToString();
                            idyNombre = $"El empleado con Id: {txtId.Text} - Nombre: {txtNombres.Text} {txtApellidos.Text}:";
                            U.NotificacionInformation(idyNombre + Utils.srs);
                        }
                        else
                        {
                            U.NotificacionError(idyNombre + Utils.nfrs);
                        }
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    HabilitarControles();
                    btnOperacion.Enabled = true;
                    btnCargar.Enabled = true;
                    LlenarCombos();
                    ActualizaDgv();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpModificar)
            {
                if (ValidarControles())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.modificandoRegistro);
                    DeshabilitarControles();
                    btnOperacion.Enabled = false;
                    try
                    {
                        var empleado = new Empleado
                        {
                            EmployeeID = Convert.ToInt32(txtId.Text),
                            FirstName = txtNombres.Text.Trim(),
                            LastName = txtApellidos.Text.Trim(),
                            Title = txtTitulo.Text.Trim(),
                            TitleOfCourtesy = txtTitCortesia.Text.Trim(),
                            BirthDate = dtpFNacimiento.Value == dtpFNacimiento.MinDate ? (DateTime?)null : dtpFNacimiento.Value,
                            HireDate = dtpFContratacion.Value == dtpFContratacion.MinDate ? (DateTime?)null : dtpFContratacion.Value,
                            Address = txtDomicilio.Text.Trim(),
                            City = txtCiudad.Text.Trim(),
                            Region = txtRegion.Text.Trim(),
                            PostalCode = txtCodigoP.Text.Trim(),
                            Country = txtPais.Text.Trim(),
                            HomePhone = txtTelefono.Text.Trim(),
                            Extension = txtExtension.Text.Trim(),
                            Notes = txtNotas.Text.Trim(),
                            ReportsTo = cboReportaA.SelectedValue.ToString() == "0" ? (int?)null : Convert.ToInt32(cboReportaA.SelectedValue),
                            RowVersion = txtId.Tag as byte[]
                        };
                        if (Convert.ToInt32(txtId.Text) <= 9)
                        {
                            empleado.Photo = fotoOriginalOle; // conservas el OLE original
                        }
                        else
                        {
                            empleado.Photo = Utils.ImageToByteArray(picFoto.Image);
                        }
                        int numRegs = _empleadoBLL.Actualizar(empleado);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se actualizaron {(numRegs < 0 ? 0 : numRegs)} registros");
                        string idyNombre = $"El empleado con Id: {txtId.Text} - Nombre: {txtNombres.Text} {txtApellidos.Text}:";
                        if (numRegs > 0)
                            U.NotificacionInformation(idyNombre + Utils.sms);
                        else if (numRegs == -1)
                            U.NotificacionError(idyNombre + Utils.nfmfe);
                        else if (numRegs == -2)
                            U.NotificacionError(idyNombre + Utils.nfmfm);
                        else
                           U.NotificacionError(idyNombre + Utils.nfmmd);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    LlenarCombos();
                    ActualizaDgv();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpEliminar)
            {
                if (U.NotificacionQuestion($"[orange]¿Está seguro de eliminar el empleado con Id: {txtId.Text} - Nombre: {txtNombres.Text} {txtApellidos.Text}?") == DialogResult.Yes)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
                    btnOperacion.Enabled = false;
                    try
                    {
                        int numRegs = _empleadoBLL.Eliminar(Convert.ToInt32(txtId.Text), (byte[])txtId.Tag);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se eliminaron {(numRegs < 0 ? 0 : numRegs)} registros");
                        string idyNombre = $"El empleado con Id: {txtId.Text} - Nombre: {txtNombres.Text} {txtApellidos.Text}:";
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
                    LlenarCombos();
                    ActualizaDgv();
                }
            }
            CargarValoresOriginales();
        }

        private void CargarValoresOriginales()
        {
            valoresOriginales = Utils.CapturarValoresOriginales(this);
        }
    }
}
