using DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.EF
{
    public class EmployeeBLL
    {
            public EmployeeBLL()
            {
                // No se necesita conexión en esta clase, ya que se utiliza el contexto de Entity Framework
            }

        public static List<Employee> ObtenerTodosLosEmpleados()
        {
            try
            {
                using (var context = new NorthwindEntities())
                {
                    // Obtiene todos los empleados directamente de la tabla
                    var empleados = context.Employees.ToList();
                    return empleados;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener todos los empleados: " + ex.Message);
            }
        }

        public static Employee ObtenerEmpleadoPorId(int id)
        {
            try
            {
                using (var context = new NorthwindEntities())
                {
                    // Obtiene el empleado por su ID directamente de la tabla
                    var empleado = context.Employees.FirstOrDefault(e => e.EmployeeID == id);
                    return empleado;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el empleado por ID: " + ex.Message);
            }
        }
    }
}
