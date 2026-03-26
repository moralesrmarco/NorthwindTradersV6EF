using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class ControlTotalesDeLaVenta : UserControl
    {
        public NudNoWheel NudNumProd => nudNumProd;
        public NudNoWheel NudTotalDeUnidades => nudTotalDeUnidades;
        public NudNoWheel NudSubtotalDelImporte => nudSubtotalDelImporte;
        public NudNoWheel NudSubtotalDelImporteDelDescuento => nudSubtotalDelImporteDelDescuento;
        public NudNoWheel NudSubtotalDelImporteConDescuento => nudSubtotalDelImporteConDescuento;
        public NudNoWheel NudSubtotalDelImporteSinIVA => nudSubtotalDelImporteSinIVA;
        public NudNoWheel NudSubtotalDelImporteDelIVA => nudSubtotalDelImporteDelIVA;
        public NudNoWheel NudTotal => nudTotal;

        public ControlTotalesDeLaVenta()
        {
            InitializeComponent();
        }
    }
}
