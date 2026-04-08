using DAL.EF;
using System.Collections.Generic;
using System.Linq;

namespace BLL.EF.Services
{
    public class EmployeeService
    {
        public static List<dynamic> ObtenerEmpleadosCbo()
        {
            using (var context = new NorthwindContext())
            {
                var empleados = context.Employees
                    .Select(e => new
                    {
                        e.EmployeeID,
                        EmployeeName = e.LastName + ", " + e.FirstName
                    })
                    .ToList<dynamic>();

                // Insertar la fila "Seleccione" al inicio
                empleados.Insert(0, new
                {
                    EmployeeID = 0,
                    EmployeeName = "»--- Seleccione ---«"
                });

                return empleados;
            }
        }
    }
}
