using DAL.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BLL.EF.Services
{
    public class CategoryService
    {
        public static List<Category> ObtenerCategoriasCbo()
        {
            try
            {
                using (var context = new NorthwindContext())
                {
                    var categorias = context.Categories
                        .OrderBy(c => c.CategoryName)
                        .ToList();
                    var opcionSeleccione = new Category
                    {
                        CategoryID = 0,
                        CategoryName = "»--- Seleccione ---«"
                    };
                    categorias.Insert(0, opcionSeleccione);
                    return categorias;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las categorías" + ex.Message);
            }
        }
    }
}
