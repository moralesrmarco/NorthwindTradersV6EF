using DTOs.EF;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace Utilities
{
    public static class ReportDataTableAdapterHelper
    {
        public static DataTable ConvertirVentasMensuales(List<DtoVentasMensuales> ventas)
        {
            var dt = new DataTable();
            dt.Columns.Add("Mes", typeof(int));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("NombreMes", typeof(string));
            foreach (var v in ventas)
                dt.Rows.Add(v.Mes, v.Total, v.NombreMes);
            return dt;
        }

        public static DataTable ConvertirVentaAnualComparativa(List<DtoVentasMensualesPorAños> ventas)
        {
            var dt = new DataTable();
            dt.Columns.Add("Mes", typeof(int));
            dt.Columns.Add("NombreMes", typeof(string));
            dt.Columns.Add("Año", typeof(int));
            dt.Columns.Add("Total", typeof(decimal));
            foreach (var v in ventas)
            {
                dt.Rows.Add(v.Mes, v.NombreMes, v.Year, v.Total);
            }
            return dt;
        }

        //public static DataTable ConvertirProductoMasVendido(List<DtoProductoMasVendido> productos)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add("Posicion", typeof(int));
        //    dt.Columns.Add("NombreProducto", typeof(string));
        //    dt.Columns.Add("CantidadVendida", typeof(int));
        //    foreach (var p in productos)
        //        dt.Rows.Add(p.Posicion, p.NombreProducto, p.CantidadVendida);
        //    return dt;
        //}

        //public static DataTable ConvertirVendedorTotalVentas(List<DtoVendedorTotalVentas> vendedorTotalVentas)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add("Vendedor", typeof(string));
        //    dt.Columns.Add("TotalVentas", typeof(decimal));
        //    foreach (var vtv in vendedorTotalVentas)
        //        dt.Rows.Add(vtv.Vendedor, vtv.TotalVentas);
        //    return dt;
        //}

        public static DataTable ConvertirVendedorTotalVentas(List<(string Vendedor, decimal TotalVentas)> lista)
        {
            var dt = new DataTable();
            dt.Columns.Add("Vendedor", typeof(string));
            dt.Columns.Add("TotalVentas", typeof(decimal));
            foreach (var item in lista)
            {
                var row = dt.NewRow();
                row["Vendedor"] = item.Vendedor ?? string.Empty;
                row["TotalVentas"] = item.TotalVentas;
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
