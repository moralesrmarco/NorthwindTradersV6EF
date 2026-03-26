using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class ProductoDAL
    {
        private readonly string _connectionString;

        public ProductoDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Insertar(Producto producto)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProductoInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", 0);
                    cmd.Parameters["@ProductID"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@ProductName", producto.ProductName);
                    cmd.Parameters.AddWithValue("@SupplierID", producto.Proveedor?.SupplierID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", producto.Categoria?.CategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuantityPerUnit", (object)producto.QuantityPerUnit ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitPrice", (object)producto.UnitPrice ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsInStock", (object)producto.UnitsInStock ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsOnOrder", (object)producto.UnitsOnOrder ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReorderLevel", (object)producto.ReorderLevel ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discontinued", producto.Discontinued);
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                    producto.ProductID = (int)cmd.Parameters["@ProductID"].Value;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }

        public int Actualizar(Producto producto)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProductoActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", producto.ProductID);
                    cmd.Parameters.AddWithValue("@ProductName", producto.ProductName);
                    cmd.Parameters.AddWithValue("@SupplierID", producto.Proveedor?.SupplierID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", producto.Categoria?.CategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuantityPerUnit", (object)producto.QuantityPerUnit ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitPrice", (object)producto.UnitPrice ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsInStock", (object)producto.UnitsInStock ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsOnOrder", (object)producto.UnitsOnOrder ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReorderLevel", (object)producto.ReorderLevel ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discontinued", producto.Discontinued);
                    cmd.Parameters.AddWithValue("@RowVersion", producto.RowVersion ?? (object)DBNull.Value);
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }

        public int Eliminar(int productId, byte[] rowVersion)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProductoEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@RowVersion", rowVersion ?? (object)DBNull.Value);
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }

        public List<Producto> ObtenerProductos(bool selectorRealizaBusqueda, DtoProductosBuscar criterios, bool top100)
        {
            var productos = new List<Producto>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (selectorRealizaBusqueda)
                    {
                        cmd.CommandText = "SpProductoBuscar";
                        cmd.Parameters.AddWithValue("@IdIni", criterios.IdIni);
                        cmd.Parameters.AddWithValue("@IdFin", criterios.IdFin);
                        cmd.Parameters.AddWithValue("@Producto", criterios.Producto ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Categoria", criterios.Categoria);
                        cmd.Parameters.AddWithValue("@Proveedor", criterios.Proveedor);
                    }
                    else
                    {
                        cmd.CommandText = "SpProductoObtener";
                        cmd.Parameters.AddWithValue("@Top100", top100);
                    }
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new Producto
                            {
                                ProductID = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : 0,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : null,
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : null,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),
                                // Relación con Proveedor
                                Proveedor = new Proveedor
                                {
                                    SupplierID = reader["SupplierID"] != DBNull.Value ? Convert.ToInt32(reader["SupplierID"]) : 0,
                                    CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : null
                                },
                                // Relación con Categoria
                                Categoria = new Categoria
                                {
                                    CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                                    CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : null,
                                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null
                                }
                            };
                            productos.Add(producto);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return productos;
        }

        public List<Producto> ObtenerProductos(DtoProductosBuscar criterios)
        {
            var productos = new List<Producto>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SpProductoBuscarV4";
                    cmd.Parameters.AddWithValue("@IdIni", criterios.IdIni);
                    cmd.Parameters.AddWithValue("@IdFin", criterios.IdFin);
                    cmd.Parameters.AddWithValue("@Producto", criterios.Producto ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Categoria", criterios.Categoria);
                    cmd.Parameters.AddWithValue("@Proveedor", criterios.Proveedor);
                    cmd.Parameters.AddWithValue("@OrdenadoPor", criterios.OrdenadoPor);
                    cmd.Parameters.AddWithValue("@AscDesc", criterios.AscDesc);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new Producto
                            {
                                ProductID = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : 0,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : null,
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : null,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),
                                // Relación con Proveedor
                                Proveedor = new Proveedor
                                {
                                    SupplierID = reader["SupplierID"] != DBNull.Value ? Convert.ToInt32(reader["SupplierID"]) : 0,
                                    CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : null
                                },
                                // Relación con Categoria
                                Categoria = new Categoria
                                {
                                    CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                                    CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : null,
                                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null
                                }
                            };
                            productos.Add(producto);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return productos;
        }

        public Producto ObtenerProductoPorId(int productId)
        {
            Producto producto = new Producto();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProductoObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Producto
                            {
                                ProductID = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : 0,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : null,
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : null,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),
                                RowVersion = reader["RowVersion"] != DBNull.Value ? (byte[])reader["RowVersion"] : null,

                                // Relación con Proveedor
                                Proveedor = new Proveedor
                                {
                                    SupplierID = reader["SupplierID"] != DBNull.Value ? Convert.ToInt32(reader["SupplierID"]) : 0
                                },
                                // Relación con Categoria
                                Categoria = new Categoria
                                {
                                    CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                                    CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : null
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return producto;
        }

        public List<DtoProductosPorProveedor> ObtenerProductosPorProveedor()
        {
            var productosPorProveedor = new List<DtoProductosPorProveedor>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProductosPorProveedorObtener", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dto = new DtoProductosPorProveedor
                            {
                                ProductID = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : (int?)null,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : "Sin producto",
                                CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : string.Empty,
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : string.Empty,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : (decimal?)null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : (short?)null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : (short?)null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : (short?)null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),
                                CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : "Sin categoría"
                            };
                            productosPorProveedor.Add(dto);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return productosPorProveedor;
        }

        public List<DtoProductosPorProveedorConDetProv> ObtenerProductosPorProveedorConDetProv()
        {
            var productosPorProveedor = new List<DtoProductosPorProveedorConDetProv>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProductosPorProveedorConDetProvObtener", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dto = new DtoProductosPorProveedorConDetProv
                            {
                                // Suppliers
                                SupplierID = reader["SupplierID"] != DBNull.Value ? Convert.ToInt32(reader["SupplierID"]) : 0,
                                CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : string.Empty,
                                ContactName = reader["ContactName"] != DBNull.Value ? reader["ContactName"].ToString() : string.Empty,
                                ContactTitle = reader["ContactTitle"] != DBNull.Value ? reader["ContactTitle"].ToString() : string.Empty,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : string.Empty,
                                City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty,
                                Region = reader["Region"] != DBNull.Value ? reader["Region"].ToString() : string.Empty,
                                PostalCode = reader["PostalCode"] != DBNull.Value ? reader["PostalCode"].ToString() : string.Empty,
                                Country = reader["Country"] != DBNull.Value ? reader["Country"].ToString() : string.Empty,
                                Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : string.Empty,
                                Fax = reader["Fax"] != DBNull.Value ? reader["Fax"].ToString() : string.Empty,

                                // Products
                                ProductID = reader["ProductID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ProductID"]) : null,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : "Sin producto",
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : string.Empty,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),

                                // Categories
                                CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : "Sin categoría"
                            };

                            productosPorProveedor.Add(dto);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return productosPorProveedor;
        }

        public decimal ObtenerPrecioPromedio()
        {
            decimal precioPromedio = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("Select Avg(UnitPrice) As PrecioPromedio from products", con))
                {
                    con.Open();
                    precioPromedio = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al calcular el precio promedio: " + ex.Message);
            }
            return precioPromedio;
        }

        public DataTable ObtenerProductosPorEncimaDelPrecioPromedio()
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("Select * from VwProductosPorEncimaDelPrecioPromedio", con))
                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener los productos por encima del precio promedio: " + ex.Message);
            }
            return dt;
        }

        public DtoProductoCostoEInventario ObtenerProductoCostoEInventario(int productId)
        {
            DtoProductoCostoEInventario dtoProductoCostoEInventario = null;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProductoObtenerCostoEInventario", con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("ProductID", productId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return null;
                    DataRow dr = dt.Rows[0];
                    dtoProductoCostoEInventario = new DtoProductoCostoEInventario
                    {
                        UnitPrice = dr["UnitPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["UnitPrice"]),
                        UnitsInStock = dr["UnitsInStock"] == DBNull.Value ? (short)0 : Convert.ToInt16(dr["UnitsInStock"])
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el costo e inventario del producto: " + ex.Message);
            }
            return dtoProductoCostoEInventario;
        }
    }
}
