using System.Drawing;
using System.Windows.Forms;

namespace Utilities
{
    public static class ValidarCantidadEInventarioHelper
    {
        // en si ValidarCantidad y ValidarInventario no son precisas en el caso en que otro usuario haya modificado el inventario en otra sesion (por la concurrencia optimista) entre el momento en que se cargó el formulario y se realiza la modificación. Solo funcionarían bien si no hubiera concurrencia de otro usuario al mismo tiempo. Por lo que es más seguro que las validaciones se hagan en los stored procedures, como ya estan programados con esas validaciones. Los mensajes se dejaran como warnings

        public static bool ValidarCantidad
        (
            decimal cantidadNueva,
            decimal cantidadVieja,
            decimal inventarioViejo,
            decimal inventarioActual,
            NumericUpDown nudCantidad,
            ToolTip toolTip,
            PictureBox pbError,
            PictureBox pbInfo,
            PictureBox pbWarning,
            ErrorProvider errorProvider
        )
        {
            // Stock disponible total para este pedido
            decimal disponible = inventarioViejo + cantidadVieja;
            // Inventario inicial real (solo lo que había en almacén)
            decimal inventarioInicial = disponible - cantidadVieja;
            // Inventario remanente REAL en DB después de reservar la nueva cantidad
            decimal inventarioNuevoDb = disponible - cantidadNueva;

            const decimal SmallintMax = 32767M;

            // Condiciones de error
            bool condErrorCantidadCero = cantidadNueva <= 0;

            bool showError = condErrorCantidadCero;

            // Construir mensaje acumulado
            string errorMsg = "";
            if (condErrorCantidadCero)
                errorMsg += "- La cantidad debe ser mayor que cero.\n\n";
            errorMsg += "No se puede realizar la operación;";
            // Información
            bool showInfo = cantidadNueva >= 0;
            // Warnings
            bool condWarningInventarioCero = inventarioActual == 0;
            bool condWarningInventarioBajo = inventarioActual > 0 && inventarioActual <= 50;
            bool condWarningExcedeInvent = cantidadNueva > disponible;
            bool condWarningOverflowSmall = inventarioNuevoDb > SmallintMax;

            bool showWarning = true;// condWarningInventarioCero || condWarningInventarioBajo || condWarningExcedeInvent || condWarningOverflowSmall;

            string warningMsg = "";
            string msgPreventivo = "Las validaciones reales de existencia de producto en inventario\n" +
                                   "y de la cantidad vendida aplicada al inventario se realizarán\n" +
                                   "del lado del servidor SQL, debido a la concurrencia de usuarios\n" +
                                   "(concurrencia optimista).";
            string msgPreventivo2 = "\n\nLos siguientes mensajes son solo preventivos:";
            if (condWarningInventarioCero)
                warningMsg += "- No hay este producto en existencia.\n";
            if (condWarningInventarioBajo)
                warningMsg += "- La existencia en inventario es baja.\n";
            if (condWarningExcedeInvent)
                warningMsg += $"- La cantidad excede el inventario inicial disponible ({inventarioInicial}).\n";
            if (condWarningOverflowSmall)
                warningMsg += "- La cantidad de producto devuelto más las unidades en inventario\n" +
                              "  excede el límite maximo que se puede almacenar en la base de datos\n" +
                              "  (32,767 unidades).";
            if (warningMsg != "")
                warningMsg = msgPreventivo2 + "\n" + warningMsg;
            warningMsg = msgPreventivo + warningMsg;
            // Mostrar íconos con StatusIconHelper
            StatusIconHelper.ShowIcons(
                nudCantidad,
                toolTip,
                (pbError, (Image)errorProvider.Icon.ToBitmap(), errorMsg, showError),
                (pbInfo, (Image)SystemIcons.Information.ToBitmap(),
                    "- La cantidad de producto devuelto se añade al inventario.\n- La cantidad de producto añadido se descuenta del inventario.",
                    showInfo),
                (pbWarning, (Image)SystemIcons.Warning.ToBitmap(),
                    warningMsg,
                    showWarning)
            );
            return !showError;
        }

        public static void ValidarInventario
        (
            decimal cantidadNueva,
            decimal cantidadVieja,
            decimal inventarioViejo,
            decimal inventarioActual,
            NumericUpDown nudUInventario,
            ToolTip toolTip,
            PictureBox pbError1,
            PictureBox pbInfo1,
            PictureBox pbWarning1,
            ErrorProvider errorProvider
        )
        {
            // Stock total disponible para este pedido
            decimal disponible = inventarioViejo + cantidadVieja;
            // Inventario remanente REAL en DB después de reservar la nueva cantidad
            decimal inventarioNuevoDb = disponible - cantidadNueva;

            const decimal SmallintMax = 32767M;
            // Errores (ninguno por ahora)
            bool showError = false;
            string errorMsg = "";

            // Información (siempre mostrar)
            bool showInfo = true;

            // Warnings
            bool condWarningInventarioCero = inventarioActual == 0;
            bool condWarningInventarioBajo = inventarioActual > 0 && inventarioActual <= 50;
            bool condWarningExcedeInvent = cantidadNueva > disponible;
            bool condWarningOverflowSmall = inventarioNuevoDb > SmallintMax;

            bool showWarning = true; // condWarningInventarioCero || condWarningInventarioBajo || condWarningExcedeInvent || condWarningOverflowSmall;

            string warningMsg = "";
            string msgPreventivo = "Las validaciones reales de existencia de producto en inventario\n" +
                                   "y de la cantidad vendida aplicada al inventario se realizarán\n" +
                                   "del lado del servidor SQL, debido a la concurrencia de usuarios\n" +
                                   "(concurrencia optimista).";
            string msgPreventivo2 = "\n\nLos siguientes mensajes son solo preventivos:";
            if (condWarningInventarioCero)
                warningMsg += "- No hay este producto en existencia.\n";
            if (condWarningInventarioBajo)
                warningMsg += "- La existencia en inventario es baja.\n";
            if (condWarningExcedeInvent)
                warningMsg += $"- La cantidad excede el inventario inicial disponible ({disponible - cantidadVieja}).\n";
            if (condWarningOverflowSmall)
                warningMsg += "- La cantidad de producto devuelto más las unidades en inventario\n" +
                              "  excede el límite maximo que se puede almacenar en la base de datos\n" +
                              "  (32,767 unidades).";
            if (warningMsg != "")
                warningMsg = msgPreventivo2 + "\n" + warningMsg;
            warningMsg = msgPreventivo + warningMsg;
            // Mostrar íconos con StatusIconHelper en nudUInventario
            StatusIconHelper.ShowIcons(
                nudUInventario,
                toolTip,
                (pbError1, (Image)errorProvider.Icon.ToBitmap(), errorMsg, showError),
                (pbInfo1, (Image)SystemIcons.Information.ToBitmap(),
                    "- La cantidad de producto devuelto se añade al inventario.\n" +
                    "- La cantidad de producto añadido se descuenta del inventario.",
                    showInfo),
                (pbWarning1, (Image)SystemIcons.Warning.ToBitmap(), warningMsg, showWarning)
            );
        }
    }
}
