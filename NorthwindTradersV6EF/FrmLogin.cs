using BLL;
using System;
using System.Configuration;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmLogin : Form
    {
        string _connectionString = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly UsuarioBLL _usuarioBLL;
        public bool IsAuthenticated { get; private set; } = false;
        public string UsuarioAutenticado;
        public int IdUsuarioAutenticado;
        public string NombreUsuarioAutenticado;
        bool _imagenMostrada = true;
        byte numeroIntentos = 0;

        public FrmLogin()
        {
            InitializeComponent();
            this.Text = Utils.nwtr;
            _usuarioBLL = new UsuarioBLL(_connectionString);
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            var usuario = txtUsuario.Text.Trim();
            var pass = txtPwd.Text.Trim();
            if (ValidarUsuario(usuario, pass))
            {
                IsAuthenticated = true;
                UsuarioAutenticado = usuario;
                this.Close();
            }
            else
            {
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
        }

        private bool ValidarUsuario(string usuario, string pass)
        {
            try
            {
                string hashed = Utils.ComputeSha256Hash(pass);
                string nombreUsuarioAutenticado;
                IdUsuarioAutenticado = _usuarioBLL.ValidarUsuario(usuario, hashed, out nombreUsuarioAutenticado);
                NombreUsuarioAutenticado = nombreUsuarioAutenticado;
                return IdUsuarioAutenticado > 0;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
                return false;
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
