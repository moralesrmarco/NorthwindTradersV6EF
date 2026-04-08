using DAL.EF;
using System.Collections.Generic;
using System.Linq;

namespace BLL.EF.Services
{
    public class CustomerService
    {
        public static List<dynamic> ObtenerClientesCbo()
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
                    .ToList<dynamic>();

                // Insertar la fila "Seleccione" al inicio
                clientes.Insert(0, new 
                {
                    CustomerID = "00000",
                    CompanyName = "»--- Seleccione ---«"
                });

                return clientes;
            }
        }
    }
}
