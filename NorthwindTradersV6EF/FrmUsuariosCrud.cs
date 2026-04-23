using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmUsuariosCrud : Form
    {
        bool _imagenMostrada = true;
        internal Dictionary<string, object> valoresOriginales;

        public FrmUsuariosCrud()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmUsuariosCrud_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        internal void FrmUsuariosCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrar) == DialogResult.No)
                    e.Cancel = true;
        }

        private void tabcOperacion_DrawItem(object sender, DrawItemEventArgs e) => Utils.DibujarPestañas(sender as TabControl, e);

        private void FrmUsuariosCrud_Load(object sender, EventArgs e)
        {
            tabcOperacion.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabcOperacion.DrawItem += tabcOperacion_DrawItem;
            DeshabilitarControles();
            LlenarDgv(false);
            CargarValoresOriginales();
        }

        private void DeshabilitarControles()
        {
            txtPaterno.ReadOnly = txtMaterno.ReadOnly = txtNombres.ReadOnly = txtUsuario.ReadOnly = txtPwd.ReadOnly = txtConfirmarPwd.ReadOnly = true;
            lblFechaCaptura.Text = lblFechaModificacion.Text = string.Empty;
            chkbEstatus.Enabled = false;
            btnTogglePwd1.Enabled = false;
        }

        private void HabilitarControles()
        {
            txtPaterno.ReadOnly = txtMaterno.ReadOnly = txtNombres.ReadOnly = txtUsuario.ReadOnly = txtPwd.ReadOnly = txtConfirmarPwd.ReadOnly = false;
            chkbEstatus.Enabled = true;
            btnTogglePwd1.Enabled = true;
        }

        private void LlenarDgv(bool selectorRealizaBusqueda)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DTOs.EF.DtoUsuariosBuscar dtoUsuariosBuscar = new DTOs.EF.DtoUsuariosBuscar()
                {
                    IdIni = Convert.ToInt32(nudBIdIni.Value),
                    IdFin = Convert.ToInt32(nudBIdFin.Value),
                    Paterno = txtBPaterno.Text.Trim(),
                    Materno = txtBMaterno.Text.Trim(),
                    Nombres = txtBNombres.Text.Trim(),
                    Usuario = txtBUsuario.Text.Trim()
                };
                DataTable dt;
                if (!selectorRealizaBusqueda)
                    dtoUsuariosBuscar = null;
                dt = BLL.EF.UsuarioBLL.ObtenerUsuarios(dtoUsuariosBuscar);
                // Agrega una nueva columna "EstatusTexto" de tipo string
                dt.Columns.Add("EstatusTexto", typeof(string));

                // Llena la nueva columna con el texto equivalente
                foreach (DataRow row in dt.Rows)
                {
                    bool estatus = Convert.ToBoolean(row["Estatus"]);
                    row["EstatusTexto"] = estatus ? "Activo" : "Inactivo";
                }

                // Opcional: eliminar la columna original si ya no la necesitas
                dt.Columns.Remove("Estatus");

                // Opcional: renombrar la columna nueva para mantener el nombre original
                dt.Columns["EstatusTexto"].ColumnName = "Estatus";

                Dgv.DataSource = dt;

                Utils.ConfDgv(Dgv);
                ConfDgv();
                if (!selectorRealizaBusqueda)
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran los últimos {Dgv.RowCount} usuarios registrados");
                else
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {Dgv.RowCount} registros");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void ConfDgv()
        {
            Dgv.Columns["RowVersionStr"].Visible = false;

            Dgv.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Paterno"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Materno"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Nombres"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Usuario"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Password"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["FechaCaptura"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["FechaModificacion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Estatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["Usuario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Password"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["FechaCaptura"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["FechaModificacion"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Estatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Dgv.Columns["Paterno"].HeaderText = "Apellido Paterno";
            Dgv.Columns["Materno"].HeaderText = "Apellido Materno";
            Dgv.Columns["Password"].HeaderText = "Contraseña";
            Dgv.Columns["FechaCaptura"].HeaderText = "Fecha de creación";
            Dgv.Columns["FechaModificacion"].HeaderText = "Fecha de modificación";

            Dgv.Columns["FechaCaptura"].DefaultCellStyle.Format = "dd/MMMM/yyyy\nhh:mm:ss tt";
            Dgv.Columns["FechaModificacion"].DefaultCellStyle.Format = "dd/MMMM/yyyy\nhh:mm:ss tt";
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarDatosUsuario();
            BorrarMensajesError();
            BorrarDatosBusqueda();
            PonerNoVisibleBtnTogglePwd1();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(false);
            CargarValoresOriginales();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosUsuario();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(true);
            CargarValoresOriginales();
        }

        private void BorrarDatosUsuario()
        {
            txtId.Text = txtPaterno.Text = txtMaterno.Text = txtNombres.Text = txtUsuario.Text = txtPwd.Text = txtConfirmarPwd.Text = string.Empty;
            chkbEstatus.Checked = false;
            lblFechaCaptura.Text = lblFechaModificacion.Text = string.Empty;
        }

        private void BorrarMensajesError() => errorProvider1.Clear();

        private void BorrarDatosBusqueda()
        {
            nudBIdFin.Value = nudBIdIni.Value = 0;
            txtBPaterno.Text = txtBMaterno.Text = txtBNombres.Text = txtBUsuario.Text = string.Empty;
        }

        private bool ValidarControles()
        {
            bool valida = true;
            if (string.IsNullOrWhiteSpace(txtNombres.Text))
            {
                errorProvider1.SetError(txtNombres, "El nombre es obligatorio");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                errorProvider1.SetError(txtUsuario, "El usuario es obligatorio");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtPwd.Text))
            {
                errorProvider1.SetError(txtPwd, "La contraseña es obligatoria");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtConfirmarPwd.Text))
            {
                errorProvider1.SetError(txtConfirmarPwd, "La confirmación de la contraseña es obligatoria");
                valida = false;
            }
            if (valida)
            {
                // Validar que el usuario no exista en la base de datos
                if (BLL.EF.UsuarioBLL.ValidarExisteUsuario(txtUsuario.Text.Trim()))
                {
                    errorProvider1.SetError(txtUsuario, "El usuario ya existe, por favor elige otro");
                    valida = false;
                }
                // Validar que las contraseñas coincidan
                if (txtPwd.Text != txtConfirmarPwd.Text)
                {
                    errorProvider1.SetError(txtPwd, "Las contraseñas no coinciden");
                    errorProvider1.SetError(txtConfirmarPwd, "Las contraseñas no coinciden");
                    valida = false;
                }
            }
            return valida;
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            if (tabcOperacion.SelectedTab != tbpRegistrar) 
            {
                PonerNoVisibleBtnTogglePwd1();
                DeshabilitarControles();
                DataGridViewRow dgvr = Dgv.CurrentRow;
                if (dgvr != null)
                {
                    txtId.Text = dgvr.Cells["Id"].Value.ToString();
                    txtId.Tag = dgvr.Cells["RowVersionStr"].Value;
                    txtPaterno.Text = dgvr.Cells["Paterno"].Value.ToString();
                    txtMaterno.Text = dgvr.Cells["Materno"].Value.ToString();
                    txtNombres.Text = dgvr.Cells["Nombres"].Value.ToString();
                    txtUsuario.Text = dgvr.Cells["Usuario"].Value.ToString();
                    txtPwd.Text = dgvr.Cells["Password"].Value.ToString();
                    txtConfirmarPwd.Text = txtPwd.Text; // Para que coincidan al editar
                    if (dgvr.Cells["FechaCaptura"].Value != null)
                        lblFechaCaptura.Text = Convert.ToDateTime(dgvr.Cells["FechaCaptura"].Value).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                    else
                        lblFechaCaptura.Text = "Nulo";
                    if (dgvr.Cells["FechaModificacion"].Value != null)
                        lblFechaModificacion.Text = Convert.ToDateTime(dgvr.Cells["FechaModificacion"].Value).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                    else
                        lblFechaModificacion.Text = "Nulo";
                    chkbEstatus.Checked = dgvr.Cells["Estatus"].Value?.ToString() == "Activo";
                    txtPwd.Text.Trim(); // Almacena la contraseña hasheada antes de modificarla
                }
                if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    HabilitarControles();
                    btnOperacion.Enabled = true;
                    btnTogglePwd1.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                    btnOperacion.Enabled = true;
            }
            CargarValoresOriginales();
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (Dgv.Columns[e.ColumnIndex].Name == "Password" && e.Value != null)
                e.Value = new string('●', 10);
            string estado = e.Value.ToString();
            if (estado == "Activo")
            {
                e.CellStyle.BackColor = Color.LightGreen;
                e.CellStyle.ForeColor = Color.Black;
            }
            else if (estado == "Inactivo")
            {
                e.CellStyle.BackColor = Color.Red;
                e.CellStyle.ForeColor = Color.White;
            }
        }

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            BorrarDatosUsuario();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                Dgv.CellClick -= Dgv_CellClick; 
                Dgv.CellClick -= Dgv_CellClick; 
                BorrarDatosBusqueda();
                HabilitarControles();
                btnOperacion.Text = "Registrar usuario";
                btnOperacion.Visible = true;
                btnOperacion.Enabled = true;
            }
            else
            {
                Dgv.CellClick -= Dgv_CellClick; 
                Dgv.CellClick += Dgv_CellClick; 
                DeshabilitarControles();
                btnOperacion.Enabled = false;
                if (tabcOperacion.SelectedTab == tbpConsultar)
                {
                    btnOperacion.Visible = false;
                    btnOperacion.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    btnOperacion.Text = "Modificar usuario";
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = false;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnOperacion.Text = "Eliminar usuario";
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = false;
                }
            }
            CargarValoresOriginales();
        }

        private void btnOperacion_Click(object sender, EventArgs e)
        {
            sbyte numRegs = 0;
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                if (ValidarControles())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
                    DeshabilitarControles();
                    PonerNoVisibleBtnTogglePwd1();
                    btnOperacion.Enabled = false;
                    try
                    {
                        string passHasheada = Utils.ComputeSha256Hash(txtPwd.Text.Trim());
                        var usuario = new DAL.EF.Usuario
                        {
                            Paterno = txtPaterno.Text.Trim(),
                            Materno = txtMaterno.Text.Trim(),
                            Nombres = txtNombres.Text.Trim(),
                            Usuario1 = txtUsuario.Text.Trim(),
                            Password = passHasheada,
                            FechaCaptura = DateTime.Now,
                            FechaModificacion = DateTime.Now,
                            Estatus = chkbEstatus.Checked
                        };
                        numRegs = (sbyte)BLL.EF.UsuarioBLL.Insertar(usuario);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se insertaron {(numRegs < 0 ? 0 : numRegs)} registro(s)");
                        string nombreUsuario = $"El usuario con Id: {txtId.Text} y Nombre: " + $"{txtPaterno.Text.Trim()} {txtMaterno.Text.Trim()} {txtNombres.Text.Trim()}".Trim();
                        if (numRegs > 0)
                        {
                            txtId.Text = usuario.Id.ToString();
                            txtId.Tag = usuario.RowVersionStr;
                            lblFechaCaptura.Text = Convert.ToDateTime(usuario.FechaCaptura).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                            lblFechaModificacion.Text = Convert.ToDateTime(usuario.FechaModificacion).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                            U.NotificacionInformation(nombreUsuario + Utils.srs);
                        }
                        else
                            U.NotificacionError(nombreUsuario + Utils.nfrs);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    HabilitarControles();
                    btnOperacion.Enabled = true;
                    btnLimpiar.PerformClick();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpModificar)
            {
                if (!Utils.HayCambios(this, valoresOriginales))
                {
                    U.NotificacionWarning(Utils.ndc);
                    return;
                }
                string usuarioOld = Utils.GetValorOriginal(txtUsuario.Name, valoresOriginales);
                string pwdTextoOriginal = Utils.GetValorOriginal("txtPwd", valoresOriginales);     // texto plano original (si lo guardaste así)
                string passHasheadaOld = Utils.GetValorOriginal("txtPwdHash", valoresOriginales); // hash original que ya estaba en BD
                if ((txtUsuario.Text.Trim() == usuarioOld && ValidarControles1()) || (txtUsuario.Text.Trim() != usuarioOld && ValidarControles()))
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.modificandoRegistro);
                    DeshabilitarControles();
                    PonerNoVisibleBtnTogglePwd1();
                    btnOperacion.Enabled = false;
                    try
                    {
                        // Si el texto actual es igual al texto original → conserva el hash original
                        // Si cambió → genera hash nuevo
                        string passFinal = (txtPwd.Text.Trim() == pwdTextoOriginal)
                                           ? passHasheadaOld
                                           : Utils.ComputeSha256Hash(txtPwd.Text.Trim());
                        var usuario = new DAL.EF.Usuario
                        {
                            Id = Convert.ToInt32(txtId.Text.Trim()),
                            RowVersion = DAL.EF.Usuario.ConvertirRowVersion(txtId.Tag),
                            Paterno = txtPaterno.Text.Trim(),
                            Materno = txtMaterno.Text.Trim(),
                            Nombres = txtNombres.Text.Trim(),
                            Usuario1 = txtUsuario.Text.Trim(),
                            Password = passFinal,
                            FechaModificacion = DateTime.Now,
                            Estatus = chkbEstatus.Checked
                        };
                        numRegs = BLL.EF.UsuarioBLL.Actualizar(usuario);
                        txtId.Tag = usuario.RowVersionStr; // Actualiza el RowVersion en el Tag del TextBox
                        MDIPrincipal.ActualizarBarraDeEstado($"Se actualizaron {(numRegs < 0 ? 0 : numRegs)} registro(s)");
                        string nombreUsuario = $"El usuario con Id: {txtId.Text} y Nombre: " + $"{txtPaterno.Text.Trim()} {txtMaterno.Text.Trim()} {txtNombres.Text.Trim()}".Trim();
                        if (numRegs > 0)
                        {
                            lblFechaModificacion.Text = Convert.ToDateTime(usuario.FechaModificacion).ToString("dd/MMMM/yyyy hh:mm:ss tt");
                            U.NotificacionInformation(nombreUsuario + Utils.sms);
                        }
                        else if (numRegs == -1)
                            U.NotificacionError(nombreUsuario + Utils.nfmfe);
                        else if (numRegs == -2)
                            U.NotificacionError(nombreUsuario + Utils.nfmfm);
                        else
                            U.NotificacionError(nombreUsuario + Utils.nfmmd);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    btnLimpiar.PerformClick();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpEliminar)
            {
                if (U.NotificacionQuestion($"¿Está seguro de eliminar el usuario con Id: {txtId.Text} y Nombre: " + $"{txtPaterno.Text} {txtMaterno.Text} {txtNombres.Text}".Trim() + ", considere que también se eliminaran los permisos que se le hayan concedido en el sistema?") == DialogResult.Yes)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
                    btnOperacion.Enabled = false;
                    try
                    {
                        var usuario = new DAL.EF.Usuario
                        {
                            Id = Convert.ToInt32(txtId.Text.Trim()),
                            RowVersion = DAL.EF.Usuario.ConvertirRowVersion(txtId.Tag)
                        };
                        numRegs = BLL.EF.UsuarioBLL.Eliminar(usuario);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se eliminaron {(numRegs < 0 ? 0 : numRegs)} registro(s)");
                        string nombreUsuario = $"El usuario con Id: {txtId.Text} y Nombre: " + $"{txtPaterno.Text.Trim()} {txtMaterno.Text.Trim()} {txtNombres.Text.Trim()}".Trim();
                        if (numRegs > 0)
                            U.NotificacionInformation(nombreUsuario + Utils.ses);
                        else if (numRegs == -1)
                            U.NotificacionError(nombreUsuario + Utils.nfefe);
                        else if (numRegs == -2)
                            U.NotificacionError(nombreUsuario + Utils.nfefm);
                        else
                            U.NotificacionError(nombreUsuario + Utils.nfemd);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                }
                btnLimpiar.PerformClick();
            }
            CargarValoresOriginales();
        }

        private void btnTogglePwd1_Click(object sender, EventArgs e)
        {
            _imagenMostrada = !_imagenMostrada;
            txtPwd.UseSystemPasswordChar = !txtPwd.UseSystemPasswordChar;
            txtConfirmarPwd.UseSystemPasswordChar = !txtConfirmarPwd.UseSystemPasswordChar;
            btnTogglePwd1.Image = _imagenMostrada ? Properties.Resources.mostrarCh : Properties.Resources.ocultarCh;
        }

        private void PonerNoVisibleBtnTogglePwd1()
        {
            txtPwd.UseSystemPasswordChar = txtConfirmarPwd.UseSystemPasswordChar = true;
            btnTogglePwd1.Image = Properties.Resources.mostrarCh;
        }

        private bool ValidarControles1()
        {
            bool valida = true;
            if (string.IsNullOrWhiteSpace(txtNombres.Text))
            {
                errorProvider1.SetError(txtNombres, "El nombre es obligatorio");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                errorProvider1.SetError(txtUsuario, "El usuario es obligatorio");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtPwd.Text))
            {
                errorProvider1.SetError(txtPwd, "La contraseña es obligatoria");
                valida = false;
            }
            if (string.IsNullOrWhiteSpace(txtConfirmarPwd.Text))
            {
                errorProvider1.SetError(txtConfirmarPwd, "La confirmación de la contraseña es obligatoria");
                valida = false;
            }
            if (valida)
            {
                // Validar que las contraseñas coincidan
                if (txtPwd.Text != txtConfirmarPwd.Text)
                {
                    errorProvider1.SetError(txtPwd, "Las contraseñas no coinciden");
                    errorProvider1.SetError(txtConfirmarPwd, "Las contraseñas no coinciden");
                    valida = false;
                }
            }
            return valida;
        }

        private void txtEnter(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                // Seleccionar todo el texto al recibir el foco
                this.BeginInvoke((Action)(() => tb.SelectAll()));
            }
        }

        private void nud_Enter(object sender, EventArgs e)
        {
            if (sender is NumericUpDown nud && nud.Controls[1] is TextBox tb)
            {
                // Diferir la selección para que ocurra después de que el TextBox reciba el foco
                tb.BeginInvoke((Action)(() => tb.SelectAll()));
            }
        }

        private void nudBIdIni_Leave(object sender, EventArgs e) => Utils.ValidarRango(sender, nudBIdIni, nudBIdFin);

        private void nudBIdFin_Leave(object sender, EventArgs e) => Utils.ValidarRango(sender, nudBIdIni, nudBIdFin);

        private void nudBIdIni_ValueChanged(object sender, EventArgs e) => Utils.ValidarRango(sender, nudBIdIni, nudBIdFin);

        private void nudBIdFin_ValueChanged(object sender, EventArgs e) => Utils.ValidarRango(sender, nudBIdIni, nudBIdFin);

        private void Dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var col = Dgv.Columns[e.ColumnIndex];
            // Si la columna es Password, no hacer nada
            if (col.Name == "Password" || col.DataPropertyName == "Password")
                return;
            Utils.OrdenarPorColumna(Dgv, e);
        }

        private void txtPwd_MouseUp(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void txtConfirmarPwd_MouseUp(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void txtPwd_TextChanged(object sender, EventArgs e)
        {
            if (tabcOperacion.SelectedTab == tbpModificar)
            {
                string passHasheadaOld = Utils.GetValorOriginal(txtPwd.Name, valoresOriginales);
                string passHasheadaActual = Utils.ComputeSha256Hash(txtPwd.Text.Trim());
                btnTogglePwd1.Enabled = passHasheadaActual != passHasheadaOld;
            }
        }

        private void txtConfirmarPwd_TextChanged(object sender, EventArgs e)
        {
            if (tabcOperacion.SelectedTab == tbpModificar)
            {

                string passHasheadaOld = Utils.GetValorOriginal(txtConfirmarPwd.Name, valoresOriginales);
                string passHasheadaActual = Utils.ComputeSha256Hash(txtConfirmarPwd.Text.Trim());
                btnTogglePwd1.Enabled = passHasheadaActual != passHasheadaOld;
            }
        }

        private void CargarValoresOriginales()
        {
            // Captura inicial usando la utilidad
            valoresOriginales = Utils.CapturarValoresOriginales(this);
            // Guardar directamente la cadena hasheada que ya está en el TextBox
            valoresOriginales["txtPwdHash"] = txtPwd.Text.Trim();
        }

        private void tabcOperacion_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrarPestaña) == DialogResult.No)
                    e.Cancel = true;
        }
    }
}
