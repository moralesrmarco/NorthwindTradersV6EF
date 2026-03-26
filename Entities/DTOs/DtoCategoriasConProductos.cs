namespace Entities.DTOs
{
    public class DtoCategoriasConProductos
    {
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        // Campos planos para el reporte
        public string CategoryName { get; set; }
        // nombre de la compañia del proveedor
        public string CompanyName { get; set; }
    }
}
