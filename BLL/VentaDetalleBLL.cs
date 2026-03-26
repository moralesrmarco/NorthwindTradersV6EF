using DAL;
using Entities;
using System.Collections.Generic;
using System.Data;

namespace BLL
{
    public class VentaDetalleBLL
    {
        private readonly VentaDetalleDAL _ventaDetalleDAL;

        public VentaDetalleBLL(string _connectionString) 
        { 
            _ventaDetalleDAL = new VentaDetalleDAL(_connectionString);
        }

        public int Insertar(VentaDetalle ventaDetalle)
        {
            return _ventaDetalleDAL.Insertar(ventaDetalle);
        }

        public int Eliminar(VentaDetalle ventaDetalle)
        {
            return _ventaDetalleDAL.Eliminar(ventaDetalle);
        }

        public int Actualizar(VentaDetalle ventaDetalle)
        {
            return _ventaDetalleDAL.Actualizar(ventaDetalle);
        }

        public List<VentaDetalle> ObtenerVentaDetallePorVentaId(int orderId)
        {
            return _ventaDetalleDAL.ObtenerVentaDetallePorVentaId(orderId);
        }

        public DataTable ObtenerVentaDetallePorVentaIdDt(int orderId)
        {
            List<VentaDetalle> ventaDetalles = _ventaDetalleDAL.ObtenerVentaDetallePorVentaId(orderId);
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("UnitPrice", typeof(decimal));
            dt.Columns.Add("Quantity", typeof(short));
            dt.Columns.Add("Importe", typeof(decimal));
            dt.Columns.Add("Discount", typeof(decimal));
            dt.Columns.Add("ImporteDelDescuento", typeof(decimal));
            dt.Columns.Add("ImporteConDescuento", typeof(decimal));
            dt.Columns.Add("TasaIVA", typeof(decimal)); 
            dt.Columns.Add("ImporteDelIVA", typeof(decimal));
            dt.Columns.Add("Subtotal", typeof(decimal));
            foreach (var ventaDetalle in ventaDetalles)
            {
                DataRow dr = dt.NewRow();
                dr["ProductName"] = ventaDetalle.Producto.ProductName;
                dr["UnitPrice"] = ventaDetalle.UnitPrice;
                dr["Quantity"] = ventaDetalle.Quantity;
                dr["Importe"] = ventaDetalle.SubtotalDelImporteConIVAIncluido;
                dr["Discount"] = ventaDetalle.Discount;
                dr["ImporteDelDescuento"] = ventaDetalle.SubtotalDelAhorroTotalDespuesDescuento;
                dr["ImporteConDescuento"] = ventaDetalle.SubtotalDelImporteConIVAConDescuento;
                dr["TasaIVA"] = ventaDetalle.TasaIVA;
                dr["ImporteDelIVA"] = ventaDetalle.SubtotalIVADespuesDelDescuento;
                dr["Subtotal"] = ventaDetalle.Subtotal;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public byte[] ObtenerVentaDetalleRowVersion(int orderId, int productId)
        {
            return _ventaDetalleDAL.ObtenerVentaDetalleRowVersion(orderId, productId);
        }

        public short ObtenerUInventario(int productId)
        {
            return _ventaDetalleDAL.ObtenerUInventario(productId);
        }
    }
}
