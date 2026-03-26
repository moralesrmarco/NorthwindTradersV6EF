using BLL;
using Microsoft.Reporting.WinForms;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptCategorias : Form
    {
        string _connectionString = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        CategoriaBLL _categoriaBLL;

        public FrmRptCategorias()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            _categoriaBLL = new CategoriaBLL(_connectionString);
            reportViewer1.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmRptCategorias_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptCategorias_Load(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var categorias = _categoriaBLL.ObtenerCategorias(false, null, true);
                OleImageHelper.CleanOleHeader(categorias, "CategoryID", "Picture", 1, 8);
                MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {categorias.Count} registro(s)");
                ReportDataSource reportDataSource = new ReportDataSource("DataSet1", categorias);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                reportViewer1.LocalReport.Refresh();
                reportViewer1.BackColor = Color.White;
                reportViewer1.RefreshReport();
                if (categorias.Count == 0)
                    U.NotificacionWarning(Utils.noDatos);
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }
    }
}
