using DAL.EF;
using DTOs.EF;
using System;
using System.Data;
using System.Linq;

namespace BLL.EF.Services
{
    public class ProductService
    {
        public static DataTable ObtenerProductosPorCategoriaCbo(int categoriaId)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var productos = context.Products
                        .Where(p => p.CategoryID == categoriaId && p.Discontinued == false)
                        .Select(p => new
                        {
                            p.ProductID,
                            p.ProductName
                        })
                        .OrderBy(p => p.ProductName)
                        .ToList();
                    var dt = new DataTable();
                    dt.Columns.Add("ProductID", typeof(int));
                    dt.Columns.Add("ProductName", typeof(string));
                    dt.Rows.Add(0, "»--- Seleccione ---«");
                    foreach (var producto in productos)
                    {
                        dt.Rows.Add(producto.ProductID, producto.ProductName);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos por categoría: " + ex.Message);
            }
        }

        public static DtoProductoCostoEInventario ObtenerProductoCostoEInventario(int productoId)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    return context.Products
                        .Where(p => p.ProductID == productoId)
                        .Select(p => new DtoProductoCostoEInventario
                        {
                            UnitPrice = (decimal)p.UnitPrice,
                            UnitsInStock = (short)p.UnitsInStock
                        })
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el costo e inventario del producto: " + ex.Message);
            }
        }
    }
}
