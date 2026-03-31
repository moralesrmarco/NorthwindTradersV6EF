using DAL.EF;
using System;
using System.Data;
using System.Linq;

namespace BLL.EF
{
    public class CustomerBLL
    {
        public static DataTable ObtenerClientesPaisesCbo()
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var paises = context.Customers
                        .AsNoTracking()
                        .Select ( c => new 
                        { 
                            Id = c.Country,
                            Pais = c.Country
                        })
                        .Distinct()
                        .OrderBy(c => c.Id)
                        .ToList();
                    paises.Insert(0, new { Id = "", Pais = "»--- Seleccione ---«" });
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Id", typeof(string));
                    dt.Columns.Add("Pais", typeof(string));
                    foreach (var p in paises)
                    {
                        dt.Rows.Add(p.Id, p.Pais);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los paises. " + ex.Message);
            }
        }
    }
}
