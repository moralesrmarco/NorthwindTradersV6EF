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
                    .Where(o => o.OrderDate.HasValue && (year == -1 || o.OrderDate.Value.Year == year))
                    .SelectMany(o => o.Order_Details
                        .Select(od => new
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
                        Total = g.Sum(x =>
                            x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount)
                        )
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
                                     // se usa un año ficticio solo para calcular bien el nombre del mes
                                     NombreMes = FormatearMes(m,year),
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
                                     NombreMes = FormatearMes(m,a),
                                     Total = v != null ? Math.Round(v.Total, 2) : 0m
                                 })
                                 .OrderByDescending(r => r.Year)
                                 .ThenBy(r => r.Mes)
                                 .ToList();

                return resultado;
            }
        }

        public static DataTable ObtenerTop10AñosDeVentas(bool conFilaSeleccione = true)
        {
            using (var ctx = new NorthwindContext())
            {
                var años = ctx.Orders
                    .Where(o => o.OrderDate.HasValue)
                    .Select(o => o.OrderDate.Value.Year.ToString())
                    .Distinct()
                    .OrderByDescending(y => y)
                    .Take(10)
                    .ToList();

                int añoActual = DateTime.Today.Year;

                DataTable dt = new DataTable();
                dt.Columns.Add("Texto", typeof(string));
                dt.Columns.Add("Valor", typeof(int));
                if (conFilaSeleccione)
                {
                    dt.Rows.Add("»--- Seleccione ---«", 0);
                }
                dt.Rows.Add("Todos los años", -1);
                foreach (var año in años)
                    dt.Rows.Add(año.ToString(), año);

                // Validamos si el año actual ya existe en el DataTable
                bool existe = dt.AsEnumerable().Any(r => r.Field<int>("Valor") == añoActual);

                if (!existe)
                {
                    // Creamos la fila del año actual
                    DataRow filaActual = dt.NewRow();
                    filaActual["Texto"] = añoActual.ToString();
                    filaActual["Valor"] = añoActual;

                    // Insertamos en la posición 3 (índice 2)
                    dt.Rows.InsertAt(filaActual, 2);
                }
                return dt;
            }
        }

        public static DataTable ObtenerTopProductos(int cantidad, int anio)
        {
            using (var ctx = new NorthwindContext())
            {
                var query = ctx.Order_Details
                    .Where (od => anio == -1 || (od.Order.OrderDate.HasValue && od.Order.OrderDate.Value.Year == anio))
                    .GroupBy(od => od.Product.ProductName)
                    .Select(g => new
                    {
                        NombreProducto = g.Key,
                        CantidadVendida = g.Sum(x => x.Quantity)
                    })
                    .OrderByDescending(x => x.CantidadVendida)
                    .ThenBy(x => x.NombreProducto)
                    .Take(cantidad)
                    .ToList();
                DataTable dt = new DataTable();
                dt.Columns.Add("NombreProducto", typeof(string));
                dt.Columns.Add("CantidadVendida", typeof(int));
                foreach(var item in query)
                {
                    DataRow dr = dt.NewRow();
                    dr["NombreProducto"] = item.NombreProducto;
                    dr["CantidadVendida"] = item.CantidadVendida;
                    dt.Rows.Add(dr);
                }
                return dt;
            }
        }

        public static List<(string Vendedor, decimal TotalVentas)> ObtenerVentasPorVendedores(int anio)
        {
            using (var ctx = new NorthwindContext())
            {
                var query = ctx.Orders
                    .Where(o => anio == -1 || (o.OrderDate.HasValue && o.OrderDate.Value.Year == anio))
                    .SelectMany(o => o.Order_Details, (o, od) => new
                    {
                        Vendedor = o.Employee.FirstName + " " + o.Employee.LastName,
                        Total = (double)od.UnitPrice * od.Quantity * (1 - od.Discount)
                    })
                    .GroupBy(x  => x.Vendedor)
                    .Select(g => new
                    {
                        Vendedor = g.Key,
                        TotalVentas = g.Sum(x => x.Total)
                    })
                    .OrderByDescending(x => x.TotalVentas)
                    .ToList();

                var resultados = query
                    .Select(x => (x.Vendedor, (decimal)x.TotalVentas))
                    .ToList();
                return resultados;
            }
        }

        public static DataTable ObtenerVentasMensualesPorVendedoresPorAño(int anio)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Vendedor", typeof(string));
            dt.Columns.Add("Mes", typeof(int));
            dt.Columns.Add("NombreMes", typeof(string));
            dt.Columns.Add("TotalVentas", typeof(decimal));

            using (var context = new NorthwindContext())
            {
                var meses = Enumerable.Range(1, 12);

                if (anio == -1)
                {
                    // Caso: todos los años, sumar por vendedor y mes
                    var ventas = context.Orders
                        .Where(o => o.OrderDate.HasValue)
                        .SelectMany(o => o.Order_Details.Select(od => new
                        {
                            Vendedor = o.Employee.FirstName + " " + o.Employee.LastName,
                            Mes = o.OrderDate.Value.Month,
                            UnitPrice = od.UnitPrice,
                            Quantity = od.Quantity,
                            Discount = od.Discount
                        }))
                        .ToList() // cálculo en memoria
                        .GroupBy(x => new { x.Vendedor, x.Mes })
                        .Select(g => new
                        {
                            Vendedor = g.Key.Vendedor,
                            Mes = g.Key.Mes,
                            TotalVentas = g.Sum(x => (decimal)x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                        })
                        .ToList();

                    // Cross join vendedores × meses
                    var vendedores = ventas.Select(v => v.Vendedor).Distinct().ToList();

                    var resultado = (from v in vendedores
                                     from m in meses
                                     join ve in ventas
                                         on new { Vendedor = v, Mes = m }
                                         equals new { ve.Vendedor, ve.Mes }
                                         into gj
                                     from ve in gj.DefaultIfEmpty()
                                     orderby v, m
                                     select new
                                     {
                                         Vendedor = v,
                                         Mes = m,
                                         NombreMes = FormatearMes(m),
                                         TotalVentas = ve != null ? Math.Round(ve.TotalVentas, 2) : 0m
                                     }).ToList();

                    foreach (var r in resultado)
                    {
                        dt.Rows.Add(r.Vendedor, r.Mes, r.NombreMes, r.TotalVentas);
                    }
                }
                else
                {
                    // Caso: un año específico
                    var vendedores = context.Orders
                        .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == anio)
                        .Select(o => o.Employee.FirstName + " " + o.Employee.LastName)
                        .Distinct()
                        .ToList();

                    var ventas = context.Orders
                        .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == anio)
                        .SelectMany(o => o.Order_Details.Select(od => new
                        {
                            Vendedor = o.Employee.FirstName + " " + o.Employee.LastName,
                            Mes = o.OrderDate.Value.Month,
                            UnitPrice = od.UnitPrice,
                            Quantity = od.Quantity,
                            Discount = od.Discount
                        }))
                        .ToList()
                        .GroupBy(x => new { x.Vendedor, x.Mes })
                        .Select(g => new
                        {
                            Vendedor = g.Key.Vendedor,
                            Mes = g.Key.Mes,
                            TotalVentas = g.Sum(x => (decimal)x.UnitPrice * x.Quantity * (1 - (decimal)x.Discount))
                        })
                        .ToList();

                    var resultado = (from v in vendedores
                                     from m in meses
                                     join ve in ventas
                                         on new { Vendedor = v, Mes = m }
                                         equals new { ve.Vendedor, ve.Mes }
                                         into gj
                                     from ve in gj.DefaultIfEmpty()
                                     orderby v, m
                                     select new
                                     {
                                         Vendedor = v,
                                         Mes = m,
                                         NombreMes = FormatearMes(m),
                                         TotalVentas = ve != null ? Math.Round(ve.TotalVentas, 2) : 0m
                                     }).ToList();

                    foreach (var r in resultado)
                    {
                        dt.Rows.Add(r.Vendedor, r.Mes, r.NombreMes, r.TotalVentas);
                    }
                }
            }

            return dt;
        }

        public static DataTable ObtenerTopProductosRpt(int cantidad, int anio)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Posicion", typeof(int));
            dt.Columns.Add("NombreProducto", typeof(string));
            dt.Columns.Add("CantidadVendida", typeof(int));

            using (var ctx = new NorthwindContext())
            {
                var query = ctx.Order_Details
                    .Where(od => anio == -1 ||
                                 (od.Order.OrderDate.HasValue && od.Order.OrderDate.Value.Year == anio))
                    .GroupBy(od => od.Product.ProductName)
                    .Select(g => new
                    {
                        ProductName = g.Key,
                        CantidadVendida = g.Sum(x => x.Quantity)
                    })
                    .OrderByDescending(x => x.CantidadVendida)
                    .ThenBy(x => x.ProductName)
                    .Take(cantidad)
                    .ToList();

                int pos = 1;
                foreach (var item in query)
                {
                    DataRow row = dt.NewRow();
                    row["Posicion"] = pos;
                    row["NombreProducto"] = $"{pos}.- {item.ProductName}";
                    row["CantidadVendida"] = item.CantidadVendida;
                    dt.Rows.Add(row);
                    pos++;
                }
            }

            return dt;
        }

        // Helper para formatear el nombre del mes
        private static string FormatearMes(int mes)
        {
            var nombreMes = new DateTime(1900, mes, 1)
                .ToString("MMM", new System.Globalization.CultureInfo("es-ES"));
            return char.ToUpper(nombreMes[0]) + nombreMes.Substring(1).ToLower() + ".";
        }

        //El nombre del mes se calcula con un año fijo (1900) solo para obtener la abreviatura, usando el helper FormatearMes
        private static string FormatearMes(int mes, int year)
        {
            var nombreMes = new DateTime(year > 0 ? year : 1900, mes, 1)
                .ToString("MMM", new System.Globalization.CultureInfo("es-ES"));

            // Primera letra mayúscula, resto minúsculas, más el punto
            return char.ToUpper(nombreMes[0]) + nombreMes.Substring(1).ToLower() + ".";
        }

    }
}
