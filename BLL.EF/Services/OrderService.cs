using DAL.EF;
using DTOs.EF;
using System;
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
    }
}
