using System;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmCambiarContrasena : Form
    {
        public string UsuarioLogueado;
        bool _imagenMostrada = true;
        short numIntentos = 0;

        public FrmCambiarContrasena()
        {
            InitializeComponent();
        }

        private void FrmCambiarContrasena_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmCambiarContrasena_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (txtPwd.Text != string.Empty || txtNewPwd.Text != string.Empty || txtConfirmarPwd.Text != string.Empty)
                if (U.NotificacionQuestion(Utils.preguntaCerrar) == DialogResult.No)
                    e.Cancel = true;
        }

        private void btnTogglePwd_Click(object sender, EventArgs e)
        {
            _imagenMostrada = !_imagenMostrada;
            txtPwd.UseSystemPasswordChar = !txtPwd.UseSystemPasswordChar;
            txtNewPwd.UseSystemPasswordChar = !txtNewPwd.UseSystemPasswordChar;
            txtConfirmarPwd.UseSystemPasswordChar = !txtConfirmarPwd.UseSystemPasswordChar;
            btnTogglePwd.Image = _imagenMostrada ? Properties.Resources.mostrarCh : Properties.Resources.ocultarCh;
        }

        private void FrmCambiarContrasena_Load(object sender, EventArgs e)
        {
            txtUsuario.Text = UsuarioLogueado;
            txtPwd.Focus();
        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            PonerNoVisibleBtnTogglePwd();
            if (!ValidarNuevaContraseña())
                return;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.modificandoRegistro);
                string pwdNuevaContraseñaHasheada = Utils.ComputeSha256Hash(txtNewPwd.Text.Trim());
                int numRegs = BLL.EF.UsuarioBLL.ActualizarContraseña(txtUsuario.Text, pwdNuevaContraseñaHasheada);
                MDIPrincipal.ActualizarBarraDeEstado();
                if (numRegs > 0)
                {
                    U.NotificacionInformation("Contraseña cambiada correctamente.");
                    txtPwd.Text = txtNewPwd.Text = txtConfirmarPwd.Text = string.Empty;
                    this.Close();
                }
                else
                    U.NotificacionError("No se pudo cambiar la contraseña. Verifique que su cuenta esté activa.");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private bool ValidarNuevaContraseña()
        {
            errorProvider1.Clear();
            bool valida = true;
            txtPwd.Text = txtPwd.Text.Trim();
            if (string.IsNullOrWhiteSpace(txtPwd.Text))
            {
                errorProvider1.SetError(txtPwd, "Debe ingresar su contraseña actual");
                valida = false;
            }
            if (valida)
            {
                try
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                    string pwdHasheada = Utils.ComputeSha256Hash(txtPwd.Text.Trim());
                    int numRegs = BLL.EF.UsuarioBLL.ValidarContraseñaActual(txtUsuario.Text, pwdHasheada);
                    MDIPrincipal.ActualizarBarraDeEstado();
                    if (numRegs == 0)
                    {
                        errorProvider1.SetError(txtPwd, "La contraseña actual es incorrecta");
                        valida = false;
                    }
                }
                catch (Exception ex)
                {
                    U.MsgCatchOue(ex);
                    valida = false;
                }
                txtNewPwd.Text = txtNewPwd.Text.Trim();
                txtConfirmarPwd.Text = txtConfirmarPwd.Text.Trim();
                if (string.IsNullOrWhiteSpace(txtNewPwd.Text.Trim()))
                {
                    errorProvider1.SetError(txtNewPwd, "La nueva contraseña es obligatoria");
                    valida = false;
                }
                if (string.IsNullOrWhiteSpace(txtConfirmarPwd.Text.Trim()))
                {
                    errorProvider1.SetError(txtConfirmarPwd, "La confirmación de la contraseña es obligatoria");
                    valida = false;
                }
                if (valida)
                {
                    // Validar que las contraseñas coincidan
                    if (txtNewPwd.Text != txtConfirmarPwd.Text)
                    {
                        errorProvider1.SetError(txtNewPwd, "La nueva contraseña y la confirmación de la contraseña no coinciden");
                        errorProvider1.SetError(txtConfirmarPwd, "La nueva contraseña y la confirmación de la contraseña no coinciden");
                        valida = false;
                    }
                }
            }
            if (!valida)
            {
                numIntentos++;
                if (numIntentos >= 3)
                {
                    U.NotificacionError("Demasiados intentos fallidos.\n\nPor favor, inténtelo de nuevo más tarde.");
                    this.FormClosing -= FrmCambiarContrasena_FormClosing;
                    this.Close();
                }
            }
            return valida;
        }

        private void PonerNoVisibleBtnTogglePwd()
        {
            txtPwd.UseSystemPasswordChar = txtNewPwd.UseSystemPasswordChar = txtConfirmarPwd.UseSystemPasswordChar = true;
            btnTogglePwd.Image = Properties.Resources.mostrarCh;
        }
    }
}
