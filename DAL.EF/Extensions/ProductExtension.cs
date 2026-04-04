namespace DAL.EF
{
    public partial class Product
    {
        // Propiedad calculada para el nombre de la categoría
        public string CategoryName => Category?.CategoryName ?? string.Empty;
        // Propiedad calculada para la descripción de la categoría
        public string CategoryDescription => Category?.Description ?? string.Empty;
        // Propiedad calculada para el nombre de la compañía del proveedor
        public string SupplierCompanyName => Supplier?.CompanyName ?? string.Empty;
    }
}