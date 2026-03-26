using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public class U
    {
        public enum NotificationType
        {
            Information,
            Error,
            Warning,
            Question,
            Shield,
            Default
        }

        public enum NotificationMode
        {
            Aceptar,
            SiNo
        }

        public static DialogResult NotificacionInformation(string mensaje)
        {
            SystemSounds.Asterisk.Play();
            return ShowNotificacion(mensaje, NotificationType.Information, NotificationMode.Aceptar);
        }

        public static DialogResult NotificacionError(string mensaje)
        {
            SystemSounds.Hand.Play(); // Sonido de error (ícono de mano)
            return ShowNotificacion(mensaje, NotificationType.Error, NotificationMode.Aceptar);
        }

        public static DialogResult NotificacionWarning(string mensaje)
        {
            SystemSounds.Exclamation.Play();
            return ShowNotificacion(mensaje, NotificationType.Warning, NotificationMode.Aceptar);
        }

        public static DialogResult NotificacionQuestion(string mensaje)
        {
            SystemSounds.Question.Play();
            return ShowNotificacion(mensaje, NotificationType.Question, NotificationMode.SiNo);
        }

        public static DialogResult NotificacionShield(string mensaje)
        {
            SystemSounds.Beep.Play();
            return ShowNotificacion(mensaje, NotificationType.Shield, NotificationMode.Aceptar);
        }

        public static DialogResult NotificacionDefault(string mensaje)
        {
            SystemSounds.Beep.Play();
            return ShowNotificacion(mensaje, NotificationType.Default, NotificationMode.Aceptar);
        }

        public static DialogResult ShowNotificacion(string mensaje, NotificationType tipo, NotificationMode modo)
        {
            Color colorTexto;
            Icon icono;

            switch (tipo)
            {
                case NotificationType.Information:
                    colorTexto = Color.Green;
                    icono = SystemIcons.Information;
                    break;
                case NotificationType.Error:
                    colorTexto = Color.Red;
                    icono = SystemIcons.Error;
                    break;
                case NotificationType.Warning:
                    colorTexto = Color.Orange;
                    icono = SystemIcons.Warning;
                    break;
                case NotificationType.Question:
                    colorTexto = Color.Blue;
                    icono = SystemIcons.Question;
                    break;
                case NotificationType.Shield:
                    colorTexto = Color.DarkSlateGray;
                    icono = SystemIcons.Shield;
                    break;
                default:
                    colorTexto = Color.Black;
                    icono = SystemIcons.Application;
                    break;
            }

            using (var frm = new FrmNotificacion(mensaje, icono, colorTexto, modo))
            {
                return frm.ShowDialog();
            }
        }

        public static void MsgCatchOue(Exception ex)
        {
            Utils.MsgCatchOue(ex, () => MDIPrincipal.ActualizarBarraDeEstado());
        }
        
        public static void MsgWarning(string mensaje) => Utils.MsgWarning(mensaje);

        public static void MsgExclamation(string mensaje) => Utils.MsgExclamation(mensaje);

        public static void MsgError(string mensaje) => Utils.MsgError(mensaje);

        public static void MsgInformation(string mensaje) => Utils.MsgInformation(mensaje);

        public static DialogResult MsgQuestion(string mensaje) => Utils.MsgQuestion(mensaje);

        public static DialogResult MsgCerrarForm() => Utils.MsgCerrarForm();

    }
}
