namespace DTOs.EF
{
    public class DtoProductoListado
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public int? CategoryID { get; set; }
        public int? SupplierID { get; set; }
    }
}
