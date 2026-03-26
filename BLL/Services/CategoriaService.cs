using DAL.Helpers;
using System.Data;

namespace BLL.Services
{
    public class CategoriaService
    {
        private readonly ComboDataHelper _dal;

        public CategoriaService(string connectionString)
        {
            _dal = new ComboDataHelper(connectionString);
        }

        public DataTable ObtenerCategoriasCbo()
        {
            var categorias = _dal.LlenarCbo("SpCategoriaObtenerCbo");
            DataRow filaSeleccione = categorias.NewRow();
            filaSeleccione["CategoryID"] = 0;
            filaSeleccione["CategoryName"] = "»--- Seleccione ---«";
            categorias.Rows.InsertAt(filaSeleccione, 0);
            return categorias;
        }
    }
}
