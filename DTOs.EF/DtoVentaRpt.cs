using System;

namespace DTOs.EF
{
    public class DtoVentaRpt
    {
        public int OrderID { get; set; }
        public string Cliente { get; set; }
        public string Vendedor { get; set; }
        public DateTime? FechaDePedido { get; set; }
        public DateTime? FechaRequerido { get; set; }
        public DateTime? FechaDeEnvio { get; set; }
        public string CompaniaTransportista { get; set; }
        public string DirigidoA { get; set; }
        public string Domicilio { get; set; }
        public string Ciudad { get; set; }
        public string Region { get; set; }
        public string CodigoPostal { get; set; }
        public string Pais { get; set; }
        public decimal Flete { get; set; }
    }
}
