using DAL.Helpers;
using System.Data;
using System.Data.SqlClient;
using Entities.DTOs;
using DAL;

namespace BLL.Services
{
    public class ProductoService
    {
        private readonly ComboDataHelper _dal;
        private readonly ProductoDAL _productoDAL;

        public ProductoService(string connectionString)
        {
            _dal = new ComboDataHelper(connectionString);
            _productoDAL = new ProductoDAL(connectionString);
        }

        public DataTable ObtenerProductosPorCategoriaCbo(int categoria)
        {
            var productos = _dal.LlenarCbo("SpProductosObtenerPorCategoriaCbo", new SqlParameter("@Categoria", categoria));
            DataRow filaSeleccione = productos.NewRow();
            filaSeleccione["ProductID"] = 0;
            filaSeleccione["ProductName"] = "»--- Seleccione ---«";
            productos.Rows.InsertAt(filaSeleccione, 0);
            return productos;
        }

        public DtoProductoCostoEInventario ObtenerProductoCostoEInventario(int productId)
        {
            return _productoDAL.ObtenerProductoCostoEInventario(productId);
        }
    }
}
