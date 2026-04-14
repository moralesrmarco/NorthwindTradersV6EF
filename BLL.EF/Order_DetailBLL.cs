using DAL.EF;
using System;
using System.Data;
using System.Data.SqlClient;

namespace BLL.EF
{
    public class Order_DetailBLL
    {
        public static int Insertar(Order_Detail ventaDetalle)
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
                            cmd.CommandText = "SpVentaDetalleInsertar";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("@OrderID", ventaDetalle.Order.OrderID));
                            cmd.Parameters.Add(new SqlParameter("@ProductID", ventaDetalle.Product.ProductID));
                            cmd.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.Decimal) { Value = ventaDetalle.UnitPrice });
                            cmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int) { Value = ventaDetalle.Quantity });
                            cmd.Parameters.Add(new SqlParameter("@Discount", SqlDbType.Real) { Value = ventaDetalle.Discount });
                            cmd.Parameters.Add(new SqlParameter("@TasaIVA", SqlDbType.Decimal) { Value = ventaDetalle.TasaIVA });
                            // Parámetro RowVersion (input/output)
                            var pRowVersion = new SqlParameter("@VentaRowVersion", SqlDbType.Binary, 8)
                            {
                                Direction = ParameterDirection.InputOutput,
                                Value = ventaDetalle.Order.RowVersion ?? new byte[8]
                            };
                            cmd.Parameters.Add(pRowVersion);
                            // Parámetro de retorno
                            var pReturn = new SqlParameter("@ReturnVal", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.ReturnValue
                            };
                            cmd.Parameters.Add(pReturn);

                            cmd.ExecuteNonQuery();

                            // Actualizar RowVersion en la entidad Venta
                            ventaDetalle.Order.RowVersion = (byte[])pRowVersion.Value;

                            // Devolver el código de retorno del SP
                            return (int)pReturn.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el detalle de la venta: " + ex.Message, ex);
            }
        }

        public static int Actualizar(Order_Detail ventaDetalle)
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
                            cmd.CommandText = "SpVentaDetalleActualizar";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@OrderID", ventaDetalle.Order.OrderID));
                            cmd.Parameters.Add(new SqlParameter("@ProductID", ventaDetalle.Product.ProductID));
                            cmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int) { Value = ventaDetalle.Quantity });
                            cmd.Parameters.Add(new SqlParameter("@Discount", SqlDbType.Real) { Value = ventaDetalle.Discount });
                            cmd.Parameters.Add(new SqlParameter("@VentaDetalleRowVersion", SqlDbType.Binary, 8) { Value = ventaDetalle.RowVersion ?? new byte[8] });
                            cmd.Parameters.Add(new SqlParameter("@VentaRowVersion", SqlDbType.Binary, 8) { Value = ventaDetalle.Order.RowVersion ?? new byte[8] });
                            // Parámetro de retorno
                            var pReturn = new SqlParameter("@ReturnVal", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.ReturnValue
                            };
                            cmd.Parameters.Add(pReturn);
                            cmd.ExecuteNonQuery();
                            // Devolver el código de retorno del SP
                            return (int)pReturn.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el detalle de la venta: " + ex.Message, ex);
            }
        }

        public static int Eliminar(Order_Detail ventaDetalle)
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
                            cmd.CommandText = "SpVentaDetalleEliminar";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@OrderID", ventaDetalle.Order.OrderID));
                            cmd.Parameters.Add(new SqlParameter("@ProductID", ventaDetalle.Product.ProductID));
                            cmd.Parameters.Add(new SqlParameter("@VentaDetalleRowVersion", SqlDbType.Binary, 8) { Value = ventaDetalle.RowVersion ?? new byte[8] });
                            cmd.Parameters.Add(new SqlParameter("@VentaRowVersion", SqlDbType.Binary, 8) { Value = ventaDetalle.Order.RowVersion ?? new byte[8] });
                            // Parámetro de retorno
                            var pReturn = new SqlParameter("@ReturnVal", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.ReturnValue
                            };
                            cmd.Parameters.Add(pReturn);
                            cmd.ExecuteNonQuery();
                            // Devolver el código de retorno del SP
                            return (int)pReturn.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el detalle de la venta: " + ex.Message, ex);
            }
        }
    }
}
