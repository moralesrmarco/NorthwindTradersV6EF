// https://www.youtube.com/watch?v=2-YkNo1Os3Y&list=PL_1AVI-bgZKQ2MSDejVmaaxNenhETwwx_&index=7
// https://www.youtube.com/watch?v=7AvCaq7a1fc&list=PL_1AVI-bgZKQ2MSDejVmaaxNenhETwwx_&index=5
using BLL.EF.Services;
using DAL.EF;
using DTOs.EF;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptVentasPorDiferentesCriterios : Form
    {
        public FrmRptVentasPorDiferentesCriterios()
        {
            InitializeComponent();
            controlBuscarVenta.grbBuscar.Font = new Font(controlBuscarVenta.grbBuscar.Font.FontFamily, 9.75f, FontStyle.Bold);
            // Lista de labels a los que quieres redefinir la fuente, porque la fuente se heredo del groupbox y no me gusta que se vean mas grandes los controles, por eso se le redefine el tamaño al tamaño de la fuente que se usa en todos los formularios
            List<Label> labelsPersonalizados = new List<Label>
            {
                controlBuscarVenta.label5,
                controlBuscarVenta.label6,
                controlBuscarVenta.label7,
                controlBuscarVenta.label8,
                controlBuscarVenta.label11,
                controlBuscarVenta.label14,
                controlBuscarVenta.label17,
                controlBuscarVenta.label19
            };
            // Restaurar la fuente original a los controles internos
            foreach (Label lbl in labelsPersonalizados)
            {
                lbl.Font = new Font(lbl.Font.FontFamily, 8.00f, FontStyle.Bold);
            }
            controlBuscarVenta.btnLimpiar.Font = new Font(controlBuscarVenta.btnLimpiar.Font.FontFamily, 8.00f, FontStyle.Regular);
            controlBuscarVenta.btnBuscar.Font = new Font(controlBuscarVenta.btnBuscar.Font.FontFamily, 8.00f, FontStyle.Bold);
            controlBuscarVenta.btnBuscar.Text = "Mostrar reporte";
            controlBuscarVenta.btnLimpiar.Size = new Size(140, 22);
            controlBuscarVenta.btnBuscar.Size = new Size(140, 32);
            controlBuscarVenta.btnLimpiar.Location = new Point(controlBuscarVenta.btnLimpiar.Location.X - 60, controlBuscarVenta.btnLimpiar.Location.Y);
            controlBuscarVenta.btnBuscar.Location = new Point(controlBuscarVenta.btnBuscar.Location.X - 25, controlBuscarVenta.btnBuscar.Location.Y - 5);
            // Hacer que se pinten en negro los groupboxes de los controles anidados
            foreach (var gb in controlBuscarVenta.Controls.OfType<GroupBox>())
                gb.Paint += GrbPaint;
            controlBuscarVenta.BuscarClick += ControlBuscarVentaMostrarReporte_ClickHandler;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptVentasPorDiferentesCriterios_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void ControlBuscarVentaMostrarReporte_ClickHandler(object sender, EventArgs e)
        {
            try
            {
                reportViewer1.BackColor = Color.White;
                string subtitulo = string.Empty;
                if (controlBuscarVenta.NudBIdIni.Value > 0)
                    subtitulo += $"[Id inicial: {controlBuscarVenta.NudBIdIni.Value}] - [Id final: {controlBuscarVenta.NudBIdFin.Value}] ";
                if (controlBuscarVenta.TxtBCliente.Text != "")
                    subtitulo += $"[Cliente: %{controlBuscarVenta.TxtBCliente.Text}%] ";
                if (controlBuscarVenta.dtpBFVentaIni.Checked)
                    subtitulo += $"[Fecha de venta inicial: {controlBuscarVenta.dtpBFVentaIni.Value.ToShortDateString()}] - [Fecha de venta final: {controlBuscarVenta.dtpBFVentaFin.Value.ToShortDateString()}] ";
                if (controlBuscarVenta.chkbBFVentaNull.Checked)
                    subtitulo += "[Fecha de venta inicial: Nulo] - [Fecha de venta final: Nulo] ";
                if (controlBuscarVenta.dtpBFRequeridoIni.Checked)
                    subtitulo += $"[Fecha requerido inicial: {controlBuscarVenta.dtpBFRequeridoIni.Value.ToShortDateString()} ] - [Fecha requerido final:  {controlBuscarVenta.dtpBFRequeridoFin.Value.ToShortDateString()}] ";
                if (controlBuscarVenta.chkbBFRequeridoNull.Checked)
                    subtitulo += "[Fecha requerido inicial: Nulo] - [Fecha requerido final: Nulo] ";
                if (controlBuscarVenta.dtpBFEnvioIni.Checked)
                    subtitulo += $"[Fecha de envío inicial: {controlBuscarVenta.dtpBFEnvioIni.Value.ToShortDateString()} ] - [Fecha de envío final:  {controlBuscarVenta.dtpBFEnvioFin.Value.ToShortDateString()}] ";
                if (controlBuscarVenta.chkbBFEnvioNull.Checked)
                    subtitulo += "[Fecha de envío inicial: Nulo] - [Fecha de envío final: Nulo] ";
                if (controlBuscarVenta.txtBEmpleado.Text != "")
                    subtitulo += $"[Vendedor: %{controlBuscarVenta.txtBEmpleado.Text}%] ";
                if (controlBuscarVenta.txtBCompañiaT.Text != "")
                    subtitulo += $"[Transportista: %{controlBuscarVenta.txtBCompañiaT.Text}%] ";
                if (controlBuscarVenta.txtBDirigidoa.Text != "")
                    subtitulo += $"[Enviar a: %{controlBuscarVenta.txtBDirigidoa.Text}%]";
                if (subtitulo == "")
                    subtitulo = "Ningun criterio  de selección fue especificado ( incluye todos los registros de ventas )";
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DtoVentasBuscar criterios = new DtoVentasBuscar
                {
                    IdIni = Convert.ToInt32(controlBuscarVenta.NudBIdIni.Value),
                    IdFin = Convert.ToInt32(controlBuscarVenta.NudBIdFin.Value),
                    Cliente = controlBuscarVenta.TxtBCliente.Text.Trim(),

                    FVenta = controlBuscarVenta.DtpFVentaIni.Checked && controlBuscarVenta.DtpFVentaFin.Checked,
                    FVentaIni = controlBuscarVenta.DtpFVentaIni.Checked ? controlBuscarVenta.DtpFVentaIni.Value.Date : (DateTime?)null,
                    FVentaFin = controlBuscarVenta.DtpFVentaFin.Checked ? controlBuscarVenta.DtpFVentaFin.Value.Date.AddDays(1) : (DateTime?)null,
                    FVentaNull = controlBuscarVenta.ChkbFVentaNull.Checked,

                    FRequerido = controlBuscarVenta.DtpFRequeridoIni.Checked && controlBuscarVenta.DtpFRequeridoFin.Checked,
                    FRequeridoIni = controlBuscarVenta.DtpFRequeridoIni.Checked ? controlBuscarVenta.DtpFRequeridoIni.Value.Date : (DateTime?)null,
                    FRequeridoFin = controlBuscarVenta.DtpFRequeridoFin.Checked ? controlBuscarVenta.DtpFRequeridoFin.Value.Date.AddDays(1) : (DateTime?)null,
                    FRequeridoNull = controlBuscarVenta.ChkbFRequeridoNull.Checked,

                    FEnvio = controlBuscarVenta.DtpFEnvioIni.Checked && controlBuscarVenta.DtpFEnvioFin.Checked,
                    FEnvioIni = controlBuscarVenta.DtpFEnvioIni.Checked ? controlBuscarVenta.DtpFEnvioIni.Value.Date : (DateTime?)null,
                    FEnvioFin = controlBuscarVenta.DtpFEnvioFin.Checked ? controlBuscarVenta.DtpFEnvioFin.Value.Date.AddDays(1) : (DateTime?)null,
                    FEnvioNull = controlBuscarVenta.ChkbFEnvioNull.Checked,

                    Empleado = controlBuscarVenta.TxtBEmpleado.Text.Trim(),
                    CompañiaT = controlBuscarVenta.TxtBCompañiaT.Text.Trim(),
                    DirigidoA = controlBuscarVenta.TxtBDirigidoa.Text.Trim()
                };
                List<DtoVentaRpt> ventas = OrderService.ObtenerVentasRpt(true, criterios);
                MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {ventas.Count} registros");
                if (ventas.Count > 0)
                {
                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", ventas);
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    ReportParameter reportParameter = new ReportParameter("subtitulo", subtitulo);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter[] { reportParameter });
                    reportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(OrderDetailsSubReportProcessing);
                    reportViewer1.RefreshReport();
                }
                else
                {
                    reportViewer1.LocalReport.DataSources.Clear();
                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", new List<Order>());
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    ReportParameter reportParameter = new ReportParameter("subtitulo", subtitulo);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter[] { reportParameter });
                    reportViewer1.RefreshReport();
                    MessageBox.Show(Utils.noDatos, Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { U.MsgCatchOue(ex); }
        }

        private void OrderDetailsSubReportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int orderID = int.Parse(e.Parameters["OrderID"].Values[0].ToString());
            List<Order_Detail> ventaDetalles = Order_DetailService.ObtenerVentaDetallePorVentaId(orderID);
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", ventaDetalles);
            e.DataSources.Add(reportDataSource);
        }
    }
}
