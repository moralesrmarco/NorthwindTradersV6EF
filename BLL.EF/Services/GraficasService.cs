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

        public static int ObtenerTotalAñosConVentas()
        {
            using (var context = new NorthwindContext())
            {
                return context.Orders
                    .Where(o => o.OrderDate.HasValue)
                    .Select(o => o.OrderDate.Value.Year)
                    .Distinct()
                    .Count();
            }
        }

        public static List<DtoVentasMensualesPorAños> ObtenerVentasMensualesPorAños(int years)
        {
            using (var ctx = new NorthwindContext())
            {
                // 1. Obtener los años distintos de Orders
                var anios = ctx.Orders
                    .Where (o => o.OrderDate.HasValue)
                    .Select(o => o.OrderDate.Value.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .Take(years)
                    .ToList();

                // 2. Definir meses 1..12
                var meses = Enumerable.Range(1, 12).ToList();

                // 3. Calcular ventas mensuales
                var ventasMensuales = ctx.Orders
                    .Where(o => o.OrderDate.HasValue)
                    .SelectMany(o => o.Order_Details.Select(od => new
                    {
                        Year = o.OrderDate.Value.Year,
                        Mes = o.OrderDate.Value.Month,
                        // EF6 soporta double, no decimal en SQL
                        Total = (double)od.UnitPrice * od.Quantity * (1 - od.Discount)
                    }))
                    .GroupBy(x => new { x.Year, x.Mes })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Mes,
                        Total = g.Sum(x => x.Total)
                    })
                    .ToList() // aquí se ejecuta en SQL y vuelve a memoria
                        .Select(g => new
                        {
                            g.Year,
                            g.Mes,
                            Total = (decimal)g.Total // conversión segura en memoria
                        })
                        .ToList();
                // 4. Combinar años y meses con ventas
                var resultado = (from a in anios
                                 from m in meses
                                 join v in ventasMensuales
                                     on new { Year = a, Mes = m }
                                     equals new { v.Year, v.Mes }
                                     into gj
                                 from v in gj.DefaultIfEmpty()
                                 select new DtoVentasMensualesPorAños
                                 {
                                     Year = a,
                                     Mes = m,
                                     NombreMes = new DateTime(a, m, 1)
                                         .ToString("MMM", new System.Globalization.CultureInfo("es-ES")) + ".",
                                     Total = v != null ? Math.Round(v.Total, 2) : 0m
                                 })
                                 .OrderByDescending(r => r.Year)
                                 .ThenBy(r => r.Mes)
                                 .ToList();

                return resultado;
            }
        }

    }
}
