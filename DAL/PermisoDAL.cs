using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class PermisoDAL
    {
        private readonly string _connectionString;
    
        public PermisoDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertarPermiso(int idUsuario, int permisoId)
        {
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpPermisoInsertar", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsuarioId", idUsuario);
                    cmd.Parameters.AddWithValue("@PermisoId", permisoId);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // Ignorar error de clave duplicada (permiso ya asignado)
            }
            catch
            {
                throw;
            }
        }

        public void EliminarPermiso(int idUsuario, int permisoId)
        {
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpPermisoEliminar", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsuarioId", idUsuario);
                    cmd.Parameters.AddWithValue("@PermisoId", permisoId);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertarPermisos(int idUsuario, IEnumerable<int> permisosIds)
        {
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                {
                    cn.Open();
                    using (var transaction = cn.BeginTransaction())
                    {
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = "SpPermisoInsertar";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UsuarioId", idUsuario);
                            var pPermiso = cmd.Parameters.Add("@PermisoId", SqlDbType.Int);
                            try
                            {
                                foreach (var pid in permisosIds)
                                {
                                    pPermiso.Value = pid;
                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                            catch
                            {
                                try { transaction.Rollback(); } catch { /* opcional: log */ }
                                throw;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public int EliminarPermisos(int idUsuario)
        {
            int numRegs = 0;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpPermisosEliminar", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsuarioId", idUsuario);
                    cn.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar los permisos del usuario: " + ex.Message);
            }
            return numRegs;
        }

        public DataTable ObtenerPermisosDeCatalogo()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpPermisosObtenerDeCatalogo", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al llenar el catálogo de permisos: " + ex.Message);
            }
            return dt;
        }

        public DataTable ObtenerPermisosConcedidos(int usuarioId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpPermisosObtenerConcedidos", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al llenar el catálogo de permisos concedidos");
            }
            return dt;
        }

        public HashSet<int> ObtenerPermisosPorUsuarioId(int idUsuario)
        {
            HashSet<int> permisosIds = new HashSet<int>();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpPermisosObtenerPorUsuarioId", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsuarioId", idUsuario);
                    cn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permisosIds.Add(reader.GetInt32(reader.GetOrdinal("PermisoId")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los permisos concedidos del usuario: " + ex.Message);
            }
            return permisosIds;
        }
    }
}
