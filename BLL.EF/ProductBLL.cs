using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.EF
{
    public class ProductBLL
    {

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
    }
}
