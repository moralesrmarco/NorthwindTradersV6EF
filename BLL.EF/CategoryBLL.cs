using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace BLL.EF
{
    public class CategoryBLL
    {
        public static int Insertar(Category categoria)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var idParam = new ObjectParameter("CategoryID", typeof(int));

                    // Llamas al SP importado en el EDMX
                    // Supongamos que tu SP se llama SpCategoriaInsertar y devuelve el número de registros afectados
                    int result = context.SpCategoriaInsertar(
                                categoria.CategoryName,
                                categoria.Description,
                                categoria.Picture,
                                idParam
                            ); // El SP devuelve un int, no una colección

                    // Recuperar el ID generado
                    categoria.CategoryID = (int)idParam.Value;

                    // Si tu SP devuelve número de registros afectados
                    return result; // Devuelve el int directamente
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                throw new System.Exception("Error al insertar la categoría: " + ex.Message);
            }
        }

        public static int Actualizar(Category categoria)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    int result = context.Database
                        .SqlQuery<int>(
                            "EXEC SpCategoriaActualizar_EF @CategoryID, @CategoryName, @Description, @Picture, @RowVersion",
                            new SqlParameter("@CategoryID", categoria.CategoryID),
                            new SqlParameter("@CategoryName", categoria.CategoryName),
                            new SqlParameter("@Description", categoria.Description),
                            new SqlParameter("@Picture", categoria.Picture),
                            new SqlParameter("@RowVersion", categoria.RowVersion)
                        )
                        .FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar la categoría: " + ex.Message);
            }
        }

        public static int Eliminar(Category categoria)
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    int result = context.Database
                        .SqlQuery<int>(
                            "EXEC SpCategoriaEliminar_EF @CategoryID, @RowVersion",
                            new SqlParameter("@CategoryID", categoria.CategoryID),
                            new SqlParameter("@RowVersion", categoria.RowVersion)
                        )
                        .FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la categoría: " + ex.Message);
            }
        }

        public static List<Category> ObtenerCategorias(bool selectorRealizaBusqueda, DtoCategoriasBuscar criterios, bool top100)
        {
            using (var context = new NorthwindContext())
            {
                if (selectorRealizaBusqueda)
                {
                    var result = context.SpCategoriaBuscar(criterios.IdIni, criterios.IdFin, criterios.CategoryName).ToList();

                    return result.Select(r => new Category
                    {
                        CategoryID = r.CategoryID,
                        CategoryName = r.CategoryName,
                        Description = r.Description,
                        Picture = r.Picture,
                        RowVersion = r.RowVersion
                    }).ToList();
                }
                else
                {
                    var result = context.SpCategoriaObtener(top100).ToList();

                    return result.Select(r => new Category
                    {
                        CategoryID = r.CategoryID,
                        CategoryName = r.CategoryName,
                        Description = r.Description,
                        Picture = r.Picture,
                        RowVersion = r.RowVersion
                    }).ToList();
                }
            }
        }
    }
}