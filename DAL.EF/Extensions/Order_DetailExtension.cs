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

        // Helpers privados para centralizar lógica y redondeo
        private decimal TasaIVADecimal => (decimal)TasaIVA;

        private decimal DescuentoDecimal => (decimal)Discount;

        private decimal FactorDescuento => 1 - DescuentoDecimal;

        private decimal FactorIVA => 1 + TasaIVADecimal;

        private decimal Bruto() => UnitPrice * Quantity;

        private decimal BaseDesdeTotalIncluyeIva(decimal totalConIva)
            => (TasaIVADecimal == 0) ? totalConIva : totalConIva / FactorIVA;

        private decimal IvaDesdeTotalIncluyeIva(decimal totalConIva) => totalConIva - BaseDesdeTotalIncluyeIva(totalConIva);

        private decimal TotalConDescuentoSinRedondeo() => Bruto() * FactorDescuento;

        private decimal Round2(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        // Base sin IVA (separando el impuesto del precio con IVA)
        // Precio unitario sin IVA después del descuento
        public decimal PrecioBaseSinIva => Round2(PrecioPorUnidadConIVADespuesDescuento / FactorIVA);

        public decimal PrecioPorUnidadSinIVASinDescuento => Round2(UnitPrice / FactorIVA);

        public decimal IVADelPrecioPorUnidadSinDescuento => Round2(UnitPrice - PrecioPorUnidadSinIVASinDescuento);

        public decimal PrecioPorUnidadConIVADespuesDescuento => Round2(UnitPrice * FactorDescuento);

        public decimal IVADelPrecioporUnidadDespuesDescuento => Round2(PrecioPorUnidadConIVADespuesDescuento - PrecioPorUnidadSinIVADepuesDescuento);

        public decimal PrecioPorUnidadSinIVADepuesDescuento => Round2(PrecioPorUnidadConIVADespuesDescuento / FactorIVA);

        public decimal AhorroPorUnidadSinIVA => Round2(PrecioPorUnidadSinIVASinDescuento - PrecioPorUnidadSinIVADepuesDescuento);

        public decimal AhorroEnIVAPorUnidadDespuesDescuento => Round2(IVADelPrecioPorUnidadSinDescuento - IVADelPrecioporUnidadDespuesDescuento);

        public decimal AhorroTotalPorUnidadConIVA => Round2(UnitPrice - PrecioPorUnidadConIVADespuesDescuento);

        // Tasas expresadas en porcentaje
        public decimal TasaDescuentoPorcentaje => Round2((decimal)Discount * 100m);
        public decimal TasaIVAPorcentaje => Round2((decimal)TasaIVA * 100m);

        // Importe bruto (con IVA incluido)
        public decimal SubtotalDelImporteConIVAIncluido => Round2(Bruto());

        public decimal SubtotalDelImporteSinIVASinDescuento => Round2(PrecioPorUnidadSinIVASinDescuento * Quantity);

        public decimal SubtotalDelImporteDelIVASinDescuento => Round2(IVADelPrecioPorUnidadSinDescuento * Quantity);

        // Importe neto con descuento (todavía incluye IVA)
        public decimal SubtotalDelImporteConIVAConDescuento => Round2(TotalConDescuentoSinRedondeo());

        // IVA calculado sobre el total con descuento (parte incluida en el total)
        public decimal SubtotalIVADespuesDelDescuento
        {
            get
            {
                decimal totalSinRedondeo = TotalConDescuentoSinRedondeo();
                return Round2(IvaDesdeTotalIncluyeIva(totalSinRedondeo));
            }
        }

        public decimal SubtotalDelImporteSinIVAConDescuento
            => Round2(BaseDesdeTotalIncluyeIva(TotalConDescuentoSinRedondeo()));

        public decimal SubtotalDelAhorroSinIvaDespuesDescuento => Round2(AhorroPorUnidadSinIVA * Quantity);

        public decimal SubtotalDelAhorroEnIVADespuesDescuento => Round2(AhorroEnIVAPorUnidadDespuesDescuento * Quantity);

        // El importe del ahorro total debe ser: importe total - importe con descuento.
        public decimal SubtotalDelAhorroTotalDespuesDescuento
        {
            get
            {
                decimal bruto = Bruto();
                decimal conDescuento = TotalConDescuentoSinRedondeo();
                return Round2(bruto - conDescuento);
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