using DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; // Agregar este using para habilitar Include con expresiones lambda

namespace BLL.EF.Services
{
    public class Order_DetailService
    {
        public static List<Order_Detail> ObtenerVentaDetallePorVentaId(int orderId)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var detalles = context.Order_Details
                        .Include(od => od.Product)
                        .Include(od => od.Order)
                        .Where(od => od.OrderID == orderId)
                        .ToList();
                    return detalles;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los detalles de la venta: " + ex.Message);
            }
        }
    }
}
