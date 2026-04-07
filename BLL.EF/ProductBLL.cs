using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace BLL.EF
{
    public class ProductBLL
    {

        public static int Insertar(Product producto)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    // Agregamos el producto al contexto
                    context.Products.Add(producto);

                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar producto: " + ex.Message);
            }
        }

        public static int Actualizar(Product producto)
        {
            using (var context = new NorthwindContext())
            {
                context.Products.Attach(producto);
                context.Entry(producto).State = EntityState.Modified;
                context.Entry(producto).OriginalValues["RowVersion"] = producto.RowVersion;

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
                    return -3; // Otro error
                }
            }
        }

        public static int Eliminar(Product producto)
        {
            using (var context = new NorthwindContext())
            {
                context.Products.Attach(producto);
                context.Entry(producto).State = EntityState.Deleted;
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

        public static List<Product> ObtenerProductos(bool selectorRealizaBusqueda, DtoProductosBuscar criterios, bool top100)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    IQueryable<Product> query = context.Products
                        .Include("Supplier")
                        .Include("Category");

                    if (selectorRealizaBusqueda)
                    {
                        query = query.OrderBy(p => p.ProductID);

                        if (criterios.IdIni > 0)
                            query = query.Where(p => p.ProductID >= criterios.IdIni);

                        if (criterios.IdFin > 0)
                            query = query.Where(p => p.ProductID <= criterios.IdFin);

                        if (!string.IsNullOrEmpty(criterios.Producto))
                            query = query.Where(p => p.ProductName.Contains(criterios.Producto));

                        if (criterios.Categoria > 0)
                            query = query.Where(p => p.CategoryID == criterios.Categoria);

                        if (criterios.Proveedor > 0)
                            query = query.Where(p => p.SupplierID == criterios.Proveedor);
                    }
                    else
                    {
                        if (!top100)
                            query = query.OrderByDescending(p => p.ProductID).Take(20);
                    }

                    return query
                           .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener productos: " + ex.Message);
            }
        }

        public static List<DtoProductoListado> ObtenerProductos(DtoProductosBuscar criterios)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var productos = context.Database.SqlQuery<DtoProductoListado>(
                        "EXEC SpProductoBuscarV4 @IdIni, @IdFin, @Producto, @Categoria, @Proveedor, @OrdenadoPor, @AscDesc",
                        new SqlParameter("@IdIni", criterios.IdIni),
                        new SqlParameter("@IdFin", criterios.IdFin),
                        new SqlParameter("@Producto", criterios.Producto ?? string.Empty),
                        new SqlParameter("@Categoria", criterios.Categoria),
                        new SqlParameter("@Proveedor", criterios.Proveedor),
                        new SqlParameter("@OrdenadoPor", criterios.OrdenadoPor),
                        new SqlParameter("@AscDesc", criterios.AscDesc)
                    ).ToList();

                    return productos;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener productos: " + ex.Message);
            }
        }

        public static Product ObtenerProductoPorId(int productId)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    // Incluimos las relaciones para que se carguen junto con el producto
                    var producto = context.Products
                                          .Include("Supplier")
                                          .Include("Category")
                                          .FirstOrDefault(p => p.ProductID == productId);

                    return producto;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener producto por ID: " + ex.Message);
            }
        }

        public static List<DtoProductosPorProveedor> ObtenerProductosPorProveedor()
        {
            using (var context = new NorthwindContext())
            {
                var query = from s in context.Suppliers
                            join p in context.Products
                                on s.SupplierID equals p.SupplierID into prodGroup
                            from p in prodGroup.DefaultIfEmpty()
                            join c in context.Categories
                                on p.CategoryID equals c.CategoryID into catGroup
                            from c in catGroup.DefaultIfEmpty()
                            select new DtoProductosPorProveedor
                            {
                                ProductID = p != null ? p.ProductID : (int?)null,
                                ProductName = p != null ? p.ProductName : "Sin producto",
                                CompanyName = s.CompanyName ?? string.Empty,
                                QuantityPerUnit = p != null ? p.QuantityPerUnit : string.Empty,
                                UnitPrice = p != null ? (decimal?)p.UnitPrice : null,
                                UnitsInStock = p != null ? (short?)p.UnitsInStock : null,
                                UnitsOnOrder = p != null ? (short?)p.UnitsOnOrder : null,
                                ReorderLevel = p != null ? (short?)p.ReorderLevel : null,
                                Discontinued = p != null && p.Discontinued,
                                CategoryName = c != null ? c.CategoryName : "Sin categoría"
                            };

                return query.ToList();
            }
        }

        public static List<DtoProductosPorProveedorConDetProv> ObtenerProductosPorProveedorConDetProv()
        {
            using (var context = new NorthwindContext())
            {
                var query = from s in context.Suppliers
                            join p in context.Products
                                on s.SupplierID equals p.SupplierID into prodGroup
                            from p in prodGroup.DefaultIfEmpty()
                            join c in context.Categories
                                on p.CategoryID equals c.CategoryID into catGroup
                            from c in catGroup.DefaultIfEmpty()
                            orderby s.CompanyName, p.ProductName
                            select new DtoProductosPorProveedorConDetProv
                            {
                                SupplierID = s.SupplierID,
                                CompanyName = s.CompanyName ?? string.Empty,
                                ContactName = s.ContactName ?? string.Empty,
                                ContactTitle = s.ContactTitle ?? string.Empty,
                                Address = s.Address ?? string.Empty,
                                City = s.City ?? string.Empty,
                                Region = s.Region ?? string.Empty,
                                PostalCode = s.PostalCode ?? string.Empty,
                                Country = s.Country ?? string.Empty,
                                Phone = s.Phone ?? string.Empty,
                                Fax = s.Fax ?? string.Empty,

                                ProductID = p != null ? p.ProductID : (int?)null,
                                ProductName = p != null ? p.ProductName : "Sin producto",
                                QuantityPerUnit = p != null ? p.QuantityPerUnit : string.Empty,
                                UnitPrice = p != null ? (decimal?)p.UnitPrice : null,
                                UnitsInStock = p != null ? (short?)p.UnitsInStock : null,
                                UnitsOnOrder = p != null ? (short?)p.UnitsOnOrder : null,
                                ReorderLevel = p != null ? (short?)p.ReorderLevel : null,
                                Discontinued = p != null && p.Discontinued,

                                CategoryName = c != null ? c.CategoryName : "Sin categoría"
                            };

                return query.ToList();
            }
        }

        public static decimal ObtenerPrecioPromedio()
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    // Devuelve null si no hay registros
                    var promedio = context.Products
                                          .Select(p => (decimal?)p.UnitPrice)
                                          .Average();

                    return promedio ?? 0m; // Si es null, regresa 0
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el precio promedio: " + ex.Message);
            }
        }

        public static DataTable ObtenerProductosPorEncimaDelPrecioPromedio()
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var productos = context.VwProductosPorEncimaDelPrecioPromedios
                                                   .OrderBy(p => p.Fila)
                                                   .ToList();

                    var dt = new DataTable();
                    dt.Columns.Add("Fila", typeof(int));
                    dt.Columns.Add("Producto", typeof(string));
                    dt.Columns.Add("Precio", typeof(decimal));
                    dt.Columns.Add("PrecioPromedio", typeof(decimal));
                    dt.Columns.Add("Diferencia", typeof(decimal));
                    dt.Columns.Add("PorcentajeSobrePromedio", typeof(decimal));
                    dt.Columns.Add("Categoria", typeof(string));
                    dt.Columns.Add("Proveedor", typeof(string));

                    foreach (var p in productos)
                    {
                        dt.Rows.Add(p.Fila, p.Producto, p.Precio, p.PrecioPromedio,
                                    p.Diferencia, p.PorcentajeSobrePromedio,
                                    p.Categoria, p.Proveedor);
                    }

                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener productos por encima del precio promedio: " + ex.Message);
            }
        }
    }
}
