using BLL.EF;
using DAL.EF;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptEmpleado : Form
    {

        public int Id { get; set; }

        public FrmRptEmpleado()
        {
            InitializeComponent();
            reportViewer1.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmRptEmpleado_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptEmpleado_Load(object sender, EventArgs e)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var empleado = EmployeeBLL.ObtenerEmpleadoPorId(Id);
                Employee.NormalizarFotos(new List<Employee> { empleado });
                if (empleado != null) 
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontró el registro con el Id: {empleado.EmployeeID}");
                // Crear una lista con un solo empleado
                List<Employee> empleados = new List<Employee> { empleado };
                // Asignar la lista como fuente de datos del informe
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", empleados));
                reportViewer1.BackColor = Color.White;
                reportViewer1.RefreshReport();
                if (empleado == null)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.noDatos);
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
