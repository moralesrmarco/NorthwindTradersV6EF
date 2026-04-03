using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace BLL.EF
{
    public class SupplierBLL
    {

        public static int Insertar(Supplier proveedor)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    context.Suppliers.Add(proveedor);
                    return context.SaveChanges(); 
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el proveedor. " + ex.Message);
            }
        }

        public static int Actualizar(Supplier proveedor)
        {
            using (var context = new NorthwindContext())
            {
                context.Suppliers.Attach(proveedor);
                context.Entry(proveedor).State = EntityState.Modified;
                context.Entry(proveedor).OriginalValues["RowVersion"] = proveedor.RowVersion; // Para control de concurrencia optimista
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

        public static int Eliminar(Supplier proveedor)
        {
            using (var context = new NorthwindContext())
            {
                context.Suppliers.Attach(proveedor);
                context.Entry(proveedor).State = EntityState.Deleted;
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
                    return -3; // Error inesperado
                }
            }
        }

        public static List<Supplier> ObtenerProveedores(bool selectorRealizaBusqueda, DtoProveedoresBuscar criterios, bool top100)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var query = context.Suppliers.AsNoTracking().AsQueryable();
                    if (selectorRealizaBusqueda)
                    {
                        if (criterios.IdIni > 0)
                            query = query.Where(s => s.SupplierID >= criterios.IdIni && s.SupplierID <= criterios.IdFin);
                        if (!string.IsNullOrEmpty(criterios.CompanyName))
                            query = query.Where(s => s.CompanyName.Contains(criterios.CompanyName));
                        if (!string.IsNullOrEmpty(criterios.ContactName))
                            query = query.Where(s => s.ContactName.Contains(criterios.ContactName));
                        if (!string.IsNullOrEmpty(criterios.Address))
                            query = query.Where(s => s.Address.Contains(criterios.Address));
                        if (!string.IsNullOrEmpty(criterios.City))
                            query = query.Where(s => s.City.Contains(criterios.City));
                        if (!string.IsNullOrEmpty(criterios.Region))
                            query = query.Where(s => s.Region.Contains(criterios.Region));
                        if (!string.IsNullOrEmpty(criterios.PostalCode))
                            query = query.Where(s => s.PostalCode.Contains(criterios.PostalCode));
                        if (!string.IsNullOrEmpty(criterios.Country))
                            query = query.Where(s => s.Country.Contains(criterios.Country));
                        if (!string.IsNullOrEmpty(criterios.Phone))
                            query = query.Where(s => s.Phone.Contains(criterios.Phone));
                        if (!string.IsNullOrEmpty(criterios.Fax))
                            query = query.Where(s => s.Fax.Contains(criterios.Fax));
                    }
                    else
                        if (!top100)
                            query = query.OrderByDescending(s => s.SupplierID).Take(20);

                    return query.ToList().Select(s => new Supplier // doble tolist es para Proyección para evitar problemas con el rowversion
                        {
                            SupplierID = s.SupplierID,
                            CompanyName = s.CompanyName,
                            ContactName = s.ContactName,
                            ContactTitle = s.ContactTitle,
                            Address = s.Address,
                            City = s.City,
                            Region = s.Region,
                            PostalCode = s.PostalCode,
                            Country = s.Country,
                            Phone = s.Phone,
                            Fax = s.Fax
                        }).ToList(); // doble tolist es para Proyección para evitar problemas con el rowversion al desplegarse en el datagridview, en la segunda proyección se omite el rowversion 
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los proveedores. " + ex.Message);
            }
        }

        public static Supplier ObtenerProveedorPorId(int supplierId)
        {
            try
            {
                using ( var context = new NorthwindContext())
                {
                    
                    return context.Suppliers
                        .AsNoTracking()
                        .FirstOrDefault(s => s.SupplierID == supplierId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el proveedor por ID. " + ex.Message);
            }
        }

        public static DataTable ObtenerProveedorPaisesCbo()
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var paises = context.Suppliers
                        .AsNoTracking()
                        .Select(s => new
                        {
                            Id = s.Country,
                            Pais = s.Country
                        })
                        .Distinct()
                        .OrderBy(s => s.Id)
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
    }
}
