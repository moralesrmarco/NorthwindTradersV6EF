using DAL.EF;
using System.Data;
using System.Linq;

namespace BLL.EF.Services
{
    public class CustomerService
    {
        public static DataTable ObtenerClientesCbo()
        {
            using (var context = new NorthwindContext())
            {
                // Traer todos los clientes desde la tabla
                var clientes = context.Customers
                    .Select(c => new 
                    {
                        c.CustomerID,
                        c.CompanyName
                    })
                    .OrderBy(c => c.CompanyName) // opcional, si quieres ordenarlos
                    .ToList();

                // Crear el DataTable con las columnas necesarias
                DataTable dt = new DataTable();
                dt.Columns.Add("CustomerID", typeof(string));
                dt.Columns.Add("CompanyName", typeof(string));

                // Insertar la fila "Seleccione" al inicio
                dt.Rows.Add("", "»--- Seleccione ---«");
                foreach (var cliente in clientes)
                {
                    dt.Rows.Add(cliente.CustomerID, cliente.CompanyName);
                }
                return dt;
            }
        }
    }
}
