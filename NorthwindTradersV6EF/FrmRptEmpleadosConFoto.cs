using BLL.EF;
using DAL.EF;
using Microsoft.Reporting.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptEmpleadosConFoto : Form
    {
        public FrmRptEmpleadosConFoto()
        {
            InitializeComponent();
            reportViewer1.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmRptEmpleadosConFoto_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptEmpleadosConFoto_Load(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var empleados = EmployeeBLL.ObtenerTodosLosEmpleados();
                Employee.NormalizarFotos(empleados);
                MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {empleados.Count} registro(s)");
                var rds = new ReportDataSource("DataSet1", empleados);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.BackColor = Color.White;
                reportViewer1.RefreshReport();
                if (empleados.Count == 0)
                    U.NotificacionWarning(Utils.noDatos);
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }
    }
}
