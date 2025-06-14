using System.ComponentModel.DataAnnotations;

namespace servidor.Modelos
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public string NombreCliente { get; set; } = "";
        public string ApellidoeCliente { get; set; } = "";
        public string EmailCliente { get; set; } = "";

        public List<ItemCompra> Items { get; set; } = new List<ItemCompra>();
    }
}