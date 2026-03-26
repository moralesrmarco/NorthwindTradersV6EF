using DAL;
using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BLL
{
    public class VentaBLL
    {
        private readonly VentaDAL _ventaDAL;

        public VentaBLL(string _connectionString)
        {
            _ventaDAL = new VentaDAL(_connectionString);
        }

        public int InsertarVentaCompleta(Venta venta, out int orderId, out byte[] rowVersion)
        {
            return _ventaDAL.InsertarVentaCompleta(venta, out orderId, out rowVersion);
        }

        public int Actualizar(Venta venta)
        {
            return _ventaDAL.Actualizar(venta);
        }

        public int Eliminar(Venta venta, out string productoExcede)
        {
            return _ventaDAL.Eliminar(venta, out productoExcede);
        }

        public List<DtoVentaDgv> ObtenerVentas(bool selectorRealizaBusqueda, DtoVentasBuscar criterios, bool top100 = false)
        {
            var ventasTemp = _ventaDAL.ObtenerVentas(selectorRealizaBusqueda, criterios, top100);
            // RowVersion convertido a string es para evitar problemas en el DataGridView
            var ventas = ventasTemp.Select(v => new DtoVentaDgv
            {
                OrderID = v.OrderID,
                CustomerCompanyName = v.Cliente.CompanyName,
                CustomerContactName = v.Cliente.ContactName,
                OrderDate = v.OrderDate,
                RequiredDate = v.RequiredDate,
                ShippedDate = v.ShippedDate,
                EmployeeName = v.Empleado.NameByLastName,
                ShipperCompanyName = v.Transportista.CompanyName,
                ShipName = v.ShipName,
                RowVersionStr = v.RowVersionString
            }).ToList();
            return ventas;
        }

        public List<DtoVentaRpt> ObtenerVentasRpt(bool selectorRealizaBusqueda, DtoVentasBuscar criterios)
        {
            List<Venta> ventas = _ventaDAL.ObtenerVentas(selectorRealizaBusqueda, criterios);
            var ventasRpt = ventas.Select(v => new DtoVentaRpt
            {
                OrderID = v.OrderID,
                Cliente = v.Cliente.CompanyName,
                Vendedor = v.Empleado.NameByLastName,
                FechaDePedido = v.OrderDate,
                FechaRequerido = v.RequiredDate,
                FechaDeEnvio = v.ShippedDate,
                CompaniaTransportista = v.Transportista.CompanyName,
                DirigidoA = v.ShipName,
                Domicilio = v.ShipAddress,
                Ciudad = v.ShipCity,
                Region = v.ShipRegion,
                CodigoPostal = v.ShipPostalCode,
                Pais = v.ShipCountry,
                Flete = v.Freight ?? 0m
            }).ToList();
            return ventasRpt;
        }

        public Venta ObtenerVentaPorId(int orderId)
        {
            return _ventaDAL.ObtenerVentaPorId(orderId);
        }

        public DataTable ObtenerVentaPorIdDt(int orderId)
        {
            Venta venta = _ventaDAL.ObtenerVentaPorId(orderId);
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
            dr["Cliente"] = venta.Cliente.CompanyName;
            dr["Vendedor"] = venta.Empleado.NameByLastName;
            dr["FechaDePedido"] = venta.OrderDate ?? (object)DBNull.Value;
            dr["FechaRequerido"] = venta.RequiredDate ?? (object)DBNull.Value;
            dr["FechaDeEnvio"] = venta.ShippedDate ?? (object)DBNull.Value;
            dr["CompaniaTransportista"] = venta.Transportista.CompanyName;
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

        public DataTable ObtenerVentasPorFechaVenta(DateTime? fechaVentaIni, DateTime? fechaVentaFin)
        {
            return _ventaDAL.ObtenerVentasPorFechaVenta(fechaVentaIni, fechaVentaFin);
        }
    }
}
