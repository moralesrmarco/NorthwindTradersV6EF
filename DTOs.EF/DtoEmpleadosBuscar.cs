using System;

namespace DTOs.EF
{
    public class DtoEmpleadosBuscar
    {
        public string IdIniTxt { get; set; }
        public string IdFinTxt { get; set; }

        public int IdIni => string.IsNullOrEmpty(IdIniTxt) ? 0 : Convert.ToInt32(IdIniTxt);
        public int IdFin => string.IsNullOrEmpty(IdFinTxt) ? 0 : Convert.ToInt32(IdFinTxt);

        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Titulo { get; set; }
        public string Domicilio { get; set; }
        public string Ciudad { get; set; }
        public string Region { get; set; }
        public string CodigoP { get; set; }
        public string Pais { get; set; }
        public string Telefono { get; set; }
    }
}
