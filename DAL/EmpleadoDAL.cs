using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utilities;

namespace DAL
{

    //La tabla Employees de Northwind tiene una relación jerárquica consigo misma: cada empleado puede tener un jefe(ReportsTo) que también es un empleado
    // la relación FK_Employees_Employees se modela directamente en su clase de entidad como propiedades que reflejen la jerarquía

    public class EmpleadoDAL
    {

        private readonly string _connectionString;

        public EmpleadoDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Insertar(Empleado empleado)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", 0);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Nombres", empleado.FirstName);
                    cmd.Parameters.AddWithValue("@Apellidos", empleado.LastName);
                    cmd.Parameters.AddWithValue("@Titulo", empleado.Title);
                    cmd.Parameters.AddWithValue("@TitCortesia", empleado.TitleOfCourtesy);
                    cmd.Parameters.AddWithValue("@FNacimiento", empleado.BirthDate);
                    cmd.Parameters.AddWithValue("@FContratacion", empleado.HireDate);
                    cmd.Parameters.AddWithValue("@Domicilio", empleado.Address);
                    cmd.Parameters.AddWithValue("@Ciudad", empleado.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(empleado.Region) ? (object)DBNull.Value : (object)empleado.Region);
                    cmd.Parameters.AddWithValue("@CodigoP", string.IsNullOrWhiteSpace(empleado.PostalCode) ? (object)DBNull.Value : (object)empleado.PostalCode);
                    cmd.Parameters.AddWithValue("@Pais",empleado.Country);
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrWhiteSpace(empleado.HomePhone) ? (object)DBNull.Value : (object)empleado.HomePhone);
                    cmd.Parameters.AddWithValue("@Extension", string.IsNullOrWhiteSpace(empleado.Extension) ? (object)DBNull.Value : (object)empleado.Extension);
                    cmd.Parameters.AddWithValue("@Notas", string.IsNullOrWhiteSpace(empleado.Notes) ? (object)DBNull.Value : (object)empleado.Notes);
                    var reportaA = cmd.Parameters.Add("@Reportaa", SqlDbType.Int);
                    reportaA.Value = empleado.ReportsTo.HasValue && empleado.ReportsTo.Value != 0
                        ? (object)empleado.ReportsTo.Value
                        : DBNull.Value;
                    var byteFoto = cmd.Parameters.Add("@Foto", SqlDbType.VarBinary, empleado.Photo?.Length ?? -1);
                    byteFoto.Value = (object)empleado.Photo ?? DBNull.Value;
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                    empleado.EmployeeID = (int)cmd.Parameters["@Id"].Value;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }

        public int Actualizar(Empleado empleado)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", empleado.EmployeeID);
                    cmd.Parameters.AddWithValue("@Nombres", empleado.FirstName);
                    cmd.Parameters.AddWithValue("@Apellidos", empleado.LastName);
                    cmd.Parameters.AddWithValue("@Titulo", empleado.Title);
                    cmd.Parameters.AddWithValue("@TitCortesia", empleado.TitleOfCourtesy);
                    cmd.Parameters.AddWithValue("@FNacimiento", empleado.BirthDate);
                    cmd.Parameters.AddWithValue("@FContratacion", empleado.HireDate);
                    cmd.Parameters.AddWithValue("@Domicilio", empleado.Address);
                    cmd.Parameters.AddWithValue("@Ciudad", empleado.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(empleado.Region) ? (object)DBNull.Value : (object)empleado.Region);
                    cmd.Parameters.AddWithValue("@CodigoP", string.IsNullOrWhiteSpace(empleado.PostalCode) ? (object)DBNull.Value : (object)empleado.PostalCode);
                    cmd.Parameters.AddWithValue("@Pais", empleado.Country);
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrWhiteSpace(empleado.HomePhone) ? (object)DBNull.Value : (object)empleado.HomePhone);
                    cmd.Parameters.AddWithValue("@Extension", string.IsNullOrWhiteSpace(empleado.Extension) ? (object)DBNull.Value : (object)empleado.Extension);
                    cmd.Parameters.AddWithValue("@Notas", string.IsNullOrWhiteSpace(empleado.Notes) ? (object)DBNull.Value : (object)empleado.Notes);
                    var reportaA = cmd.Parameters.Add("@Reportaa", SqlDbType.Int);
                    reportaA.Value = empleado.ReportsTo.HasValue && empleado.ReportsTo.Value != 0
                        ? (object)empleado.ReportsTo.Value
                        : DBNull.Value;
                    var byteFoto = cmd.Parameters.Add("@Foto", SqlDbType.Image);
                    byteFoto.Value = (object)empleado.Photo ?? DBNull.Value;
                    var rowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    rowVersion.Value = empleado.RowVersion ?? (object)DBNull.Value;
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }

        public int Eliminar(int empleadoId, byte[] rowVersion)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", empleadoId);
                    var pRrowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    pRrowVersion.Value = rowVersion ?? (object)DBNull.Value;
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }

        public DataTable ObtenerEmpleadoReportaaCbo()
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoObtenerReportaaCbo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dt;
        }

        public List<DtoEmpleadosDgvRaw> ObtenerEmpleadosDgvRaw(bool selectorRealizaBusqueda, DtoEmpleadosBuscar criterios)
        {
            List<DtoEmpleadosDgvRaw> employees = new List<DtoEmpleadosDgvRaw>();
            string query;
            if (!selectorRealizaBusqueda)
            {
                query = @"
                            SELECT TOP 20
                                e.EmployeeID,
                                e.FirstName,
                                e.LastName,
                                e.Title,
                                e.BirthDate,
                                e.City,
                                e.Country,
                                e.Photo,
                                e2.LastName + ', ' + e2.FirstName AS ReportsToName
                            FROM Employees AS e
                            LEFT JOIN Employees AS e2
                                ON e.ReportsTo = e2.EmployeeID
                            ORDER BY e.EmployeeID DESC;
                            ";
            }
            else
            {
                query = @"
                            SELECT
                                e.EmployeeID,
                                e.FirstName,
                                e.LastName,
                                e.Title,
                                e.BirthDate,
                                e.City,
                                e.Country,
                                e.Photo,
                                e2.LastName + ', ' + e2.FirstName AS ReportsToName
                            FROM Employees AS e
                            LEFT JOIN Employees AS e2
                                ON e.ReportsTo = e2.EmployeeID
                            WHERE
                                (@IdIni = 0 OR e.EmployeeID BETWEEN @IdIni AND @IdFin)
                                AND (@Nombres = '' OR e.FirstName LIKE '%' + @Nombres + '%')
                                AND (@Apellidos = '' OR e.LastName LIKE '%' + @Apellidos + '%')
                                AND (@Titulo = '' OR e.Title LIKE '%' + @Titulo + '%')
                                AND (@Domicilio = '' OR e.Address LIKE '%' + @Domicilio + '%')
                                AND (@Ciudad = '' OR e.City LIKE '%' + @Ciudad + '%')
                                AND (@Region = '' OR e.Region LIKE '%' + @Region + '%')
                                AND (@CodigoP = '' OR e.PostalCode LIKE '%' + @CodigoP + '%')
                                AND (@Pais = '' OR e.Country LIKE '%' + @Pais + '%')
                                AND (@Telefono = '' OR e.HomePhone LIKE '%' + @Telefono + '%')
                            ORDER BY e.EmployeeID DESC;
                            ";
            }
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(query, con))
                {
                    if (selectorRealizaBusqueda)
                    {
                        cmd.Parameters.AddWithValue("@IdIni", criterios.IdIni);
                        cmd.Parameters.AddWithValue("@IdFin", criterios.IdFin);
                        cmd.Parameters.AddWithValue("@Nombres", criterios.Nombres);
                        cmd.Parameters.AddWithValue("@Apellidos", criterios.Apellidos);
                        cmd.Parameters.AddWithValue("@Titulo", criterios.Titulo);
                        cmd.Parameters.AddWithValue("@Domicilio", criterios.Domicilio);
                        cmd.Parameters.AddWithValue("@Ciudad", criterios.Ciudad);
                        cmd.Parameters.AddWithValue("@Region", criterios.Region);
                        cmd.Parameters.AddWithValue("@CodigoP", criterios.CodigoP);
                        cmd.Parameters.AddWithValue("@Pais", criterios.Pais);
                        cmd.Parameters.AddWithValue("@Telefono", criterios.Telefono);
                    }
                    con.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            DtoEmpleadosDgvRaw employee = new DtoEmpleadosDgvRaw()
                            {
                                EmployeeID = rdr["EmployeeID"],
                                LastName = rdr["LastName"],
                                FirstName = rdr["FirstName"],
                                Title = rdr["Title"],
                                BirthDate = rdr["BirthDate"],
                                City = rdr["City"],
                                Country = rdr["Country"],
                                Photo = rdr["Photo"],
                                ReportsToName = rdr["ReportsToName"]
                            };
                            employees.Add(employee);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return employees;
        }

        public List<DtoEmpleadosPaisesCbo> ObtenerEmpleadosPaisesCbo()
        {
            List<DtoEmpleadosPaisesCbo> paises = new List<DtoEmpleadosPaisesCbo>();
            string query = "SELECT DISTINCT Country As Id, Country As Pais FROM Employees ORDER BY Pais;";
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                DtoEmpleadosPaisesCbo pais = new DtoEmpleadosPaisesCbo()
                                {
                                    Id = rdr["Id"]?.ToString(),
                                    Pais = rdr["Pais"]?.ToString()
                                };
                                paises.Add(pais);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return paises;
        }


        public Empleado ObtenerEmpleadoPorId(int employeeID)
        {
            Empleado empleado = null;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoObtenerPorId", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", employeeID);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            empleado = MapearEmpleado(rdr);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return empleado;
        }

        public List<Empleado> ObtenerTodosLosEmpleados()
        {
            var empleados = new List<Empleado>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoObtenerTodos", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empleados.Add(MapearEmpleado(reader));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return empleados;
        }

        // Obtiene un empleado junto con su jefe y sus subordinados
        // este metodo no lo estoy usando pero puede ser util para futuras funcionalidades
        // muestra la estructura jerarquica de la implementación de los metodos de un empleado
        public Empleado ObtenerEmpleadoConJerarquia(int id)
        {
            // no se necesita el try catch aqui porque ya lo maneja los metodos que acceden a la base de datos

            // 1. Obtener empleado base
            var empleado = ObtenerEmpleadoPorId(id);

            if (empleado == null)
                return null;

            // 2. Cargar jefe si existe
            if (empleado.ReportsTo.HasValue)
                empleado.Jefe = ObtenerEmpleadoPorId(empleado.ReportsTo.Value);

            // 3. Cargar subordinados
            empleado.EmpleadosSubordinados = ObtenerEmpleadoConSubordinados(id);

            return empleado;
        }

        // Obtiene la lista de empleados que reportan al manager con managerId
        // este metodo creo que no lo estoy usando pero puede ser util para futuras funcionalidades
        // muestra la estructura jerarquica de la implementación de los metodos de un empleado
        public List<Empleado> ObtenerEmpleadoConSubordinados(int managerId)
        {
            var empleados = new List<Empleado>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoObtenerConSubordinados", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", managerId);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empleados.Add(MapearEmpleado(reader));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return empleados;
        }

        private Empleado MapearEmpleado(SqlDataReader rdr)
        {
            var empleado = new Empleado();

            int ordEmployeeID = rdr.GetOrdinal("EmployeeID");
            int ordRowVersion = rdr.GetOrdinal("RowVersion");
            int ordFirstName = rdr.GetOrdinal("FirstName");
            int ordLastName = rdr.GetOrdinal("LastName");
            int ordTitle = rdr.GetOrdinal("Title");
            int ordTitleCourtesy = rdr.GetOrdinal("TitleOfCourtesy");
            int ordBirthDate = rdr.GetOrdinal("BirthDate");
            int ordHireDate = rdr.GetOrdinal("HireDate");
            int ordAddress = rdr.GetOrdinal("Address");
            int ordCity = rdr.GetOrdinal("City");
            int ordRegion = rdr.GetOrdinal("Region");
            int ordPostalCode = rdr.GetOrdinal("PostalCode");
            int ordCountry = rdr.GetOrdinal("Country");
            int ordHomePhone = rdr.GetOrdinal("HomePhone");
            int ordExtension = rdr.GetOrdinal("Extension");
            int ordNotes = rdr.GetOrdinal("Notes");
            int ordReportsTo = rdr.GetOrdinal("ReportsTo");
            int ordPhoto = rdr.GetOrdinal("Photo");
            int ordPhotoPath = rdr.GetOrdinal("PhotoPath");

            empleado.EmployeeID = rdr.GetInt32(ordEmployeeID);
            empleado.RowVersion = rdr.IsDBNull(ordRowVersion) ? null : (byte[])rdr[ordRowVersion];

            empleado.FirstName = rdr.IsDBNull(ordFirstName) ? null : rdr.GetString(ordFirstName);
            empleado.LastName = rdr.IsDBNull(ordLastName) ? null : rdr.GetString(ordLastName);
            empleado.Title = rdr.IsDBNull(ordTitle) ? null : rdr.GetString(ordTitle);
            empleado.TitleOfCourtesy = rdr.IsDBNull(ordTitleCourtesy) ? null : rdr.GetString(ordTitleCourtesy);

            empleado.BirthDate = rdr.IsDBNull(ordBirthDate) ? (DateTime?)null : rdr.GetDateTime(ordBirthDate);
            empleado.HireDate = rdr.IsDBNull(ordHireDate) ? (DateTime?)null : rdr.GetDateTime(ordHireDate);

            empleado.Address = rdr.IsDBNull(ordAddress) ? null : rdr.GetString(ordAddress);
            empleado.City = rdr.IsDBNull(ordCity) ? null : rdr.GetString(ordCity);
            empleado.Region = rdr.IsDBNull(ordRegion) ? null : rdr.GetString(ordRegion);
            empleado.PostalCode = rdr.IsDBNull(ordPostalCode) ? null : rdr.GetString(ordPostalCode);
            empleado.Country = rdr.IsDBNull(ordCountry) ? null : rdr.GetString(ordCountry);
            empleado.HomePhone = rdr.IsDBNull(ordHomePhone) ? null : rdr.GetString(ordHomePhone);
            empleado.Extension = rdr.IsDBNull(ordExtension) ? null : rdr.GetString(ordExtension);
            empleado.Notes = rdr.IsDBNull(ordNotes) ? null : rdr.GetString(ordNotes);

            empleado.ReportsTo = rdr.IsDBNull(ordReportsTo) ? (int?)null : rdr.GetInt32(ordReportsTo);

            empleado.Photo = rdr.IsDBNull(ordPhoto)
                ? null
                : Utils.StripOleHeader((byte[])rdr[ordPhoto], empleado.EmployeeID);

            empleado.PhotoPath = rdr.IsDBNull(ordPhotoPath) ? null : rdr.GetString(ordPhotoPath);

            return empleado;
        }
    }
}
