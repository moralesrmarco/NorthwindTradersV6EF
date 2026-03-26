using System;
using System.Windows.Forms;

namespace Utilities
{
    public static class InventarioHelper
    {
        public enum ModoInventario
        {
            Disponibilidad,   // inventarioRealDb - cantidadReservada
            Reconstruido      // inventarioViejo + cantidadVieja - cantidadNueva
        }
        // finalmente llegue a esta version que unifica los otros dos helpers que se muestran mas abajo
        public static decimal ActualizarInventarioUi(
        decimal cantidadNueva,
        decimal cantidadVieja,
        decimal inventarioViejo,
        NumericUpDown nudInventario,
        ModoInventario modo)
        {
            decimal inventarioUi;

            switch (modo)
            {
                case ModoInventario.Disponibilidad:
                    // Inventario disponible = inventario real - cantidad reservada
                    if (cantidadNueva >= inventarioViejo)
                        inventarioUi = 0;
                    else
                        inventarioUi = inventarioViejo - cantidadNueva;
                    break;

                case ModoInventario.Reconstruido:
                    // Inventario remanente real = inventario viejo + cantidad vieja - cantidad nueva
                    decimal disponible = inventarioViejo + cantidadVieja;
                    inventarioUi = disponible - cantidadNueva;
                    break;

                default:
                    inventarioUi = inventarioViejo;
                    break;
            }

            // Ajustar a límites del control
            inventarioUi = Math.Min(inventarioUi, nudInventario.Maximum);
            inventarioUi = Math.Max(inventarioUi, nudInventario.Minimum);

            nudInventario.Value = inventarioUi;
            return inventarioUi;
        }


        /// <summary>
        /// Calcula el inventario remanente y actualiza el control NumericUpDown,
        /// aplicando sus límites de mínimo y máximo. Devuelve el valor calculado.
        /// </summary>
        /// <param name="cantidadReservada">Cantidad reservada actualmente</param>
        /// <param name="inventarioRealDb">Inventario real en la base de datos</param>
        /// <param name="nudInventario">Control NumericUpDown a actualizar</param>
        /// <returns>Valor calculado de inventario disponible</returns>
        /// <summary>
        /// Calcula el inventario remanente y actualiza el control NumericUpDown,
        /// aplicando sus límites de mínimo y máximo. Devuelve el valor calculado.
        /// </summary>
        /// <param name="cantidadReservada">Cantidad reservada actualmente</param>
        /// <param name="inventarioRealDb">Inventario real en la base de datos</param>
        /// <param name="nudInventario">Control NumericUpDown a actualizar</param>
        /// <returns>Valor calculado de inventario disponible</returns>
        public static decimal ActualizarInventarioUi(
            decimal cantidadReservada,
            decimal inventarioRealDb,
            NumericUpDown nudInventario,
            bool mostrarInventarioReal = false
        )
        {
            decimal inventarioUi;
            if (mostrarInventarioReal)
            {
                // Mostrar siempre inventario real
                inventarioUi = inventarioRealDb;
            }
            else
            {
                // Nueva regla: si la reserva excede o iguala el inventario real, mostrar 0
                if (cantidadReservada >= inventarioRealDb)
                    inventarioUi = 0;
                else
                    inventarioUi = inventarioRealDb - cantidadReservada;
            }

            // Aplicar límites del NumericUpDown
            inventarioUi = Math.Min(inventarioUi, nudInventario.Maximum);
            inventarioUi = Math.Max(inventarioUi, nudInventario.Minimum);

            nudInventario.Value = inventarioUi;

            return inventarioUi; // ✅ devuelve el valor calculado
        }

        /// <summary>
        /// Calcula el inventario remanente y actualiza el control NumericUpDown
        /// aplicando sus límites de mínimo y máximo.
        /// </summary>
        /// <param name="cantidadNueva">Cantidad nueva reservada</param>
        /// <param name="cantidadVieja">Cantidad anterior reservada</param>
        /// <param name="inventarioViejo">Inventario actual en DB</param>
        /// <param name="nudInventario">Control NumericUpDown a actualizar</param>
        public static void ActualizarInventarioUi(
            decimal cantidadNueva,
            decimal cantidadVieja,
            decimal inventarioViejo,
            NumericUpDown nudInventario)
        {
            // Stock total disponible para este pedido (reservado + inventario)
            decimal disponible = inventarioViejo + cantidadVieja;

            //Inventario remanente REAL en DB después de reservar la nueva cantidad
            decimal inventarioNuevoDb = disponible - cantidadNueva;

            //// Inventario remanente REAL en DB después de ajustar la reserva
            //decimal inventarioNuevoDb = inventarioViejo + cantidadVieja - cantidadNueva;

            // Aplica límites del NumericUpDown solo para mostrar en UI
            decimal inventarioNuevoUi = inventarioNuevoDb;
            inventarioNuevoUi = Math.Min(inventarioNuevoUi, nudInventario.Maximum); // evita lanzar una excepción
            inventarioNuevoUi = Math.Max(inventarioNuevoUi, nudInventario.Minimum); // evita lanzar una excepción

            nudInventario.Value = inventarioNuevoUi;
        }
    }
}