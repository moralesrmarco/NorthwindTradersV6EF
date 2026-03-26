using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Utilities
{
    public static class StatusIconHelper
    {
        public static void ShowIcons(
            Control target,
            ToolTip toolTip,
            (PictureBox pb, Image icon, string message, bool visible) error,
            (PictureBox pb, Image icon, string message, bool visible) info,
            (PictureBox pb, Image icon, string message, bool visible) warning)
        {
            // Oculta los tres antes de dibujar
            error.pb.Visible = false;
            info.pb.Visible = false;
            warning.pb.Visible = false;

            // Construye lista en orden Error → Information → Warning
            var icons = new List<(PictureBox pb, Image icon, string message, bool visible)>
        {
            error, info, warning
        };

            // Posiciona sólo los visibles, acumulando offset
            int offset = 12;
            foreach (var iconData in icons)
            {
                if (!iconData.visible) continue;

                PositionIcon(target, iconData.pb, offset);
                iconData.pb.Image = iconData.icon;
                iconData.pb.SizeMode = PictureBoxSizeMode.Zoom;
                iconData.pb.Visible = true;
                toolTip.SetToolTip(iconData.pb, iconData.message);

                offset += 18; // separación horizontal entre íconos
            }
        }

        private static void PositionIcon(Control target, PictureBox pb, int offset)
        {
            pb.Size = new Size(16, 16);
            pb.Location = new Point(
                target.Right + 5 + offset,
                target.Top + (target.Height - pb.Height) / 2
            );
            pb.BringToFront();
        }

        // 👉 Método para ocultar los PictureBox
        public static void HideIcons(params PictureBox[] pictureBoxes)
        {
            foreach (var pb in pictureBoxes)
            {
                if (pb != null)
                    pb.Visible = false;
            }
        }
    }
}
