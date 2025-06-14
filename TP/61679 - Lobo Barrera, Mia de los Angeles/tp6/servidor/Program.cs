using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data.Common;
using servidor.Modelos;
using Microsoft.Extensions.DependencyInjection;
#nullable enable

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar controladores si es necesario
builder.Services.AddControllers();

builder.Services.AddDbContext<TiendaDbContext>(options => options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    db.Database.Migrate();
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

//endpoins

//almaceno en memoria los carritos

var carritos = new Dictionary<Guid, Carrito>();

//1. GET de productos con busqueda
app.MapGet("/api/productos", async (TiendaDbContext db, string? query) =>
{
    var productos = string.IsNullOrEmpty(query)
? await db.Productos.ToListAsync()
: await db.Productos
.Where(p => p.Nombre.Contains(query) || p.Descripcion.Contains(query))
.ToListAsync();
});

//POST: inicializar el carrito y retorna su ID
app.MapPost("/api/carritos", () =>
{
    var id = Guid.NewGuid();
    carritos[id] = new Carrito { Id = id, Items = new List<CarritoItem>() };
    return Results.Created($"/api/carritos/{id}", id);
});

//3. GET retorna los items del carrito
app.MapGet("/api/carritos/{id:guid}", (Guid id) =>
{
    if (carritos.TryGetValue(id, out var carrito))
        return Results.Ok(carrito);
    return Results.NotFound("Carrito no encontrado");
});

//4. DELETE vacia el carrito y regresa productos al stock
app.MapDelete("/api/carritos/{id:guid}", async (Guid id, TiendaDbContext db) =>
{
    if (carritos.TryGetValue(id, out var carrito))
    {
        foreach (var item in carrito.Items)
        {
            var producto = await db.Productos.FindAsync(item.ProductoId);
            if (producto != null)
                producto.Stock += item.Cantidad;
        }
        carritos.Remove(id);
        await db.SaveChangesAsync();
        return Results.Ok("Su carrito se vacio con éxito.");
    }
    return Results.NotFound("Carrito no encontrado");
});

//5. PUT agrega o actualiza la cantidad de un producto en el carrito
app.MapPut("/api/carritos/{id:guid}/{productoId:int}", async (Guid id, int productoId, TiendaDbContext db) =>
{
    if (!carritos.TryGetValue(id, out var carrito))
        return Results.NotFound("Carrito no encontrado.");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound("Lo siento, producto no encontrado.");

    //validacion de stock disponible

    if (producto.Stock < 1)
        return Results.BadRequest("Lo siento, stock insuficiente");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item is null)
    {
        carrito.Items.Add(new CarritoItem { ProductoId = productoId, Producto = producto, Cantidad = 1 });
    }
    else
    {
        item.Cantidad += 1;
    }
    producto.Stock -= 1; //reducir stock

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

//6. DELETE elimina/reeduce la cantidad de un producto en el carrito
app.MapDelete("/api/carritos/{id:guid}/{productoId:int}", async (Guid id, int productoId, TiendaDbContext db) =>
{
    if (!carritos.TryGetValue(id, out var carrito))
        return Results.NotFound("Carrito no encontrado");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado.");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("El producto no se encuentra en el carrito.");

    //reducir e incrementar el stock
    item.Cantidad -= 1;
    producto.Stock += 1;
    if (item.Cantidad <= 0)
        carrito.Items.Remove(item);

    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

//7. Confirma/registra y limpia el carrito
app.MapPut("/api/carritos/{id:guid}/confirmar", async (Guid id, CompraDto compraDto, TiendaDbContext db) =>
{
    if (!carritos.TryGetValue(id, out var carrito))
        return Results.BadRequest("Carrito no encontrado");

    if (carrito.Items.Count == 0)
        return Results.BadRequest("El carrito esta vacio");

    decimal total = carrito.Items.Sum(i => i.Cantidad * i.Producto.Precio);

    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = total,
        NombreCliente = compraDto.NombreCliente,
        ApellidoeCliente = compraDto.ApellidoCliente,
        EmailCliente = compraDto.EmailCliente,
        Items = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.Producto.Precio
        }).ToList()
    };

    db.Compras.Add(compra);
    carritos.Remove(id);
    await db.SaveChangesAsync();
    return Results.Ok(compra);
});

app.Run();