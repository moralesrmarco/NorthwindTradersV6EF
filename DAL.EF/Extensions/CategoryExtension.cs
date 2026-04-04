using System;

namespace DAL.EF
{
    public partial class Category
    {
        public string RowVersionStr { get; set; }
        // Propiedad auxiliar para que no tenga conflicto el DataGridView
        public string RowVersionString
        {
            get => RowVersion != null
                ? BitConverter.ToInt64(RowVersion, 0).ToString()
                : string.Empty;
        }
    }
}
