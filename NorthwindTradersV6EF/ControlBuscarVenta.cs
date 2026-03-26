using System;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class ControlBuscarVenta : UserControl
    {
        // Propiedades públicas para acceder a los valores
        public GroupBox grbBuscar => GrbBuscar;

        public NumericUpDown NudBIdIni => nudBIdIni;
        public NumericUpDown NudBIdFin => nudBIdFin;

        public TextBox TxtBCliente => txtBCliente;

        // Exponer directamente los DateTimePicker
        public DateTimePicker DtpFVentaIni => dtpBFVentaIni;
        public DateTimePicker DtpFVentaFin => dtpBFVentaFin;

        public DateTimePicker DtpFRequeridoIni => dtpBFRequeridoIni;
        public DateTimePicker DtpFRequeridoFin => dtpBFRequeridoFin;

        public DateTimePicker DtpFEnvioIni => dtpBFEnvioIni;
        public DateTimePicker DtpFEnvioFin => dtpBFEnvioFin;

        public CheckBox ChkbFVentaNull => chkbBFVentaNull;
        public CheckBox ChkbFRequeridoNull => chkbBFRequeridoNull;
        public CheckBox ChkbFEnvioNull => chkbBFEnvioNull;

        public TextBox TxtBEmpleado => txtBEmpleado;

        public TextBox TxtBCompañiaT => txtBCompañiaT;

        public TextBox TxtBDirigidoa => txtBDirigidoa;

        // Declaras el evento aquí
        public event EventHandler LimpiarClick;
        public event EventHandler BuscarClick;

        public ControlBuscarVenta()
        {
            InitializeComponent();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Comportamiento interno del control
            LimpiarInterno();

            // Dispara el evento hacia el formulario para que también pueda realizar acciones adicionales si lo desea, solo si el formulario se ha suscrito al evento
            LimpiarClick?.Invoke(this, EventArgs.Empty);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Dispara el evento hacia el formulario
            BuscarClick?.Invoke(this, EventArgs.Empty);
        }

        private void LimpiarInterno() 
        {
            NudBIdIni.Value = 0;
            NudBIdFin.Value = 0;

            TxtBCliente.Text = "";
            TxtBEmpleado.Text = "";
            TxtBCompañiaT.Text = "";
            TxtBDirigidoa.Text = "";

            DtpFVentaIni.Value = DateTime.Today;
            DtpFVentaFin.Value = DateTime.Today;
            DtpFVentaIni.Checked = false;
            DtpFVentaFin.Checked = false;
            ChkbFVentaNull.Checked = false;

            DtpFRequeridoIni.Value = DateTime.Today;
            DtpFRequeridoFin.Value = DateTime.Today;
            DtpFRequeridoIni.Checked = false;
            DtpFRequeridoFin.Checked = false;
            ChkbFRequeridoNull.Checked = false;

            DtpFEnvioIni.Value = DateTime.Today;
            DtpFEnvioFin.Value = DateTime.Today;
            DtpFEnvioIni.Checked = false;
            DtpFEnvioFin.Checked = false;
            ChkbFEnvioNull.Checked = false;
        }

        private void nudEnter(object sender, EventArgs e)
        {
            if (sender is NumericUpDown nud && nud.Controls[1] is TextBox tb)
            {
                // Diferir la selección para que ocurra después de que el TextBox reciba el foco
                tb.BeginInvoke((Action)(() => tb.SelectAll()));
            }
        }

        private void nudLeave(object sender, EventArgs e) => Utils.ValidarRango(sender, NudBIdIni, NudBIdFin);

        private void txtEnter(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                // Seleccionar todo el texto al recibir el foco
                this.BeginInvoke((Action)(() => tb.SelectAll()));
            }
        }

        private void dtpBFVentaIni_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFVentaIni.Checked)
            {
                dtpBFVentaFin.Checked = true;
                chkbBFVentaNull.Checked = false;
            }
            else
                dtpBFVentaFin.Checked = false;
        }

        private void dtpBFVentaFin_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFVentaFin.Checked)
            {
                dtpBFVentaIni.Checked = true;
                chkbBFVentaNull.Checked = false;
            }
            else
                dtpBFVentaIni.Checked = false;
        }

        private void dtpBFRequeridoIni_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFRequeridoIni.Checked)
            {
                dtpBFRequeridoFin.Checked = true;
                chkbBFRequeridoNull.Checked = false;
            }
            else
                dtpBFRequeridoFin.Checked = false;
        }

        private void dtpBFRequeridoFin_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFRequeridoFin.Checked)
            {
                dtpBFRequeridoIni.Checked = true;
                chkbBFRequeridoNull.Checked = false;
            }
            else
                dtpBFRequeridoIni.Checked = false;
        }

        private void dtpBFEnvioIni_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFEnvioIni.Checked)
            {
                dtpBFEnvioFin.Checked = true;
                chkbBFEnvioNull.Checked = false;
            }
            else
                dtpBFEnvioFin.Checked = false;
        }

        private void dtpBFEnvioFin_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBFEnvioFin.Checked)
            {
                dtpBFEnvioIni.Checked = true;
                chkbBFEnvioNull.Checked = false;
            }
            else
                dtpBFEnvioIni.Checked = false;
        }

        private void chkbBFVentaNull_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbBFVentaNull.Checked)
            {
                dtpBFVentaIni.Checked = false;
                dtpBFVentaFin.Checked = false;
            }
        }

        private void chkbBFRequeridoNull_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbBFRequeridoNull.Checked)
            {
                dtpBFRequeridoIni.Checked = false;
                dtpBFRequeridoFin.Checked = false;
            }
        }

        private void chkbBFEnvioNull_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbBFEnvioNull.Checked)
            {
                dtpBFEnvioIni.Checked = false;
                dtpBFEnvioFin.Checked = false;
            }
        }

        private void dtpBFVentaIni_Leave(object sender, EventArgs e)
        {
            if (dtpBFVentaIni.Checked && dtpBFVentaFin.Checked)
                if (dtpBFVentaFin.Value < dtpBFVentaIni.Value)
                    dtpBFVentaFin.Value = dtpBFVentaIni.Value;
        }

        private void dtpBFVentaFin_Leave(object sender, EventArgs e)
        {
            if (dtpBFVentaIni.Checked && dtpBFVentaFin.Checked)
                if (dtpBFVentaFin.Value < dtpBFVentaIni.Value)
                    dtpBFVentaIni.Value = dtpBFVentaFin.Value;
        }

        private void dtpBFRequeridoIni_Leave(object sender, EventArgs e)
        {
            if (dtpBFRequeridoIni.Checked && dtpBFRequeridoFin.Checked)
                if (dtpBFRequeridoFin.Value < dtpBFRequeridoIni.Value)
                    dtpBFRequeridoFin.Value = dtpBFRequeridoIni.Value;
        }

        private void dtpBFRequeridoFin_Leave(object sender, EventArgs e)
        {
            if (dtpBFRequeridoIni.Checked && dtpBFRequeridoFin.Checked)
                if (dtpBFRequeridoFin.Value < dtpBFRequeridoIni.Value)
                    dtpBFRequeridoIni.Value = dtpBFRequeridoFin.Value;
        }

        private void dtpBFEnvioIni_Leave(object sender, EventArgs e)
        {
            if (dtpBFEnvioIni.Checked && dtpBFEnvioFin.Checked)
                if (dtpBFEnvioFin.Value < dtpBFEnvioIni.Value)
                    dtpBFEnvioFin.Value = dtpBFEnvioIni.Value;
        }

        private void dtpBFEnvioFin_Leave(object sender, EventArgs e)
        {
            if (dtpBFEnvioIni.Checked && dtpBFEnvioFin.Checked)
                if (dtpBFEnvioFin.Value < dtpBFEnvioIni.Value)
                    dtpBFEnvioIni.Value = dtpBFEnvioFin.Value;
        }
    }
}
