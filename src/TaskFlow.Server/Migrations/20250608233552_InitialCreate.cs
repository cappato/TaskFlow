using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskFlow.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultValue = table.Column<string>(type: "TEXT", nullable: true),
                    ValidationRules = table.Column<string>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SKU = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Brand = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Articles_Users_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ArticleAttributeValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArticleId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomAttributeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleAttributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleAttributeValues_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleAttributeValues_CustomAttributes_CustomAttributeId",
                        column: x => x.CustomAttributeId,
                        principalTable: "CustomAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SKU = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ArticleId = table.Column<int>(type: "INTEGER", nullable: false),
                    Size = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Stock = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    WholesalePrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RetailPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleVariants_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "ParentCategoryId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), "Calzado deportivo", true, "Calzado", null, null },
                    { 2, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), "Ropa deportiva", true, "Ropa", null, null }
                });

            migrationBuilder.InsertData(
                table: "CustomAttributes",
                columns: new[] { "Id", "CreatedAt", "DefaultValue", "DisplayName", "IsActive", "IsRequired", "Name", "SortOrder", "Type", "UpdatedAt", "ValidationRules" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), null, "Talle", true, true, "talle", 1, 6, null, null },
                    { 2, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), null, "Color", true, true, "color", 2, 8, null, null },
                    { 3, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), null, "Material", true, false, "material", 3, 0, null, null },
                    { 4, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), null, "Temporada", true, false, "temporada", 4, 6, null, null },
                    { 5, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), null, "Género", true, true, "genero", 5, 6, null, null },
                    { 6, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), null, "Resistente al Agua", true, false, "resistencia_agua", 6, 3, null, null },
                    { 7, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), null, "Tipo de Suela", true, false, "tipo_suela", 7, 0, null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), "admin@cruzado.com", true, "Admin Cruzado" },
                    { 2, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), "supplier@nike.com", true, "Nike Supplier" },
                    { 3, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), "supplier@adidas.com", true, "Adidas Supplier" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "ParentCategoryId", "UpdatedAt" },
                values: new object[,]
                {
                    { 3, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), "Zapatillas para correr", true, "Zapatillas Running", 1, null },
                    { 4, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), "Zapatillas de fútbol", true, "Zapatillas Fútbol", 1, null },
                    { 5, new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), "Remeras deportivas", true, "Remeras", 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleAttributeValues_ArticleId_CustomAttributeId",
                table: "ArticleAttributeValues",
                columns: new[] { "ArticleId", "CustomAttributeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleAttributeValues_CustomAttributeId_Value",
                table: "ArticleAttributeValues",
                columns: new[] { "CustomAttributeId", "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_SKU",
                table: "Articles",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_SupplierId",
                table: "Articles",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleVariants_ArticleId",
                table: "ArticleVariants",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleVariants_SKU",
                table: "ArticleVariants",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomAttributes_Name",
                table: "CustomAttributes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleAttributeValues");

            migrationBuilder.DropTable(
                name: "ArticleVariants");

            migrationBuilder.DropTable(
                name: "CustomAttributes");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
