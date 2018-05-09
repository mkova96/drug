using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DrugData.Migrations
{
    public partial class currencypackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrancyCurrencyId",
                table: "Drug",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrugSize",
                table: "Drug",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "Drug",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PackageSize",
                table: "Drug",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrencyName = table.Column<string>(maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "Package",
                columns: table => new
                {
                    PackageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PackageType = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package", x => x.PackageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drug_CurrancyCurrencyId",
                table: "Drug",
                column: "CurrancyCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Drug_PackageId",
                table: "Drug",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drug_Currency_CurrancyCurrencyId",
                table: "Drug",
                column: "CurrancyCurrencyId",
                principalTable: "Currency",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Drug_Package_PackageId",
                table: "Drug",
                column: "PackageId",
                principalTable: "Package",
                principalColumn: "PackageId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drug_Currency_CurrancyCurrencyId",
                table: "Drug");

            migrationBuilder.DropForeignKey(
                name: "FK_Drug_Package_PackageId",
                table: "Drug");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "Package");

            migrationBuilder.DropIndex(
                name: "IX_Drug_CurrancyCurrencyId",
                table: "Drug");

            migrationBuilder.DropIndex(
                name: "IX_Drug_PackageId",
                table: "Drug");

            migrationBuilder.DropColumn(
                name: "CurrancyCurrencyId",
                table: "Drug");

            migrationBuilder.DropColumn(
                name: "DrugSize",
                table: "Drug");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Drug");

            migrationBuilder.DropColumn(
                name: "PackageSize",
                table: "Drug");
        }
    }
}
