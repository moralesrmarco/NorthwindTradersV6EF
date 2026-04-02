using BLL.EF;
using DTOs.EF;
using System;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmClientesyProveedoresDirectorioxCiudad : Form
    {
        private bool EjecutarConfDgv = true;

        public FrmClientesyProveedoresDirectorioxCiudad()
        {
            InitializeComponent();
            Dgv.ColumnHeaderMouseClick += Dgv_ColumnHeaderMouseClick;
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint2(this, sender, e);

        private void FrmClientesyProveedoresDirectorioxCiudad_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        private void FrmClientesyProveedoresDirectorioxCiudad_Load(object sender, EventArgs e)
        {
            LlenarComboBox();
            Utils.ConfDgv(Dgv);
        }

        private void LlenarComboBox()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                var datos = CustomerBLL.ObtenerCiudadPaisVwCliProvCbo();
                comboBox.DataSource = datos;
                comboBox.DisplayMember = "Key";
                comboBox.ValueMember = "Value";
                MDIPrincipal.ActualizarBarraDeEstado();
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void LlenarDgv()
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                string titulo = string.Empty;
                string nombreDeFormulario = "FrmClientesyProveedoresDirectorioxCiudad";
                if (comboBox.SelectedValue.ToString() == "aaaaa" & checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    titulo = "» Directorio de clientes y proveedores por ciudad [ Todas las ciudades ] «";
                else if (comboBox.SelectedValue.ToString() != "aaaaa" & checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    titulo = $"» Directorio de clientes y proveedores por ciudad [ Ciudad: {comboBox.SelectedValue.ToString()} ] «";
                else if (comboBox.SelectedValue.ToString() == "aaaaa" & checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                    titulo = "» Directorio de clientes por ciudad [ Todas las ciudades ] «";
                else if (comboBox.SelectedValue.ToString() == "aaaaa" & !checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    titulo = "» Directorio de proveedores por ciudad [ Todas las ciudades ] «";
                else if (comboBox.SelectedValue.ToString() != "aaaaa" & checkBoxClientes.Checked & !checkBoxProveedores.Checked)
                    titulo = $"» Directorio de clientes por ciudad [ Ciudad: {comboBox.SelectedValue.ToString()} ] «";
                else if (comboBox.SelectedValue.ToString() != "aaaaa" & !checkBoxClientes.Checked & checkBoxProveedores.Checked)
                    titulo = $"» Directorio de proveedores por ciudad [ Ciudad: {comboBox.SelectedValue.ToString()} ] «";
                Grb.Text = titulo;

                var clientesProveedores = CustomerBLL.ObtenerClientesProveedores(
                    nombreDeFormulario,
                    comboBox.SelectedValue.ToString(),
                    checkBoxClientes.Checked,
                    checkBoxProveedores.Checked
                );

                Dgv.DataSource = clientesProveedores;
                if (EjecutarConfDgv)
                {
                    ConfDgv();
                    EjecutarConfDgv = false;
                }
                if (clientesProveedores.Count == 0)
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.nser, true);
                    U.NotificacionWarning(Utils.nser);
                    return;
                }

                //Conteos
                int totalClientes = clientesProveedores.Count(cp => cp.Relation == "Cliente");
                int totalProveedores = clientesProveedores.Count(cp => cp.Relation == "Proveedor");
                int total = totalClientes + totalProveedores;
                //Conteo de ciudades distintas
                int totalCiudades = clientesProveedores
                    .Select(cp => cp.City?.Trim()) // quita espacios
                    .Where(c => !string.IsNullOrEmpty(c)) // descarta vacíos
                    .Distinct(StringComparer.OrdinalIgnoreCase) // ignora mayúsculas/minúsculas
                    .Count();
                //Conteo de países distintos
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
                if (string.IsNullOrEmpty(leyenda))
                    leyenda = "No se encontraron registros";
                MDIPrincipal.ActualizarBarraDeEstado(leyenda);
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void Dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // debe estar vinculado a la clase List<> a la cual esta vinculado el DataGridView.DataSource
            Utils.OrdenarPorColumna<DtoClienteProveedor>(Dgv, e);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex <= 0 | (!checkBoxClientes.Checked & !checkBoxProveedores.Checked))
            {
                Grb.Text = "» Directorio de clientes y proveedores por ciudad «";
                Dgv.DataSource = null;
                U.NotificacionWarning(Utils.errorCriterioSelec);
                return;
            }
            LlenarDgv();
        }

        private void ConfDgv()
        {
            Dgv.Columns["City"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Country"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Relation"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Region"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["PostalCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["Fax"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["Country"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Relation"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Phone"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Region"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["PostalCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv.Columns["Fax"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Dgv.Columns["CompanyName"].HeaderText = "Nombre de compañía";
            Dgv.Columns["Contact"].HeaderText = "Nombre de contacto";
            Dgv.Columns["Relation"].HeaderText = "Relación";
            Dgv.Columns["Address"].HeaderText = "Domicilio";
            Dgv.Columns["City"].HeaderText = "Ciudad";
            Dgv.Columns["Region"].HeaderText = "Región";
            Dgv.Columns["PostalCode"].HeaderText = "Código postal";
            Dgv.Columns["Country"].HeaderText = "País";
            Dgv.Columns["Phone"].HeaderText = "Teléfono";

            Dgv.Columns["City"].DisplayIndex = 0;
            Dgv.Columns["Country"].DisplayIndex = 1;
            Dgv.Columns["CompanyName"].DisplayIndex = 2;
            Dgv.Columns["Contact"].DisplayIndex = 3;
            Dgv.Columns["Relation"].DisplayIndex = 4;
            Dgv.Columns["Address"].DisplayIndex = 5;
            Dgv.Columns["Region"].DisplayIndex = 6;
            Dgv.Columns["PostalCode"].DisplayIndex = 7;
            Dgv.Columns["Phone"].DisplayIndex = 8;
            Dgv.Columns["Fax"].DisplayIndex = 9;
        }
    }
}
