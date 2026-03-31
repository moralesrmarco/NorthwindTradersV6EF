using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BLL.EF
{
    public class UsuarioBLL
    {
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
    }
}
