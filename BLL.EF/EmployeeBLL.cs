using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BLL.EF
{
    public class EmployeeBLL
    {
        public EmployeeBLL()
        {
            // No se necesita conexión en esta clase, ya que se utiliza el contexto de Entity Framework
        }

        public static List<DtoEmpleadosPaisesCbo> ObtenerEmpleadosPaisesCbo()
        {
            try
            {
                using (var context = new NorthwindEntities())
                {
                    var paises = context.Employees
                                        .Select(e => e.Country)
                                        .Distinct()
                                        .OrderBy(p => p)
                                        .Select(p => new DtoEmpleadosPaisesCbo
                                        {
                                            Id = p,
                                            Pais = p
                                        })
                                        .ToList();
                    paises.Insert(0, new DtoEmpleadosPaisesCbo { Id = "", Pais = "»--- Seleccione ---«" });
                    return paises;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los países de empleados: " + ex.Message);
            }
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

        public static DataTable ObtenerEmpleadoReportaaCbo()
        {
            var dt = new DataTable();
            try
            {
                using (var context = new NorthwindEntities())
                {
                    var empleados = context.Employees
                                                   .Select(e => new
                                                   {
                                                       Id = e.EmployeeID,
                                                       Nombre = e.LastName + ", " + e.FirstName,
                                                       Orden = e.LastName + ", " + e.FirstName
                                                   })
                                                   .OrderBy(e => e.Orden)
                                                   .ToList();

                    // Construcción del DataTable
                    dt = new DataTable();
                    dt.Columns.Add("Id", typeof(int));
                    dt.Columns.Add("Nombre", typeof(string));
                    dt.Columns.Add("Orden", typeof(object));
                    // Insertar los registros obtenidos
                    foreach (var emp in empleados)
                    {
                        dt.Rows.Add(emp.Id, emp.Nombre, emp.Orden);
                    }
                    // Fila "Seleccione"
                    DataRow filaSeleccione = dt.NewRow();
                    filaSeleccione["Id"] = -1;
                    filaSeleccione["Nombre"] = "»--- Seleccione ---«";
                    filaSeleccione["Orden"] = 0; // primero
                    dt.Rows.Add(filaSeleccione);

                    // Fila "N/A"
                    DataRow filaNA = dt.NewRow();
                    filaNA["Id"] = 0;
                    filaNA["Nombre"] = "N/A";
                    filaNA["Orden"] = 1; // segundo
                    dt.Rows.Add(filaNA);

                    // Los demás con Orden = 2 si no tienen valor
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Orden"] == DBNull.Value || string.IsNullOrEmpty(row["Orden"].ToString()))
                            row["Orden"] = 2;
                    }

                    // Crear vista ordenada
                    DataView vista = dt.DefaultView;
                    vista.Sort = "Orden ASC, Nombre ASC";
                    return vista.ToTable();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener empleados para Reporta a: " + ex.Message);
            }
        }

        public static Employee ObtenerEmpleadoPorId(int id)
        {
            try
            {
                using (var context = new NorthwindEntities())
                {
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
