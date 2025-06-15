using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsCopra_Compras_CompraId",
                table: "ItemsCopra");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemsCopra_Productos_ProductoId",
                table: "ItemsCopra");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemsCopra",
                table: "ItemsCopra");

            migrationBuilder.RenameTable(
                name: "ItemsCopra",
                newName: "ItemsCompra");

            migrationBuilder.RenameIndex(
                name: "IX_ItemsCopra_ProductoId",
                table: "ItemsCompra",
                newName: "IX_ItemsCompra_ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemsCopra_CompraId",
                table: "ItemsCompra",
                newName: "IX_ItemsCompra_CompraId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemsCompra",
                table: "ItemsCompra",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsCompra_Compras_CompraId",
                table: "ItemsCompra",
                column: "CompraId",
                principalTable: "Compras",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsCompra_Productos_ProductoId",
                table: "ItemsCompra",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsCompra_Compras_CompraId",
                table: "ItemsCompra");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemsCompra_Productos_ProductoId",
                table: "ItemsCompra");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemsCompra",
                table: "ItemsCompra");

            migrationBuilder.RenameTable(
                name: "ItemsCompra",
                newName: "ItemsCopra");

            migrationBuilder.RenameIndex(
                name: "IX_ItemsCompra_ProductoId",
                table: "ItemsCopra",
                newName: "IX_ItemsCopra_ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemsCompra_CompraId",
                table: "ItemsCopra",
                newName: "IX_ItemsCopra_CompraId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemsCopra",
                table: "ItemsCopra",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsCopra_Compras_CompraId",
                table: "ItemsCopra",
                column: "CompraId",
                principalTable: "Compras",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsCopra_Productos_ProductoId",
                table: "ItemsCopra",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
