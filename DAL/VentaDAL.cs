using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class VentaDAL
    {
        private readonly string _connectionString;

        public VentaDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int InsertarVentaCompleta(Venta venta, out int orderId, out byte[] rowVersion)
        {
            orderId = 0;
            rowVersion = null;
            int filasAfectadas = 0;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                {
                    cn.Open();
                    using (var tx = cn.BeginTransaction())
                    {
                        try
                        {
                            // 1) Insertar registro padre (SP)
                            using (var cmd = new SqlCommand("SpVentaInsertar", cn, tx))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                // Parámetro de retorno
                                var returnParam = cmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                                returnParam.Direction = ParameterDirection.ReturnValue;

                                // Parámetros de salida
                                cmd.Parameters.Add("@OrderID", SqlDbType.Int).Direction = ParameterDirection.Output;
                                // RowVersion
                                cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;
                                // Parámetros de entrada
                                cmd.Parameters.AddWithValue("@CustomerID", string.IsNullOrWhiteSpace(venta.Cliente.CustomerID) ? (object)DBNull.Value : venta.Cliente.CustomerID);
                                cmd.Parameters.AddWithValue("@EmployeeID", venta.Empleado.EmployeeID);
                                cmd.Parameters.AddWithValue("@OrderDate", venta.OrderDate.HasValue ? (object)venta.OrderDate.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@RequiredDate", venta.RequiredDate.HasValue ? (object)venta.RequiredDate.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@ShippedDate", venta.ShippedDate.HasValue ? (object)venta.ShippedDate.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@ShipVia", ((object)venta.Transportista.ShipperID == null || venta.Transportista.ShipperID.Equals(0)) ? DBNull.Value : (object)venta.Transportista.ShipperID);
                                cmd.Parameters.AddWithValue("@ShipName", string.IsNullOrWhiteSpace(venta.ShipName) ? (object)DBNull.Value : venta.ShipName);
                                cmd.Parameters.AddWithValue("@ShipAddress", string.IsNullOrWhiteSpace(venta.ShipAddress) ? (object)DBNull.Value : venta.ShipAddress);
                                cmd.Parameters.AddWithValue("@ShipCity", string.IsNullOrWhiteSpace(venta.ShipCity) ? (object)DBNull.Value : venta.ShipCity);
                                cmd.Parameters.AddWithValue("@ShipRegion", string.IsNullOrWhiteSpace(venta.ShipRegion) ? (object)DBNull.Value : venta.ShipRegion);
                                cmd.Parameters.AddWithValue("@ShipPostalCode", string.IsNullOrWhiteSpace(venta.ShipPostalCode) ? (object)DBNull.Value : venta.ShipPostalCode);
                                cmd.Parameters.AddWithValue("@ShipCountry", string.IsNullOrWhiteSpace(venta.ShipCountry) ? (object)DBNull.Value : venta.ShipCountry);
                                cmd.Parameters.AddWithValue("@Freight", (object)venta.Freight ?? DBNull.Value);

                                // Ejecutar y capturar código de retorno
                                cmd.ExecuteNonQuery();
                                // Capturar código de retorno
                                var returnValue = (int)returnParam.Value;
                                // filasAfectadas no depende de ExecuteNonQuery aquí,
                                // sino del código de retorno del SP
                                filasAfectadas = returnValue;
                                if (returnValue != 1)
                                {
                                    throw new InvalidOperationException("Error al insertar la venta. Código de error: " + returnValue);
                                }
                                // Capturar valores de salida
                                orderId = Convert.ToInt32(cmd.Parameters["@OrderID"].Value);
                                // Aquí obtienes el RowVersion como arreglo de bytes
                                rowVersion = (byte[])cmd.Parameters["@RowVersion"].Value;

                                // Si quieres guardarlo en tu objeto Venta:
                                venta.RowVersion = rowVersion;
                            }
                            // 2) Preparar comandos reutilizables para cada detalle:
                            // 2.1) Preparar SELECT UnitsInStock FOR UPDATE
                            using (var cmdCheckStock = new SqlCommand("SpProductoObtenerInventarioPorIdConBloqueo", cn, tx)) // el bloqueo persiste para las demas operaciones dentro de la transacción
                            {
                                cmdCheckStock.CommandType = CommandType.StoredProcedure;
                                cmdCheckStock.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int));

                                // 2.2) Preparar UPDATE products
                                using (var cmdUpdateStock = new SqlCommand("SpProductoActualizarInventarioPorId", cn, tx))
                                {
                                    cmdUpdateStock.CommandType = CommandType.StoredProcedure;
                                    cmdUpdateStock.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int));
                                    cmdUpdateStock.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int));

                                    // 2.3) Preparar inserción de detalle (SP)
                                    using (var cmdInsertDetail = new SqlCommand("SpVentaDetalleInsertarSinActualizarInventario", cn, tx))
                                    {
                                        cmdInsertDetail.CommandType = CommandType.StoredProcedure;
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.Decimal));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.SmallInt));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@Discount", SqlDbType.Float));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@TasaIVA", SqlDbType.Float));

                                        // 3) Procesar cada detalle
                                        foreach (var d in venta.VentaDetalles)
                                        {
                                            // 3.1) Validar existencia y bloquear fila del producto
                                            cmdCheckStock.Parameters["@ProductID"].Value = d.Producto.ProductID;
                                            var stockObj = cmdCheckStock.ExecuteScalar();
                                            if (stockObj == null || stockObj == DBNull.Value)
                                            {
                                                throw new InvalidOperationException($"Producto {d.Producto.ProductID} no existe.");
                                            }

                                            int currentStock = Convert.ToInt32(stockObj);

                                            // 3.2) Validar stock suficiente
                                            if (currentStock < d.Quantity)
                                            {
                                                throw new InvalidOperationException($"Inventario insuficiente para el producto {d.Producto.ProductID} {d.Producto.ProductName}. Disponible: {currentStock}, solicitado: {d.Quantity}.");
                                            }

                                            // 3.3) Actualizar stock
                                            cmdUpdateStock.Parameters["@Quantity"].Value = d.Quantity;
                                            cmdUpdateStock.Parameters["@ProductID"].Value = d.Producto.ProductID;
                                            var rowsUpdated = cmdUpdateStock.ExecuteNonQuery();
                                            if (rowsUpdated == 0)
                                            {
                                                throw new InvalidOperationException($"No se pudo actualizar el inventario para el producto {d.Producto.ProductID}.");
                                            }

                                            // 3.4) Insertar detalle (SP)
                                            cmdInsertDetail.Parameters["@OrderID"].Value = orderId;
                                            cmdInsertDetail.Parameters["@ProductID"].Value = d.Producto.ProductID;
                                            cmdInsertDetail.Parameters["@UnitPrice"].Value = d.UnitPrice;
                                            cmdInsertDetail.Parameters["@Quantity"].Value = d.Quantity;
                                            cmdInsertDetail.Parameters["@Discount"].Value = d.Discount;
                                            cmdInsertDetail.Parameters["@TasaIVA"].Value = d.TasaIVA;

                                            filasAfectadas += cmdInsertDetail.ExecuteNonQuery();
                                        } // foreach detalles
                                    } // cmdInsertDetail
                                } // cmdUpdateStock
                            } // cmdCheckStock
                            tx.Commit();
                        }
                        catch (Exception)
                        {
                            try { tx.Rollback(); } catch { }
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar la venta: " + ex.Message);
            }
            return filasAfectadas;
        }

        public int Actualizar(Venta venta)
        {
            int returnCode = 0;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentaActualizar", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    // Parámetro de retorno
                    var returnParam = cmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;
                    // Parámetros de entrada
                    cmd.Parameters.AddWithValue("@OrderID", venta.OrderID);
                    cmd.Parameters.AddWithValue("@CustomerID", string.IsNullOrWhiteSpace(venta.Cliente.CustomerID) ? (object)DBNull.Value : venta.Cliente.CustomerID);
                    cmd.Parameters.AddWithValue("@EmployeeID", ((object)venta.Empleado.EmployeeID == null || venta.Empleado.EmployeeID.Equals(0)) ? DBNull.Value : (object)venta.Empleado.EmployeeID);
                    cmd.Parameters.AddWithValue("@OrderDate", venta.OrderDate.HasValue ? (object)venta.OrderDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@RequiredDate", venta.RequiredDate.HasValue ? (object)venta.RequiredDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ShippedDate", venta.ShippedDate.HasValue ? (object)venta.ShippedDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ShipVia", ((object)venta.Transportista.ShipperID == null || venta.Transportista.ShipperID.Equals(0)) ? DBNull.Value : (object)venta.Transportista.ShipperID);
                    cmd.Parameters.AddWithValue("@ShipName", string.IsNullOrWhiteSpace(venta.ShipName) ? (object)DBNull.Value : venta.ShipName);
                    cmd.Parameters.AddWithValue("@ShipAddress", string.IsNullOrWhiteSpace(venta.ShipAddress) ? (object)DBNull.Value : venta.ShipAddress);
                    cmd.Parameters.AddWithValue("@ShipCity", string.IsNullOrWhiteSpace(venta.ShipCity) ? (object)DBNull.Value : venta.ShipCity);
                    cmd.Parameters.AddWithValue("@ShipRegion", string.IsNullOrWhiteSpace(venta.ShipRegion) ? (object)DBNull.Value : venta.ShipRegion);
                    cmd.Parameters.AddWithValue("@ShipPostalCode", string.IsNullOrWhiteSpace(venta.ShipPostalCode) ? (object)DBNull.Value : venta.ShipPostalCode);
                    cmd.Parameters.AddWithValue("@ShipCountry", string.IsNullOrWhiteSpace(venta.ShipCountry) ? (object)DBNull.Value : venta.ShipCountry);
                    cmd.Parameters.AddWithValue("@Freight", (object)venta.Freight ?? DBNull.Value);
                    // Parámetro de salida RowVersion
                    var pRowVersion = new SqlParameter("@RowVersion", SqlDbType.Binary, 8);
                    pRowVersion.Direction = ParameterDirection.InputOutput;
                    pRowVersion.Value = venta.RowVersion;
                    cmd.Parameters.Add(pRowVersion);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    // Capturar código de retorno
                    returnCode = (int)returnParam.Value;
                    if (returnCode == 1)
                    {
                        // Éxito: actualizar RowVersion en el objeto
                        venta.RowVersion = (byte[])cmd.Parameters["@RowVersion"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la venta: " + ex.Message);
            }
            return returnCode;
        }

        public int Eliminar(Venta venta, out string produtoExcede)
        {
            int filasAfectadas = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentaEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", venta.OrderID);
                    cmd.Parameters.AddWithValue("@RowVersion", venta.RowVersion);
                    var paramProductoExcede = cmd.Parameters.Add("@ProductoExcede", SqlDbType.VarChar, 40);
                    paramProductoExcede.Direction = ParameterDirection.Output;
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    paramProductoExcede.Value = paramProductoExcede.Value == DBNull.Value ? string.Empty : paramProductoExcede.Value;
                    filasAfectadas = (int)returnParameter.Value;
                    produtoExcede = paramProductoExcede.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la venta: " + ex.Message);
            }
            return filasAfectadas;
        }

        public List<Venta> ObtenerVentas(bool selectorRealizaBusqueda, DtoVentasBuscar criterios, bool top100 = false)
        {
            var ventas = new List<Venta>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (selectorRealizaBusqueda)
                    {
                        cmd.CommandText = "SpVentaBuscar";
                        // Numéricos
                        cmd.Parameters.Add("@IdIni", SqlDbType.Int).Value = criterios.IdIni;
                        cmd.Parameters.Add("@IdFin", SqlDbType.Int).Value = criterios.IdFin;

                        // Strings
                        cmd.Parameters.Add("@Cliente", SqlDbType.NVarChar, 40).Value = criterios.Cliente;
                        cmd.Parameters.Add("@Empleado", SqlDbType.NVarChar, 31).Value = criterios.Empleado;
                        cmd.Parameters.Add("@CompañiaT", SqlDbType.NVarChar, 40).Value = criterios.CompañiaT;
                        cmd.Parameters.Add("@Dirigidoa", SqlDbType.NVarChar, 40).Value = criterios.DirigidoA;
                        // Bits
                        cmd.Parameters.Add("@FVenta", SqlDbType.Bit).Value = criterios.FVenta;
                        cmd.Parameters.Add("@FVentaNull", SqlDbType.Bit).Value = criterios.FVentaNull;

                        cmd.Parameters.Add("@FRequerido", SqlDbType.Bit).Value = criterios.FRequerido;
                        cmd.Parameters.Add("@FRequeridoNull", SqlDbType.Bit).Value = criterios.FRequeridoNull;

                        cmd.Parameters.Add("@FEnvio", SqlDbType.Bit).Value = criterios.FEnvio;
                        cmd.Parameters.Add("@FEnvioNull", SqlDbType.Bit).Value = criterios.FEnvioNull;

                        // Fechas (si son null, se manda DBNull)
                        cmd.Parameters.Add("@FVentaIni", SqlDbType.DateTime).Value =
                            criterios.FVentaIni.HasValue ? (object)criterios.FVentaIni.Value : DBNull.Value;

                        cmd.Parameters.Add("@FVentaFin", SqlDbType.DateTime).Value =
                            criterios.FVentaFin.HasValue ? (object)criterios.FVentaFin.Value : DBNull.Value;

                        cmd.Parameters.Add("@FRequeridoIni", SqlDbType.DateTime).Value =
                            criterios.FRequeridoIni.HasValue ? (object)criterios.FRequeridoIni.Value : DBNull.Value;

                        cmd.Parameters.Add("@FRequeridoFin", SqlDbType.DateTime).Value =
                            criterios.FRequeridoFin.HasValue ? (object)criterios.FRequeridoFin.Value : DBNull.Value;

                        cmd.Parameters.Add("@FEnvioIni", SqlDbType.DateTime).Value =
                            criterios.FEnvioIni.HasValue ? (object)criterios.FEnvioIni.Value : DBNull.Value;

                        cmd.Parameters.Add("@FEnvioFin", SqlDbType.DateTime).Value =
                            criterios.FEnvioFin.HasValue ? (object)criterios.FEnvioFin.Value : DBNull.Value;
                    }
                    else
                    {
                        cmd.CommandText = "SpVentaObtener";
                        cmd.Parameters.AddWithValue("@Top100", top100);
                    }
                    con.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var venta = new Venta()
                            {
                                OrderID = rdr["OrderID"] as int? ?? 0,
                                Cliente = new Cliente
                                {
                                    CustomerID = rdr["CustomerID"] as string ?? string.Empty,
                                    CompanyName = rdr["CustomerCompanyName"] as string ?? string.Empty,
                                    ContactName = rdr["ContactName"] as String ?? string.Empty
                                },
                                Empleado = new Empleado
                                {
                                    EmployeeID = rdr["EmployeeID"] as int? ?? 0,
                                    FirstName = rdr["FirstName"] as String ?? string.Empty,
                                    LastName = rdr["LastName"] as String ?? string.Empty
                                },
                                OrderDate = rdr["OrderDate"] as DateTime?,
                                RequiredDate = rdr["RequiredDate"] as DateTime?,
                                ShippedDate = rdr["ShippedDate"] as DateTime?,
                                Transportista = new Transportista
                                {
                                    ShipperID = rdr["ShipperID"] as int? ?? 0,
                                    CompanyName = rdr["ShipperCompanyName"] as string ?? string.Empty
                                },
                                Freight = rdr["Freight"] as decimal? ?? 0.00m,
                                ShipName = rdr["ShipName"] as string ?? string.Empty,
                                ShipAddress = rdr["ShipAddress"] as string ?? string.Empty,
                                ShipCity = rdr["ShipCity"] as string ?? string.Empty,
                                ShipRegion = rdr["ShipRegion"] as string ?? string.Empty,
                                ShipPostalCode = rdr["ShipPostalCode"] as string ?? string.Empty,
                                ShipCountry = rdr["ShipCountry"] as string ?? String.Empty,
                                RowVersion = rdr["RowVersion"] as byte[]
                            };
                            ventas.Add(venta);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ventas;
        }

        public DtoEnvioInformacion ObtenerUltimaInformacionDeEnvio(string customerId)
        {
            DtoEnvioInformacion dtoEnvioInformacion = null;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentaObtenerUltimaInformacionDeEnvio", con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return null;
                    DataRow row = dt.Rows[0];
                    dtoEnvioInformacion = new DtoEnvioInformacion
                    {
                        ShipName = row["ShipName"] == DBNull.Value ? string.Empty : row["ShipName"].ToString(),
                        ShipAddress = row["ShipAddress"] == DBNull.Value ? string.Empty : row["ShipAddress"].ToString(),
                        ShipCity = row["ShipCity"] == DBNull.Value ? string.Empty : row["ShipCity"].ToString(),
                        ShipRegion = row["ShipRegion"] == DBNull.Value ? string.Empty : row["ShipRegion"].ToString(),
                        ShipPostalCode = row["ShipPostalCode"] == DBNull.Value ? string.Empty : row["ShipPostalCode"].ToString(),
                        ShipCountry = row["ShipCountry"] == DBNull.Value ? string.Empty : row["ShipCountry"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la información de envío: " + ex.Message);
            }
            return dtoEnvioInformacion;
        }

        public Venta ObtenerVentaPorId(int orderId)
        {
            Venta venta = null;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentaObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (rdr.Read())
                        {
                            venta = new Venta()
                            {
                                OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID")),
                                Cliente = new Cliente
                                {
                                    CustomerID = rdr.IsDBNull(rdr.GetOrdinal("CustomerID")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerID")),
                                    CompanyName = rdr.IsDBNull(rdr.GetOrdinal("CustomerCompanyName")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerCompanyName"))
                                },
                                Empleado = new Empleado
                                {
                                    EmployeeID = rdr.GetInt32(rdr.GetOrdinal("EmployeeID")),
                                    LastName = rdr["LastName"].ToString(),
                                    FirstName = rdr["FirstName"].ToString()
                                },
                                OrderDate = rdr.IsDBNull(rdr.GetOrdinal("OrderDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("OrderDate")),
                                RequiredDate = rdr.IsDBNull(rdr.GetOrdinal("RequiredDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("RequiredDate")),
                                ShippedDate = rdr.IsDBNull(rdr.GetOrdinal("ShippedDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("ShippedDate")),
                                Transportista = new Transportista
                                {
                                    ShipperID = rdr.GetInt32(rdr.GetOrdinal("ShipVia")),
                                    CompanyName = rdr["ShipperCompanyName"] == DBNull.Value
                                      ? string.Empty
                                      : rdr["ShipperCompanyName"].ToString()
                                },
                                Freight = rdr.IsDBNull(rdr.GetOrdinal("Freight")) ? (decimal?)null : rdr.GetDecimal(rdr.GetOrdinal("Freight")),
                                ShipName = rdr.IsDBNull(rdr.GetOrdinal("ShipName")) ? null : rdr.GetString(rdr.GetOrdinal("ShipName")),
                                ShipAddress = rdr.IsDBNull(rdr.GetOrdinal("ShipAddress")) ? null : rdr.GetString(rdr.GetOrdinal("ShipAddress")),
                                ShipCity = rdr.IsDBNull(rdr.GetOrdinal("ShipCity")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCity")),
                                ShipRegion = rdr.IsDBNull(rdr.GetOrdinal("ShipRegion")) ? null : rdr.GetString(rdr.GetOrdinal("ShipRegion")),
                                ShipPostalCode = rdr.IsDBNull(rdr.GetOrdinal("ShipPostalCode")) ? null : rdr.GetString(rdr.GetOrdinal("ShipPostalCode")),
                                ShipCountry = rdr.IsDBNull(rdr.GetOrdinal("ShipCountry")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCountry")),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion")) ? null : (byte[])rdr["RowVersion"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la venta por ID: " + ex.Message);
            }
            return venta;
        }

        public DataTable ObtenerVentasPorFechaVenta(DateTime? fechaVentaIni, DateTime? fechaVentaFin)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentasObtenerPorRangoFechaVenta", con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FVentaIni", fechaVentaIni.HasValue ? (object)fechaVentaIni.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@FVentaFin", fechaVentaFin.HasValue ? (object)fechaVentaFin.Value : DBNull.Value);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ventas por fecha de venta: " + ex.Message);
            }
            return dt;
        }
    }
}
