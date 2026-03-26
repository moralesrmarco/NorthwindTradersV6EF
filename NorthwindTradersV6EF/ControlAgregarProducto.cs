using NorthwindTradersV6EF.Helpers;
using System;
using System.Data;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class ControlAgregarProducto : UserControl
    {
        // Propiedades públicas para exponer los controles internos
        public ComboBox CboCategoria => cboCategoria;
        public ComboBox CboProducto => cboProducto;
        public Button BtnAgregar => btnAgregar;
        public GroupBox GrbProducto => grbProducto;

        public NudNoWheel NudPrecioConIVAIncluido => nudPrecioConIVAIncluido;
        public NudNoWheel NudUInventario => nudUInventario;
        public NudNoWheel NudCantidad => nudCantidad;
        public NudNoWheel NudDescuento => nudDescuento;

        public NudNoWheel NudPrecioPorUnidadSinIVAIncluidoAntesDescuento => nudPrecioPorUnidadSinIVAIncluidoAntesDescuento;
        public NudNoWheel NudIVADelPrecioPorUnidadAntesDescuento => nudIVADelPrecioPorUnidadAntesDescuento;
        public NudNoWheel NudPrecioPorUnidadSinIVADepuesDescuento => nudPrecioPorUnidadSinIVADepuesDescuento;
        public NudNoWheel NudAhorroPorUnidadSinIVA => nudAhorroPorUnidadSinIVA;
        public NudNoWheel NudIVADelPrecioPorUnidadDespuesDescuento => nudIVADelPrecioPorUnidadDespuesDescuento;
        public NudNoWheel NudAhorroEnIVAPorUnidadDespuesDescuento => nudAhorroEnIVAPorUnidadDespuesDescuento;
        public NudNoWheel NudPrecioPorUnidadConIVADespuesDescuento => nudPrecioPorUnidadConIVADespuesDescuento;
        public NudNoWheel NudAhorroTotalPorUnidadConIVA => nudAhorroTotalPorUnidadConIVA;

        public NudNoWheel NudSubtotalDelImporteConIVAIncluido2 => nudSubtotalDelImporteConIVAIncluido2;
        public NudNoWheel NudSubtotalDelImporteSinIVASinDescuento2 => nudSubtotalDelImporteSinIVASinDescuento2;
        public NudNoWheel NudSubtotalDelImporteDelIVASinDescuento2 => nudSubtotalDelImporteDelIVASinDescuento2;
        public NudNoWheel NudSubtotalDelImporteSinIVAConDescuento2 => nudSubtotalDelImporteSinIVAConDescuento2;
        public NudNoWheel NudSubtotalIVADespuesDelDescuento2 => nudSubtotalIVADespuesDelDescuento2;
        public NudNoWheel NudSubtotalDelAhorroSinIvaDespuesDescuento2 => nudSubtotalDelAhorroSinIvaDespuesDescuento2;
        public NudNoWheel NudSubtotalDelAhorroEnIVADespuesDescuento2 => nudSubtotalDelAhorroEnIVADespuesDescuento2;
        public NudNoWheel NudSubtotalDelAhorroTotalDespuesDescuento2 => nudSubtotalDelAhorroTotalDespuesDescuento2;
        public NudNoWheel NudTotal2 => nudTotal2;

        public PictureBox PbError => pbError;
        public PictureBox PbInfo => pbInfo;
        public PictureBox PbWarning => pbWarning;

        public PictureBox PbError1 => pbError1;
        public PictureBox PbInfo1 => pbInfo1;
        public PictureBox PbWarning1 => pbWarning1;

        // Declaras el evento aquí
        public event EventHandler NudEnter;
        public event EventHandler NudCantidadDescuento_LeaveValueChanged;
        public event EventHandler CboCategoria_SelectedIndexChanged;
        public event EventHandler CboProducto_SelectedIndexChanged;
        public event EventHandler BtnAgregar_Click;

        public ControlAgregarProducto()
        {
            InitializeComponent();
            grbProducto.Paint += (s, e) => Utils.GrbPaint(this.ParentForm, s, e);
            GrbSubtotales.Paint += (s, e) => Utils.GrbPaint2(this.ParentForm, s, e);
        }

        private void NudEnter_Handler(object sender, EventArgs e)
        {
            // Dispara el evento hacia el formulario
            NudEnter?.Invoke(sender, e);
        }

        private void NudCantidadDescuento_LeaveValueChanged_Handler(object sender, EventArgs e)
        {
            // Dispara el evento hacia el formulario
            NudCantidadDescuento_LeaveValueChanged?.Invoke(sender, e);
        }

        private void CboCategoria_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            // Dispara el evento hacia el formulario
            CboCategoria_SelectedIndexChanged?.Invoke(sender, e);
        }

        private void CboProducto_SelectedIndexChanged_Handler(object sender, EventArgs e)
        {
            // Dispara el evento hacia el formulario
            CboProducto_SelectedIndexChanged?.Invoke(sender, e);
        }

        private void BtnAgregar_Click_Handler(object sender, EventArgs e)
        {
            // Dispara el evento hacia el formulario
            BtnAgregar_Click?.Invoke(sender, e);
        }

        public void LlenarCboCategoria(DataTable categorias)
        {
            ComboBoxHelper.LlenarCbo(cboCategoria, categorias, "CategoryName", "CategoryID");
        }
    }
}
