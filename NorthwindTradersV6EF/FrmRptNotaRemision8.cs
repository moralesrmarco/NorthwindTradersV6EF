// requeri instalar los paquetes nugget PdfiumViewer de 64 bits, PdfSharp-gdi (esta version es para .net framework)
// tuve que modificar en propiedades del proyecto NorthwindTradersV6EF en Compilacion - Plataforma de destino: cambie la opción que decia Any CPU lo puse a x64 ya que la libreria PdfiumViewer que instale es para plataformas de 64 bits, debi de haber instalado la version de x86.
using BLL;
using Entities;
using Microsoft.Reporting.WinForms;
using NorthwindTradersV6EF.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptNotaRemision8 : Form
    {

        public int Id;
        private string cnStr = ConfigurationManager.ConnectionStrings["Northwind2ConnectionString"].ConnectionString;
        private VentaBLL _ventaBLL;
        private VentaDetalleBLL _ventaDetalleBLL;
        private MemoryStream _pdfStream;
        private PdfiumViewer.PdfDocument _pdfDoc;

        public FrmRptNotaRemision8()
        {
            InitializeComponent();
            _ventaBLL = new VentaBLL(cnStr);
            _ventaDetalleBLL = new VentaDetalleBLL(cnStr);
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptNotaRemision8_FormClosed(object sender, FormClosedEventArgs e)
        {
            _pdfDoc?.Dispose();
            _pdfStream?.Dispose();
            MDIPrincipal.ActualizarBarraDeEstado();
        }

        private void FrmRptNotaRemision8_Load(object sender, EventArgs e)
        {
            try
            {
                string strFecha = DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");

                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                
                // Crear LocalReport que es solo el motor de renderizado del RDLC.
                LocalReport localReport = new LocalReport();

                // Obtener el recurso embebido
                Assembly asm = Assembly.GetExecutingAssembly();
                using (Stream rdlcStream = asm.GetManifestResourceStream("NorthwindTradersV6EF.RptNotaRemision9.rdlc"))
                {
                    localReport.LoadReportDefinition(rdlcStream);
                }

                localReport.DataSources.Clear();
                DataTable dtDummy = new DataTable("DataSetDummy");
                dtDummy.Columns.Add("Dummy_", typeof(int));
                dtDummy.Rows.Add(1); // una fila ficticia
                ReportDataSource rdsDummy = new ReportDataSource("DataSetDummy", dtDummy);
                localReport.DataSources.Add(rdsDummy);

                // 1. Renderizar Cliente
                ReportParameter[] parametersCliente = new ReportParameter[3];
                parametersCliente[0] = new ReportParameter("PedidoId", Id.ToString());
                parametersCliente[1] = new ReportParameter("FechaHora", $"Fecha: {strFecha}");
                parametersCliente[2] = new ReportParameter("Para", "Para: Cliente.");
                localReport.SetParameters(parametersCliente);

                DataTable dtVenta = _ventaBLL.ObtenerVentaPorIdDt(Id);
                List<VentaDetalle> ventaDetalles = _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(Id);
                localReport.DataSources.Add(new ReportDataSource("DataSetVenta", dtVenta));
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ventaDetalles));

                byte[] pdfCliente = localReport.Render("PDF");
                
                // 2. Renderizar ControlInterno
                ReportParameter[] parametersControl = new ReportParameter[3];
                parametersControl[0] = new ReportParameter("PedidoId", Id.ToString());
                parametersControl[1] = new ReportParameter("FechaHora", $"Fecha: {strFecha}");
                parametersControl[2] = new ReportParameter("Para", "Para: Control Interno.");
                localReport.SetParameters(parametersControl);

                // Reusar mismos DataSources
                localReport.DataSources.Clear();
                localReport.DataSources.Add(rdsDummy);
                localReport.DataSources.Add(new ReportDataSource("DataSetVenta", dtVenta));
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ventaDetalles));

                byte[] pdfControl = localReport.Render("PDF");

                // 3. Combinar ambos PDFs
                var reporteServicePdfHelper = new ReporteServicePdfHelper();
                byte[] pdfFinal = reporteServicePdfHelper.CombinarPDFs(new[] { pdfCliente, pdfControl });
                // 4. Mostrar el PDF final en el visor del diseñador
                CargarPdf(pdfFinal);

                // 4. Mostrar el PDF final en visor embebido dentro del groupbox1
                byte[] pdfBytes = localReport.Render("PDF");
                var ms = new MemoryStream(pdfBytes);
                var doc = PdfiumViewer.PdfDocument.Load(ms);
                var pdfDocument = PdfiumViewer.PdfDocument.Load(ms);
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        public void CargarPdf(byte[] pdfBytes)
        {
            _pdfStream = new MemoryStream(pdfBytes);
            _pdfDoc = PdfiumViewer.PdfDocument.Load(_pdfStream);
            pdfViewer.Document = _pdfDoc;
            pdfViewer.ZoomMode = PdfiumViewer.PdfViewerZoomMode.FitWidth;
        }

    }
}
