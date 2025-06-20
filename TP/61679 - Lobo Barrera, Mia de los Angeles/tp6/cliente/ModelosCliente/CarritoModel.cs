using System.Collections.Generic;
using cliente.Modelos;
namespace cliente.Modelos
{
    public class CarritoModel
    {
        public int Id { get; set; } // Necesario para saber el ID del carrito
        public List<CarritoItem> Items { get; set; } = new(); // Necesario para inicializar un carrito nuevo
    }
}