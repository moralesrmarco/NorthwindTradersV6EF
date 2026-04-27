using DAL.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace BLL.EF
{
    public class PermisosBLL
    {
        public static void Insertar(int idUsuario, int permisoId)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    if (!context.Permisos.Any(p => p.UsuarioId == idUsuario && p.PermisoId == permisoId))
                    {
                        var permiso = new Permiso
                        {
                            UsuarioId = idUsuario,
                            PermisoId = permisoId,
                            FechaDeCreacion = DateTime.Now
                        };
                        context.Permisos.Add(permiso);
                        context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                // Detectar error de clave duplicada (permiso ya asignado)
                // EF envuelve el SqlException dentro de InnerException.InnerException
                var sqlEx = ex.InnerException?.InnerException as SqlException;
                if (sqlEx != null && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    // Ignorar duplicado
                }
                else
                {
                    throw new Exception("Error al insertar el permiso: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el permiso: " + ex.Message);
            }
        }

        public static void Eliminar(int idUsuario, int permisoId)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var permiso = context.Permisos.FirstOrDefault(p => p.UsuarioId == idUsuario && p.PermisoId == permisoId);
                    if (permiso != null)
                    {
                        context.Permisos.Remove(permiso);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el permiso: " + ex.Message);
            }
        }

        public static void InsertarPermisos(int idUsuario, List<int> permisosIdsAInsertar)
        {
            try
            {
                using (var context = new NorthwindContext())
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var pid in permisosIdsAInsertar)
                        {
                            if (!context.Permisos.Any(p => p.UsuarioId == idUsuario && p.PermisoId == pid))
                            {
                                var permiso = new Permiso
                                {
                                    UsuarioId = idUsuario,
                                    PermisoId = pid,
                                    FechaDeCreacion = DateTime.Now
                                };
                                context.Permisos.Add(permiso);
                            }
                        }
                        context.SaveChanges();
                        tx.Commit();
                    }
                    catch (DbUpdateException ex)
                    {
                        // Detectar error de clave duplicada (respaldo contra condiciones de carrera)
                        var sqlEx = ex.InnerException?.InnerException as SqlException;
                        if (sqlEx != null && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                        {
                            // Ignorar duplicado
                        }
                        else
                        {
                            tx.Rollback();
                            throw new Exception("Error al insertar permisos: " + ex.Message, ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw new Exception("Error al insertar los permisos: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar los permisos: " + ex.Message);
            }
        }

        public static int EliminarPermisos(int idUsuario)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    // Selecciona todos los permisos del usuario
                    var permisos = context.Permisos.Where(p => p.UsuarioId == idUsuario).ToList();
                    // Si no hay permisos, regresa 0
                    if (!permisos.Any())
                        return 0;
                    context.Permisos.RemoveRange(permisos);
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar los permisos del usuario: " + ex.Message);
            }
        }
    }
}
