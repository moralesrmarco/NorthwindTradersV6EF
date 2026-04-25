using DAL.EF;
using System;
using System.Data;
using System.Linq;

namespace BLL.EF.Services
{
    public class PermisosService
    {
        public static DataTable ObtenerPermisosDeCatalogo()
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var permisos = context.CatalogoPermisos
                        .Where(p => p.Estatus == true)
                        .OrderBy(p => p.PermisoId)
                        .Select(p => new
                        {
                            p.PermisoId,
                            p.Descripción
                        })
                        .ToList();

                    // Convertir la lista a DataTable
                    DataTable dt = new DataTable();
                    dt.Columns.Add("PermisoId", typeof(int));
                    dt.Columns.Add("Descripción", typeof(string));

                    foreach (var permiso in permisos)
                    {
                        dt.Rows.Add(permiso.PermisoId, permiso.Descripción);
                    }

                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al llenar el catálogo de permisos: " + ex.Message);
            }
        }

        public static DataTable ObtenerPermisosConcedidos(int usuarioId)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var permisosConcedidos = context.Permisos
                        .Where(p => p.UsuarioId == usuarioId && p.CatalogoPermiso.Estatus == true)
                        .OrderBy(p => p.PermisoId)
                        .Select(p => new
                        {
                            p.PermisoId,
                            p.CatalogoPermiso.Descripción
                        })
                        .ToList();
                    // Convertir la lista a DataTable
                    DataTable dt = new DataTable();
                    dt.Columns.Add("PermisoId", typeof(int));
                    dt.Columns.Add("Descripción", typeof(string));
                    foreach (var permiso in permisosConcedidos)
                    {
                        dt.Rows.Add(permiso.PermisoId, permiso.Descripción);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al llenar el catálogo de permisos concedidos: " + ex.Message);
            }
        }
    }
}
