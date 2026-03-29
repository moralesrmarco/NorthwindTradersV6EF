namespace DAL.EF
{
    public partial class Usuario
    {
        public string NombreCompleto => $"{Nombres} {Paterno} {Materno}".Trim();
    }
}
