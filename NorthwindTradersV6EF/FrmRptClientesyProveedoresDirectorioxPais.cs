using BLL.EF;
using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmRptClientesyProveedoresDirectorioxPais : Form
    {
        public FrmRptClientesyProveedoresDirectorioxPais()
        {
            InitializeComponent();
            reportViewer1.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmRptClientesyProveedoresDirectorioxPais_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmRptClientesyProveedoresDirectorioxPais_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
        }

        void LlenarComboBox()
        {
            try
            {
                var datos = CustomerBLL.ObtenerPaisVwCliProvCbo();
                comboBox.DataSource = datos;
                comboBox.DisplayMember = "Key";
                comboBox.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox.SelectedIndex <= 0 | (!checkBoxClientes.Checked & !checkBoxProveedores.Checked))
                {
                    groupBox1.Text = "» Reporte directorio de clientes y proveedores por país «";
                    U.NotificacionWarning(Utils.errorCriterioSelec);
                    return;
                }
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                string titulo = string.Empty;
                if (comboBox.SelectedValue.ToString() == "aaaaa" & checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    titulo = "» Reporte directorio de clientes y proveedores por país [ Todos los países ] «";
                else if (comboBox.SelectedValue.ToString() != "aaaaa" & checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    titulo = $"» Reporte directorio de clientes y proveedores por país [ País: {comboBox.SelectedValue.ToString()} ] «";
                else if (comboBox.SelectedValue.ToString() == "aaaaa" & checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                    titulo = "» Reporte directorio de clientes por país [ Todos los países ] «";
                else if (comboBox.SelectedValue.ToString() == "aaaaa" & !checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    titulo = "» Reporte directorio de proveedores por país [ Todos los países ] «";
                else if (comboBox.SelectedValue.ToString() != "aaaaa" & checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                    titulo = $"» Reporte directorio de clientes por país [ País: {comboBox.SelectedValue.ToString()} ] «";
                else if (comboBox.SelectedValue.ToString() != "aaaaa" & !checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    titulo = $"» Reporte directorio de proveedores por país [ País: {comboBox.SelectedValue.ToString()} ] «";
                groupBox1.Text = titulo;
                string nombreDeFormulario = "FrmRptClientesyProveedoresDirectorioxPais";
                var clientesProveedores = CustomerBLL.ObtenerClientesProveedores(nombreDeFormulario, comboBox.SelectedValue.ToString(), checkBoxClientes.Checked, checkBoxProveedores.Checked);
                // Conteos
                int totalClientes = clientesProveedores.Count(cp => cp.Relation == "Cliente");
                int totalProveedores = clientesProveedores.Count(cp => cp.Relation == "Proveedor");
                int total = totalClientes + totalProveedores;
                // Conteo de ciudades distintas
                int totalCiudades = clientesProveedores
                    .Select(cp => cp.City?.Trim()) // quita espacios
                    .Where(c => !string.IsNullOrEmpty(c)) // descarta vacíos
                    .Distinct(StringComparer.OrdinalIgnoreCase) // ignora mayúsculas/minúsculas
                    .Count();
                // Conteo de países distintos
                int totalPaises = clientesProveedores
                    .Select(cp => cp.Country?.Trim()) // quita espacios
                    .Where(p => !string.IsNullOrEmpty(p)) // descarta vacíos
                    .Distinct(StringComparer.OrdinalIgnoreCase) // ignora mayúsculas/minúsculas
                    .Count();
                string leyenda = string.Empty;
                if (totalClientes > 0)
                    leyenda = $"Se encontraron {totalClientes} cliente(s)";
                if (totalProveedores > 0)
                {
                    if (!string.IsNullOrEmpty(leyenda))
                        leyenda += $" y {totalProveedores} proveedor(es)";
                    else
                        leyenda = $"Se encontraron {totalProveedores} proveedor(es)";
                }
                if (totalClientes > 0 && totalProveedores > 0)
                    leyenda += $" (total: {total})";
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
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", clientesProveedores));
                ReportParameter rp = new ReportParameter("titulo", titulo);
                reportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
                reportViewer1.BackColor = Color.White;
                reportViewer1.RefreshReport();
                if (clientesProveedores.Count == 0)
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
