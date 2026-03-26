using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class ClienteDAL
    {

        private readonly string _connectionString;

        public ClienteDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ObtenerClientesPaisesCbo()
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpClienteObtenerPaisesCbo", con))
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

        public List<Cliente> ObtenerClientes(bool selectorRealizaBusqueda, Cliente criterios, bool top100)
        {
            List<Cliente> clientes = new List<Cliente>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (selectorRealizaBusqueda)
                    {
                        cmd.CommandText = "SpClienteBuscar";
                        cmd.Parameters.AddWithValue("@Id", criterios.CustomerID);
                        cmd.Parameters.AddWithValue("@Compañia", criterios.CompanyName);
                        cmd.Parameters.AddWithValue("@Contacto", criterios.ContactName);
                        cmd.Parameters.AddWithValue("@Domicilio", criterios.Address);
                        cmd.Parameters.AddWithValue("@Ciudad", criterios.City);
                        cmd.Parameters.AddWithValue("@Region", criterios.Region);
                        cmd.Parameters.AddWithValue("@CodigoP", criterios.PostalCode);
                        cmd.Parameters.AddWithValue("@Pais", criterios.Country);
                        cmd.Parameters.AddWithValue("@Telefono", criterios.Phone);
                        cmd.Parameters.AddWithValue("@Fax", criterios.Fax);
                    }
                    else
                    {
                        cmd.CommandText = "SpClienteObtener";
                        cmd.Parameters.AddWithValue("@top100", top100);
                    }                        
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cliente = new Cliente
                            {
                                CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : reader["CustomerID"].ToString(),
                                CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? null : reader["CompanyName"].ToString(),
                                ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? null : reader["ContactName"].ToString(),
                                ContactTitle = reader.IsDBNull(reader.GetOrdinal("ContactTitle")) ? null : reader["ContactTitle"].ToString(),
                                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
                                City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader["City"].ToString(),
                                Region = reader.IsDBNull(reader.GetOrdinal("Region")) ? null : reader["Region"].ToString(),
                                PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? null : reader["PostalCode"].ToString(),
                                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader["Country"].ToString(),
                                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"].ToString(),
                                Fax = reader.IsDBNull(reader.GetOrdinal("Fax")) ? null : reader["Fax"].ToString()
                                // la siguiente linea esta comentada porque causa un error a la hora de ejecutar la renderizacion del DataGridView en la capa de presentacion
                                //RowVersion = reader.IsDBNull(reader.GetOrdinal("RowVersion")) ? null : (byte[])reader["RowVersion"]
                            };
                            clientes.Add(cliente);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return clientes;
        }

        public Cliente ObtenerClientePorId(string idCliente)
        {
            Cliente cliente = null;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpClienteObtenerPorId", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", idCliente);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = MapearCliente(reader);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return cliente;
        }

        private Cliente MapearCliente(SqlDataReader reader)
        {
            var cliente = new Cliente()
            {
                CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : reader["CustomerID"].ToString(),
                CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? null : reader["CompanyName"].ToString(),
                ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? null : reader["ContactName"].ToString(),
                ContactTitle = reader.IsDBNull(reader.GetOrdinal("ContactTitle")) ? null : reader["ContactTitle"].ToString(),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
                City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader["City"].ToString(),
                Region = reader.IsDBNull(reader.GetOrdinal("Region")) ? null : reader["Region"].ToString(),
                PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? null : reader["PostalCode"].ToString(),
                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader["Country"].ToString(),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"].ToString(),
                Fax = reader.IsDBNull(reader.GetOrdinal("Fax")) ? null : reader["Fax"].ToString(),
                RowVersion = reader.IsDBNull(reader.GetOrdinal("RowVersion")) ? null : (byte[])reader["RowVersion"]
            };
            return cliente;
        }

        public int Insertar(Cliente cliente)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpClienteInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", cliente.CustomerID);
                    cmd.Parameters.AddWithValue("@Compañia", cliente.CompanyName);
                    cmd.Parameters.AddWithValue("@Contacto", cliente.ContactName);
                    cmd.Parameters.AddWithValue("@Titulo", cliente.ContactTitle);
                    cmd.Parameters.AddWithValue("@Domicilio", cliente.Address);
                    cmd.Parameters.AddWithValue("@Ciudad", cliente.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(cliente.Region) ? (object)DBNull.Value : cliente.Region);
                    cmd.Parameters.AddWithValue("@CodigoP", string.IsNullOrWhiteSpace(cliente.PostalCode) ? (object)DBNull.Value : cliente.PostalCode);
                    cmd.Parameters.AddWithValue("@Pais", cliente.Country);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Phone);
                    cmd.Parameters.AddWithValue("@Fax", string.IsNullOrWhiteSpace(cliente.Fax) ? (object)DBNull.Value : cliente.Fax);
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }

        public int Actualizar(Cliente cliente)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpClienteActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", cliente.CustomerID);
                    cmd.Parameters.AddWithValue("@Compañia", cliente.CompanyName);
                    cmd.Parameters.AddWithValue("@Contacto", cliente.ContactName);
                    cmd.Parameters.AddWithValue("@Titulo", cliente.ContactTitle);
                    cmd.Parameters.AddWithValue("@Domicilio", cliente.Address);
                    cmd.Parameters.AddWithValue("@Ciudad", cliente.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(cliente.Region) ? (object)DBNull.Value : cliente.Region);
                    cmd.Parameters.AddWithValue("@CodigoP", string.IsNullOrWhiteSpace(cliente.PostalCode) ? (object)DBNull.Value : cliente.PostalCode);
                    cmd.Parameters.AddWithValue("@Pais", cliente.Country);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Phone);
                    cmd.Parameters.AddWithValue("@Fax", string.IsNullOrWhiteSpace(cliente.Fax) ? (object)DBNull.Value : cliente.Fax);
                    var rowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    rowVersion.Value = cliente.RowVersion ?? (object)DBNull.Value;
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

        public int Eliminar(string clienteId, byte[] rowVersion)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpClienteEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", clienteId);
                    cmd.Parameters.AddWithValue("@RowVersion", rowVersion);
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = Convert.ToInt32(returnParameter.Value);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }

        public List<DtoClienteProveedor> ObtenerClientesProveedores(string nombreDeFormulario, string comboBoxSelectedValue, bool checkBoxClientes, bool checkBoxProveedores)
        {
            List<DtoClienteProveedor> clientesProveedores = new List<DtoClienteProveedor>();
            string query = string.Empty;
            if (nombreDeFormulario == "FrmClientesyProveedoresDirectorio" || nombreDeFormulario == "FrmRptClientesyProveedoresDirectorio")
            {
                if (checkBoxClientes & checkBoxProveedores)
                    query = "Select * from VwClientesProveedores Order by Relation, CompanyName;";
                else if (checkBoxClientes & !checkBoxProveedores)
                    query = "Select * from VwClientesProveedores Where Relation = 'Cliente' Order by CompanyName;";
                else if (!checkBoxClientes & checkBoxProveedores)
                    query = "Select * from VwClientesProveedores Where Relation = 'Proveedor' Order by CompanyName;";
            }
            else if (nombreDeFormulario == "FrmClientesyProveedoresDirectorioxCiudad" || nombreDeFormulario == "FrmRptClientesyProveedoresDirectorioxCiudad")
            {
                if (comboBoxSelectedValue == "aaaaa" & checkBoxClientes & checkBoxProveedores)
                    query = "Select * from VwClientesProveedores Order by City, Country, CompanyName";
                else if (comboBoxSelectedValue != "aaaaa" & checkBoxClientes & checkBoxProveedores)
                {
                    // comboBoxSelectedValue = "Aachen, Germany"
                    var partes = comboBoxSelectedValue.Split(',');
                    // Trim para quitar espacios en blanco
                    var ciudad = partes[0].Trim();
                    var pais = partes.Length > 1 ? partes[1].Trim() : string.Empty;
                    query = $"Select * from VwClientesProveedores Where City = '{ciudad}' And Country = '{pais}' Order by Country, CompanyName";
                }
                else if (comboBoxSelectedValue == "aaaaa" & checkBoxClientes & !checkBoxProveedores)
                    query = "Select * from VwClientesProveedores Where Relation = 'Cliente' Order by City, Country, CompanyName";
                else if (comboBoxSelectedValue == "aaaaa" & !checkBoxClientes & checkBoxProveedores)
                    query = "Select * from VwClientesProveedores Where Relation = 'Proveedor' Order by City, Country, CompanyName";
                else if (comboBoxSelectedValue != "aaaaa" & checkBoxClientes & !checkBoxProveedores)
                {
                    // comboBoxSelectedValue = "Aachen, Germany"
                    var partes = comboBoxSelectedValue.Split(',');
                    // Trim para quitar espacios en blanco
                    var ciudad = partes[0].Trim();
                    var pais = partes.Length > 1 ? partes[1].Trim() : string.Empty;
                    query = $"Select * from VwClientesProveedores Where City = '{ciudad}' And Country = '{pais}' And Relation = 'Cliente' Order by Country, CompanyName";
                }
                else if (comboBoxSelectedValue != "aaaaa" & !checkBoxClientes & checkBoxProveedores)
                {
                    // comboBoxSelectedValue = "Aachen, Germany"
                    var partes = comboBoxSelectedValue.Split(',');
                    // Trim para quitar espacios en blanco
                    var ciudad = partes[0].Trim();
                    var pais = partes.Length > 1 ? partes[1].Trim() : string.Empty;
                    query = $"Select * from VwClientesProveedores Where City = '{ciudad}' And Country = '{pais}' And Relation = 'Proveedor' Order by Country, CompanyName";
                }
            }
            else if (nombreDeFormulario == "FrmClientesyProveedoresDirectorioxPais" || nombreDeFormulario == "FrmRptClientesyProveedoresDirectorioxPais")
            {
                if (comboBoxSelectedValue == "aaaaa" & checkBoxClientes & checkBoxProveedores)
                    query = "Select * from VwClientesProveedores Order by Country, City, CompanyName";
                else if (comboBoxSelectedValue != "aaaaa" & checkBoxClientes & checkBoxProveedores)
                    query = $"Select * from VwClientesProveedores Where Country = '{comboBoxSelectedValue}' Order by City, CompanyName";
                else if (comboBoxSelectedValue == "aaaaa" & checkBoxClientes & !checkBoxProveedores)
                    query = "Select * from VwClientesProveedores Where Relation = 'Cliente' Order by Country, City, CompanyName";
                else if (comboBoxSelectedValue == "aaaaa" & !checkBoxClientes & checkBoxProveedores)
                    query = "Select * from VwClientesProveedores Where Relation = 'Proveedor' Order by Country, City, CompanyName";
                else if (comboBoxSelectedValue != "aaaaa" & checkBoxClientes & !checkBoxProveedores)
                    query = $"Select * from VwClientesProveedores Where Country = '{comboBoxSelectedValue}' And Relation = 'Cliente' Order by City, CompanyName";
                else if (comboBoxSelectedValue != "aaaaa" & !checkBoxClientes & checkBoxProveedores)
                    query = $"Select * from VwClientesProveedores Where Country = '{comboBoxSelectedValue}' And Relation = 'Proveedor' Order by City, CompanyName";
            }
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var clienteProveedor = new DtoClienteProveedor
                                {
                                    CompanyName = reader["CompanyName"].ToString(),
                                    Contact = reader["Contact"].ToString(),
                                    Relation = reader["Relation"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    City = reader["City"].ToString(),
                                    Region = reader["Region"].ToString(),
                                    PostalCode = reader["PostalCode"].ToString(),
                                    Country = reader["Country"].ToString(),
                                    Phone = reader["Phone"].ToString(),
                                    Fax = reader["Fax"].ToString()
                                };
                                clientesProveedores.Add(clienteProveedor);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return clientesProveedores;
        }

        public List<DtoCiudadPaisVwClientesProveedores> ObtenerCiudadPaisVwCliProvCbo()
        {
            List<DtoCiudadPaisVwClientesProveedores> ciudadesPaises = new List<DtoCiudadPaisVwClientesProveedores>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpClientesProveedoresCiudadPaisVwCliProvCbo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var ciudadPais = new DtoCiudadPaisVwClientesProveedores()
                                {
                                    CiudadPais = reader["CiudadPais"].ToString()
                                };
                                ciudadesPaises.Add(ciudadPais);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ciudadesPaises;
        }

        public List<DtoPaisVwClientesProveedores> ObtenerPaisVwCliProvCbo()
        {
            List<DtoPaisVwClientesProveedores> paises = new List<DtoPaisVwClientesProveedores>();
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpClientesProveedoresPaisVwCliProvCbo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var pais = new DtoPaisVwClientesProveedores()
                                {
                                    Pais = reader["Pais"].ToString()
                                };
                                paises.Add(pais);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return paises;
        }
    }
}
