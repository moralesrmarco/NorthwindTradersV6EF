using BLL.Services;
using Microsoft.Reporting.WinForms;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptGraficaDeVentasDeVendedoresPorAnio : Form
    {

        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly GraficasService _graficasService;

        public FrmRptGraficaDeVentasDeVendedoresPorAnio()
        {
            InitializeComponent();
            _graficasService = new GraficasService(cnStr);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptGraficaDeVentasDeVendedoresPorAnio_Load(object sender, EventArgs e)
        {
            LlenarCmbVentas();
        }

        private void LlenarCmbVentas()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var dt = _graficasService.ObtenerAñosDeVentas(false);
                foreach (DataRow row in dt.Rows)
                    CmbVentas.Items.Add(Convert.ToInt32(row["YearOrderDate"]));
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            CmbVentas.SelectedIndex = 0; 
        }

        private void CmbVentas_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenarGrafico(Convert.ToInt32(CmbVentas.Text.ToString()));
        }

        private void LlenarGrafico(int year)
        {
            groupBox1.Text = $"» Reporte gráfico de ventas por vendedores del año {year} «";
            DataTable dt = null;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dt = ReportDataTableAdapterHelper.ConvertirVendedorTotalVentas(_graficasService.ObtenerVentasPorVendedores(year));
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            reportViewer1.LocalReport.DataSources.Clear();
            var rds = new ReportDataSource("DataSet1", dt);
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", $"Ventas por vendedores del año {year}"));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Anio", year.ToString()));
            reportViewer1.RefreshReport();
        }
    }
}
