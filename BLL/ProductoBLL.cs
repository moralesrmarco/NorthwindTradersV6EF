using DAL;
using Entities;
using Entities.DTOs;
using System.Collections.Generic;
using System.Data;

namespace BLL
{
    public class ProductoBLL
    {
        private readonly ProductoDAL _productoDAL;

        public ProductoBLL(string _connectionString)
        {
            _productoDAL = new ProductoDAL(_connectionString);
        }

        public int Insertar(Producto producto)
        {
            return _productoDAL.Insertar(producto);
        }

        public int Actualizar(Producto producto)
        {
            return _productoDAL.Actualizar(producto);
        }

        public int Eliminar(int productId, byte[] rowVersion)
        {
            return _productoDAL.Eliminar(productId, rowVersion);
        }

        public List<Producto> ObtenerProductos(bool selectorRealizaBusqueda, DtoProductosBuscar criterios, bool top100)
        {
            return _productoDAL.ObtenerProductos(selectorRealizaBusqueda, criterios, top100);
        }

        public List<Producto> ObtenerProductos(DtoProductosBuscar criterios)
        {
            return _productoDAL.ObtenerProductos(criterios);
        }

        public Producto ObtenerProductoPorId(int productId)
        {
            return _productoDAL.ObtenerProductoPorId(productId);
        }

        public List<DtoProductosPorProveedor> ObtenerProductosPorProveedor()
        {
            return _productoDAL.ObtenerProductosPorProveedor();
        }

        public List<DtoProductosPorProveedorConDetProv> ObtenerProductosPorProveedorConDetProv()
        {
            return _productoDAL.ObtenerProductosPorProveedorConDetProv();
        }

        public decimal ObtenerPrecioPromedio()
        {
            return _productoDAL.ObtenerPrecioPromedio();
        }

        public DataTable ObtenerProductosPorEncimaDelPrecioPromedio()
        {
            return _productoDAL.ObtenerProductosPorEncimaDelPrecioPromedio();
        }
    }
}
