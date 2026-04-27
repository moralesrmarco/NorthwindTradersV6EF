using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmPermisosCrud : Form
    {
        public FrmPermisosCrud()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void GrbPaint2(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmPermisosCrud_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmPermisosCrud_Load(object sender, EventArgs e)
        {
            DeshabilitarControles();
            LlenarListBoxCatalogo();
            LlenarDgv(false);
        }

        private void DeshabilitarControles()
        {
            listBoxCatalogo.Enabled = false;
            listBoxConcedidos.Enabled = false;
            listBoxCatalogo.Visible = false;
            listBoxConcedidos.Visible = false;
            txtUsuario.Visible = false;
            txtId.Visible = false;
            txtNombre.Visible = false;
            BtnAgregar.Enabled = BtnQuitar.Enabled = BtnAgregarTodos.Enabled = BtnQuitarTodos.Enabled = false;
        }

        private void HabilitarControles()
        {
            listBoxCatalogo.Enabled = true;
            listBoxConcedidos.Enabled = true;
            listBoxCatalogo.Visible = true;
            listBoxConcedidos.Visible = true;
            txtUsuario.Visible = true;
            txtId.Visible = true;
            txtNombre.Visible = true;
            BtnAgregar.Enabled = BtnQuitar.Enabled = BtnAgregarTodos.Enabled = BtnQuitarTodos.Enabled = true;
        }

        private void LlenarListBoxCatalogo()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dt = BLL.EF.Services.PermisosService.ObtenerPermisosDeCatalogo();
                listBoxCatalogo.DataSource = dt;
                listBoxCatalogo.DisplayMember = "Descripción";
                listBoxCatalogo.ValueMember = "PermisoId";
                listBoxCatalogo.SelectedIndex = -1;
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

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarDatosPermisos();
            BorrarDatosBusqueda();
            DeshabilitarControles();
            LlenarDgv(false);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarDatosPermisos();
            DeshabilitarControles();
            LlenarDgv(true);
        }

        private void BorrarDatosPermisos()
        {
            listBoxConcedidos.DataSource = null;
            listBoxConcedidos.Items.Clear();
        }

        private void BorrarDatosBusqueda()
        {
            nudBIdFin.Value = nudBIdIni.Value = 0;
            txtBPaterno.Text = txtBMaterno.Text = txtBNombres.Text = txtBUsuario.Text = string.Empty;
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            HabilitarControles();
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            if (e.RowIndex >= 0 && e.RowIndex < Dgv.RowCount)
            {
                DataGridViewRow row = Dgv.Rows[e.RowIndex];
                txtId.Text = row.Cells["Id"].Value.ToString();
                txtUsuario.Text = row.Cells["Usuario"].Value.ToString();
                txtNombre.Text = $"{row.Cells["Paterno"].Value} {row.Cells["Materno"].Value} {row.Cells["Nombres"].Value}".Trim();
                LlenarListBoxConcedidos();
            }
        }

        private void LlenarListBoxConcedidos()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dt = BLL.EF.Services.PermisosService.ObtenerPermisosConcedidos(Convert.ToInt32(txtId.Text));
                listBoxConcedidos.DataSource = dt;
                listBoxConcedidos.DisplayMember = "Descripción";
                listBoxConcedidos.ValueMember = "PermisoId";
                listBoxConcedidos.SelectedIndex = -1;
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (listBoxCatalogo.SelectedIndex < 0)
            {
                U.NotificacionWarning("Debe seleccionar un permiso del catálogo para agregarlo.");
                return;
            }
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
                int permisoId = Convert.ToInt32(listBoxCatalogo.SelectedValue);
                BLL.EF.PermisosBLL.Insertar(Convert.ToInt32(txtId.Text), permisoId);
                LlenarListBoxConcedidos();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
        }

        private void BtnQuitar_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBoxConcedidos.SelectedIndex < 0)
                {
                    U.NotificacionWarning("Debe seleccionar un permiso concedido para eliminarlo.");
                    return;
                }
                MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
                int permisoId = Convert.ToInt32(listBoxConcedidos.SelectedValue);
                BLL.EF.PermisosBLL.Eliminar(Convert.ToInt32(txtId.Text), permisoId);
                LlenarListBoxConcedidos();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
        }

        private void BtnAgregarTodos_Click(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
                // Leer catálogo de permisos desde la lista
                var pesmisosCatalogo = new List<int>();
                foreach (DataRowView drv in listBoxCatalogo.Items)
                    pesmisosCatalogo.Add(Convert.ToInt32(drv["PermisoId"]));
                var existentes = new HashSet<int>(
                    listBoxConcedidos.Items
                        .OfType<DataRowView>()
                        .Select(drv => Convert.ToInt32(drv["PermisoId"]))
                );
                // Filtrar los permisos que no existen aún
                var permisosAInsertar = new List<int>();
                foreach (var pid in pesmisosCatalogo)
                    if (!existentes.Contains(pid))
                        permisosAInsertar.Add(pid);
                if (permisosAInsertar.Count == 0)
                {
                    U.NotificacionInformation("No hay nuevos permisos para insertar"); ;
                    MDIPrincipal.ActualizarBarraDeEstado();
                    return;
                }
                // Insertar en bloque (transacción manejada por el repositorio)
                BLL.EF.PermisosBLL.InsertarPermisos(Convert.ToInt32(txtId.Text), permisosAInsertar);
                LlenarListBoxConcedidos();
                U.NotificacionInformation($"Se concedieron todos los permisos al usuario {txtUsuario.Text}");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
        }

        private void BtnQuitarTodos_Click(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
                int filasAfectadas = BLL.EF.PermisosBLL.EliminarPermisos(Convert.ToInt32(txtId.Text));
                LlenarListBoxConcedidos();
                if (filasAfectadas > 0)
                    U.NotificacionInformation($"Se eliminaron {filasAfectadas} permisos concedidos al usuario {txtUsuario.Text}");
                else
                    U.NotificacionInformation($"No se encontraron permisos concedidos al usuario {txtUsuario.Text}");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
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

    }
}
