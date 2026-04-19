using BLL.EF.Services;
using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{

    public partial class FrmRptGraficaVentasMensuales : Form
    {
        public FrmRptGraficaVentasMensuales()
        {
            InitializeComponent();
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
                DataTable dt = GraficasService.ObtenerTop10AñosDeVentas();
                CmbVentasMensualesDelAño.DisplayMember = "Texto";
                CmbVentasMensualesDelAño.ValueMember = "Valor";
                CmbVentasMensualesDelAño.DataSource = dt;
                CmbVentasMensualesDelAño.SelectedIndex = -1;
                CmbVentasMensualesDelAño.SelectedIndexChanged += CmbVentasMensualesDelAño_SelectedIndexChanged;
                CmbVentasMensualesDelAño.SelectedValue = DateTime.Today.Year;
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
            if (CmbVentasMensualesDelAño.SelectedIndex == 0)
            {
                MessageBox.Show("Seleccione un año válido.");
                return;
            }
            DataTable dt = null;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dt = ReportDataTableAdapterHelper.ConvertirVentasMensuales(GraficasService.ObtenerVentasMensuales(Convert.ToInt32(CmbVentasMensualesDelAño.SelectedValue)));
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
                string tit = string.Empty;
                if (Convert.ToInt32(CmbVentasMensualesDelAño.SelectedValue) > 0)
                    tit = $"» Reporte gráfico de ventas mensuales ({CmbVentasMensualesDelAño.SelectedValue}) «";
                else 
                    tit = $"» Reporte gráfico de ventas mensuales (todos los años) «";
                groupBox1.Text = tit ;
                string subTit = string.Empty ;
                if (Convert.ToInt32(CmbVentasMensualesDelAño.SelectedValue) > 0)
                    subTit = $"Ventas mensuales ({CmbVentasMensualesDelAño.SelectedValue})";
                else
                    subTit = "Ventas mensuales (todos los años)";
                    // 1. Limpia fuentes previas
                reportViewer1.LocalReport.DataSources.Clear();
                // 2. Usa el nombre EXACTO del DataSet del RDLC
                var rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Anio", $"({CmbVentasMensualesDelAño.SelectedValue.ToString()})"));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", subTit));
                // 3. Refresca el reporte
                reportViewer1.RefreshReport();
            }
        }
    }
}
