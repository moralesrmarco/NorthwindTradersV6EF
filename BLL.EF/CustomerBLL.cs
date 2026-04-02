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
    public class CustomerBLL
    {
        public static int Insertar(Customer cliente)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    context.Customers.Add(cliente);
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el cliente. " + ex.Message);
            }
        }

        public static int Actualizar(Customer cliente)
        {
            using (var context = new NorthwindContext())
            {
                context.Customers.Attach(cliente);
                context.Entry(cliente).State = EntityState.Modified;
                context.Entry(cliente).OriginalValues["RowVersion"] = cliente.RowVersion;

                try
                {
                    return context.SaveChanges();
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
                    return -3;
                }
            }
        }

        public static int Eliminar(Customer cliente)
        {
            using (var context = new NorthwindContext())
            {
                context.Customers.Attach(cliente);
                context.Entry(cliente).State = EntityState.Deleted;
                try
                {
                    return context.SaveChanges();
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

        public static DataTable ObtenerClientesPaisesCbo()
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var paises = context.Customers
                        .AsNoTracking()
                        .Select(c => new
                        {
                            Id = c.Country,
                            Pais = c.Country
                        })
                        .Distinct()
                        .OrderBy(c => c.Id)
                        .ToList();
                    paises.Insert(0, new { Id = "", Pais = "»--- Seleccione ---«" });
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Id", typeof(string));
                    dt.Columns.Add("Pais", typeof(string));
                    foreach (var p in paises)
                    {
                        dt.Rows.Add(p.Id, p.Pais);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los paises. " + ex.Message);
            }
        }

        public static (List<Customer> clientes, string mensajeEstado) ObtenerClientes(bool selectorRealizaBusqueda, Customer criterios, bool top100 = false)
        {
            try
            {
                var clientes = new List<Customer>();
                using (var context = new NorthwindContext())
                {
                    IQueryable<Customer> query = context.Customers.AsNoTracking();
                    if (selectorRealizaBusqueda)
                    {
                        if (!string.IsNullOrEmpty(criterios.CustomerID))
                            query = query.Where(c => c.CustomerID.Contains(criterios.CustomerID));
                        if (!string.IsNullOrEmpty(criterios.CompanyName))
                            query = query.Where(c => c.CompanyName.Contains(criterios.CompanyName));
                        if (!string.IsNullOrEmpty(criterios.ContactName))
                            query = query.Where(c => c.ContactName.Contains(criterios.ContactName));
                        if (!string.IsNullOrEmpty(criterios.City))
                            query = query.Where(c => c.City.Contains(criterios.City));
                        if (!string.IsNullOrEmpty(criterios.Region))
                            query = query.Where(c => c.Region.Contains(criterios.Region));
                        if (!string.IsNullOrEmpty(criterios.PostalCode))
                            query = query.Where(c => c.PostalCode.Contains(criterios.PostalCode));
                        if (!string.IsNullOrEmpty(criterios.Country))
                            query = query.Where(c => c.Country.Contains(criterios.Country));
                        if (!string.IsNullOrEmpty(criterios.Phone))
                            query = query.Where(c => c.Phone.Contains(criterios.Phone));
                        if (!string.IsNullOrEmpty(criterios.Fax))
                            query = query.Where(c => c.Fax.Contains(criterios.Fax));
                    }
                    else
                    {
                        if (!top100)
                            query = query.Take(20);
                    }
                    clientes = query.ToList().Select(c => new Customer // doble tolist es para Proyección para evitar problemas
                    {
                        CustomerID = c.CustomerID,
                        CompanyName = c.CompanyName,
                        ContactName = c.ContactName,
                        Address = c.Address,
                        City = c.City,
                        Region = c.Region,
                        PostalCode = c.PostalCode,
                        Country = c.Country,
                        Phone = c.Phone,
                        Fax = c.Fax
                    }).ToList(); // doble tolist es para Proyección para evitar problemas
                }
                string mensajeEstado = selectorRealizaBusqueda
                    ? $"Se encontraron {clientes.Count} cliente(s) con los criterios proporcionados."
                    : $"Se muestran los primeros {clientes.Count} cliente(s) registrados.";
                return (clientes, mensajeEstado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los clientes. " + ex.Message);
            }
        }

        public static Customer ObtenerClientePorId(string customerId)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    Customer cliente = context.Customers
                        .AsNoTracking()
                        .FirstOrDefault(c => c.CustomerID == customerId);
                    return cliente;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el cliente por ID. " + ex.Message);
            }
        }

        public static List<DtoClienteProveedor> ObtenerClientesProveedores(
            string nombreDeFormulario,
            string comboBoxSelectedValue,
            bool checkBoxClientes,
            bool checkBoxProveedores)
        {
            using (var context = new NorthwindContext())
            {
                IQueryable<VwClientesProveedore> query = context.VwClientesProveedores;

                // Filtro por relación
                if (checkBoxClientes && !checkBoxProveedores)
                    query = query.Where(x => x.Relation == "Cliente");
                else if (!checkBoxClientes && checkBoxProveedores)
                    query = query.Where(x => x.Relation == "Proveedor");

                // Filtro por ciudad
                if (nombreDeFormulario.Contains("DirectorioxCiudad"))
                {
                    if (comboBoxSelectedValue != "aaaaa")
                    {
                        var partes = comboBoxSelectedValue.Split(',');
                        var ciudad = partes[0].Trim();
                        var pais = partes.Length > 1 ? partes[1].Trim() : string.Empty;

                        query = query.Where(x => x.City == ciudad && x.Country == pais);
                    }
                    query = query.OrderBy(x => x.City).ThenBy(x => x.Country).ThenBy(x => x.CompanyName);
                }
                // Filtro por país
                else if (nombreDeFormulario.Contains("DirectorioxPais"))
                {
                    if (comboBoxSelectedValue != "aaaaa")
                        query = query.Where(x => x.Country == comboBoxSelectedValue);

                    query = query.OrderBy(x => x.Country).ThenBy(x => x.City).ThenBy(x => x.CompanyName);
                }
                // Directorio general
                else
                {
                    query = checkBoxClientes && checkBoxProveedores
                        ? query.OrderBy(x => x.Relation).ThenBy(x => x.CompanyName)
                        : query.OrderBy(x => x.CompanyName);
                }

                // Proyección al DTO
                return query.Select(x => new DtoClienteProveedor
                {
                    CompanyName = x.CompanyName,
                    Contact = x.Contact,
                    Relation = x.Relation,
                    Address = x.Address,
                    City = x.City,
                    Region = x.Region,
                    PostalCode = x.PostalCode,
                    Country = x.Country,
                    Phone = x.Phone,
                    Fax = x.Fax
                }).ToList();
            }
        }

        public static List<KeyValuePair<string, string>> ObtenerCiudadPaisVwCliProvCbo()
        {
            using (var context = new NorthwindContext())
            {
                var ciudadesPaises = context.VwClientesProveedores
                    .Select(x => x.City + ", " + x.Country)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                // Construir la lista final con las opciones especiales
                var resultado = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string,string>("»--- Seleccione ---«", " "),
                    new KeyValuePair<string,string>("»--- Todas las ciudades ---«", "aaaaa")
                };

                // Agregar las ciudades reales
                resultado.AddRange(ciudadesPaises.Select(c => new KeyValuePair<string, string>(c, c)));

                return resultado;
            }
        }

        public static List<KeyValuePair<string, string>> ObtenerPaisVwCliProvCbo()
        {
            using (var context = new NorthwindContext())
            {
                var paises = context.VwClientesProveedores
                    .Select(x => x.Country)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                // Construir la lista final con las opciones especiales
                var resultado = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string,string>("»--- Seleccione ---«", " "),
                    new KeyValuePair<string,string>("»--- Todos los países ---«", "aaaaa")
                };
                // Agregar los países reales
                resultado.AddRange(paises.Select(p => new KeyValuePair<string, string>(p, p)));
                return resultado;
            }
        }
    }
}
