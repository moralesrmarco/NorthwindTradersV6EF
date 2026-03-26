using Entities;
using Entities.DTOs;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class UsuarioDAL
    {
        private readonly string _connectionString;

        public UsuarioDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public byte Insertar(Usuario usuario)
        {
            byte numRegs = 0;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpUsuarioInsertar", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetro de salida
                    var paramId = new SqlParameter("@Id", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(paramId);
                    cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;

                    // Parámetros de entrada
                    cmd.Parameters.AddWithValue("@Paterno", usuario.Paterno ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Materno", usuario.Materno ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nombres", usuario.Nombres);
                    cmd.Parameters.AddWithValue("@Usuario", usuario.User);
                    cmd.Parameters.AddWithValue("@Password", usuario.Password);
                    cmd.Parameters.AddWithValue("@FechaCaptura", usuario.FechaCaptura);
                    cmd.Parameters.AddWithValue("@FechaModificacion", usuario.FechaModificacion);
                    cmd.Parameters.AddWithValue("@Estatus", usuario.Estatus);

                    cn.Open();
                    numRegs = Convert.ToByte(cmd.ExecuteNonQuery());

                    // Recuperar el valor de salida
                    usuario.Id = Convert.ToInt32(paramId.Value);
                    usuario.RowVersion = (byte[])cmd.Parameters["@RowVersion"].Value;
                }
            }
            catch (SqlException ex) when (ex.Number == 2627) // Violación de clave única
            {
                throw new Exception("El usuario ya existe. Por favor, elije otro.");
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al insertar el usuario: " + ex.Message);
            }

            return numRegs;
        }

        public sbyte Actualizar(Usuario usuario)
        {
            sbyte numRegs = 0;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpUsuarioActualizar", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    cmd.Parameters.AddWithValue("@Id", usuario.Id);
                    cmd.Parameters.AddWithValue("@Paterno", usuario.Paterno ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Materno", usuario.Materno ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nombres", usuario.Nombres);
                    cmd.Parameters.AddWithValue("@Usuario", usuario.User);
                    cmd.Parameters.AddWithValue("@Password", usuario.Password);
                    cmd.Parameters.AddWithValue("@FechaModificacion", usuario.FechaModificacion);
                    cmd.Parameters.AddWithValue("@Estatus", usuario.Estatus);
                    var pRowVersion = new SqlParameter("@RowVersion", SqlDbType.Binary, 8);
                    pRowVersion.Direction = ParameterDirection.InputOutput;
                    pRowVersion.Value = usuario.RowVersion;
                    cmd.Parameters.Add(pRowVersion);
                    // Parámetro de retorno
                    var returnParameter = new SqlParameter
                    {
                        ParameterName = "@ReturnVal",
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(returnParameter);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    // Capturar el valor de retorno del SP de forma segura
                    var rawReturn = returnParameter.Value;
                    int intReturn = (rawReturn == null || rawReturn == DBNull.Value) ? 0 : Convert.ToInt32(rawReturn);
                    numRegs = (sbyte)intReturn;
                    if (numRegs > 0)
                        usuario.RowVersion = (byte[])cmd.Parameters["@RowVersion"].Value;
                    else 
                        usuario.RowVersion = null; // Indica que no se pudo actualizar por concurrencia o no existe el registro
                }
            }
            catch (SqlException ex) when (ex.Number == 2627) // Violación de clave única
            {
                throw new Exception("El usuario ya existe. Por favor, elije otro.");
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al modificar el usuario: " + ex.Message);
            }

            return numRegs;
        }

        public sbyte Eliminar(Usuario usuario)
        {
            sbyte numRegs = 0;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpUsuarioEliminar", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", usuario.Id);
                    cmd.Parameters.AddWithValue("@RowVersion", usuario.RowVersion);

                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    numRegs = Convert.ToSByte(returnParameter.Value);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al eliminar el usuario: " + ex.Message);
            }
            return numRegs;
        }

        public byte ActualizarContraseña(string usuario, string nuevaContrasena)
        {
            byte numRegs = 0;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpUsuarioActualizarContrasena", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@password", nuevaContrasena);
                    cn.Open();
                    numRegs = Convert.ToByte(cmd.ExecuteNonQuery());
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al cambiar la contraseña: " + ex.Message);
            }
            return numRegs;
        }

        public byte ValidarContraseñaActual(string usuario, string contrasenaActual)
        {
            byte numRegs = 0;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpUsuarioValidarContrasenaActual", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Password", contrasenaActual);
                    cn.Open();
                    numRegs = Convert.ToByte(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al validar la contraseña actual: " + ex.Message);
            }
            return numRegs;
        }

        public int ValidarUsuario(string usuario, string password, out string nombreUsuarioAutenticado)
        {
            int idUsuario = 0;
            nombreUsuarioAutenticado = string.Empty;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpUsuarioValidarLogin", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idUsuario = Convert.ToInt32(reader["Id"]);
                            string paterno = reader["Paterno"].ToString();
                            string materno = reader["Materno"].ToString();
                            string nombres = reader["Nombres"].ToString();

                            nombreUsuarioAutenticado = $"{nombres} {paterno} {materno}";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener el usuario: " + ex.Message);
            }
            return idUsuario;
        }

        public DataTable ObtenerUsuarios(DtoUsuariosBuscar dtoUsuariosBuscar)
        {
            var dt = new DataTable();
            string query;
            if (dtoUsuariosBuscar == null)
                query = "SpUsuarioObtener";
            else
                query = "spUsuarioBuscar";
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(query, cn))
                {
                    if (dtoUsuariosBuscar != null)
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdIni", dtoUsuariosBuscar.IdIni);
                        cmd.Parameters.AddWithValue("@IdFin", dtoUsuariosBuscar.IdFin);
                        cmd.Parameters.AddWithValue("@Paterno", dtoUsuariosBuscar.Paterno);
                        cmd.Parameters.AddWithValue("@Materno", dtoUsuariosBuscar.Materno);
                        cmd.Parameters.AddWithValue("@Nombres", dtoUsuariosBuscar.Nombres);
                        cmd.Parameters.AddWithValue("@Usuario", dtoUsuariosBuscar.Usuario);
                    }
                    using (var da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios: " + ex.Message);
            }
            return dt;
        }

        public bool ValidarExisteUsuario(string usuario)
        {
            bool existe = false;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpUsuarioValidarExiste", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cn.Open();
                    byte count = Convert.ToByte(cmd.ExecuteScalar());
                    existe = count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar el usuario: " + ex.Message);
            }
            return existe;
        }
    }
}
