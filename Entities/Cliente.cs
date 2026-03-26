//https://www.youtube.com/watch?v=VjBAQV_cFxM&list=PLgvaYP_E7xkKhk3QYJCvNXndiypRugCrf&index=6
using System.Collections.Generic;

namespace Entities
{
    public class Cliente
    {
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public byte[] RowVersion { get; set; }

        // del diagrama entidad-relación podemos ver que
        // un cliente puede tener muchas ventas asociadas
        public List<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
