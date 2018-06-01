using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DrugData.Migrations
{
    public partial class newpackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drug_Currency_CurrancyCurrencyId",
                table: "Drug");

            migrationBuilder.DropForeignKey(
                name: "FK_Drug_Manufacturer_ManufacturerId",
                table: "Drug");

            migrationBuilder.DropColumn(
                name: "DrugSize",
                table: "Drug");

            migrationBuilder.DropColumn(
                name: "PackageSize",
                table: "Drug");

            migrationBuilder.RenameColumn(
                name: "CurrancyCurrencyId",
                table: "Drug",
                newName: "CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Drug_CurrancyCurrencyId",
                table: "Drug",
                newName: "IX_Drug_CurrencyId");

            migrationBuilder.AddColumn<int>(
                name: "IndividualSize",
                table: "Package",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MeasureId",
                table: "Package",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Package",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ManufacturerId",
                table: "Drug",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "Measure",
                columns: table => new
                {
                    MeasureId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MeasureName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measure", x => x.MeasureId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Package_MeasureId",
                table: "Package",
                column: "MeasureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drug_Currency_CurrencyId",
                table: "Drug",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Drug_Manufacturer_ManufacturerId",
                table: "Drug",
                column: "ManufacturerId",
                principalTable: "Manufacturer",
                principalColumn: "ManufacturerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Package_Measure_MeasureId",
                table: "Package",
                column: "MeasureId",
                principalTable: "Measure",
                principalColumn: "MeasureId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drug_Currency_CurrencyId",
                table: "Drug");

            migrationBuilder.DropForeignKey(
                name: "FK_Drug_Manufacturer_ManufacturerId",
                table: "Drug");

            migrationBuilder.DropForeignKey(
                name: "FK_Package_Measure_MeasureId",
                table: "Package");

            migrationBuilder.DropTable(
                name: "Measure");

            migrationBuilder.DropIndex(
                name: "IX_Package_MeasureId",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "IndividualSize",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "MeasureId",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Package");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                table: "Drug",
                newName: "CurrancyCurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Drug_CurrencyId",
                table: "Drug",
                newName: "IX_Drug_CurrancyCurrencyId");

            migrationBuilder.AlterColumn<int>(
                name: "ManufacturerId",
                table: "Drug",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrugSize",
                table: "Drug",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PackageSize",
                table: "Drug",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Drug_Currency_CurrancyCurrencyId",
                table: "Drug",
                column: "CurrancyCurrencyId",
                principalTable: "Currency",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Drug_Manufacturer_ManufacturerId",
                table: "Drug",
                column: "ManufacturerId",
                principalTable: "Manufacturer",
                principalColumn: "ManufacturerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
