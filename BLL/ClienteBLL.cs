using DAL;
using Entities;
using Entities.DTOs;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace BLL
{
    public class ClienteBLL
    {

        private readonly ClienteDAL _clienteDAL;

        public ClienteBLL(string _connectionString)
        {
            _clienteDAL = new ClienteDAL(_connectionString);
        }

        public DataTable ObtenerClientesPaisesCbo()
        {
            var paises = _clienteDAL.ObtenerClientesPaisesCbo();
            DataRow filaSeleccione = paises.NewRow();
            filaSeleccione["Id"] = "";
            filaSeleccione["Pais"] = "»--- Seleccione ---«";
            paises.Rows.InsertAt(filaSeleccione, 0);
            return paises;
        }

        public (List<Cliente> clientes, string mensajeEstado) ObtenerClientes(bool selectorRealizaBusqueda, Cliente criterios, bool top100 = false)
        {
            var clientes = _clienteDAL.ObtenerClientes(selectorRealizaBusqueda, criterios, top100);
            string mensajeEstado = selectorRealizaBusqueda
                ? $"Se encontraron {clientes.Count} cliente(s)."
                : $"Se muestran los primeros {clientes.Count} cliente(s) registrados.";
            return (clientes, mensajeEstado);
        }

        public Cliente ObtenerClientePorId(string idCliente)
        {
            return _clienteDAL.ObtenerClientePorId(idCliente);
        }

        public int Insertar(Cliente cliente)
        {
            return _clienteDAL.Insertar(cliente);
        }

        public int Actualizar(Cliente cliente)
        {
            return _clienteDAL.Actualizar(cliente);
        }

        public int Eliminar(string clienteId, byte[] rowVersion)
        {
            return _clienteDAL.Eliminar(clienteId, rowVersion);
        }

        public List<DtoClienteProveedor> ObtenerClientesProveedores(string nombreDeFormulario, string comboBoxSelectedValue, bool checkBoxClientes, bool checkBoxProveedores)
        {
            return _clienteDAL.ObtenerClientesProveedores(nombreDeFormulario, comboBoxSelectedValue, checkBoxClientes, checkBoxProveedores);
        }

        public List<KeyValuePair<string, string>> ObtenerCiudadPaisVwCliProvCbo()
        {
            var ciudadesPaises = _clienteDAL.ObtenerCiudadPaisVwCliProvCbo();
            var ciudadesPaisesKvp = new List<KeyValuePair<string, string>>();
            // Insertar opción "Seleccione"
            ciudadesPaisesKvp.Add(new KeyValuePair<string, string>("»--- Seleccione ---«", " "));
            // Insertar opción "Todas las ciudades"
            ciudadesPaisesKvp.Add(new KeyValuePair<string, string>("»--- Todas las ciudades ---«", "aaaaa"));
            // Agregar el resto de ciudades desde la DAL
            foreach (var item in ciudadesPaises)
            {
                ciudadesPaisesKvp.Add(new KeyValuePair<string, string>(item.CiudadPais, item.CiudadPais));
            }
            return ciudadesPaisesKvp;
        }

        public List<KeyValuePair<string, string>> ObtenerPaisVwCliProvCbo()
        {
            var paises = _clienteDAL.ObtenerPaisVwCliProvCbo();
            var paisesKvp = new List<KeyValuePair<string, string>>();
            // Insertar opción "Seleccione"
            paisesKvp.Add(new KeyValuePair<string, string>("»--- Seleccione ---«", " "));
            // Insertar opción "Todas las ciudades"
            paisesKvp.Add(new KeyValuePair<string, string>("»--- Todos los países ---«", "aaaaa"));
            // Agregar el resto de ciudades desde la DAL
            foreach (var item in paises)
            {
                paisesKvp.Add(new KeyValuePair<string, string>(item.Pais, item.Pais));
            }
            return paisesKvp;
        }
    }
}
