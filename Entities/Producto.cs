//https://www.youtube.com/watch?v=VjBAQV_cFxM&list=PLgvaYP_E7xkKhk3QYJCvNXndiypRugCrf&index=6
using System.Collections.Generic;

namespace Entities
{
    public class Producto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Proveedor Proveedor { get; set; }
        public Categoria Categoria { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public byte[] RowVersion { get; set; }

        // del diagrama entidad-relación podemos ver que
        // un producto tiene muchaas ventasdetalle asociadas
        public List<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    }
}
