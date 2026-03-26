using BLL;
using System;
using System.Windows.Forms;

namespace NorthwindTradersV6EF
{
    public partial class FrmEjemploUsoJerarquiaClaseEmpleado : Form
    {

        private readonly EmpleadoBLL _empleadoBLL;

        public FrmEjemploUsoJerarquiaClaseEmpleado()
        {
            InitializeComponent();

            // Inicializa la capa de negocio con tu cadena de conexión
            string _connectionString = "Data Source=.;Initial Catalog=Northwind2;Integrated Security=True;";
            _empleadoBLL = new EmpleadoBLL(_connectionString);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIdEmpleado.Text, out int id))
            {
                // Llamada a la capa de negocio
                var empleado = _empleadoBLL.ObtenerEmpleadoConJerarquia(id);

                if (empleado != null)
                {
                    // Mostrar datos básicos
                    txtNombre.Text = empleado.NameByFirstName;

                    // Mostrar jefe
                    txtJefe.Text = empleado.Jefe?.NameByLastName ?? "Sin jefe";

                    // Mostrar subordinados
                    lstSubordinados.DataSource = empleado.EmpleadosSubordinados;
                    lstSubordinados.DisplayMember = "NameByLastName";
                }
                else
                {
                    MessageBox.Show("Empleado no encontrado.");
                }
            }
            else
            {
                MessageBox.Show("Ingrese un ID válido.");
            }
        }

    }
}
