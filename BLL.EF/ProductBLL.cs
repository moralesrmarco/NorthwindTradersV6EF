using DAL.EF;
using DTOs.EF;
using System.Collections.Generic;
using System.Linq;

namespace BLL.EF
{
    public class ProductBLL
    {
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
    }
}
