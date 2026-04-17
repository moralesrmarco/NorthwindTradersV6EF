using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BLL.EF.Services
{
    public class GraficasService
    {
        public static DataTable ObtenerAñosDeVentas(bool conFilaSeleccione = true)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("YearOrderDate", typeof(string));
            using (var context = new NorthwindContext())
            {
                var años = context.Orders
                    .Where(o => o.OrderDate != null)
                    .Select(o => o.OrderDate.Value.Year.ToString())
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList();
                if (conFilaSeleccione)
                { 
                    DataRow filaSeleccione = dt.NewRow();
                    filaSeleccione["YearOrderDate"] = "»--- Seleccione ---«";
                    dt.Rows.InsertAt(filaSeleccione, 0);
                }
                foreach (var año in años)
                {
                    dt.Rows.Add(año);
                }
            }
            return dt;
        }

        public static List<DtoVentasMensuales> ObtenerVentasMensuales(int year)
        {
            using (var context = new NorthwindContext())
            {
                // Generar lista de meses 1..12
                var meses = Enumerable.Range(1, 12);

                // Consulta de ventas agrupadas por mes
                var ventasMensuales = context.Orders
                    .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
                    .SelectMany(o => o.Order_Details.Select(od => new
                    {
                        Mes = o.OrderDate.Value.Month,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        Discount = od.Discount
                    }))
                    .ToList() // aquí ya estás en memoria
                    .GroupBy(x => x.Mes)
                    .Select(g => new
                    {
                        Mes = g.Key,
                        Total = g.Sum(x => x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                    })
                    .ToList();

                // Proyección final con left join
                var resultado = (from m in meses
                                 join v in ventasMensuales
                                     on m equals v.Mes into gj
                                 from v in gj.DefaultIfEmpty()
                                 orderby m
                                 select new DtoVentasMensuales
                                 {
                                     Mes = m,
                                     NombreMes = new DateTime(year, m, 1)
                                         .ToString("MMM", new System.Globalization.CultureInfo("es-ES")) + ".",
                                     Total = v != null ? Math.Round(v.Total, 2) : 0m
                                 }).ToList();

                return resultado;
            }
        }
    }
}
