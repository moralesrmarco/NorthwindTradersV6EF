using DAL.Helpers;
using System.Data;

namespace BLL.Services
{
    public class TransportistaService
    {
        private readonly ComboDataHelper _dal;

        public TransportistaService(string connectionString)
        {
            _dal = new ComboDataHelper(connectionString);
        }

        public DataTable ObtenerTransportistasCbo()
        {
            var transportistas = _dal.LlenarCbo("SpTransportistaObtenerCbo");
            DataRow filaSeleccione = transportistas.NewRow();
            filaSeleccione["ShipperID"] = 0;
            filaSeleccione["CompanyName"] = "»--- Seleccione ---«";
            transportistas.Rows.InsertAt(filaSeleccione, 0);
            return transportistas;
        }
    }
}
