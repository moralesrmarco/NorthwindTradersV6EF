using System;

namespace Utilities
{
    public static class RowVersionHelper
    {
        public static byte[] RowVersionObjToByteArray(object rowVersionObj)
        {

            return (rowVersionObj != null && long.TryParse(rowVersionObj.ToString(), out long tagVal))
                                ? BitConverter.GetBytes(tagVal)
                                : null; // para evitar excepcion devuelve null si el valor no es convertible a long
        }
    }
}
