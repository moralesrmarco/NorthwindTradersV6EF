//https://www.youtube.com/watch?v=VjBAQV_cFxM&list=PLgvaYP_E7xkKhk3QYJCvNXndiypRugCrf&index=6
using System;
using System.Collections.Generic;

namespace Entities
{
    public class Categoria
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public byte[] RowVersion { get; set; }
        // ojo no quitar
        public string RowVersionStr { get; set; }
        // Propiedad auxiliar para que no tenga conflicto el DataGridView
        public string RowVersionString
        {
            get => RowVersion != null
                ? BitConverter.ToInt64(RowVersion, 0).ToString()
                : string.Empty;
        }

        // del diagrama entidad-relacion podemos ver que
        // una categoria tiene muchos productos asociados
        public List<Producto> Productos { get; set; } = new List<Producto>();
    }
}
