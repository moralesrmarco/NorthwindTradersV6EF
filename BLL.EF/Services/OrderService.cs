using DAL.EF;
using DTOs.EF;
using System;
using System.Data;
using System.Data.Entity; // Agregar esta directiva para habilitar Include
using System.Linq;

namespace BLL.EF.Services
{
    public class OrderService
    {
        public static DtoEnvioInformacion ObtenerUltimaInformacionDeEnvio(string clienteId) 
        {
			try
			{
				using (var context = new NorthwindContext())
				{
					return context.Orders
						.Where(o => o.CustomerID == clienteId)
						.OrderByDescending(o => o.OrderID)
						.Select(o => new DtoEnvioInformacion
                        {
                            ShipName = o.ShipName,
                            ShipAddress = o.ShipAddress,
                            ShipCity = o.ShipCity,
                            ShipRegion = o.ShipRegion,
                            ShipPostalCode = o.ShipPostalCode,
                            ShipCountry = o.ShipCountry
                        })
                        .FirstOrDefault();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error al obtener la última informacion del envío: " + ex.Message);
			}
        }

        public static Order ObtenerVentaPorId(int orderId)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var order = context.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.Employee)
                        .Include(o => o.Shipper)
                        .SingleOrDefault(o => o.OrderID == orderId);

                    if (order == null)
                        return null;

                    return new Order
                    {
                        OrderID = order.OrderID,
                        Customer = new Customer
                        {
                            CustomerID = order.Customer.CustomerID,
                            CompanyName = order.Customer.CompanyName,
                            ContactName = order.Customer.ContactName
                        },
                        Employee = new Employee
                        {
                            EmployeeID = order.Employee.EmployeeID,
                            FirstName = order.Employee.FirstName,
                            LastName = order.Employee.LastName
                        },
                        OrderDate = order.OrderDate,
                        RequiredDate = order.RequiredDate,
                        ShippedDate = order.ShippedDate,
                        Shipper = new Shipper
                        {
                            ShipperID = order.Shipper.ShipperID,
                            CompanyName = order.Shipper.CompanyName
                        },
                        Freight = order.Freight,
                        ShipName = order.ShipName,
                        ShipAddress = order.ShipAddress,
                        ShipCity = order.ShipCity,
                        ShipRegion = order.ShipRegion,
                        ShipPostalCode = order.ShipPostalCode,
                        ShipCountry = order.ShipCountry,
                        RowVersion = order.RowVersion

                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la venta por ID: " + ex.Message);
            }
        }

        public static DataTable ObtenerVentaPorIdDt(int orderId)
        {
            Order venta = ObtenerVentaPorId(orderId);
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Cliente", typeof(string));
            dt.Columns.Add("Vendedor", typeof(string));
            dt.Columns.Add("FechaDePedido", typeof(DateTime));
            dt.Columns.Add("FechaRequerido", typeof(DateTime));
            dt.Columns.Add("FechaDeEnvio", typeof(DateTime));
            dt.Columns.Add("CompaniaTransportista", typeof(string));
            dt.Columns.Add("DirigidoA", typeof(string));
            dt.Columns.Add("Domicilio", typeof(string));
            dt.Columns.Add("Ciudad", typeof(string));
            dt.Columns.Add("Region", typeof(string));
            dt.Columns.Add("CodigoPostal", typeof(string));
            dt.Columns.Add("Pais", typeof(string));
            dt.Columns.Add("Flete", typeof(decimal));
            DataRow dr = dt.NewRow();
            dr["Id"] = venta.OrderID;
            dr["Cliente"] = venta.Customer.CompanyName;
            dr["Vendedor"] = venta.Employee.NameByLastName;
            dr["FechaDePedido"] = venta.OrderDate ?? (object)DBNull.Value;
            dr["FechaRequerido"] = venta.RequiredDate ?? (object)DBNull.Value;
            dr["FechaDeEnvio"] = venta.ShippedDate ?? (object)DBNull.Value;
            dr["CompaniaTransportista"] = venta.Shipper.CompanyName;
            dr["DirigidoA"] = venta.ShipName;
            dr["Domicilio"] = venta.ShipAddress;
            dr["Ciudad"] = venta.ShipCity;
            dr["Region"] = venta.ShipRegion;
            dr["CodigoPostal"] = venta.ShipPostalCode;
            dr["Pais"] = venta.ShipCountry;
            dr["Flete"] = venta.Freight;
            dt.Rows.Add(dr);
            return dt;
        }

        public static DataTable ObtenerVentasPorFechaVenta(DateTime? fechaVentaIni, DateTime? fechaVentaFin)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var query = context.Orders
                        .Include(o => o.Customer)
                        .AsQueryable();
                    // Caso 1: ambas fechas nulas → registros sin fecha
                    if (!fechaVentaIni.HasValue && !fechaVentaFin.HasValue)
                    {
                        query = query.Where(o => o.OrderDate == null);
                    }
                    // Caso 2: rango de fechas
                    else if (fechaVentaIni.HasValue && fechaVentaFin.HasValue)
                    {
                        query = query.Where(o => o.OrderDate >= fechaVentaIni.Value &&
                                                    o.OrderDate < fechaVentaFin.Value);
                    }
                    var ventas = query
                                .OrderByDescending(o => o.OrderDate)
                                .ThenBy(o => o.Customer.CompanyName)
                                .Select(o => new
                                {
                                    o.OrderDate,
                                    o.RequiredDate,
                                    o.ShippedDate,
                                    CompanyName = o.Customer.CompanyName,
                                    o.OrderID,
                                    o.Freight
                                })
                                .AsNoTracking()
                                .ToList();
                    // Crear DataTable con las columnas del SP
                    DataTable dt = new DataTable();
                    dt.Columns.Add("OrderDate", typeof(DateTime));
                    dt.Columns.Add("RequiredDate", typeof(DateTime));
                    dt.Columns.Add("ShippedDate", typeof(DateTime));
                    dt.Columns.Add("CompanyName", typeof(string));
                    dt.Columns.Add("OrderID", typeof(int));
                    dt.Columns.Add("Freight", typeof(decimal));
                    // Poblar filas
                    foreach (var venta in ventas)
                    {
                        DataRow dr = dt.NewRow();
                        dr["OrderDate"] = venta.OrderDate ?? (object)DBNull.Value;
                        dr["RequiredDate"] = venta.RequiredDate ?? (object)DBNull.Value;
                        dr["ShippedDate"] = venta.ShippedDate ?? (object)DBNull.Value;
                        dr["CompanyName"] = venta.CompanyName;
                        dr["OrderID"] = venta.OrderID;
                        dr["Freight"] = venta.Freight ?? (object)DBNull.Value;
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ventas por fecha de venta.", ex);
            }
        }
    }
}