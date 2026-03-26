using DAL.Helpers;
using System.Data;

namespace BLL.Services
{
    public class ClienteService
    {
        private readonly ComboDataHelper _dal;

        public ClienteService(string connectionString)
        {
            _dal = new ComboDataHelper (connectionString);
        }

        public DataTable ObtenerClientesCbo()
        {
            var clientes = _dal.LlenarCbo("SpClienteObtenerCbo");
            DataRow filaSeleccione = clientes.NewRow();
            filaSeleccione["CustomerID"] = 0;
            filaSeleccione["CompanyName"] = "»--- Seleccione ---«";
            clientes.Rows.InsertAt(filaSeleccione, 0);
            return clientes;
        }
    }
}
