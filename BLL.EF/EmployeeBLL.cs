using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;   // EF6
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace BLL.EF
{
    public class EmployeeBLL
    {
        public static int Insertar(Employee empleado)
        {
            using (var context = new NorthwindContext())
            {
                try
                {
                    // Agregamos la entidad al DbSet
                    context.Employees.Add(empleado);
                    return context.SaveChanges();
                }
                catch (Exception)
                {
                    return -3; // Error al insertar
                }
            }
        }

        public static int Actualizar(Employee empleado)
        {
            using (var context = new NorthwindContext())
            {
                // lo voy a manejar mejor en el catch, es mas seguro, porque si no existe el empleado, el Attach lo va a crear como nuevo y luego al marcarlo como modificado, va a intentar actualizar un registro que no existe, lo que generará una excepción que podemos capturar para devolver -1.
                //// Validar existencia previa
                //bool existe = context.Employees.Any(e => e.EmployeeID == empleado.EmployeeID);
                //if (!existe)
                //    return -1; // No existe el empleado

                // Adjuntar el objeto directamente
                context.Employees.Attach(empleado);
                context.Entry(empleado).State = EntityState.Modified;

                // Aquí controlas la foto
                if (empleado.EmployeeID <= 9)
                {
                    // No modificar la foto
                    context.Entry(empleado).Property(e => e.Photo).IsModified = false;
                }

                context.Entry(empleado).OriginalValues["RowVersion"] = empleado.RowVersion;

                try
                {
                    return context.SaveChanges(); // Éxito: devuelve número de filas afectadas
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var databaseEntity = entry.GetDatabaseValues();

                    if (databaseEntity == null)
                    {
                        return -1; // Eliminado previamente
                    }
                    else
                    {
                        return -2; // Modificado previamente (RowVersion distinto)
                    }
                }
                catch (Exception)
                {
                    return -3; // Otro error
                }
            }
        }

        public static int Eliminar(int id, byte[] rowVersion)
        {
            using (var context = new NorthwindContext())
            {
                var empleado = new Employee { EmployeeID = id };
                context.Employees.Attach(empleado);
                context.Entry(empleado).State = EntityState.Deleted;

                // Control de concurrencia con RowVersion
                context.Entry(empleado).OriginalValues["RowVersion"] = rowVersion;

                try
                {
                    return context.SaveChanges(); // >0 = éxito
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var databaseEntity = entry.GetDatabaseValues();

                    if (databaseEntity == null)
                    {
                        return -1; // Ya eliminado previamente
                    }
                    else
                    {
                        return -2; // Modificado previamente (RowVersion distinto)
                    }
                }
                catch (Exception)
                {
                    return -3; // Error inesperado
                }
            }
        }

        public static List<DtoEmpleadosPaisesCbo> ObtenerEmpleadosPaisesCbo()
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var paises = context.Employees
                                        .AsNoTracking() // Mejora el rendimiento al no rastrear las entidades
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
                using (var context = new NorthwindContext())
                {
                    // Obtiene todos los empleados directamente de la tabla
                    var empleados = context.Employees.AsNoTracking().ToList();
                    return empleados;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener todos los empleados: " + ex.Message);
            }
        }

        public static (List<DtoEmpleadosDgv> empleados, string mensajeEstado) ObtenerEmpleadosDgv(
            bool selectorRealizaBusqueda,
            DtoEmpleadosBuscar criterios)
        {
            try
            {
                using (var context = new NorthwindContext()) // tu DbContext
                {
                    IQueryable<Employee> query = context.Employees.Include(e => e.Employee1);

                    if (selectorRealizaBusqueda)
                    {
                        // Filtros dinámicos
                        if (criterios.IdIni > 0)
                            query = query.Where(e => e.EmployeeID >= criterios.IdIni && e.EmployeeID <= criterios.IdFin);

                        if (!string.IsNullOrEmpty(criterios.Nombres))
                            query = query.Where(e => e.FirstName.Contains(criterios.Nombres));

                        if (!string.IsNullOrEmpty(criterios.Apellidos))
                            query = query.Where(e => e.LastName.Contains(criterios.Apellidos));

                        if (!string.IsNullOrEmpty(criterios.Titulo))
                            query = query.Where(e => e.Title.Contains(criterios.Titulo));

                        if (!string.IsNullOrEmpty(criterios.Domicilio))
                            query = query.Where(e => e.Address.Contains(criterios.Domicilio));

                        if (!string.IsNullOrEmpty(criterios.Ciudad))
                            query = query.Where(e => e.City.Contains(criterios.Ciudad));

                        if (!string.IsNullOrEmpty(criterios.Region))
                            query = query.Where(e => e.Region.Contains(criterios.Region));

                        if (!string.IsNullOrEmpty(criterios.CodigoP))
                            query = query.Where(e => e.PostalCode.Contains(criterios.CodigoP));

                        if (!string.IsNullOrEmpty(criterios.Pais))
                            query = query.Where(e => e.Country.Contains(criterios.Pais));

                        if (!string.IsNullOrEmpty(criterios.Telefono))
                            query = query.Where(e => e.HomePhone.Contains(criterios.Telefono));
                    }
                    else
                    {
                        // Últimos 20 registros
                        query = query.OrderByDescending(e => e.EmployeeID).Take(20);
                    }

                    var empleados = query
                        .AsNoTracking() // Mejora el rendimiento al no rastrear las entidades
                                        //.OrderByDescending(e => e.EmployeeID)
                        .Select(e => new DtoEmpleadosDgv
                        {
                            EmployeeID = e.EmployeeID,
                            FirstName = e.FirstName,
                            LastName = e.LastName,
                            Title = e.Title,
                            BirthDate = e.BirthDate,
                            City = e.City,
                            Country = e.Country,
                            Photo = e.Photo,
                            ReportsToName = e.Employee1 != null
                                            ? e.Employee1.LastName + ", " + e.Employee1.FirstName
                                            : "N/A"
                        })
                        .ToList();

                    string mensaje = selectorRealizaBusqueda
                        ? $"Se encontraron {empleados.Count} empleado(s) con los criterios proporcionados."
                        : $"Se muestran los últimos {empleados.Count} empleado(s) registrados.";

                    return (empleados, mensaje);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener empleados. " + ex.Message);
            }
        }

        public static DataTable ObtenerEmpleadoReportaaCbo()
        {
            var dt = new DataTable();
            try
            {
                using (var context = new NorthwindContext())
                {
                    var empleados = context.Employees
                                                   .AsNoTracking()
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
                using (var context = new NorthwindContext())
                {
                    var empleado = context.Employees.AsNoTracking().FirstOrDefault(e => e.EmployeeID == id);
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
