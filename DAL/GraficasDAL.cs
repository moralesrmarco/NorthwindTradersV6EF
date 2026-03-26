using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class GraficasDAL
    {

        private readonly string _connectionString;

        public GraficasDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ObtenerAñosDeVentas()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentasObtenerAños", cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los años de ventas" + ex.Message);
            }
            return dt;
        }

        public List<DtoVentasMensuales> ObtenerVentasMensuales(int year)
        {
            var lista = new List<DtoVentasMensuales>();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentasObtenerMensuales", cn))
                {
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        int colMes = reader.GetOrdinal("Mes");
                        int colNombreMes = reader.GetOrdinal("NombreMes");
                        int colTotal = reader.GetOrdinal("Total");
                        while (reader.Read())
                        {
                            int mes = reader.IsDBNull(colMes) ? 0 : Convert.ToInt32(reader.GetValue(colMes));
                            string nombre = reader.IsDBNull(colNombreMes) ? string.Empty : reader.GetString(colNombreMes);
                            decimal total = reader.IsDBNull(colTotal) ? 0m : Convert.ToDecimal(reader.GetValue(colTotal));
                            lista.Add(new DtoVentasMensuales { Mes = mes, NombreMes = nombre, Total = total });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ventas mensuales: " + ex.Message);
            }
            return lista;
        }

        public int ObtenerTotalAñosConVentas()
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();

                string sql = @"
                                SELECT COUNT(DISTINCT YEAR(OrderDate))
                                FROM Orders";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public List<DtoVentasMensualesPorAños> ObtenerVentasMensualesPorAños(int years)
        {
            var lista = new List<DtoVentasMensualesPorAños>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand("SpVentasObtenerMensualesPorAnios", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@years", years);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new DtoVentasMensualesPorAños
                            {
                                Year = Convert.ToInt32(dr["Year"]),
                                Mes = Convert.ToInt32(dr["Mes"]),
                                NombreMes = Convert.ToString(dr["NombreMes"]),
                                Total = Convert.ToDecimal(dr["Total"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public DataTable ObtenerTopProductos(int cantidad, int anio)
        {
            var dt = new DataTable();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentasObtenerTopProductos", cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@Anio", anio);
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos más vendidos: " + ex.Message);
            }
            return dt;
        }

        public DataTable ObtenerTopProductosRpt(int cantidad, int anio)
        {
            var dt = new DataTable();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentasObtenerTopProductosRpt", cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@Anio", anio);
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos más vendidos: " + ex.Message);
            }
            return dt;
        }

        public DataTable ObtenerTop10AñosDeVentas()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpVentasObtenerTop10Años", cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los años de ventas" + ex.Message);
            }
            return dt;
        }

        public List<(string Vendedor, decimal TotalVentas)> ObtenerVentasPorVendedores(int anio)
        {
            var resultados = new List<(string Vendedor, decimal TotalVentas)>();
            try
            {
                using (SqlConnection cn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SpVentasObtenerPorVendedor", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Anio", anio);
                    cn.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string vendedor = rdr["Vendedor"].ToString();
                            decimal totalVentas = Convert.ToDecimal(rdr["TotalVentas"]);
                            resultados.Add((vendedor, totalVentas));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ventas por vendedor: " + ex.Message);
            }
            return resultados;
        }

        public DataTable ObtenerVentasMensualesPorVendedoresPorAño(int anio)
        {
            var dt = new DataTable();
            try
            {
                using (SqlConnection cn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SpVentasObtenerMensualesPorVendedoresPorAnio", cn))
                using (SqlDataAdapter dap = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType= CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Anio", anio);
                    dap.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ventas mensuales: " + ex.Message);
            }
            return dt;
        }
    }
}
