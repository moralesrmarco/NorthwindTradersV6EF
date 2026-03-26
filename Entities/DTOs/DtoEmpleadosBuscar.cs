namespace Entities.DTOs
{
    public class DtoEmpleadosBuscar
    {
        public int? IdIni { get; set; }
        public int? IdFin { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Titulo { get; set; }
        public string Domicilio { get; set; }
        public string Ciudad { get; set; }
        public string Region { get; set; }
        public string CodigoP { get; set; }
        public string Pais { get; set; }
        public string Telefono { get; set; }

        public string IdIniTxt { get; set; }
        public string IdFinTxt { get; set; }
    }
}
