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
        public static int InsertarVentaCompleta(Order venta)
        {
            try
            {
                int filasAfectadas = 0;
                using (var context = new NorthwindContext())
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        // 1) Insertar registro padre (Order)
                        var order = new Order()
                        {
                            CustomerID = venta.Customer.CustomerID,
                            EmployeeID = venta.Employee.EmployeeID,
                            OrderDate = venta.OrderDate,
                            RequiredDate = venta.RequiredDate,
                            ShippedDate = venta.ShippedDate,
                            ShipVia = venta.ShipVia,
                            Freight = venta.Freight,
                            ShipName = venta.ShipName,
                            ShipAddress = venta.ShipAddress,
                            ShipCity = venta.ShipCity,
                            ShipRegion = venta.ShipRegion,
                            ShipPostalCode = venta.ShipPostalCode,
                            ShipCountry = venta.ShipCountry
                        };
                        context.Orders.Add(order);
                        context.SaveChanges(); // Guarda para obtener el OrderID generado
                        filasAfectadas++;

                        // 2) Procesar cada detalle
                        foreach (var d in venta.Order_Details)
                        {
                            // 2.1) Validar existencia y stock
                            var producto = context.Products
                                .SingleOrDefault(p => p.ProductID == d.Product.ProductID);
                            if (producto == null)
                                throw new InvalidOperationException($"Producto con ID {d.Product.ProductID} no existe.");
                            if (producto.UnitsInStock < d.Quantity)
                                throw new InvalidOperationException($"Producto 'Id:{producto.ProductID}, {producto.ProductName}' no tiene suficiente inventario. Disponible: {producto.UnitsInStock}, Requerido: {d.Quantity}.");
                            // 2.2) Actualizar stock
                            producto.UnitsInStock -= d.Quantity;
                            context.Entry(producto).State = EntityState.Modified;
                            // 2.3) Insertar detalle
                            var detalle = new Order_Detail()
                            {
                                OrderID = order.OrderID, // Asocia el detalle con el OrderID generado
                                ProductID = producto.ProductID,
                                UnitPrice = d.UnitPrice,
                                Quantity = d.Quantity,
                                Discount = d.Discount
                            };
                            context.Order_Details.Add(detalle);
                            filasAfectadas++;
                        }
                        // Guardar cambios de detalles y stock
                        context.SaveChanges();
                        tx.Commit();
                    }
                    catch 
                    {
                        try { tx.Rollback(); } catch { }
                        throw;
                    }
                }
                return filasAfectadas;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar la venta completa: " + ex.Message, ex);
            }
        }

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
