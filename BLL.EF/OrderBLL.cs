using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity; // Habilita .Include con expresiones
using System.Linq;

namespace BLL.EF
{
    public class OrderBLL
    {
        public static List<DtoVentaDgv> ObtenerVentas(bool selectorRealizaBusqueda, DtoVentasBuscar criterios, bool top100 = false)
        {
            // Validación defensiva del criterio para evitar NRE
            criterios = criterios ?? new DtoVentasBuscar();

            using (var context = new NorthwindContext())
            {
                // Lectura en modo sin seguimiento para consultas de solo lectura (mejora de rendimiento)
                IQueryable<Order> query = context.Orders
                    .AsNoTracking()
                    .Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .Include(o => o.Shipper);

                if (selectorRealizaBusqueda)
                {
                    if (criterios.IdIni > 0)
                        query = query.Where(o => o.OrderID >= criterios.IdIni && o.OrderID <= criterios.IdFin);

                    if (!string.IsNullOrWhiteSpace(criterios.Cliente))
                        query = query.Where(o => o.Customer != null && o.Customer.CompanyName.Contains(criterios.Cliente));

                    if (criterios.FVenta)
                        query = query.Where(o => o.OrderDate >= criterios.FVentaIni && o.OrderDate <= criterios.FVentaFin);
                    if (criterios.FVentaNull)
                        query = query.Where(o => o.OrderDate == null);

                    if (criterios.FRequerido)
                        query = query.Where(o => o.RequiredDate >= criterios.FRequeridoIni && o.RequiredDate <= criterios.FRequeridoFin);
                    if (criterios.FRequeridoNull)
                        query = query.Where(o => o.RequiredDate == null);

                    if (criterios.FEnvio)
                        query = query.Where(o => o.ShippedDate >= criterios.FEnvioIni && o.ShippedDate <= criterios.FEnvioFin);
                    if (criterios.FEnvioNull)
                        query = query.Where(o => o.ShippedDate == null);

                    if (!string.IsNullOrWhiteSpace(criterios.Empleado))
                        query = query.Where(o => o.Employee != null && (o.Employee.LastName + " " + o.Employee.FirstName).Contains(criterios.Empleado));

                    if (!string.IsNullOrWhiteSpace(criterios.CompañiaT))
                        query = query.Where(o => o.Shipper != null && o.Shipper.CompanyName.Contains(criterios.CompañiaT));

                    if (!string.IsNullOrWhiteSpace(criterios.DirigidoA))
                        query = query.Where(o => o.ShipName.Contains(criterios.DirigidoA));
                }
                else
                {
                    if (top100)
                        query = query.OrderByDescending(o => o.OrderID).Take(100);
                    else
                        query = query.OrderByDescending(o => o.OrderID).Take(20);
                }

                // Aseguramos orden consistente antes de materializar
                query = query.OrderByDescending(o => o.OrderID);

                // Materializamos la lista de Order para poder usar la propiedad de extensión RowVersionStr
                var orders = query.ToList();

                // Mapear a DTO usando la propiedad RowVersionStr del partial Order
                var ventas = orders
                    .Select(o => new DtoVentaDgv
                    {
                        OrderID = o.OrderID,
                        CustomerCompanyName = o.Customer?.CompanyName,
                        CustomerContactName = o.Customer?.ContactName,
                        OrderDate = o.OrderDate,
                        RequiredDate = o.RequiredDate,
                        ShippedDate = o.ShippedDate,
                        EmployeeName = o.Employee?.NameByLastName,
                        ShipperCompanyName = o.Shipper?.CompanyName,
                        ShipName = o.ShipName,
                        RowVersionStr = o.RowVersionStr
                    })
                    .ToList();

                return ventas;
            }
        }
    }
}
