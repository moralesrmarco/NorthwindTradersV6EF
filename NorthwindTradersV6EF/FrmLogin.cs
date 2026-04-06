using BLL.EF;
using DAL.EF;
using System;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmLogin : Form
    {
        public Usuario Usuario { get; private set; }
        bool _imagenMostrada = true;
        byte numeroIntentos = 0;

        public FrmLogin()
        {
            InitializeComponent();
            this.Text = Utils.nwtr;

            // Al presionar Enter se ejecuta btnEntrar_Click
            this.AcceptButton = btnEntrar;
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario = new Usuario
                {
                    Usuario1 = txtUsuario.Text.Trim(),
                    Password = Utils.ComputeSha256Hash(txtPwd.Text.Trim())
                };
                Usuario = UsuarioBLL.ValidarUsuario(Usuario);
                if (Usuario.Id > 0)
                {
                    this.Close();
                    return;
                }
                numeroIntentos++;
                if (numeroIntentos >= 3)
                {
                    U.NotificacionError("Demasiados intentos fallidos. La aplicación se cerrará.");
                    Application.Exit();
                    return;
                }
                U.NotificacionError("Error de autenticación.\nUsuario o contraseña incorrectos.");
                txtPwd.Clear();
                txtPwd.Focus();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void btnTogglePwd_Click(object sender, EventArgs e)
        {
            _imagenMostrada = !_imagenMostrada;
            txtPwd.UseSystemPasswordChar = !txtPwd.UseSystemPasswordChar;
            btnTogglePwd.Image = _imagenMostrada ? Properties.Resources.mostrarCh : Properties.Resources.ocultarCh;
        }
    }
}
