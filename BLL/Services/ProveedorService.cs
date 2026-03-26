using DAL.Helpers;
using System.Data;

namespace BLL.Services
{
    public class ProveedorService
    {
        private readonly ComboDataHelper _dal;

        public ProveedorService(string connectionString)
        {
            _dal = new ComboDataHelper(connectionString);
        }

        public DataTable ObtenerProveedoresCbo()
        {
            var proveedores = _dal.LlenarCbo("SpProveedorObtenerCbo");
            DataRow filaSeleccione = proveedores.NewRow();
            filaSeleccione["SupplierID"] = 0;
            filaSeleccione["CompanyName"] = "»--- Seleccione ---«";
            proveedores.Rows.InsertAt(filaSeleccione, 0);
            return proveedores;
        }
    }
}
