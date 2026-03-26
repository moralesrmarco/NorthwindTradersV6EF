using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Utilities;
// es una version mas general y compacta de DateTimePickerHelper
namespace Utilities
{
    public static class DateTimeCascadeHelper
    {
        private static readonly DateTime FechaMinSql = new DateTime(1753, 1, 1);

        /// <summary>
        /// Sincroniza una lista de pares (fecha, hora) en cascada.
        /// Cada par debe respetar la jerarquía: fecha[i] >= fecha[i-1],
        /// y si son iguales, hora[i] >= hora[i-1].
        /// </summary>
        public static void SincronizarFechasHoras(
            IList<(DateTimePicker dtpFecha, DateTimePicker dtpHora)> jerarquia,
            bool esModificable)
        {
            if (jerarquia == null || jerarquia.Count == 0)
                return;

            // Habilitar todos los controles según el contexto
            foreach (var (dtpFecha, dtpHora) in jerarquia)
            {
                dtpFecha.Enabled = esModificable;
                dtpHora.Enabled = dtpFecha.Checked && esModificable;
            }

            // Aplicar cascada
            for (int i = 0; i < jerarquia.Count; i++)
            {
                var (dtpFecha, dtpHora) = jerarquia[i];

                if (!dtpFecha.Checked)
                {
                    // Si no está marcada, su MinDate se reinicia
                    dtpFecha.MinDate = FechaMinSql;
                    continue;
                }

                if (i > 0)
                {
                    var (dtpFechaPrev, dtpHoraPrev) = jerarquia[i - 1];

                    // Ajustar fecha mínima
                    dtpFecha.MinDate = dtpFechaPrev.Value.Date;

                    if (dtpFecha.Value.Date < dtpFechaPrev.Value.Date)
                        dtpFecha.Value = dtpFechaPrev.Value.Date;

                    // Ajustar hora si coinciden fechas
                    if (dtpFecha.Value.Date == dtpFechaPrev.Value.Date &&
                        dtpHora.Value.TimeOfDay < dtpHoraPrev.Value.TimeOfDay)
                    {
                        dtpHora.Value = dtpFecha.Value.Date.Add(dtpHoraPrev.Value.TimeOfDay);
                    }
                }
            }
        }
    }
}

//Uso en tu formulario
//En lugar de llamar a métodos separados, defines tu jerarquía y la pasas al helper:
//private void SincronizarJerarquiaFechas()
//{
//    var jerarquia = new List<(DateTimePicker, DateTimePicker)>
//    {
//        (dtpVenta, dtpHoraVenta),
//        (dtpRequerido, dtpHoraRequerido),
//        (dtpEnvio, dtpHoraEnvio)
//    };

//    bool esModificable = (tabcOperacion.SelectedTab == tabpModificar || tabcOperacion.SelectedTab == tabpRegistrar);

//    DateTimeCascadeHelper.SincronizarFechasHoras(jerarquia, esModificable);
//}

