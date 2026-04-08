using DAL.EF;
using System.Data;
using System.Linq;

namespace BLL.EF.Services
{
    public class ShipperService
    {
        public static DataTable ObtenerTransportistasCbo()
        {
            using (var context = new NorthwindContext())
            {
                var transportistas = context.Shippers
                    .Select(s => new
                    {
                        s.ShipperID,
                        s.CompanyName
                    })
                    .OrderBy(s => s.CompanyName)
                    .ToList();
                DataTable dt = new DataTable();
                dt.Columns.Add("ShipperID", typeof(int));
                dt.Columns.Add("CompanyName", typeof(string));
                dt.Rows.Add(0, "»--- Seleccione ---«");
                foreach (var transportista in transportistas)
                {
                    dt.Rows.Add(transportista.ShipperID, transportista.CompanyName);
                }
                return dt;
            }
        }
    }
}
