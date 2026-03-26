using BLL.Services;
using Microsoft.Reporting.WinForms;
using System;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{

    public partial class FrmRptGraficaVentasMensuales : Form
    {

        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly GraficasService _graficasService;

        public FrmRptGraficaVentasMensuales()
        {
            InitializeComponent();
            _graficasService = new GraficasService(cnStr);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptGraficaVentasMensuales_Load(object sender, EventArgs e)
        {
            LlenarCmbVentasMensualesDelAño();
        }

        private void LlenarCmbVentasMensualesDelAño()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                CmbVentasMensualesDelAño.SelectedIndexChanged -= CmbVentasMensualesDelAño_SelectedIndexChanged;
                DataTable dt = _graficasService.ObtenerAñosDeVentas(false);
                CmbVentasMensualesDelAño.DisplayMember = "YearOrderDate";
                CmbVentasMensualesDelAño.ValueMember = "YearOrderDate";
                CmbVentasMensualesDelAño.DataSource = dt;
                CmbVentasMensualesDelAño.SelectedIndex = -1;
                CmbVentasMensualesDelAño.SelectedIndexChanged += CmbVentasMensualesDelAño_SelectedIndexChanged;
                CmbVentasMensualesDelAño.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
                return;
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
        }

        private void CmbVentasMensualesDelAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = null;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dt = ReportDataTableAdapterHelper.ConvertirVentasMensuales(_graficasService.ObtenerVentasMensuales(Convert.ToInt32(CmbVentasMensualesDelAño.SelectedValue)));
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
                return;
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            if (dt != null)
            {
                groupBox1.Text = $"» Reporte gráfico de ventas mensuales del año {CmbVentasMensualesDelAño.SelectedValue} «";
                // 1. Limpia fuentes previas
                reportViewer1.LocalReport.DataSources.Clear();
                // 2. Usa el nombre EXACTO del DataSet del RDLC
                var rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Anio", CmbVentasMensualesDelAño.SelectedValue.ToString()));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", $"Ventas mensuales del año {CmbVentasMensualesDelAño.SelectedValue}"));
                // 3. Refresca el reporte
                reportViewer1.RefreshReport();
            }
        }
    }
}
