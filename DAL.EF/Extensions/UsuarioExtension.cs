using System;
using Utilities;

namespace DAL.EF
{
    public partial class Usuario
    {
        public string NombreCompleto => $"{Nombres} {Paterno} {Materno}".Trim();

        public string RowVersionStr
        {
            get => RowVersion != null
                ? BitConverter.ToInt64(RowVersion, 0).ToString()
                : string.Empty;
        }

        /// <summary>
        /// Convierte un objeto (por ejemplo el Tag de un TextBox) en un arreglo de bytes para RowVersion.
        /// </summary>
        public static byte[] ConvertirRowVersion(object valor)
        {
            return RowVersionHelper.RowVersionObjToByteArray(valor);
        }
    }
}
