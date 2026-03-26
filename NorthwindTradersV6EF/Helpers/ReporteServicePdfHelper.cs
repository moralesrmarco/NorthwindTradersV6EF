using System.IO;
using Microsoft.Reporting.WinForms;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace NorthwindTradersV6EF.Helpers
{
    public class ReporteServicePdfHelper
    {
        public byte[] GenerarReporteFinal(LocalReport localReport)
        {
            // 1. Renderizar reporte Cliente
            var parametrosCliente = new ReportParameter("TipoTanto", "Cliente");
            localReport.SetParameters(new[] { parametrosCliente });
            byte[] pdfCliente = localReport.Render("PDF");

            // 2. Renderizar reporte ControlInterno
            var parametrosControl = new ReportParameter("TipoTanto", "ControlInterno");
            localReport.SetParameters(new[] { parametrosControl });
            byte[] pdfControl = localReport.Render("PDF");

            // 3. Combinar ambos PDFs en memoria
            return CombinarPDFs(new[] { pdfCliente, pdfControl });
        }

        public byte[] CombinarPDFs(byte[][] pdfs)
        {
            using (var output = new MemoryStream())
            {
                PdfDocument pdfFinal = new PdfDocument();

                foreach (var pdf in pdfs)
                {
                    using (var ms = new MemoryStream(pdf))
                    {
                        PdfDocument doc = PdfReader.Open(ms, PdfDocumentOpenMode.Import);
                        foreach (PdfPage page in doc.Pages)
                            pdfFinal.AddPage(page);
                    }
                }

                pdfFinal.Save(output, false);
                return output.ToArray();
            }
        }
    }
}
