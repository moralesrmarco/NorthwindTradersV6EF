using System.Collections.Generic;
using System.Linq;

namespace DAL.EF
{
    // Clase parcial para extender la entidad generada automáticamente
    public partial class Employee
    {
        // Propiedad calculada: Nombre por nombre primero
        public string NameByFirstName => $"{FirstName} {LastName}".Trim();

        // Propiedad calculada: Nombre por apellido primero
        public string NameByLastName =>
            string.IsNullOrWhiteSpace(FirstName)
                ? LastName
                : $"{LastName}, {FirstName}";

        // Propiedad para acceder al jefe (ya existe Employee1 como navegación)
        public Employee Jefe => Employee1;

        // Propiedad para acceder a subordinados (ya existe Employees1 como colección)
        public List<Employee> EmpleadosSubordinados => Employees1.ToList();

        // Propiedades adicionales para mostrar el nombre del jefe
        public string JefeNameByLastName =>
            Jefe == null ? "N/A"
            : string.IsNullOrEmpty(Jefe.FirstName) ? Jefe.LastName
            : $"{Jefe.LastName}, {Jefe.FirstName}";

        public string JefeNameByFirstName => Jefe?.NameByFirstName ?? "";

        // Sobrescribir ToString para mostrar el nombre
        public override string ToString() => NameByFirstName;

        public static void NormalizarFotos(IEnumerable<Employee> empleados)
        {
            foreach (var empleado in empleados)
            {
                if (empleado.Photo != null)
                {
                    empleado.Photo = Utilities.Utils.StripOleHeader(empleado.Photo, empleado.EmployeeID);
                }
            }
        }
    }
}
