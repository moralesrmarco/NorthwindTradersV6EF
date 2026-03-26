using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class VentaDetalleDAL
    {
        private readonly string _connectionString;

        public VentaDetalleDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public int Insertar(VentaDetalle ventaDetalle)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentaDetalleInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", ventaDetalle.Venta.OrderID);
                    cmd.Parameters.AddWithValue("@ProductID", ventaDetalle.Producto.ProductID);
                    cmd.Parameters.AddWithValue("@UnitPrice", ventaDetalle.UnitPrice);
                    cmd.Parameters.AddWithValue("@Quantity", ventaDetalle.Quantity);
                    cmd.Parameters.AddWithValue("@Discount", ventaDetalle.Discount);
                    cmd.Parameters.AddWithValue("@TasaIVA", ventaDetalle.TasaIVA);
                    var paramRowVersion = new SqlParameter("@VentaRowVersion", SqlDbType.Binary, 8)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = ventaDetalle.Venta.RowVersion
                    };
                    cmd.Parameters.Add(paramRowVersion);
                    con.Open();
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    // Obtener el nuevo RowVersion
                    ventaDetalle.Venta.RowVersion = (byte[])paramRowVersion.Value;
                    return (int)returnParameter.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el detalle de la venta: " + ex.Message, ex);
            }
        }

        public int Eliminar(VentaDetalle ventaDetalle)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentaDetalleEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", ventaDetalle.Venta.OrderID);
                    cmd.Parameters.AddWithValue("@ProductID", ventaDetalle.Producto.ProductID);
                    cmd.Parameters.AddWithValue("@VentaDetalleRowVersion", ventaDetalle.RowVersion);
                    cmd.Parameters.AddWithValue("@VentaRowVersion", ventaDetalle.Venta.RowVersion);
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return (int)returnParameter.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el detalle de la venta: " + ex.Message, ex);
            }
        }

        public int Actualizar(VentaDetalle ventaDetalle)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentaDetalleActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", ventaDetalle.Venta.OrderID);
                    cmd.Parameters.AddWithValue("@ProductID", ventaDetalle.Producto.ProductID);
                    cmd.Parameters.AddWithValue("@Quantity", ventaDetalle.Quantity);
                    cmd.Parameters.AddWithValue("@Discount", ventaDetalle.Discount);
                    cmd.Parameters.AddWithValue("@VentaDetalleRowVersion", ventaDetalle.RowVersion);
                    cmd.Parameters.AddWithValue("@VentaRowVersion", ventaDetalle.Venta.RowVersion);
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return (int)returnParameter.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el detalle de la venta: " + ex.Message, ex);
            }
        }

        public List<VentaDetalle> ObtenerVentaDetallePorVentaId(int orderId)
        {
            List<VentaDetalle> ventaDetalles = new List<VentaDetalle>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentaDetalleObtenerPorVentaId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (rdr.Read())
                        {
                            var ventaDetalle = new VentaDetalle
                            {
                                Venta = new Venta
                                {
                                    OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID"))
                                },
                                Producto = new Producto
                                {
                                    ProductID = rdr.GetInt32(rdr.GetOrdinal("ProductID")),
                                    ProductName = rdr.IsDBNull(rdr.GetOrdinal("ProductName")) ? null : rdr.GetString(rdr.GetOrdinal("ProductName"))
                                },
                                UnitPrice = rdr.GetDecimal(rdr.GetOrdinal("UnitPrice")),
                                Quantity = rdr.GetInt16(rdr.GetOrdinal("Quantity")),
                                Discount = (decimal)rdr.GetFloat(rdr.GetOrdinal("Discount")),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion")) ? null : (byte[])rdr["RowVersion"]
                            };
                            ventaDetalles.Add(ventaDetalle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los detalles de la venta: " + ex.Message);
            }
            return ventaDetalles;
        }

        public byte[] ObtenerVentaDetalleRowVersion(int orderId, int productId)
        {
            if (orderId <= 0 || productId <= 0) return null;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentaDetalleObtenerRowVersion", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    con.Open();
                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        return null; 

                    return (byte[])result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el RowVersion del detalle de la venta: " + ex.Message, ex);
            }
        }

        public short ObtenerUInventario(int productId)
        {
            if (productId <= 0) return 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProductoObtenerUInventario", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                        return 0;
                    else
                        return Convert.ToInt16(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las unidades en inventario del producto: " + ex.Message, ex);
            }
        }
    }
}
