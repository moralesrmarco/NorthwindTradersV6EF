using System;
using System.Windows.Forms;

namespace NorthwindTradersV6EF
{
    public partial class ControlDetalleDeLaVenta : UserControl
    {
        // Propiedad pública para acceder al DataGridView interno
        public DataGridView DgvDetalle => dgvDetalle;

        public event DataGridViewCellEventHandler DgvDetalle_CellClick;

        public ControlDetalleDeLaVenta()
        {
            InitializeComponent();
            dgvDetalle.CellClick += DgvDetalle_CellClick_Handler;
        }

        private void DgvDetalle_CellClick_Handler(object sender, DataGridViewCellEventArgs e)
        {
            DgvDetalle_CellClick?.Invoke(sender, e);
        }
    }
}
