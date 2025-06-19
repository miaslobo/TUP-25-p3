using Microsoft.AspNetCore.Mvc;
using servidor.Modelos;

namespace servidor.Controllers
{
    [ApiController]
    [Route("carritos")]
    public class CarritoController : ControllerBase
    {
        private static Dictionary<int, Carrito> carritos = new();
        private static int contadorId = 1;

        [HttpPost]
        public IActionResult CrearCarrito()
        {
            var nuevoCarrito = new Carrito { Id = contadorId++ };
            carritos[nuevoCarrito.Id] = nuevoCarrito;

            return Ok(new { Id = nuevoCarrito.Id });
        }

        [HttpPut("{carritoId}/{productoId}")]
        public IActionResult AgregarProducto(int carritoId, int productoId, [FromBody] Dictionary<string, int> body)
        {
            if (!body.TryGetValue("cantidad", out int cantidad))
                return BadRequest("Falta la cantidad.");

            if (!carritos.TryGetValue(carritoId, out var carrito))
                return NotFound("Carrito no encontrado.");

            // Buscar producto en base de datos (mock acá)
            var producto = ProductosBD.FirstOrDefault(p => p.Id == productoId);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            var itemExistente = carrito.Items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Items.Add(new CarritoItem
                {
                    Producto = producto,
                    Cantidad = cantidad
                });
            }

            return Ok();
        }

        [HttpGet("{carritoId}")]
        public IActionResult ObtenerCarrito(int carritoId)
        {
            if (!carritos.TryGetValue(carritoId, out var carrito))
                return NotFound("Carrito no encontrado.");

            return Ok(carrito);
        }

        // Mock de productos (reemplazá esto por acceso real a DB si lo tenés)
        private static List<Producto> ProductosBD = new List<Producto>
        {
            new Producto { Id = 1, Nombre = "Hamburguesa", Descripcion = "Con cheddar", Precio = 1500, Stock = 10, ImagenUrl = "" },
            new Producto { Id = 2, Nombre = "Papas Fritas", Descripcion = "Grandes", Precio = 1000, Stock = 20, ImagenUrl = "" },
            // Agregá todos los productos que necesites
        };
    }
}