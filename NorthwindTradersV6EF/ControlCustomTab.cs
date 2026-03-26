using System.Drawing;
using System.Windows.Forms;

namespace NorthwindTradersV6EF
{
    public class ControlCustomTab : TabControl
    {
        private const int ExtraMargin = 0;

        public ControlCustomTab()
        {
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.SizeMode = TabSizeMode.Normal; // importante: Normal para permitir anchos distintos
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            TabPage page = this.TabPages[e.Index];
            bool isSelected = (e.Index == this.SelectedIndex);

            // Colores según selección
            Color backColor = isSelected ? SystemColors.Highlight : SystemColors.GradientActiveCaption;
            Color textColor = isSelected ? SystemColors.HighlightText : SystemColors.ActiveCaptionText;
            Font textFont = isSelected ? new Font(e.Font, FontStyle.Bold) : e.Font;

            Rectangle rect = e.Bounds;

            // Fondo
            using (SolidBrush brush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(brush, rect);

            // Ícono (apagado o encendido según selección)
            string key = isSelected ? "tabIconOn" : "tabIcon";
            Image img = null;
            if (this.ImageList != null && this.ImageList.Images.ContainsKey(key))
                img = this.ImageList.Images[key];

            int iconWidth = img != null ? img.Width : 11;
            int iconHeight = img != null ? img.Height : 14;

            // Posición del ícono: fijo al borde derecho
            int iconX = rect.Right - iconWidth - 5; // margen de 5px
            int iconY = rect.Top + (rect.Height - iconHeight) / 2;

            // Área de texto: desde la izquierda hasta justo antes del ícono
            Rectangle textRect = new Rectangle(
                rect.Left + 4,
                rect.Top,
                (iconX - rect.Left) - 6, // separación entre texto e ícono
                rect.Height
            );

            // Texto con puntos suspensivos si no cabe
            TextRenderer.DrawText(e.Graphics, page.Text, textFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Ícono
            if (img != null)
                e.Graphics.DrawImage(img, iconX, iconY, iconWidth, iconHeight);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            AjustarTabSizes();
        }

        public void AjustarTabSizes()
        {
            foreach (TabPage page in this.TabPages)
            {
                Size textSize = TextRenderer.MeasureText(page.Text, this.Font);
                int iconWidth = (page.ImageIndex >= 0 && this.ImageList != null)
                    ? this.ImageList.ImageSize.Width + 4
                    : 0;

                int ancho = textSize.Width + ExtraMargin + iconWidth;
                page.Tag = ancho; // guardamos ancho individual en Tag
            }
        }

        public void ConfigurarIconos(Image iconOff, Image iconOn)
        {
            this.ImageList = new ImageList();
            this.ImageList.ImageSize = new Size(11, 14);
            this.ImageList.Images.Add("tabIcon", iconOff);
            this.ImageList.Images.Add("tabIconOn", iconOn);
        }
    }
}
