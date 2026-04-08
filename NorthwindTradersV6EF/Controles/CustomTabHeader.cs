using System.Drawing;
using System.Windows.Forms;

namespace NorthwindTradersV6EF.Controles
{
    public class CustomTabHeader : FlowLayoutPanel
    {
        public TabControl TabControl { get; set; }

        public Image IconOn { get; set; }
        public Image IconOff { get; set; }

        public CustomTabHeader()
        {
            Height = 28;
            Dock = DockStyle.Top;
            DoubleBuffered = true;
            WrapContents = false;
            AutoScroll = false;
            FlowDirection = FlowDirection.LeftToRight;
        }

        public void Build()
        {
            if (TabControl == null) return;

            Controls.Clear();
            for (int i = 0; i < TabControl.TabPages.Count; i++)
            {
                var btn = new TabHeaderButton(
                    TabControl.TabPages[i].Text,
                    i,
                    this
                );
                Controls.Add(btn);
            }

            TabControl.SelectedIndexChanged += (s, e) => UpdateState();
            UpdateState();
        }

        internal void UpdateState()
        {
            foreach (TabHeaderButton btn in Controls)
                btn.Update(TabControl.SelectedIndex);
        }
    }
}
