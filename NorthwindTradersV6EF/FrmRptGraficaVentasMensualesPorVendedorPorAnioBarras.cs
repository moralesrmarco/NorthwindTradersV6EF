using BLL.EF.Services;
using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptGraficaVentasMensualesPorVendedorPorAnioBarras : Form
    {
        public FrmRptGraficaVentasMensualesPorVendedorPorAnioBarras()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptGraficaVentasMensualesPorVendedorPorAnioBarras_Load(object sender, EventArgs e)
        {
            LlenarCmbVentasDelAño();
        }

        private void LlenarCmbVentasDelAño()
        {
            try
            {
                CmbVentasDelAño.SelectedIndexChanged -= CmbVentasDelAño_SelectedIndexChanged;
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dt = BLL.EF.Services.GraficasService.ObtenerTop10AñosDeVentas();
                CmbVentasDelAño.DataSource = dt;
                CmbVentasDelAño.DisplayMember = "Texto";
                CmbVentasDelAño.ValueMember = "Valor";
                CmbVentasDelAño.SelectedIndexChanged += CmbVentasDelAño_SelectedIndexChanged;
                CmbVentasDelAño.SelectedValue = DateTime.Today.Year;
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
        }

        private void CmbVentasDelAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbVentasDelAño.SelectedIndex == 0)
            {
                MessageBox.Show("Seleccione un año válido.");
                return;
            }
            LlenarGrafico(Convert.ToInt32(CmbVentasDelAño.SelectedValue.ToString()));
        }

        private void LlenarGrafico(int year) 
        {
            string tit = string.Empty;
            if (year > 0)
                tit = $"» Reporte gráfico comparativo de ventas mensuales por vendedores ({year}) «";
            else
                tit = "» Reporte gráfico comparativo de ventas mensuales por vendedores (todos los años) «";
            string subTit = string.Empty;
            if (year > 0)
                subTit = $"Ventas mensuales por vendedores ({year})";
            else
                subTit = "Ventas mensuales por vendedores (todos los años)";
            groupBox1.Text = tit;
            DataTable dt = null;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dt = GraficasService.ObtenerVentasMensualesPorVendedoresPorAño(year);
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
            reportViewer1.LocalReport.DataSources.Clear();
            var rds = new ReportDataSource("DataSet1", dt);
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", subTit));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Anio", year.ToString()));
            reportViewer1.RefreshReport();
        }
    }
}
