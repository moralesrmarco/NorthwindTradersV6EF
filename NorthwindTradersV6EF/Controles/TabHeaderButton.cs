using System.Drawing;
using System.Windows.Forms;

namespace NorthwindTradersV6EF.Controles
{
    public class TabHeaderButton : Panel
    {
        private Label lbl;
        private PictureBox pic;
        private CustomTabHeader owner;
        private Font regularFont;
        private Font boldFont;

        public int Index { get; }

        public TabHeaderButton(string text, int index, CustomTabHeader owner)
        {
            this.owner = owner;
            Index = index;

            Padding = new Padding(8, 4, 8, 4);
            Margin = Padding.Empty;
            Cursor = Cursors.Hand;
            //Height = owner.Height - 4;
            Height = owner.Height;
            DoubleBuffered = true;

            lbl = new Label
            {
                Text = text,
                AutoSize = true,
                Dock = DockStyle.Left
            };

            pic = new PictureBox
            {
                Width = 11,
                Dock = DockStyle.Right,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            Controls.Add(pic);
            Controls.Add(lbl);

            regularFont = lbl.Font;
            boldFont = new Font(lbl.Font, FontStyle.Bold);

            Width = lbl.Width + pic.Width + Padding.Left + Padding.Right;

            Click += (s, e) => owner.TabControl.SelectedIndex = Index;
            lbl.Click += (s, e) => owner.TabControl.SelectedIndex = Index;
            pic.Click += (s, e) => owner.TabControl.SelectedIndex = Index;
        }

        public void Update(int selectedIndex)
        {
            bool selected = Index == selectedIndex;

            BackColor = selected
                ? SystemColors.Highlight
                : SystemColors.GradientActiveCaption;

            lbl.ForeColor = selected
                ? SystemColors.HighlightText
                : SystemColors.ActiveCaptionText;

            lbl.Font = selected ? boldFont : regularFont;
            lbl.AutoSize = true;

            Width = lbl.Width + pic.Width + Padding.Left + Padding.Right;

            pic.Image = selected
                ? owner.IconOn
                : owner.IconOff;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            ControlPaint.DrawBorder(
                e.Graphics,
                ClientRectangle,
                SystemColors.ControlLightLight, 2, ButtonBorderStyle.Outset, // arriba-izq claros
                SystemColors.ControlDark, 2, ButtonBorderStyle.Outset,       // abajo-der oscuros
                SystemColors.ControlLightLight, 2, ButtonBorderStyle.Outset,
                SystemColors.ControlDark, 2, ButtonBorderStyle.Outset);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                boldFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
