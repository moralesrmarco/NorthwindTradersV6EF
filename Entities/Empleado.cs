//https://www.youtube.com/watch?v=VjBAQV_cFxM&list=PLgvaYP_E7xkKhk3QYJCvNXndiypRugCrf&index=6
using System;
using System.Collections.Generic;

namespace Entities
{
    public class Empleado
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string Extension { get; set; }
        public byte[] Photo { get; set; }
        public string Notes { get; set; }
        public string PhotoPath { get; set; }
        public byte[] RowVersion { get; set; }

        // Clave foránea hacia otro empleado
        public int? ReportsTo { get; set; }

        public string NameByFirstName
        {
            get { return FirstName + " " + LastName; }
        }

        public string NameByLastName
        {
            get 
            { 
                    return string.IsNullOrWhiteSpace(FirstName)
                        ? LastName 
                        : LastName + ", " + FirstName; 
            }
        }

        // Propiedades de navegación (no automáticas en ADO.NET, las llenas tú en la capa DAL/BLL)
        public Empleado Jefe { get; set; }

        public List<Empleado> EmpleadosSubordinados { get; set; } = new List<Empleado>();

        public override string ToString()
        {
            return NameByFirstName;
        }

        // Propiedades adicionales para facilitar el acceso al nombre del jefe desde el reportviewer
        public string JefeNameByLastName
        {
            get 
            {
                //return Jefe != null ? Jefe.NameByLastName : ""; 
                // Si no hay jefe, devuelve "N/A"
                if (Jefe == null)
                    return "N/A";

                // Si el FirstName está vacío, devuelve solo el LastName
                if (string.IsNullOrEmpty(Jefe.FirstName))
                    return Jefe.LastName;

                // Caso normal: "Apellido, Nombre"
                return Jefe.LastName + ", " + Jefe.FirstName;
            }
        }

        public string JefeNameByFirstName
        {
            get { return Jefe != null ? Jefe.NameByFirstName : ""; }
        }

        // del diagrama entidad-relación podemos ver que 
        // un empleado puede tener muchos órdenes asociadas
        public List<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
