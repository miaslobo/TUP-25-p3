namespace servidor.Modelos
{
    public class Carrito
    {
        public Guid Id { get; set; }
        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
    }
}