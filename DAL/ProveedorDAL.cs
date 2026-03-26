using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class ProveedorDAL
    {
        private readonly string _connectionString;

        public ProveedorDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Insertar(Proveedor proveedor)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProveedorInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplierID", 0);
                    cmd.Parameters["@SupplierID"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@CompanyName", proveedor.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactName", proveedor.ContactName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactTitle", proveedor.ContactTitle ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", proveedor.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", proveedor.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Region", proveedor.Region ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PostalCode", proveedor.PostalCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country", proveedor.Country ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", proveedor.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", proveedor.Fax ?? (object)DBNull.Value);
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                    proveedor.SupplierID = (int)cmd.Parameters["@SupplierID"].Value;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }

        public int Actualizar(Proveedor proveedor)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProveedorActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplierID", proveedor.SupplierID);
                    cmd.Parameters.AddWithValue("@CompanyName", proveedor.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactName", proveedor.ContactName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactTitle", proveedor.ContactTitle ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", proveedor.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", proveedor.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Region", proveedor.Region ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PostalCode", proveedor.PostalCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country", proveedor.Country ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", proveedor.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", proveedor.Fax ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RowVersion", proveedor.RowVersion ?? (object)DBNull.Value);
                    // Parámetro de retorno
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

        public int Eliminar(int supplierId, byte[] rowVersion)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProveedorEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                    cmd.Parameters.AddWithValue("@RowVersion", rowVersion ?? (object)DBNull.Value);
                    // Parámetro de retorno
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

        public Proveedor ObtenerProveedorPorId(int supplierId)
        {
            var proveedor = new Proveedor();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProveedorObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", supplierId);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            proveedor.SupplierID = dr["SupplierID"] != DBNull.Value ? Convert.ToInt32(dr["SupplierID"]) : 0;
                            proveedor.CompanyName = dr["CompanyName"] != DBNull.Value ? dr["CompanyName"].ToString() : null;
                            proveedor.ContactName = dr["ContactName"] != DBNull.Value ? dr["ContactName"].ToString() : null;
                            proveedor.ContactTitle = dr["ContactTitle"] != DBNull.Value ? dr["ContactTitle"].ToString() : null;
                            proveedor.Address = dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null;
                            proveedor.City = dr["City"] != DBNull.Value ? dr["City"].ToString() : null;
                            proveedor.Region = dr["Region"] != DBNull.Value ? dr["Region"].ToString() : null;
                            proveedor.PostalCode = dr["PostalCode"] != DBNull.Value ? dr["PostalCode"].ToString() : null;
                            proveedor.Country = dr["Country"] != DBNull.Value ? dr["Country"].ToString() : null;
                            proveedor.Phone = dr["Phone"] != DBNull.Value ? dr["Phone"].ToString() : null;
                            proveedor.Fax = dr["Fax"] != DBNull.Value ? dr["Fax"].ToString() : null;
                            proveedor.RowVersion = dr["RowVersion"] != DBNull.Value ? (byte[])dr["RowVersion"] : null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return proveedor;
        }

        public DataTable ObtenerProveedorPaisesCbo()
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProveedorObtenerPaisesCbo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dt;
        }

        public List<Proveedor> ObtenerProveedores(bool selectorRealizaBusqueda, DtoProveedoresBuscar criterios, bool top100)
        {
            var proveedores = new List<Proveedor>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (selectorRealizaBusqueda)
                    {
                        cmd.CommandText = "SpProveedorBuscar";
                        cmd.Parameters.AddWithValue("@IdIni", criterios.IdIni);
                        cmd.Parameters.AddWithValue("@IdFin", criterios.IdFin);
                        cmd.Parameters.AddWithValue("@CompanyName", criterios.CompanyName);
                        cmd.Parameters.AddWithValue("@ContactName", criterios.ContactName);
                        cmd.Parameters.AddWithValue("@Address", criterios.Address);
                        cmd.Parameters.AddWithValue("@City", criterios.City);
                        cmd.Parameters.AddWithValue("@Region", criterios.Region);
                        cmd.Parameters.AddWithValue("@PostalCode", criterios.PostalCode);
                        cmd.Parameters.AddWithValue("@Country", criterios.Country);
                        cmd.Parameters.AddWithValue("@Phone", criterios.Phone);
                        cmd.Parameters.AddWithValue("@Fax", criterios.Fax);
                    }
                    else
                    {
                        cmd.CommandText = "SpProveedorObtener";
                        cmd.Parameters.AddWithValue("@top100", top100);
                    }
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var proveedor = new Proveedor
                            {
                                SupplierID = dr["SupplierID"] != DBNull.Value ? Convert.ToInt32(dr["SupplierID"]) : 0,
                                CompanyName = dr["CompanyName"] != DBNull.Value ? dr["CompanyName"].ToString() : null,
                                ContactName = dr["ContactName"] != DBNull.Value ? dr["ContactName"].ToString() : null,
                                ContactTitle = dr["ContactTitle"] != DBNull.Value ? dr["ContactTitle"].ToString() : null,
                                Address = dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null,
                                City = dr["City"] != DBNull.Value ? dr["City"].ToString() : null,
                                Region = dr["Region"] != DBNull.Value ? dr["Region"].ToString() : null,
                                PostalCode = dr["PostalCode"] != DBNull.Value ? dr["PostalCode"].ToString() : null,
                                Country = dr["Country"] != DBNull.Value ? dr["Country"].ToString() : null,
                                Phone = dr["Phone"] != DBNull.Value ? dr["Phone"].ToString() : null,
                                Fax = dr["Fax"] != DBNull.Value ? dr["Fax"].ToString() : null
                            };
                            proveedores.Add(proveedor);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return proveedores;
        }

        public DataSet ObtenerProveedoresProductosDgv()
        {
            var ds = new DataSet();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var dapProveedores = new SqlDataAdapter("SpProveedorObtener", con))
                    {
                        dapProveedores.SelectCommand.CommandType = CommandType.StoredProcedure;
                        dapProveedores.SelectCommand.Parameters.AddWithValue("@top100", true);
                        dapProveedores.Fill(ds, "Proveedores");
                    }
                    using (var dapProductos = new SqlDataAdapter("SpProductosConCategoriaProveedorDgv", con))
                    {
                        dapProductos.SelectCommand.CommandType = CommandType.StoredProcedure;
                        dapProductos.SelectCommand.Parameters.AddWithValue("@top100", true);
                        dapProductos.Fill(ds, "Productos");
                    }
                }
                // Quitar columnas que me causan conflicto al pintar el DataGridView
                ds.Tables["Proveedores"].Columns.Remove("RowVersion");
                ds.Tables["Proveedores"].Columns.Remove("HomePage");
                // en la siguiente instrucción se deben de proporcionar los nombres de los campos (alias) que devuelve el store procedure
                DataRelation dataRelation = new DataRelation("ProveedoresProductos", ds.Tables["Proveedores"].Columns["SupplierID"], ds.Tables["Productos"].Columns["SupplierID"]);
                ds.Relations.Add(dataRelation);
            }
            catch (Exception)
            {
                throw;
            }
            return ds;
        }
    }
}
