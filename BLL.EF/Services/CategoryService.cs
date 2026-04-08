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

        public static DataTable ObtenerCategoriasCbo(bool varGhost = true)
        {
            var dt = new DataTable();
            dt.Columns.Add("CategoryID", typeof(int));
            dt.Columns.Add("CategoryName", typeof(string));
            foreach (var categoria in ObtenerCategoriasCbo())
            {
                dt.Rows.Add(categoria.CategoryID, categoria.CategoryName);
            }
            return dt;
        }
    }
}
