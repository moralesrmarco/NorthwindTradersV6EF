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
            public UsuarioBLL()
            {
                // No se necesita conexión en esta clase, ya que se utiliza el contexto de Entity Framework
            }

        //public byte Insertar(Usuario usuario)
        //{
        //    using (var context = new NorthwindEntities())
        //    {
        //        context.Usuarios.Add(usuario);
        //        return (byte)context.SaveChanges();
        //    }
        //}

        //public sbyte Actualizar(Usuario usuario)
        //{
        //    using (var context = new NorthwindEntities())
        //    {
        //        context.Usuarios.Update(usuario);
        //        return (sbyte)context.SaveChanges();
        //    }
        //}

        //public sbyte Eliminar(Usuario usuario)
        //{
        //    using (var context = new NorthwindEntities())
        //    {
        //        context.Usuarios.Remove(usuario);
        //        return (sbyte)context.SaveChanges();
        //    }
        //}

        public static Usuario ValidarUsuario(Usuario usuario)
        {
            try
            {
                using (var context = new NorthwindEntities())
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
                using (var context = new NorthwindEntities())
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
