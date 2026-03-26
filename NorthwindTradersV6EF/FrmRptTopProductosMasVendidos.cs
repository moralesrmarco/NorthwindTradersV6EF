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
    public partial class FrmRptTopProductosMasVendidos : Form
    {

        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private readonly GraficasService _graficasService;

        public FrmRptTopProductosMasVendidos()
        {
            InitializeComponent();
            _graficasService = new GraficasService(cnStr);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptTopProductosMasVendidos_Load(object sender, EventArgs e)
        {
            LlenarCmbTopProductos();
            LlenarCmbAños();
            CargarTopProductos(Convert.ToInt32(CmbTopProductos.SelectedValue), Convert.ToInt32(CmbAños.SelectedValue));
        }

        private void LlenarCmbTopProductos()
        {
            List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();
            for (int i = 10; i <= 50; i = i + 5)
            {
                items.Add(new KeyValuePair<string, int>($"{i} productos", i));
            }
            CmbTopProductos.DisplayMember = "Key";
            CmbTopProductos.ValueMember = "Value";
            CmbTopProductos.DataSource = items;
            CmbTopProductos.SelectedIndex = 0;
        }

        private void LlenarCmbAños()
        {
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            try
            {
                CmbAños.DataSource = _graficasService.ObtenerTop10AñosDeVentas(false);
                CmbAños.DisplayMember = "Texto";
                CmbAños.ValueMember = "Valor";
                CmbAños.SelectedIndex = 1;
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


        private void BtnMostrar_Click(object sender, EventArgs e)
        {
            CargarTopProductos(Convert.ToInt32(CmbTopProductos.SelectedValue), Convert.ToInt32(CmbAños.SelectedValue));
        }

        private void CargarTopProductos(int topProductos, int año)
        {
            groupBox1.Text = $"» Reporte gráfico top {topProductos} productos más vendidos ({CmbAños.Text}) «";
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt = null;
            try
            {
                dt = _graficasService.ObtenerTopProductosRpt(topProductos, año);
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
            finally
            {
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            if (dt != null)
            {
                reportViewer1.LocalReport.DataSources.Clear();
                var rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Titulo", groupBox1.Text));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Subtitulo", $"Top {CmbTopProductos.SelectedValue.ToString()} productos más vendidos ({CmbAños.Text})"));
                reportViewer1.RefreshReport();
            }
        }

    }
}
