using System;

namespace DTOs.EF
{
    public class DtoEmpleadosDgv
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public DateTime? BirthDate { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public byte[] Photo { get; set; }
        public string ReportsToName { get; set; }
    }
}
