namespace servidor.Modelos
{
    public class ItemCompra
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public required Producto Producto { get; set; } = null;
        public int CompraId { get; set; }
        public Compra Compra { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}