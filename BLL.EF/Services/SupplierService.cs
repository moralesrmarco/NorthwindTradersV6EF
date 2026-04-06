using DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.EF.Services
{
    public class SupplierService
    {
        public static List<Supplier> ObtenerProveedoresCbo()
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var proveedores = context.Suppliers
                        .OrderBy(s => s.CompanyName)
                        .ToList();
                    var opcionSeleccione = new Supplier
                    {
                        SupplierID = 0,
                        CompanyName = "»--- Seleccione ---«"
                    };
                    proveedores.Insert(0, opcionSeleccione);
                    return proveedores;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los proveedores" + ex.Message);
            }
        }
    }
}
