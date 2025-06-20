using System.Net.Http;
using System.Net.Http.Json;
using cliente.Modelos;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using cliente.Pages;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        private int? carritoId;
        public class CrearCarritoResponse { public int Id { get; set; } }

        public CarritoService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public List<ItemCarrito> Items { get; private set; } = new();

        public async Task AgregarProducto(int productoId, int cantidad)
        {
            var carritoId = await ObtenerOCrearCarritoId();

            var body = new Dictionary<string, int>
            {
                { "cantidad", cantidad }
            };

            var response = await _http.PutAsJsonAsync($"http://localhost:5184/carritos/{carritoId}/{productoId}", body);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al agregar producto al carrito: {msg}");
                throw new Exception("No se pudo agregar el producto al carrito.");
            }
        }

        private async Task<int> ObtenerOCrearCarritoId()
        {
            if (carritoId.HasValue)
                return carritoId.Value;

            try
            {
                var response = await _http.PostAsync("http://localhost:5184/carritos", null);
                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al crear el carrito: {msg}");
                    throw new Exception("No se pudo crear el carrito.");
                }

                var result = await response.Content.ReadFromJsonAsync<CrearCarritoResponse>();
                if (result == null)
                    throw new Exception("Respuesta vacía al crear el carrito.");

                carritoId = result.Id;
                return carritoId.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al crear el carrito: {ex.Message}");
                throw;
            }
        }
        public void IncrementarCantidad(int productoId)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null) item.Cantidad++;
        }

        public void DisminuirCantidad(int productoId)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                if (item.Cantidad > 1)
                    item.Cantidad--;
                else
                    Items.Remove(item);
            }
        }

        public void ReiniciarCarrito()
        {
            carritoId = null;
        }

        public decimal Total => Items.Sum(i => i.Cantidad * i.Producto.Precio);

        public int? ObtenerCarritoIdActual() => carritoId;

        public async Task CargarCarritoDesdeBackend()
        {
            if (!carritoId.HasValue)
                return;

            var response = await _http.GetAsync($"http://localhost:5184/carritos/{carritoId}");

            if (response.IsSuccessStatusCode)
            {
                var carrito = await response.Content.ReadFromJsonAsync<CarritoModel>();
                if (carrito != null)
                {
                    Items = carrito.Items.Select(i => new ItemCarrito
                    {
                        Producto = i.Producto,
                        Cantidad = i.Cantidad
                    }).ToList();
                }
            }
        }

        public async Task VaciarCarrito()
        {
            if (!carritoId.HasValue)
                return;

            var response = await _http.DeleteAsync($"http://localhost:5184/carritos/{carritoId}");

            if (response.IsSuccessStatusCode)
            {
                Items.Clear();
            }
        }

        public async Task ConfirmarCompra(DatosCliente datos)
        {
            if (!carritoId.HasValue)
                throw new Exception("No hay carrito");

            var response = await _http.PutAsJsonAsync($"http://localhost:5184/carritos/{carritoId}/confirmar", datos);

            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo confirmar la compra.");

            Items.Clear();
        }
    }

    public class ItemCarrito
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }

    public class DatosCliente
    {
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }
}