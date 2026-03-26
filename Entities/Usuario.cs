using System;

namespace Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public DateTime FechaCaptura { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Estatus { get; set; }
        public byte[] RowVersion { get; set; }
        public string RowVersionStr
        {
            get => RowVersion != null
                ? BitConverter.ToInt64(RowVersion, 0).ToString()
                : string.Empty;
        }
        // aqui debo poner los objetos de navegación, es decir las propiedades que representan las relaciones con otras entidades como es permisos
    }
}
