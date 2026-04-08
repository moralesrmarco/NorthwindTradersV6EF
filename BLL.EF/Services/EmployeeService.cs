using DAL.EF;
using System.Data;
using System.Linq;

namespace BLL.EF.Services
{
    public class EmployeeService
    {
        public static DataTable ObtenerEmpleadosCbo()
        {
            using (var context = new NorthwindContext())
            {
                var empleados = context.Employees
                    .Select(e => new
                    {
                        e.EmployeeID,
                        EmployeeName = e.LastName + ", " + e.FirstName
                    })
                    .OrderBy(e => e.EmployeeName)
                    .ToList();
                DataTable dt = new DataTable();
                dt.Columns.Add("EmployeeID", typeof(int));
                dt.Columns.Add("EmployeeName", typeof(string));
                dt.Rows.Add(0, "»--- Seleccione ---«");
                foreach (var empleado in empleados)
                {
                    dt.Rows.Add(empleado.EmployeeID, empleado.EmployeeName);
                }
                return dt;
            }
        }
    }
}
