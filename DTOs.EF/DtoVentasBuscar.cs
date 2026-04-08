using System;

namespace DTOs.EF
{
    public class DtoVentasBuscar
    {
        public int IdIni { get; set; }
        public int IdFin { get; set; }
        public string Cliente { get; set; }
        public bool FVenta { get; set; }
        public DateTime? FVentaIni { get; set; }
        public DateTime? FVentaFin { get; set; }
        public bool FVentaNull { get; set; }
        public bool FRequerido { get; set; }
        public DateTime? FRequeridoIni { get; set; }
        public DateTime? FRequeridoFin { get; set; }
        public bool FRequeridoNull { get; set; }
        public bool FEnvio { get; set; }
        public DateTime? FEnvioIni { get; set; }
        public DateTime? FEnvioFin { get; set; }
        public bool FEnvioNull { get; set; }
        public string Empleado { get; set; }
        public string CompañiaT { get; set; }
        public string DirigidoA { get; set; }
    }
}
