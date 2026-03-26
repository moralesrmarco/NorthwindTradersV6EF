using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Utilities
{
    //[DesignerCategory("Code")] // evita que el diseñador intente aplicar categorías extrañas
    public class NudNoWheel : NumericUpDown
    {
        public NudNoWheel() : base()
        {
            // Constructor vacío y seguro
            // No debe tener lógica externa (BD, archivos, servicios)
        }

        /// <summary>
        /// Propiedad para habilitar o deshabilitar el scroll con la rueda del mouse
        /// </summary>
        public bool WheelEnabled { get; set; } = true;

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!WheelEnabled && e is HandledMouseEventArgs hme)
            {
                hme.Handled = true;
                return;
            }

            base.OnMouseWheel(e);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (this.Site != null && this.Site.DesignMode)
                return; // estamos en el diseñador

            // Lógica solo en tiempo de ejecución
            // this.Minimum = 0;
            // this.Maximum = 100;
        }

        //protected override void OnMouseWheel(MouseEventArgs e)
        //{
        //    // Si estamos en tiempo de diseño, no hacemos nada
        //    if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        //        return;

        //    if (WheelEnabled)
        //    {
        //        base.OnMouseWheel(e); // comportamiento normal
        //    }
        //    else if (e is HandledMouseEventArgs hme)
        //    {
        //        hme.Handled = true; // bloquea el scroll
        //    }
        //}

        //protected override void OnCreateControl()
        //{
        //    base.OnCreateControl();

        //    // Evita ejecutar lógica pesada en el diseñador
        //    if (!DesignMode)
        //    {
        //        // Configuración que solo aplica en tiempo de ejecución
        //        //this.Minimum = 0;
        //        //this.Maximum = 100;
        //    }
        //}
    }
}