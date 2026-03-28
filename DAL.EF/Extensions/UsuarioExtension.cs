using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF
{
    public partial class Usuario
    {
        public string NombreCompleto => $"{Nombres} {Paterno} {Materno}".Trim();
    }
}
