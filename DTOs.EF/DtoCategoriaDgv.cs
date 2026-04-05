namespace DTOs.EF
{
    // necesario para que no se proyecte el campo RowVersion en el grid, y en su lugar se muestre la propiedad auxiliar RowVersionStr
    public class DtoCategoriaDgv
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public string RowVersionStr { get; set; }
    }
}
