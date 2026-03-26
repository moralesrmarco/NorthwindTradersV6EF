using System;
using System.Windows.Forms;

namespace Utilities
{
    /// <summary>
    /// Helper para configurar el estado de controles NudNoWheel.
    /// Permite habilitar o deshabilitar de forma centralizada:
    /// - El uso del MouseWheel
    /// - La edición del valor (ReadOnly)
    /// - El uso de teclas de flecha (InterceptArrowKeys)
    /// - Los botones internos de incremento/decremento (UpDownButtons)
    /// </summary>
    public class NudHelper
    {
        /// <summary>
        /// Habilita o deshabilita un control NudNoWheel según el parámetro indicado.
        /// </summary>
        /// <param name="nud">Instancia del NudNoWheel a configurar.</param>
        /// <param name="enabled">true para habilitar el control; false para deshabilitarlo.</param>
        public static void SetEnabled(NudNoWheel nud, bool enabled)
        {
            // Controla el comportamiento del MouseWheel
            nud.WheelEnabled = enabled;
            // Controla si el usuario puede editar directamente el valor
            nud.ReadOnly = !enabled;
            // Controla si el control intercepta las teclas de flecha
            nud.InterceptArrowKeys = enabled; // habilita o deshabilita las flechas junto con la siguiente instruccion
            // Controla los botones internos de incremento/decremento
            // Deshabilitar los botones internos (UpDownButtons)
            if (nud.Controls.Count > 0)
            {
                nud.Controls[0].Enabled = enabled;
            }
        }
    }
}
