using DAL.EF;
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
    }
}
