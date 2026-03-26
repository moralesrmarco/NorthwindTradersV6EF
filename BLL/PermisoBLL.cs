using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class PermisoBLL
    {
        private readonly PermisoDAL _permisoDAL;

        public PermisoBLL(string connectionString)
        {
            _permisoDAL = new PermisoDAL(connectionString);
        }

        public void InsertarPermiso(int idUsuario, int permisoId)
        {
            _permisoDAL.InsertarPermiso(idUsuario, permisoId);
        }

        public void EliminarPermiso(int idUsuario, int permisoId)
        {
            _permisoDAL.EliminarPermiso(idUsuario, permisoId);
        }

        public void InsertarPermisos(int idUsuario, IEnumerable<int> permisosIds)
        {
            _permisoDAL.InsertarPermisos(idUsuario, permisosIds);
        }

        public int EliminarPermisos(int idUsuario)
        {
            return _permisoDAL.EliminarPermisos(idUsuario);
        }
    }
}
