using DAL;
using Entities;
using Entities.DTOs;
using System.Collections.Generic;
using System.Data;

namespace BLL
{
    public class ProveedorBLL
    {
        private readonly ProveedorDAL _proveedorDAL;

        public ProveedorBLL(string _connectionString)
        {
            _proveedorDAL = new ProveedorDAL(_connectionString);
        }

        public int Insertar(Proveedor proveedor)
        {
            return _proveedorDAL.Insertar(proveedor);
        }

        public int Actualizar(Proveedor proveedor)
        {
            return _proveedorDAL.Actualizar(proveedor);
        }

        public int Eliminar(int supplierId, byte[] rowVersion)
        {
            return _proveedorDAL.Eliminar(supplierId, rowVersion);
        }

        public Proveedor ObtenerProveedorPorId(int supplierId)
        {
            return _proveedorDAL.ObtenerProveedorPorId(supplierId);
        }

        public DataTable ObtenerProveedorPaisesCbo()
        {
            var paises = _proveedorDAL.ObtenerProveedorPaisesCbo();
            DataRow filaSeleccione = paises.NewRow();
            filaSeleccione["Id"] = "";
            filaSeleccione["Pais"] = "»--- Seleccione ---«";
            paises.Rows.InsertAt(filaSeleccione, 0);
            return paises;
        }

        public List<Proveedor> ObtenerProveedores(bool selectorRealizaBusqueda, DtoProveedoresBuscar criterios, bool top100)
        {
            return _proveedorDAL.ObtenerProveedores(selectorRealizaBusqueda, criterios, top100);
        }

        public DataSet ObtenerProveedoresProductosDgv()
        {
            return _proveedorDAL.ObtenerProveedoresProductosDgv();
        }
    }
}
