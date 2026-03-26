using DAL;
using Entities;
using Entities.DTOs;
using System;
using System.Data;
using System.Linq;

namespace BLL
{
    public class UsuarioBLL
    {
        private readonly UsuarioDAL _usuarioDAL;

        public UsuarioBLL(string _connectionString)
        {
            _usuarioDAL = new UsuarioDAL(_connectionString);
        }

        public byte Insertar(Usuario usuario)
        {
            return _usuarioDAL.Insertar(usuario);
        }

        public sbyte Actualizar(Usuario usuario)
        {
            return _usuarioDAL.Actualizar(usuario);
        }

        public sbyte Eliminar(Usuario usuario)
        {
            return _usuarioDAL.Eliminar(usuario);
        }

        public byte ActualizarContraseña(string usuario, string nuevaContrasena)
        {
            return _usuarioDAL.ActualizarContraseña(usuario, nuevaContrasena);
        }

        public byte ValidarContraseñaActual(string usuario, string contrasenaActual)
        {
            return _usuarioDAL.ValidarContraseñaActual(usuario, contrasenaActual);
        }

        public int ValidarUsuario(string usuario, string password, out string nombreUsuarioAutenticado)
        {
            return _usuarioDAL.ValidarUsuario(usuario, password, out nombreUsuarioAutenticado);
        }

        public DataTable ObtenerUsuarios(DtoUsuariosBuscar dtoUsuariosBuscar)
        {
            DataTable dtTemp = _usuarioDAL.ObtenerUsuarios(dtoUsuariosBuscar);
            var usuarios = dtTemp.AsEnumerable()
                .Select(row => new Usuario
                {
                    Id = row.Field<int>("Id"),
                    Paterno = row.Field<string>("Paterno"),
                    Materno = row.Field<string>("Materno"),
                    Nombres = row.Field<string>("Nombres"),
                    User = row.Field<string>("Usuario"),
                    Password = row.Field<string>("Password"),
                    FechaCaptura = row.Field<DateTime>("FechaCaptura"),
                    FechaModificacion = row.Field<DateTime>("FechaModificacion"),
                    Estatus = row.Field<bool>("Estatus"),
                    RowVersion = row.Field<byte[]>("RowVersion")
                })
                .ToList();
            DataTable dt = new DataTable();

            // Definir columnas
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Paterno", typeof(string));
            dt.Columns.Add("Materno", typeof(string));
            dt.Columns.Add("Nombres", typeof(string));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Password", typeof(string));
            dt.Columns.Add("FechaCaptura", typeof(DateTime));
            dt.Columns.Add("FechaModificacion", typeof(DateTime));
            dt.Columns.Add("Estatus", typeof(bool));
            dt.Columns.Add("RowVersionStr", typeof(string)); // solo string, no byte[]

            // Llenar filas
            foreach (var u in usuarios)
            {
                dt.Rows.Add(
                    u.Id,
                    u.Paterno,
                    u.Materno,
                    u.Nombres,
                    u.User,
                    u.Password,
                    u.FechaCaptura,
                    u.FechaModificacion,
                    u.Estatus,
                    u.RowVersionStr // aquí usas la propiedad calculada
                );
            }

            return dt;
        }

        public bool ValidarExisteUsuario(string usuario)
        {
            return _usuarioDAL.ValidarExisteUsuario(usuario);
        }
    }
}
