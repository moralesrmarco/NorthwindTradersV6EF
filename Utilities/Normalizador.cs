namespace Utilities
{
    public static class Normalizador
    {
        // Función única de normalización
        public static string Normalizar(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            // Quitar espacios extra y poner en formato "Primera letra mayúscula, resto minúscula"
            valor = valor.Trim();
            return char.ToUpper(valor[0]) + valor.Substring(1).ToLower();
        }
    }
}
