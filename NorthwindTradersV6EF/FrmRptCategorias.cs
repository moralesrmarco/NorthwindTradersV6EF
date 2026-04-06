using BLL.EF;
using Microsoft.Reporting.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptCategorias : Form
    {
        public FrmRptCategorias()
        {
            InitializeComponent();
            reportViewer1.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmRptCategorias_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptCategorias_Load(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var categorias = CategoryBLL.ObtenerCategorias(false, null, true);
                OleImageHelper.CleanOleHeader(categorias, "CategoryID", "Picture", 1, 8);
                MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {categorias.Count} registro(s)");
                ReportDataSource reportDataSource = new ReportDataSource("DataSet1", categorias);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                reportViewer1.LocalReport.Refresh();
                reportViewer1.BackColor = Color.White;
                reportViewer1.RefreshReport();
                if (categorias.Count == 0)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.noDatos, true);
                    U.NotificacionWarning(Utils.noDatos);
                }

            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }
    }
}
