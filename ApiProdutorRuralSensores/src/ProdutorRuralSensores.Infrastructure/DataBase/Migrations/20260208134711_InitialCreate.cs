using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProdutorRuralSensores.Infrastructure.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sensores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TalhaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fabricante = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataInstalacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", precision: 10, scale: 8, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", precision: 11, scale: 8, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UltimaLeitura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeiturasSensores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TalhaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SensorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UmidadeSolo = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Temperatura = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Precipitacao = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    UmidadeAr = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    VelocidadeVento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    DirecaoVento = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RadiacaoSolar = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    PressaoAtmosferica = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    DataHoraLeitura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeiturasSensores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeiturasSensores_Sensores_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeiturasSensores_DataHoraLeitura",
                table: "LeiturasSensores",
                column: "DataHoraLeitura",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_LeiturasSensores_SensorId",
                table: "LeiturasSensores",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_LeiturasSensores_TalhaoId_DataHoraLeitura",
                table: "LeiturasSensores",
                columns: new[] { "TalhaoId", "DataHoraLeitura" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Sensores_Ativo",
                table: "Sensores",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Sensores_Codigo",
                table: "Sensores",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sensores_TalhaoId",
                table: "Sensores",
                column: "TalhaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeiturasSensores");

            migrationBuilder.DropTable(
                name: "Sensores");
        }
    }
}
