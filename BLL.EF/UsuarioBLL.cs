using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Utilities;

namespace BLL.EF
{
    public class UsuarioBLL
    {
        public static sbyte Insertar(Usuario usuario)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    context.Usuarios.Add(usuario);
                    context.SaveChanges();
                    return 1; // éxito
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al insertar el usuario: " + ex.Message);
            }
        }

        public static sbyte Actualizar(Usuario usuario)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    context.Usuarios.Attach(usuario);
                    var entry = context.Entry(usuario);

                    entry.State = EntityState.Modified;
                    entry.Property(u => u.FechaCaptura).IsModified = false;
                    entry.OriginalValues["RowVersion"] = usuario.RowVersion;

                    try
                    {
                        return (sbyte)context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var dbEntry = ex.Entries.Single();
                        var databaseEntity = dbEntry.GetDatabaseValues();
                        if (databaseEntity == null)
                            return -1; // eliminado
                        else
                            return -2; // modificado previamente
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al actualizar el usuario: " + ex.Message);
            }
            catch (Exception)
            {
                return -3;
            }
        }


        public static sbyte Eliminar(Usuario usuario)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    context.Usuarios.Attach(usuario);
                    var entry = context.Entry(usuario);
                    entry.State = EntityState.Deleted;
                    try
                    {
                        return (sbyte)context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var dbEntry = ex.Entries.Single();
                        var databaseEntity = dbEntry.GetDatabaseValues();
                        if (databaseEntity == null)
                            return -1; // eliminado por otro usuario
                        else
                            return -2; // modificado previamente (RowVersion distinto)
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al eliminar el usuario: " + ex.Message);
            }
            catch (Exception)
            {
                return -3; // otro error
            }
        }

        public static Usuario ValidarUsuario(Usuario usuario)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var usuarioEncontrado = context.Usuarios.FirstOrDefault(u => u.Usuario1 == usuario.Usuario1 && u.Password == usuario.Password && u.Estatus);
                    return usuarioEncontrado ?? new Usuario { Id = 0 };
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener el usuario: " + ex.Message);
            }
        }

        public static HashSet<int> ObtenerPermisosPorUsuarioId(int idUsuario)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    // Selecciona todos los PermisoId de la tabla Permisos para el UsuarioId dado
                    var permisosIds = context.Permisos
                                             .Where(p => p.UsuarioId == idUsuario)
                                             .Select(p => p.PermisoId)
                                             .ToHashSet();

                    return permisosIds;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los permisos concedidos del usuario: " + ex.Message);
            }
        }

        public static DataTable ObtenerUsuarios(DtoUsuariosBuscar criterios)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var query = context.Usuarios.AsNoTracking().AsQueryable();
                    if (criterios != null)
                    {
                        if (criterios.IdIni > 0)
                        {
                            query = query.Where(u => u.Id >= criterios.IdIni);
                        }
                        if (criterios.IdFin > 0)
                        {
                            query = query.Where(u => u.Id <= criterios.IdFin);
                        }
                        if (!string.IsNullOrEmpty(criterios.Paterno))
                        {
                            var paternoNorm = Normalizador.Normalizar(criterios.Paterno);
                            query = query.Where(u => u.Paterno.Contains(paternoNorm));
                        }
                        if (!string.IsNullOrEmpty(criterios.Materno))
                        {
                            var maternoNorm = Normalizador.Normalizar(criterios.Materno);
                            query = query.Where(u => u.Materno.Contains(maternoNorm));
                        }
                        if (!string.IsNullOrEmpty(criterios.Nombres))
                        {
                            var nombresNorm = Normalizador.Normalizar(criterios.Nombres);
                            query = query.Where(u => u.Nombres.Contains(nombresNorm));
                        }
                        if (!string.IsNullOrEmpty(criterios.Usuario))
                        {
                            query = query.Where(u => u.Usuario1.Contains(criterios.Usuario));
                        }
                        query = query.OrderBy(u => u.Paterno)
                            .ThenBy(u => u.Materno)
                            .ThenBy(u => u.Nombres)
                            .ThenBy(u => u.Usuario1);
                    }
                    else
                    {
                        query = query.OrderByDescending(u => u.Id).Take(20);
                    }

                    var usuariosList = query.ToList();
                    // Convertir la lista de usuarios a un DataTable
                    DataTable dtTemp = new DataTable();
                    dtTemp.Columns.Add("Id", typeof(int));
                    dtTemp.Columns.Add("Paterno", typeof(string));
                    dtTemp.Columns.Add("Materno", typeof(string));
                    dtTemp.Columns.Add("Nombres", typeof(string));
                    dtTemp.Columns.Add("Usuario", typeof(string));
                    dtTemp.Columns.Add("Password", typeof(string));
                    dtTemp.Columns.Add("FechaCaptura", typeof(DateTime));
                    dtTemp.Columns.Add("FechaModificacion", typeof(DateTime));
                    dtTemp.Columns.Add("Estatus", typeof(bool));
                    dtTemp.Columns.Add("RowVersionStr", typeof(string)); // Columna adicional para mostrar RowVersion como string
                    foreach (var usuario in usuariosList)
                    {
                        dtTemp.Rows.Add(usuario.Id, usuario.Paterno, usuario.Materno, usuario.Nombres, usuario.Usuario1, usuario.Password, usuario.FechaCaptura, usuario.FechaModificacion, usuario.Estatus, usuario.RowVersionStr);
                    }
                    return dtTemp;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios: " + ex.Message);
            }
        }

        public static bool ValidarExisteUsuario(string usuario)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    return context.Usuarios.Any(u => u.Usuario1 == usuario);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar el usuario: " + ex.Message);
            }
        }
    }
}
