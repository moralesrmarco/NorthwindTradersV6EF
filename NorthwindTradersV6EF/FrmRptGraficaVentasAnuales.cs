using BLL.Services;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptGraficaVentasAnuales : Form
    {
        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly GraficasService _graficasService;

        public FrmRptGraficaVentasAnuales()
        {
            InitializeComponent();
            _graficasService = new GraficasService(cnStr);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptGraficaVentasAnuales_Load(object sender, EventArgs e)
        {
            LlenarCmbVentasAnuales();
        }

        private void LlenarCmbVentasAnuales()
        {
            int totalAñosDisponibles = _graficasService.ObtenerTotalAñosConVentas();
            int limite = Math.Min(totalAñosDisponibles, 10);
            var items = new List<KeyValuePair<string, int>>();
            for (int i = 2; i <= limite; i++)
            {
                items.Add(new KeyValuePair<string, int>($"{i} Años", i));
            }
            CmbVentasAnuales.SelectedIndexChanged -= CmbVentasAnuales_SelectedIndexChanged;
            CmbVentasAnuales.DataSource = items;
            CmbVentasAnuales.DisplayMember = "Key";
            CmbVentasAnuales.ValueMember = "Value";
            CmbVentasAnuales.SelectedIndex = -1;
            CmbVentasAnuales.SelectedIndexChanged += CmbVentasAnuales_SelectedIndexChanged;
            CmbVentasAnuales.SelectedIndex = 0;
        }

        private void CmbVentasAnuales_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarComparativoVentasAnuales(Convert.ToInt32(CmbVentasAnuales.SelectedValue.ToString()));
        }

        private void CargarComparativoVentasAnuales(int years)
        {
            groupBox1.Text = $"» Comparativo de ventas anuales de los últimos {years} años «";
            DataTable dtComparativo = null;
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                dtComparativo = ReportDataTableAdapterHelper.ConvertirVentaAnualComparativa(_graficasService.ObtenerVentasMensualesPorAños(years));
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
            var rds = new ReportDataSource("DataSet1", dtComparativo);
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Anio", CmbVentasAnuales.Text));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", $"Comparativo de ventas anuales de los últimos {years} años"));
            reportViewer1.RefreshReport();
        }
    }
}
