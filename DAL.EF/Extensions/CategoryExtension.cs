using System;

namespace DAL.EF
{
    public partial class Category
    {
        // Propiedad auxiliar para mostrar en el grid
        public string RowVersionStr
        {
            get => BitConverter.ToInt64(RowVersion, 0).ToString();
            set => RowVersion = BitConverter.GetBytes(long.Parse(value)); // reconstruye el rowversion,lo asigna a la propiedad RowVersion a partir del valor string recibido
        }
    }
}
