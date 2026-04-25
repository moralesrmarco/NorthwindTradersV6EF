using DAL.EF;
using System;
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
                            PermisoId = permisoId
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
    }
}
