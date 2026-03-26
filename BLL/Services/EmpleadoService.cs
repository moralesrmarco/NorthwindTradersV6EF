using DAL.Helpers;
using System.Data;

namespace BLL.Services
{
    public class EmpleadoService
    {
        private readonly ComboDataHelper _dal;

        public EmpleadoService(string connectionString)
        {
            _dal = new ComboDataHelper(connectionString);
        }

        public DataTable ObtenerEmpleadosCbo()
        {
            var empleados = _dal.LlenarCbo("SpEmpleadoObtenerCbo");
            DataRow filaSeleccione = empleados.NewRow();
            filaSeleccione["EmployeeID"] = 0;
            filaSeleccione["EmployeeName"] = "»--- Seleccione ---«";
            empleados.Rows.InsertAt(filaSeleccione, 0);
            return empleados;
        }
    }
}
