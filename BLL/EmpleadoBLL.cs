using DAL;
using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BLL
{
    public class EmpleadoBLL
    {

        private readonly EmpleadoDAL _empleadoDAL;

        public EmpleadoBLL(string _connectionString)
        {
            _empleadoDAL = new EmpleadoDAL(_connectionString);
        }

        public int Insertar(Empleado empleado)
        {
            return _empleadoDAL.Insertar(empleado);
        }

        public int Actualizar(Empleado empleado)
        {
            return _empleadoDAL.Actualizar(empleado);
        }

        public int Eliminar(int empleadoId, byte[] rowVersion)
        {
            return _empleadoDAL.Eliminar(empleadoId, rowVersion);
        }

        public DataTable ObtenerEmpleadoReportaaCbo()
        {
            var empleados = _empleadoDAL.ObtenerEmpleadoReportaaCbo();
            DataRow filaSeleccione = empleados.NewRow();
            filaSeleccione["Id"] = -1;
            filaSeleccione["Nombre"] = "»--- Seleccione ---«";
            filaSeleccione["Orden"] = 0; // necesario para ordenar primero
            empleados.Rows.Add(filaSeleccione);
            DataRow filaNA = empleados.NewRow();
            filaNA["Id"] = 0;
            filaNA["Nombre"] = "N/A";
            filaNA["Orden"] = 1; // necesario para ordenar segundo
            empleados.Rows.Add(filaNA);
            // Para los demás, asigna Orden = 2
            foreach (DataRow row in empleados.Rows)
            {
                if (row["Orden"] == DBNull.Value)
                    row["Orden"] = 2;
            }
            // Crear vista ordenada
            DataView vista = empleados.DefaultView;
            vista.Sort = "Orden ASC, Nombre ASC";
            return vista.ToTable();
        }


        public (List<DtoEmpleadosDgv> empleados, string mensajeEstado) ObtenerEmpleadosDgv(bool selectorRealizaBusqueda, DtoEmpleadosBuscar criterios)
        {
            criterios.IdIni = string.IsNullOrEmpty(criterios.IdIniTxt) ? 0 : Convert.ToInt32(criterios.IdIniTxt);
            criterios.IdFin = string.IsNullOrEmpty(criterios.IdFinTxt) ? 0 : Convert.ToInt32(criterios.IdFinTxt);

            var empleadosRaw = _empleadoDAL.ObtenerEmpleadosDgvRaw(selectorRealizaBusqueda, criterios);

            var empleados = empleadosRaw.Select(e => new DtoEmpleadosDgv
            {
                EmployeeID = Convert.ToInt32(e.EmployeeID),
                FirstName = e.FirstName is DBNull ? null : e.FirstName.ToString(),
                LastName = e.LastName is DBNull ? null : e.LastName.ToString(),
                Title = e.Title is DBNull ? null : e.Title.ToString(),
                BirthDate = e.BirthDate is DBNull ? null : (DateTime?)Convert.ToDateTime(e.BirthDate),
                City = e.City is DBNull ? null : e.City.ToString(),
                Country = e.Country is DBNull ? null : e.Country.ToString(),
                Photo = e.Photo is DBNull ? null : (byte[])e.Photo,
                ReportsToName = string.IsNullOrEmpty(Convert.ToString(e.ReportsToName))
                                    ? "N/A"
                                    : Convert.ToString(e.ReportsToName)
            }).ToList();
            string mensaje = selectorRealizaBusqueda
                    ? $"Se encontraron {empleados.Count} empleado(s)."
                    : $"Se muestran los últimos {empleados.Count} empleado(s) registrados.";
            return (empleados, mensaje);
        }

        public List<DtoEmpleadosPaisesCbo> ObtenerEmpleadosPaisesCbo()
        {
            var paises = _empleadoDAL.ObtenerEmpleadosPaisesCbo();
            paises.Insert(0, new DtoEmpleadosPaisesCbo { Id = "", Pais = "»--- Seleccione ---«" });
            return paises;
        }

        /// <summary>
        /// Obtiene un empleado por su ID.
        /// </summary>
        public Empleado ObtenerEmpleadoPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del empleado debe ser mayor a cero.", nameof(id));
            var empleado = _empleadoDAL.ObtenerEmpleadoPorId(id);
            if (empleado != null)
            {
                if (empleado.ReportsTo.HasValue)
                    empleado.Jefe = _empleadoDAL.ObtenerEmpleadoPorId(empleado.ReportsTo.Value);
            }
            else 
            {
                // Si no tiene jefe, asignamos un objeto Empleado con LastName = "N/A"
                empleado.Jefe = new Empleado
                {
                    EmployeeID = 0,
                    LastName = "N/A",
                    FirstName = string.Empty
                };
            }
            return empleado;
        }

        /// <summary>
        /// Obtiene todos los empleados y carga la referencia al jefe de cada uno.
        /// </summary>
        public List<Empleado> ObtenerTodosLosEmpleados()
        {
            var empleados = _empleadoDAL.ObtenerTodosLosEmpleados();

            foreach (var empleado in empleados)
            {
                if (empleado.ReportsTo.HasValue)
                {
                    // Traer al jefe desde DAL
                    empleado.Jefe = _empleadoDAL.ObtenerEmpleadoPorId(empleado.ReportsTo.Value);
                }
            }
            return empleados;
        }

        /// <summary>
        /// Obtiene un empleado con su jefe y subordinados.
        /// </summary>
        public Empleado ObtenerEmpleadoConJerarquia(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del empleado debe ser mayor a cero.", nameof(id));

            return _empleadoDAL.ObtenerEmpleadoConJerarquia(id);
        }

        /// <summary>
        /// Obtiene la lista de empleados que reportan a un jefe específico.
        /// </summary>
        public List<Empleado> ObtenerEmpleadoConSubordinados(int managerId)
        {
            if (managerId <= 0)
                throw new ArgumentException("El ID del jefe debe ser mayor a cero.", nameof(managerId));

            return _empleadoDAL.ObtenerEmpleadoConSubordinados(managerId);
        }

        /// <summary>
        /// Ejemplo de regla de negocio: validar si un empleado tiene jefe asignado.
        /// </summary>
        public bool TieneJefe(int id)
        {
            var empleado = _empleadoDAL.ObtenerEmpleadoPorId(id);
            return empleado?.ReportsTo != null;
        }

        /// <summary>
        /// Ejemplo de regla de negocio: obtener el nombre completo del jefe de un empleado.
        /// </summary>
        public string ObtenerNombreDelJefe(int id)
        {
            var empleado = _empleadoDAL.ObtenerEmpleadoPorId(id);
            if (empleado?.ReportsTo != null)
            {
                var jefe = _empleadoDAL.ObtenerEmpleadoPorId(empleado.ReportsTo.Value);
                return jefe?.NameByLastName ?? "Sin jefe";
            }
            return "Sin jefe";
        }


    }
}
