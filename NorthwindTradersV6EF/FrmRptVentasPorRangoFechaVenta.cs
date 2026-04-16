// https://www.youtube.com/watch?v=2-YkNo1Os3Y&list=PL_1AVI-bgZKQ2MSDejVmaaxNenhETwwx_&index=7
// https://www.youtube.com/watch?v=7AvCaq7a1fc&list=PL_1AVI-bgZKQ2MSDejVmaaxNenhETwwx_&index=5
using BLL.EF.Services;
using DAL.EF;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptVentasPorRangoFechaVenta : Form
    {

        private readonly string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;

        public FrmRptVentasPorRangoFechaVenta()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptVentasPorRangoFechaVenta_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            MostrarReporte();
        }

        private void MostrarReporte()
        {
            string subtitulo;
            if (DtpVentaIni.Checked & DtpVentaFin.Checked)
                subtitulo = $"[ Fecha de venta inicial: {DtpVentaIni.Value.ToShortDateString()} ] - [ Fecha de venta final: {DtpVentaFin.Value.ToShortDateString()} ]";
            else
                subtitulo = "[ Fecha de venta inicial: Nulo ] - [ Fecha de venta final: Nulo ]";
            MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
            DataTable dt;
            if (DtpVentaIni.Checked & DtpVentaFin.Checked)
                dt = OrderService.ObtenerVentasPorFechaVenta(DtpVentaIni.Value.Date, DtpVentaFin.Value.Date.AddDays(1));
            else
                dt = OrderService.ObtenerVentasPorFechaVenta(null, null);
            MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {dt.Rows.Count} venta(s) para el rango de fecha de venta indicado. {subtitulo}");
            if (dt.Rows.Count > 0)
            {
                reportViewer1.BackColor = System.Drawing.Color.White;
                ReportDataSource rptDataSource = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rptDataSource);
                ReportParameter rptParameter = new ReportParameter("subtitulo", subtitulo);
                reportViewer1.LocalReport.SetParameters(new ReportParameter[] { rptParameter });
                reportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OrderDetailsSubReportProcessing);
                reportViewer1.RefreshReport();
            }
            else
            {
                reportViewer1.BackColor = System.Drawing.Color.White;
                reportViewer1.Clear();
                ReportDataSource rptDataSource = new ReportDataSource("DataSet1", new DataTable());
                reportViewer1.LocalReport.DataSources.Add(rptDataSource);
                ReportParameter rptParameter = new ReportParameter("subtitulo", subtitulo);
                reportViewer1.LocalReport.SetParameters(new ReportParameter[] { rptParameter });
                reportViewer1.RefreshReport();
                MessageBox.Show(Utils.noDatos, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OrderDetailsSubReportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int orderID = int.Parse(e.Parameters["OrderID"].Values[0].ToString());
            List<Order_Detail> ventaDetalles = Order_DetailService.ObtenerVentaDetallePorVentaId(orderID);
            ReportDataSource rptDataSource = new ReportDataSource("DataSet1", ventaDetalles);
            e.DataSources.Add(rptDataSource);
        }

        private void DtpVentaIni_ValueChanged(object sender, EventArgs e)
        {
            if (DtpVentaIni.Checked)
                DtpVentaFin.Checked = true;
            else
                DtpVentaFin.Checked = false;
        }

        private void DtpVentaFin_ValueChanged(object sender, EventArgs e)
        {
            if (DtpVentaFin.Checked)
                DtpVentaIni.Checked = true;
            else
                DtpVentaIni.Checked = false;
        }

        private void DtpVentaIni_Leave(object sender, EventArgs e)
        {
            if (DtpVentaIni.Checked && DtpVentaFin.Checked)
                if (DtpVentaFin.Value < DtpVentaIni.Value)
                    DtpVentaFin.Value = DtpVentaIni.Value;
        }

        private void DtpVentaFin_Leave(object sender, EventArgs e)
        {
            if (DtpVentaIni.Checked && DtpVentaFin.Checked)
                if (DtpVentaFin.Value < DtpVentaIni.Value)
                    DtpVentaIni.Value = DtpVentaFin.Value;
        }
    }
}
