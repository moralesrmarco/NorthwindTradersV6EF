using DAL;
using Entities;
using Entities.DTOs;
using System.Collections.Generic;
using System.Data;

namespace BLL
{
    public class CategoriaBLL
    {

        private readonly CategoriaDAL _categoriaDAL;

        public CategoriaBLL(string _connectionString)
        {
            _categoriaDAL = new CategoriaDAL(_connectionString);
        }

        public int Insertar(Categoria categoria)
        {
            return _categoriaDAL.Insertar(categoria);
        }

        public int Actualizar(Categoria categoria)
        {
            return _categoriaDAL.Actualizar(categoria);
        }

        public int Eliminar(int categoriaId, byte[] rowVersion)
        {
            return _categoriaDAL.Eliminar(categoriaId, rowVersion);
        }

        public List<Categoria> ObtenerCategorias(bool selectorRealizaBusqueda, DtoCategoriasBuscar criterios, bool top100)
        {
            return _categoriaDAL.ObtenerCategorias(selectorRealizaBusqueda, criterios, top100);
        }

        public DataSet ObtenerCategoriasProductosDgv()
        {
            return _categoriaDAL.ObtenerCategoriasProductosDgv();
        }

        public DataTable ObtenerProductosPorCategoriaListado()
        {
            return _categoriaDAL.ObtenerProductosPorCategoriaListado();
        }

        public List<Categoria> ObtenerCategoriasConProductos()
        {
            return _categoriaDAL.ObtenerCategoriasConProductos();
        }

    }
}
