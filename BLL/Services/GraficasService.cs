using DAL;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;

namespace BLL.Services
{
    public class GraficasService
    {
        private readonly GraficasDAL _graficasDAL;

        public GraficasService(string _connectionString)
        {
            _graficasDAL = new GraficasDAL(_connectionString);
        }

        public DataTable ObtenerAñosDeVentas(bool conFilaSeleccione = true)
        {
            DataTable dt = _graficasDAL.ObtenerAñosDeVentas();
            if (conFilaSeleccione)
            {
                DataRow filaSeleccione = dt.NewRow();
                filaSeleccione["YearOrderDate"] = "»--- Seleccione ---«";
                dt.Rows.InsertAt(filaSeleccione, 0);
            }
            return dt;
        }

        public List<DtoVentasMensuales> ObtenerVentasMensuales(int year)
        {
            return _graficasDAL.ObtenerVentasMensuales(year);
        }

        public int ObtenerTotalAñosConVentas()
        {
            return _graficasDAL.ObtenerTotalAñosConVentas();
        }

        public List<DtoVentasMensualesPorAños> ObtenerVentasMensualesPorAños(int years)
        {
            return _graficasDAL.ObtenerVentasMensualesPorAños(years);
        }

        public DataTable ObtenerTopProductos(int cantidad, int anio)
        {
            return _graficasDAL.ObtenerTopProductos(cantidad, anio);
        }

        public DataTable ObtenerTopProductosRpt(int cantidad, int anio)
        {
            return _graficasDAL.ObtenerTopProductosRpt(cantidad, anio);
        }

        public DataTable ObtenerTop10AñosDeVentas(bool conFilaSeleccione = true)
        {
            DataTable dtDb = _graficasDAL.ObtenerTop10AñosDeVentas();
            DataTable dt = new DataTable();
            dt.Columns.Add("Texto", typeof(string));
            dt.Columns.Add("Valor", typeof(int));
            if (conFilaSeleccione)
                dt.Rows.Add("»--- Seleccione ---«", 0);
            dt.Rows.Add("Todos los años", -1);
            foreach (DataRow row in dtDb.Rows)
            {
                int anio = Convert.ToInt32(row["YearOrderDate"]);
                dt.Rows.Add(anio.ToString(), anio);
            }
            return dt;
        }

        public List<(string Vendedor, decimal TotalVentas)> ObtenerVentasPorVendedores(int anio = 0)
        {
            return _graficasDAL.ObtenerVentasPorVendedores(anio);
        }

        public DataTable ObtenerVentasMensualesPorVendedoresPorAño(int anio)
        {
            return _graficasDAL.ObtenerVentasMensualesPorVendedoresPorAño(anio);
        }

    }
}
