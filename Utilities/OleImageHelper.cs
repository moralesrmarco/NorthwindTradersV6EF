using System;
using System.Collections.Generic;
using System.Data;

namespace Utilities
{
    public static class OleImageHelper
    {
        // Versión para List<T>
        // ejemplo de uso: OleImageHelper.CleanOleHeader(categorias, "CategoryID", "Picture", 1, 8);
        /// <summary>
        /// Limpia el encabezado OLE de la propiedad "Picture" en cualquier objeto
        /// que tenga un campo entero de ID y un campo byte[] de imagen.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto en la colección</typeparam>
        /// <param name="items">Colección de objetos</param>
        /// <param name="idPropertyName">Nombre de la propiedad que contiene el ID (ej. "CategoryID")</param>
        /// <param name="picturePropertyName">Nombre de la propiedad que contiene la imagen (ej. "Picture")</param>
        /// <param name="minId">ID mínimo para aplicar limpieza</param>
        /// <param name="maxId">ID máximo para aplicar limpieza</param>
        public static void CleanOleHeader<T>(
            IEnumerable<T> items,
            string idPropertyName,
            string picturePropertyName,
            int minId,
            int maxId)
        {
            const int OLE_HEADER_LENGTH = 78;
            foreach (var item in items)
            {
                var type = typeof(T);
                var idProp = type.GetProperty(idPropertyName);
                var picProp = type.GetProperty(picturePropertyName);

                if (idProp == null || picProp == null) continue;

                var idValue = idProp.GetValue(item);
                var picValue = picProp.GetValue(item) as byte[];

                if (idValue is int id && picValue != null &&
                    id >= minId && id <= maxId && picValue.Length > OLE_HEADER_LENGTH)
                {
                    byte[] cleanImage = new byte[picValue.Length - OLE_HEADER_LENGTH];
                    Array.Copy(picValue, OLE_HEADER_LENGTH, cleanImage, 0, cleanImage.Length);
                    picProp.SetValue(item, cleanImage);
                }
            }
        }

        // Versión para DataTable
        // ejemplo de uso: OleImageHelper.CleanOleHeader(dtCategorias, "CategoryID", "Picture", 1, 8);
        /// <summary>
        /// Limpia el encabezado OLE de la columna de imagen en un DataTable.
        /// </summary>
        /// <param name="table">El DataTable con los datos.</param>
        /// <param name="idColumnName">Nombre de la columna que contiene el ID (ej. "CategoryID").</param>
        /// <param name="pictureColumnName">Nombre de la columna que contiene la imagen (ej. "Picture").</param>
        /// <param name="minId">ID mínimo para aplicar limpieza.</param>
        /// <param name="maxId">ID máximo para aplicar limpieza.</param>
        public static void CleanOleHeader(DataTable table, string idColumnName, string pictureColumnName, int minId, int maxId)
        {
            const int OLE_HEADER_LENGTH = 78;
            if (table == null || !table.Columns.Contains(idColumnName) || !table.Columns.Contains(pictureColumnName))
                return;

            foreach (DataRow row in table.Rows)
            {
                if (row[idColumnName] != DBNull.Value && row[pictureColumnName] != DBNull.Value)
                {
                    int id = Convert.ToInt32(row[idColumnName]);
                    byte[] picValue = (byte[])row[pictureColumnName];

                    if (id >= minId && id <= maxId && picValue.Length > OLE_HEADER_LENGTH)
                    {
                        byte[] cleanImage = new byte[picValue.Length - OLE_HEADER_LENGTH];
                        Array.Copy(picValue, OLE_HEADER_LENGTH, cleanImage, 0, cleanImage.Length);
                        row[pictureColumnName] = cleanImage;
                    }
                }
            }
        }
    }
}
