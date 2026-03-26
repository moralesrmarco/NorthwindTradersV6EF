using System;
using System.Windows.Forms;

namespace Utilities
{
    public static class DateTimePickerHelper
    {
        private static readonly DateTime FechaMinSql = new DateTime(1753, 1, 1);

        public static void SincronizarJerarquiaFechas(
    DateTimePicker dtpVenta, DateTimePicker dtpHoraVenta,
    DateTimePicker dtpRequerido, DateTimePicker dtpHoraRequerido,
    DateTimePicker dtpEnvio, DateTimePicker dtpHoraEnvio,
    bool esModificable)
        {
            // Habilitar padres
            dtpVenta.Enabled = esModificable;
            dtpRequerido.Enabled = esModificable;
            dtpEnvio.Enabled = esModificable;

            // Habilitar horas solo si la fecha está marcada
            dtpHoraVenta.Enabled = dtpVenta.Checked && esModificable;
            dtpHoraRequerido.Enabled = dtpRequerido.Checked && esModificable;
            dtpHoraEnvio.Enabled = dtpEnvio.Checked && esModificable;

            bool requeridoChecked = dtpRequerido.Checked;
            bool envioChecked = dtpEnvio.Checked;

            // VENTA
            if (!dtpVenta.Checked)
            {
                dtpRequerido.MinDate = FechaMinSql;
                dtpEnvio.MinDate = FechaMinSql;
                dtpRequerido.Checked = requeridoChecked;
                dtpEnvio.Checked = envioChecked;
                return;
            }

            // REQUERIDO depende de VENTA
            dtpRequerido.MinDate = dtpVenta.Value.Date;
            if (requeridoChecked && dtpRequerido.Value.Date < dtpVenta.Value.Date)
            {
                dtpRequerido.Value = dtpVenta.Value.Date;
            }

            // ENVÍO depende de VENTA
            dtpEnvio.MinDate = dtpVenta.Value.Date;
            if (envioChecked && dtpEnvio.Value.Date < dtpVenta.Value.Date)
            {
                dtpEnvio.Value = dtpVenta.Value.Date;
            }

            // HORAS en cascada
            ValidarHorasEnCascada(dtpVenta, dtpHoraVenta, dtpRequerido, dtpHoraRequerido, dtpEnvio, dtpHoraEnvio);
        }

        // esta version es para la jerarquía de fechas/horas: Venta → Requerido → Envío
        // la dejo para usos futuros
        public static void SincronizarJerarquiaFechas_V_1(
            DateTimePicker dtpVenta, DateTimePicker dtpHoraVenta,
            DateTimePicker dtpRequerido, DateTimePicker dtpHoraRequerido,
            DateTimePicker dtpEnvio, DateTimePicker dtpHoraEnvio,
            bool esModificable)
        {
            // Habilitar padres
            dtpVenta.Enabled = esModificable;
            dtpRequerido.Enabled = esModificable;
            dtpEnvio.Enabled = esModificable;

            // Habilitar horas solo si la fecha está marcada
            dtpHoraVenta.Enabled = dtpVenta.Checked && esModificable;
            dtpHoraRequerido.Enabled = dtpRequerido.Checked && esModificable;
            dtpHoraEnvio.Enabled = dtpEnvio.Checked && esModificable;

            bool requeridoChecked = dtpRequerido.Checked;
            bool envioChecked = dtpEnvio.Checked;

            // VENTA
            if (!dtpVenta.Checked)
            {
                dtpRequerido.MinDate = FechaMinSql;
                dtpEnvio.MinDate = FechaMinSql;
                dtpRequerido.Checked = requeridoChecked;
                dtpEnvio.Checked = envioChecked;
                return;
            }

            // REQUERIDO depende de VENTA
            dtpRequerido.MinDate = dtpVenta.Value.Date;
            if (requeridoChecked)
            {
                if (dtpRequerido.Value.Date < dtpVenta.Value.Date)
                    dtpRequerido.Value = dtpVenta.Value.Date;

                dtpEnvio.MinDate = dtpRequerido.Value.Date;
            }
            else
            {
                dtpEnvio.MinDate = dtpVenta.Value.Date;
            }

            // ENVÍO depende de REQUERIDO o VENTA
            if (envioChecked)
            {
                if (dtpEnvio.Value.Date < dtpEnvio.MinDate)
                    dtpEnvio.Value = dtpEnvio.MinDate;
            }

            // HORAS en cascada
            ValidarHorasEnCascada(dtpVenta, dtpHoraVenta, dtpRequerido, dtpHoraRequerido, dtpEnvio, dtpHoraEnvio);
        }

        private static void ValidarHorasEnCascada(
            DateTimePicker dtpVenta, DateTimePicker dtpHoraVenta,
            DateTimePicker dtpRequerido, DateTimePicker dtpHoraRequerido,
            DateTimePicker dtpEnvio, DateTimePicker dtpHoraEnvio)
        {
            // VENTA → REQUERIDO
            if (dtpVenta.Checked && dtpRequerido.Checked)
            {
                AjustarHoraSiFechaCoincide(
                    dtpRequerido, dtpHoraRequerido,
                    dtpVenta.Value.Date, dtpHoraVenta.Value.TimeOfDay);
            }

            // BASE → ENVÍO
            if (dtpEnvio.Checked)
            {
                var (fechaBase, horaBase) = ObtenerBaseJerarquica(dtpVenta, dtpHoraVenta, dtpRequerido, dtpHoraRequerido);
                AjustarHoraSiFechaCoincide(dtpEnvio, dtpHoraEnvio, fechaBase, horaBase);
            }
        }

        private static (DateTime fechaBase, TimeSpan horaBase) ObtenerBaseJerarquica(
            DateTimePicker dtpVenta, DateTimePicker dtpHoraVenta,
            DateTimePicker dtpRequerido, DateTimePicker dtpHoraRequerido)
        {
            if (dtpRequerido.Checked)
                return (dtpRequerido.Value.Date, dtpHoraRequerido.Value.TimeOfDay);

            return (dtpVenta.Value.Date, dtpHoraVenta.Value.TimeOfDay);
        }

        private static void AjustarHoraSiFechaCoincide(
            DateTimePicker dtpFecha, DateTimePicker dtpHora,
            DateTime fechaBase, TimeSpan horaBase)
        {
            if (!dtpFecha.Checked)
                return;

            if (dtpFecha.Value.Date == fechaBase &&
                dtpHora.Value.TimeOfDay < horaBase)
            {
                dtpHora.Value = dtpFecha.Value.Date.Add(horaBase);
            }
        }

    }
}
//Uso en tu formulario
//En lugar de tener toda la lógica dentro del form, llamas al helper:
//private void dtpHoraVenta_ValueChanged(object sender, EventArgs e)
//{
//    DateTimePickerHelper.SincronizarJerarquiaFechas(
//        dtpVenta, dtpHoraVenta,
//        dtpRequerido, dtpHoraRequerido,
//        dtpEnvio, dtpHoraEnvio,
//        tabcOperacion.SelectedTab == tabpModificar || tabcOperacion.SelectedTab == tabpRegistrar
//    );
//}
//De esta forma, el helper es reutilizable en cualquier otro programa que tenga la misma jerarquía de fechas/horas.

