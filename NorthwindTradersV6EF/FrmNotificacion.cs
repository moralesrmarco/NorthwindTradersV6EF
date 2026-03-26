using System;
using System.Drawing;
using System.Windows.Forms;
using Utilities;
using static NorthwindTradersV6EF.U;

namespace NorthwindTradersV6EF
{
    public partial class FrmNotificacion : Form
    {

        public FrmNotificacion(string mensaje, Icon icono, Color colorTexto, NotificationMode modo = NotificationMode.Aceptar)
        {
            InitializeComponent();

            this.Text = Utils.nwtr;
            this.Icon = icono; // Ícono de la ventana
            // Verificar si el mensaje contiene tags
            if (ContieneTags(mensaje))
            {
                // Procesar el mensaje con tags de color
                MostrarMensajeConTags(mensaje);
            }
            else
            {
                // Respetar formato actual del RichTextBox
                richTextBox1.Clear();
                richTextBox1.SelectionFont = new Font("Segoe UI", 4, FontStyle.Regular);
                richTextBox1.AppendText(Environment.NewLine);
                richTextBox1.SelectionColor = colorTexto;
                richTextBox1.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
                richTextBox1.AppendText(mensaje);
            }
            // Mostrar ícono en PictureBox
            pictureBox1.Image = IconToImage(icono);
            if (modo == NotificationMode.Aceptar)
            {
                // Mostrar solo botón Aceptar
                btnAceptar.Visible = true;
                btnSi.Visible = false;
                btnNo.Visible = false;
                // Definir btnAceptar como botón por defecto
                this.AcceptButton = btnAceptar;
                // Definir btnAceptar como botón de cancelación
                this.CancelButton = btnAceptar;
            }
            else if (modo == NotificationMode.SiNo)
            {
                // Mostrar botones Sí/No
                btnAceptar.Visible = false;
                btnSi.Visible = true;
                btnNo.Visible = true;

                this.AcceptButton = btnNo;
                this.CancelButton = btnNo;
            }
        }

        private bool ContieneTags(string mensaje)
        {
            return mensaje.Contains("[black]") ||
                   mensaje.Contains("[green]") ||
                   mensaje.Contains("[red]") ||
                   mensaje.Contains("[blue]") ||
                   mensaje.Contains("[orange]") ||
                   mensaje.Contains("[gold]");
        }

        private Image IconToImage(Icon icon)
        {
            return icon.ToBitmap();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnSi_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        /// <summary>
        /// Interpreta tags como [black], [green], [red] al inicio de cada párrafo
        /// y aplica el color correspondiente en el RichTextBox.
        /// </summary>
        private void MostrarMensajeConTags(string mensaje)
        {
            richTextBox1.Clear();
            // Divide por saltos de línea
            string[] parrafos = mensaje.Split(new[] { "\n" }, StringSplitOptions.None);
            // Línea en blanco inicial
            richTextBox1.SelectionFont = new Font("Segoe UI", 4, FontStyle.Regular);
            richTextBox1.AppendText(Environment.NewLine);
            //foreach (var p in parrafos)
            for (int i = 0; i < parrafos.Length; i++)
            {
                string texto = parrafos[i];
                Color color = Color.Black; // default
                // Detectar tag al inicio
                if (texto.StartsWith("[black]"))
                {
                    color = Color.Black;
                    texto = texto.Replace("[black]", "");
                }
                else if (texto.StartsWith("[green]"))
                {
                    color = Color.Green;
                    texto = texto.Replace("[green]", "");
                }
                else if (texto.StartsWith("[red]"))
                {
                    color = Color.Red;
                    texto = texto.Replace("[red]", "");
                }
                else if (texto.StartsWith("[blue]"))
                {
                    color = Color.Blue;
                    texto = texto.Replace("[blue]", "");
                }
                else if (texto.StartsWith("[orange]"))
                {
                    color = Color.OrangeRed;
                    texto = texto.Replace("[orange]", "");
                }
                else if (texto.StartsWith("[gold]"))
                {
                    color = Color.Goldenrod;
                    texto = texto.Replace("[gold]", "");
                }
                // Aplica formato
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionColor = color;
                richTextBox1.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
                richTextBox1.AppendText(texto);
                // Solo agregar salto si no es el último párrafo
                if (i < parrafos.Length - 1)
                {
                    richTextBox1.AppendText(Environment.NewLine + Environment.NewLine);
                }
            }
        }

        private void FrmNotificacion_Shown(object sender, EventArgs e)
        {
            btnAceptar.Focus();
        }
    }
}
