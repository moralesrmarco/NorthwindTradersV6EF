using BLL.EF;
using Microsoft.Reporting.WinForms;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptClientes : Form
    {

        public FrmRptClientes()
        {
            InitializeComponent();
            reportViewer1.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void GrbPain(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmRptClientes_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptClientes_Load(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var resultado = CustomerBLL.ObtenerClientes(false, null, true);
                // Conteos
                int totalClientes = resultado.clientes.Count();
                // Conteo de ciudades distintas
                int totalCiudades = resultado.clientes
                    .Select(cp => cp.City?.Trim()) // quita espacios
                    .Where(c => !string.IsNullOrEmpty(c)) // descarta vacíos
                    .Distinct(StringComparer.OrdinalIgnoreCase) // ignora mayúsculas/minúsculas
                    .Count();
                // Conteo de países distintos
                int totalPaises = resultado.clientes
                    .Select(cp => cp.Country?.Trim()) // quita espacios
                    .Where(p => !string.IsNullOrEmpty(p)) // descarta vacíos
                    .Distinct(StringComparer.OrdinalIgnoreCase) // ignora mayúsculas/minúsculas
                    .Count();
                string leyenda = string.Empty;
                if (totalClientes > 0)
                    leyenda = $"Se encontraron {totalClientes} cliente(s)";
                if (totalCiudades > 0)
                {
                    if (!string.IsNullOrEmpty(leyenda))
                        leyenda += $", en {totalCiudades} ciudad(es)";
                }
                if (totalPaises > 0)
                {
                    if (!string.IsNullOrEmpty(leyenda))
                        leyenda += $", en {totalPaises} país(es)";
                }
                MDIPrincipal.ActualizarBarraDeEstado(leyenda);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", resultado.clientes));
                reportViewer1.BackColor = Color.White;
                reportViewer1.RefreshReport();
                if (resultado.clientes.Count == 0)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.noDatos, true);
                    U.NotificacionWarning(Utils.noDatos);
                }
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }
    }
}
