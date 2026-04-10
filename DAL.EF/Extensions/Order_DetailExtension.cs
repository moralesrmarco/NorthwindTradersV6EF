//https://www.youtube.com/watch?v=VjBAQV_cFxM&list=PLgvaYP_E7xkKhk3QYJCvNXndiypRugCrf&index=6 tiene un ejemplo de como armar una clase cuando existe relacion con otras clases
/*
    En los calculos se considero que el precio unitario ya incluye el iva.

    El IVA se calcula sobre el valor real de la transacción, es decir, el precio neto después de aplicar descuentos.
    - Si el producto tiene un descuento comercial (por promoción, volumen, etc.), ese descuento reduce la base.
    - Por lo tanto, el importe del IVA se determina sobre el precio con descuento, no sobre el precio original.
    En resumen: el IVA se calcula sobre el precio con descuento, porque ese es el valor real de la operación.


    Fórmula general cuando el precio ya incluye IVA
    Si el precio final PrecioConIVA ya incluye el IVA, y la tasa de IVA es TasaIVA (por ejemplo, 16% = 0.16), entonces:
    BaseSinIVA= PrecioConIVA / (1+TasaIVA)
    IVA = PrecioConIVA - BaseSinIVA
    */

using System;

namespace DAL.EF
{
    public partial class Order_Detail
    {
        // Propiedad expuesta para reportes
        public string ProductName
        {
            get { return Product.ProductName; }
        }

        // Base sin IVA (separando el impuesto del precio con IVA)
        // Precio unitario sin IVA después del descuento
        public decimal PrecioBaseSinIva => Math.Round(PrecioPorUnidadConIVADespuesDescuento / (1 + (decimal)TasaIVA), 2, MidpointRounding.AwayFromZero);
        public decimal PrecioPorUnidadSinIVASinDescuento => Math.Round(UnitPrice / (1 + (decimal)TasaIVA), 2, MidpointRounding.AwayFromZero);

        public decimal IVADelPrecioPorUnidadSinDescuento => Math.Round(UnitPrice - PrecioPorUnidadSinIVASinDescuento, 2, MidpointRounding.AwayFromZero);

        public decimal PrecioPorUnidadConIVADespuesDescuento => Math.Round(UnitPrice * (1 - (decimal)Discount), 2, MidpointRounding.AwayFromZero);

        public decimal IVADelPrecioporUnidadDespuesDescuento => Math.Round(PrecioPorUnidadConIVADespuesDescuento - PrecioPorUnidadSinIVADepuesDescuento, 2, MidpointRounding.AwayFromZero);

        public decimal PrecioPorUnidadSinIVADepuesDescuento => Math.Round(PrecioPorUnidadConIVADespuesDescuento / (1 + (decimal)TasaIVA), 2, MidpointRounding.AwayFromZero);

        public decimal AhorroPorUnidadSinIVA => Math.Round(PrecioPorUnidadSinIVASinDescuento - PrecioPorUnidadSinIVADepuesDescuento, 2, MidpointRounding.AwayFromZero);

        public decimal AhorroEnIVAPorUnidadDespuesDescuento => Math.Round(IVADelPrecioPorUnidadSinDescuento - IVADelPrecioporUnidadDespuesDescuento, 2, MidpointRounding.AwayFromZero);

        public decimal AhorroTotalPorUnidadConIVA => Math.Round(UnitPrice - PrecioPorUnidadConIVADespuesDescuento, 2, MidpointRounding.AwayFromZero);

        // Tasas expresadas en porcentaje
        public decimal TasaDescuentoPorcentaje => Math.Round((decimal)Discount * 100, 2, MidpointRounding.AwayFromZero);
        public decimal TasaIVAPorcentaje => Math.Round((decimal)TasaIVA * 100, 2, MidpointRounding.AwayFromZero);

        // Importe bruto (con IVA incluido)
        public decimal SubtotalDelImporteConIVAIncluido => Math.Round(UnitPrice * Quantity, 2, MidpointRounding.AwayFromZero);

        public decimal SubtotalDelImporteSinIVASinDescuento => Math.Round(PrecioPorUnidadSinIVASinDescuento * Quantity, 2, MidpointRounding.AwayFromZero);

        public decimal SubtotalDelImporteDelIVASinDescuento => Math.Round(IVADelPrecioPorUnidadSinDescuento * Quantity, 2, MidpointRounding.AwayFromZero);

        // Importe neto con descuento (todavía incluye IVA)
        public decimal SubtotalDelImporteConIVAConDescuento => Math.Round(SubtotalDelImporteConIVAIncluido * (1 - (decimal)Discount), 2, MidpointRounding.AwayFromZero);

        //public decimal SubtotalIVADespuesDelDescuento => Math.Round(SubtotalDelImporteConIVAConDescuento - SubtotalDelImporteSinIVAConDescuento, 2, MidpointRounding.AwayFromZero);

        public decimal SubtotalIVADespuesDelDescuento
        {
            get
            {
                // Total bruto (con IVA incluido) sin redondeos intermedios
                decimal bruto = UnitPrice * Quantity;

                // Total del descuento (valor absoluto)
                decimal descuento = bruto * (decimal)Discount;

                // Total con descuento (sigue incluyendo IVA)
                decimal totalConDescuento = bruto - descuento;

                // Si la tasa es 0 no hay IVA
                if (TasaIVA == 0)
                    return 0m;

                // Extraer la parte de IVA que está incluida en totalConDescuento:
                // base = total / (1 + tasa); iva = total - base
                decimal baseSinIva = totalConDescuento / (1 + (decimal)TasaIVA);
                decimal iva = totalConDescuento - baseSinIva;

                return Math.Round(iva, 2, MidpointRounding.AwayFromZero);
            }
        }

        public decimal SubtotalDelImporteSinIVAConDescuento
            => Math.Round(SubtotalDelImporteConIVAConDescuento / (1 + (decimal)TasaIVA), 2, MidpointRounding.AwayFromZero);

        public decimal SubtotalDelAhorroSinIvaDespuesDescuento => Math.Round(AhorroPorUnidadSinIVA * Quantity, 2, MidpointRounding.AwayFromZero);

        public decimal SubtotalDelAhorroEnIVADespuesDescuento => Math.Round(AhorroEnIVAPorUnidadDespuesDescuento * Quantity, 2, MidpointRounding.AwayFromZero);

        //public decimal SubtotalDelAhorroTotalDespuesDescuento => Math.Round(AhorroTotalPorUnidadConIVA * Quantity, 2, MidpointRounding.AwayFromZero);

        // El importe del ahorro total debe ser: importe total - importe con descuento.
        // Calcular sobre totales sin redondeo intermedio y redondear al final para evitar imprecisiones acumuladas.
        public decimal SubtotalDelAhorroTotalDespuesDescuento
        {
            get
            {
                decimal bruto = UnitPrice * Quantity; // sin redondeo intermedio
                decimal conDescuento = bruto * (1 - (decimal)Discount);
                decimal ahorro = bruto - conDescuento; // equivalente a bruto * Discount
                return Math.Round(ahorro, 2, MidpointRounding.AwayFromZero);
            }
        }

        // Subtotal = Importe con descuento (ya incluye IVA)
        public decimal Subtotal => SubtotalDelImporteConIVAConDescuento; // ya viene redondeado

        public Order_Detail()
        {
            Order = new Order();
            Product = new Product();
        }
    }
}
