using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Habilita .Include con expresiones
using System.Data.SqlClient;
using System.Linq;

namespace BLL.EF
{
    public class OrderBLL
    {
        public static int InsertarVentaCompleta(Order venta)
        {
            if (venta == null) throw new ArgumentNullException(nameof(venta));

            if (venta.Order_Details == null || venta.Order_Details.Count == 0)
                throw new InvalidOperationException("La venta debe contener al menos un detalle.");

            using (var context = new NorthwindContext())
            using (var tx = context.Database.BeginTransaction())
            {
                try
                {
                    // ===============================
                    // 🔥 COPIA PLANA DE DETALLES
                    // ===============================
                    var detalles = venta.Order_Details
                        .Select(d => new
                        {
                            d.ProductID,
                            d.UnitPrice,
                            d.Quantity,
                            d.Discount,
                            d.TasaIVA
                        })
                        .ToList();

                    // ===============================
                    // 🔥 ROMPER NAVEGACIONES
                    // ===============================
                    venta.Order_Details.Clear();
                    venta.Customer = null;
                    venta.Employee = null;
                    venta.Shipper = null;

                    // ===============================
                    // 🔑 VALIDACIONES
                    // ===============================
                    if (string.IsNullOrWhiteSpace(venta.CustomerID))
                        throw new InvalidOperationException("Cliente inválido.");

                    if (!venta.EmployeeID.HasValue || venta.EmployeeID == 0)
                        throw new InvalidOperationException("Empleado inválido.");

                    if (!venta.ShipVia.HasValue || venta.ShipVia == 0)
                        throw new InvalidOperationException("Transportista inválido.");

                    // ===============================
                    // 🧾 CREAR ORDER
                    // ===============================
                    var order = new Order
                    {
                        CustomerID = venta.CustomerID,
                        EmployeeID = venta.EmployeeID,
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
                    context.SaveChanges(); // 🔑 genera OrderID

                    int filasAfectadas = 1;

                    // ===============================
                    // 📦 PROCESAR DETALLES (SIN BLOQUEO)
                    // ===============================
                    foreach (var d in detalles)
                    {
                        if (d.ProductID <= 0)
                            throw new InvalidOperationException("Producto inválido.");

                        if (d.Quantity <= 0)
                            throw new InvalidOperationException($"Cantidad inválida para el producto {d.ProductID}.");

                        // 🔥 UPDATE ATÓMICO (CLAVE)
                        //🔥 FRASE CLAVE
                        //EF es para CRUD
                        //SQL es para lógica crítica(como inventario)
                        int rows = context.Database.ExecuteSqlCommand(
                            @"UPDATE Products
                                  SET UnitsInStock = UnitsInStock - @p0
                                  WHERE ProductID = @p1
                                  AND UnitsInStock >= @p0",
                            d.Quantity,
                            d.ProductID
                        );

                        // ❌ SI NO SE ACTUALIZÓ → NO HAY STOCK O NO EXISTE
                        if (rows == 0)
                        {
                            bool existe = context.Products.Any(p => p.ProductID == d.ProductID);

                            if (!existe)
                                throw new InvalidOperationException($"Producto con ID {d.ProductID} no existe.");

                            throw new InvalidOperationException(
                                $"Inventario insuficiente para el producto {d.ProductID}."
                            );
                        }

                        // 🧾 INSERTAR DETALLE
                        var detalle = new Order_Detail
                        {
                            OrderID = order.OrderID,
                            ProductID = d.ProductID,
                            UnitPrice = d.UnitPrice,
                            Quantity = d.Quantity,
                            Discount = d.Discount,
                            TasaIVA = d.TasaIVA,
                            Product = null,
                            Order = null
                        };

                        context.Order_Details.Add(detalle);
                        filasAfectadas++;
                    }

                    // ===============================
                    // 💾 GUARDAR DETALLES
                    // ===============================
                    context.SaveChanges();

                    tx.Commit();

                    // ===============================
                    // 🔁 DEVOLVER A UI
                    // ===============================
                    venta.OrderID = order.OrderID;
                    venta.RowVersion = order.RowVersion;

                    return filasAfectadas;
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }

                    Exception inner = ex;
                    while (inner.InnerException != null)
                        inner = inner.InnerException;

                    throw new Exception(inner.Message, ex);
                }
            }
        }

        public static int Actualizar(Order venta)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    using (var conn = context.Database.Connection)
                    {
                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SpVentaActualizar";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("@OrderID", venta.OrderID));
                            cmd.Parameters.Add(new SqlParameter("@CustomerID", venta.Customer.CustomerID));
                            cmd.Parameters.Add(new SqlParameter("@EmployeeID", venta.Employee.EmployeeID));
                            cmd.Parameters.Add(new SqlParameter("@OrderDate", (object)venta.OrderDate ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@RequiredDate", (object)venta.RequiredDate ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@ShippedDate", (object)venta.ShippedDate ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@ShipVia", (object)venta.Shipper.ShipperID ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@Freight", (object)venta.Freight ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@ShipName", (object)venta.ShipName ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@ShipAddress", (object)venta.ShipAddress ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@ShipCity", (object)venta.ShipCity ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@ShipRegion", (object)venta.ShipRegion ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@ShipPostalCode", (object)venta.ShipPostalCode ?? DBNull.Value));
                            cmd.Parameters.Add(new SqlParameter("@ShipCountry", (object)venta.ShipCountry ?? DBNull.Value));

                            var pRowVersion = new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                            {
                                Direction = ParameterDirection.InputOutput,
                                Value = (object)venta.RowVersion ?? DBNull.Value
                            };
                            cmd.Parameters.Add(pRowVersion);

                            var pReturn = new SqlParameter
                            {
                                ParameterName = "@RETURN_VALUE",
                                SqlDbType = SqlDbType.Int,
                                Direction = ParameterDirection.ReturnValue
                            };
                            cmd.Parameters.Add(pReturn);

                            cmd.ExecuteNonQuery();

                            int returnCode = (int)pReturn.Value;
                            if (returnCode == 1)
                            {
                                venta.RowVersion = (byte[])pRowVersion.Value;
                            }
                            return returnCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la venta: " + ex.Message);
            }
        }

        public static int Eliminar(Order venta, out string productoExcede)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    using (var conn = context.Database.Connection)
                    {
                        if (conn.State != ConnectionState.Open)
                            conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SpVentaEliminar";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@OrderID", venta.OrderID));
                            var pProductoExcede = new SqlParameter
                            {
                                ParameterName = "@ProductoExcede",
                                SqlDbType = SqlDbType.NVarChar,
                                Size = 200,
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(pProductoExcede);
                            var pRowVersion = new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                            {
                                Direction = ParameterDirection.Input,
                                Value = (object)venta.RowVersion ?? DBNull.Value
                            };
                            cmd.Parameters.Add(pRowVersion);
                            var pReturn = new SqlParameter
                            {
                                ParameterName = "@RETURN_VALUE",
                                SqlDbType = SqlDbType.Int,
                                Direction = ParameterDirection.ReturnValue
                            };
                            cmd.Parameters.Add(pReturn);

                            cmd.ExecuteNonQuery();
                            productoExcede = pProductoExcede.Value as string;
                            int returnCode = (int)pReturn.Value;
                            return returnCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la venta: " + ex.Message);
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
